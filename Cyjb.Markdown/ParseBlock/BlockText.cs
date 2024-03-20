using System.Text;
using Cyjb.Collections;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示块的文本。
/// </summary>
internal sealed class BlockText
{
	/// <summary>
	/// 词法单元队列。
	/// </summary>
	private readonly Deque<Token<BlockKind>> tokens = new();
	/// <summary>
	/// 文本的长度。
	/// </summary>
	private int length;

	/// <summary>
	/// 获取文本的起始位置。
	/// </summary>
	public int Start
	{
		get
		{
			if (tokens.TryPeekFront(out var token))
			{
				return token.Span.Start;
			}
			else
			{
				return 0;
			}
		}
	}
	/// <summary>
	/// 获取文本的结束位置。
	/// </summary>
	public int End
	{
		get
		{
			if (tokens.TryPeekBack(out var token))
			{
				return token.Span.End;
			}
			else
			{
				return 0;
			}
		}
	}
	/// <summary>
	/// 获取文本的范围。
	/// </summary>
	public TextSpan Span
	{
		get
		{
			int count = tokens.Count;
			if (count == 0)
			{
				return new TextSpan();
			}
			else if (count == 1)
			{
				return tokens.PeekFront().Span;
			}
			else
			{
				return new TextSpan(tokens.PeekFront().Span.Start, tokens.PeekBack().Span.End);
			}
		}
	}

	/// <summary>
	/// 获取文本的长度。
	/// </summary>
	public int Length => length;
	/// <summary>
	/// 获取词法单元列表。
	/// </summary>
	public IReadOnlyList<Token<BlockKind>> Tokens => tokens;

	/// <summary>
	/// 检查是否是单行文本。
	/// </summary>
	/// <returns>如果是单行文本，会返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool IsSingleLine()
	{
		int count = tokens.Count;
		for (int i = 0; i < count; i++)
		{
			if (tokens[i].Kind == BlockKind.NewLine)
			{
				return i == count - 1;
			}
			ReadOnlySpan<char> span = tokens[i].Text;
			int idx = span.IndexOf('\n');
			if (idx >= 0 && idx < span.Length - 1)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 返回指定索引的文本。
	/// </summary>
	/// <param name="index">要检查的索引。</param>
	/// <returns>指定索引的文本。</returns>
	public char this[int index]
	{
		get
		{
			int count = tokens.Count;
			for (int i = 0; i < count; i++)
			{
				StringView text = tokens[i].Text;
				int textLen = text.Length;
				if (index >= textLen)
				{
					index -= textLen;
				}
				else
				{
					return text[index];
				}
			}
			return SourceReader.InvalidCharacter;
		}
	}

	/// <summary>
	/// 移除起始位置的多个字符。
	/// </summary>
	/// <param name="length">要移除的字符个数。</param>
	public void RemoteStart(int length)
	{
		this.length -= length;
		while (tokens.TryPeekFront(out var token))
		{
			StringView text = token.Text;
			int textLen = text.Length;
			if (length >= textLen)
			{
				length -= textLen;
				tokens.PopFront();
			}
			else
			{
				token.Text = text.Substring(length);
				token.Span = token.Span with
				{
					Start = token.Span.Start + length,
				};
				break;
			}
		}
	}

	/// <summary>
	/// 移除结束位置的多个字符。
	/// </summary>
	/// <param name="length">要移除的字符个数。</param>
	public void RemoteEnd(int length)
	{
		this.length -= length;
		while (tokens.TryPeekBack(out var token))
		{
			StringView text = token.Text;
			length -= text.Length;
			if (length >= 0)
			{
				tokens.PopBack();
			}
			else
			{
				token.Text = text[..-length];
				token.Span = token.Span with
				{
					End = token.Span.Start - length,
				};
				break;
			}
		}
	}

	/// <summary>
	/// 移除起始空白。
	/// </summary>
	/// <returns>如果移除了任何起始空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimStart()
	{
		int diff = 0;
		while (tokens.TryPeekFront(out var token))
		{
			StringView text = token.Text;
			int textLen = text.Length;
			text = text.TrimStart(MarkdownUtil.WhitespaceChars);
			textLen -= text.Length;
			if (textLen == 0)
			{
				break;
			}
			diff += textLen;
			if (text.IsEmpty)
			{
				tokens.PopFront();
			}
			else
			{
				token.Text = text;
				token.Span = token.Span with
				{
					Start = token.Span.Start + textLen,
				};
				break;
			}
		}
		if (diff == 0)
		{
			return false;
		}
		length -= diff;
		return true;
	}

	/// <summary>
	/// 移除结尾空白。
	/// </summary>
	/// <returns>如果移除了任何结尾空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimEnd(bool trimNewLine = true)
	{
		int diff = 0;
		char[] chars = trimNewLine ? MarkdownUtil.WhitespaceChars : MarkdownUtil.WhitespaceCharsWithoutNewLine;
		while (tokens.TryPeekBack(out var token))
		{
			StringView text = token.Text;
			int textLen = text.Length;
			text = text.TrimEnd(chars);
			textLen -= text.Length;
			if (textLen == 0)
			{
				break;
			}
			diff += textLen;
			if (text.IsEmpty)
			{
				tokens.PopBack();
			}
			else
			{
				token.Text = text;
				token.Span = token.Span with
				{
					End = token.Span.Start + text.Length,
				};
				break;
			}
		}
		if (diff == 0)
		{
			return false;
		}
		length -= diff;
		return true;
	}

	/// <summary>
	/// 将当前文本添加到指定字符串。
	/// </summary>
	/// <param name="builder">字符串构造器。</param>
	public void AppendTo(StringBuilder builder)
	{
		int count = tokens.Count;
		for (int i = 0; i < count; i++)
		{
			builder.Append(tokens[i].Text.AsSpan());
		}
	}

	/// <summary>
	/// 添加新的词法单元。
	/// </summary>
	/// <param name="token">要添加的词法单元。</param>
	/// <param name="canConcat">是否可以连接字符串视图。</param>
	public void Add(Token<BlockKind> token, bool canConcat = false)
	{
		if (length == 0)
		{
			canConcat = false;
		}
		length += token.Text.Length;
		if (canConcat)
		{
			var lastToken = tokens.PeekBack();
			if (lastToken.Text.TryConcat(token.Text, out var concated))
			{
				lastToken.Text = concated;
				lastToken.Span = lastToken.Span with
				{
					End = token.Span.End,
				};
				return;
			}
		}
		tokens.PushBack(token);
	}

	/// <summary>
	/// 添加新的词法单元的一部分。
	/// </summary>
	/// <param name="token">要添加的词法单元。</param>
	/// <param name="start">要添加的起始索引。</param>
	/// <param name="length">要添加的长度。</param>
	public void Add(Token<BlockKind> token, int start, int length)
	{
		if (length <= 0)
		{
			return;
		}
		StringView text = token.Text;
		if (length == text.Length)
		{
			Add(token);
		}
		else
		{
			int spanStart = token.Span.Start + start;
			int spanEnd = spanStart + length;
			tokens.PushBack(new Token<BlockKind>(token.Kind, text.Slice(start, length), new TextSpan(spanStart, spanEnd)));
			this.length += length;
		}
	}

	/// <summary>
	/// 返回开始处的下一词法单元，但不将其消费。
	/// </summary>
	/// <returns>开始处的下一词法单元。</returns>
	public Token<BlockKind> PeekFront()
	{
		return tokens.PeekFront();
	}

	/// <summary>
	/// 读取并返回开始处的下一词法单元。
	/// </summary>
	/// <returns>开始处的下一词法单元。</returns>
	public Token<BlockKind> PopFront()
	{
		Token<BlockKind> token = tokens.PopFront();
		length -= token.Text.Length;
		return token;
	}

	/// <summary>
	/// 返回末尾处的下一词法单元，但不将其消费。
	/// </summary>
	/// <returns>末尾处的下一词法单元。</returns>
	public Token<BlockKind> PeekBack()
	{
		return tokens.PeekBack();
	}

	/// <summary>
	/// 读取并返回末尾处的下一词法单元。
	/// </summary>
	/// <returns>末尾处的下一词法单元。</returns>
	public Token<BlockKind> PopBack()
	{
		Token<BlockKind> token = tokens.PopBack();
		length -= token.Text.Length;
		return token;
	}

	/// <summary>
	/// 清除所有文本。
	/// </summary>
	public void Clear()
	{
		tokens.Clear();
		length = 0;
	}

	/// <summary>
	/// 将当前文本的内容添加到指定块文本。
	/// </summary>
	/// <param name="text">要添加到的块文本。</param>
	public void AppendTo(BlockText text)
	{
		int count = tokens.Count;
		for (int i = 0; i < count; i++)
		{
			text.Add(tokens[i], true);
		}
	}

	/// <summary>
	/// 读取位置映射。
	/// </summary>
	/// <param name="locMap">要写入的位置映射表。</param>
	public void GetLocationMap(LocationMap locMap)
	{
		int count = tokens.Count;
		int index = 0;
		for (int i = 0; i < count; i++)
		{
			var token = tokens[i];
			locMap.Add(index, token.Span.Start);
			index += token.Text.Length;
		}
	}

	/// <summary>
	/// 创建当前文本的副本。
	/// </summary>
	/// <returns>当前文本的副本。</returns>
	public BlockText Clone()
	{
		BlockText result = new();
		int count = tokens.Count;
		result.tokens.EnsureCapacity(count);
		result.length = length;
		for (int i = 0; i < count; i++)
		{
			result.tokens.PushBack(tokens[i]);
		}
		return result;
	}

	/// <summary>
	/// 返回当前对象的字符串视图表示形式。
	/// </summary>
	/// <returns>当前对象的字符串视图表示形式。</returns>
	public StringView ToStringView()
	{
		int count = tokens.Count;
		if (count == 0)
		{
			return StringView.Empty;
		}
		else if (count == 1)
		{
			return tokens[0].Text;
		}
		// 优先连接字符串视图。
		StringView view = tokens[0].Text;
		int i = 1;
		for (; i < count && view.TryConcat(tokens[i].Text, out var newView); i++)
		{
			view = newView;
		}
		if (i >= count)
		{
			return view;
		}
		// 存在无法连接的字符串，改为使用 StringBuilder 拼接。
		StringBuilder text = StringBuilderPool.Rent(length);
		text.Append(view.AsSpan());
		for (; i < count; i++)
		{
			text.Append(tokens[i].Text.AsSpan());
		}
		string result = text.ToString();
		StringBuilderPool.Return(text);
		return result;
	}
}
