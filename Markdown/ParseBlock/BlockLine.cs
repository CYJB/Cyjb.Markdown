using System.Text;
using Cyjb.Collections;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示块的行。
/// </summary>
internal sealed class BlockLine
{
	/// <summary>
	/// 代码缩进长度。
	/// </summary>
	public const int CodeIndent = 4;

	/// <summary>
	/// 词法单元的池。
	/// </summary>
	private readonly Stack<Token<BlockKind>> tokenPool = new();
	/// <summary>
	/// 词法单元队列。
	/// </summary>
	private readonly Deque<Token<BlockKind>> tokens = new();
	/// <summary>
	/// 文本的长度。
	/// </summary>
	private int length;
	/// <summary>
	/// 行定位器。
	/// </summary>
	private readonly LineLocator locator;
	/// <summary>
	/// 是否已计算缩进信息。
	/// </summary>
	private bool hasIndent;
	/// <summary>
	/// 缩进的起始位置。
	/// </summary>
	private int indentStart = -1;
	/// <summary>
	/// 缩进结束位置。
	/// </summary>
	private int indentEnd;
	/// <summary>
	/// 缩进缩进文本。
	/// </summary>
	private StringView indentText;
	/// <summary>
	/// 缩进原始起始位置。
	/// </summary>
	private int indentOriginalStart;
	/// <summary>
	/// 缩进起始列号。
	/// </summary>
	private int indentStartColumn;
	/// <summary>
	/// 缩进结束列号。
	/// </summary>
	private int indentEndColumn;

	/// <summary>
	/// 使用指定的行定位器初始化 <see cref="BlockLine"/> 类的新实例。
	/// </summary>
	/// <param name="locator">行定位器。</param>
	internal BlockLine(LineLocator locator)
	{
		this.locator = locator;
	}

	/// <summary>
	/// 返回指定索引的行列位置。
	/// </summary>
	/// <param name="index">要检查行列位置的索引。</param>
	/// <returns>指定索引的行列位置。</returns>
	public LinePosition GetPosition(int index)
	{
		return locator.GetPosition(index);
	}

	/// <summary>
	/// 获取当前缩进宽度。
	/// </summary>
	public int Indent
	{
		get
		{
			GetIndent();
			return indentEndColumn - indentStartColumn;
		}
	}
	/// <summary>
	/// 是否是段落可以跳过的缩进。
	/// </summary>
	public bool ParagraphSkippable { get; set; } = true;
	/// <summary>
	/// 获取是否是代码缩进。
	/// </summary>
	public bool IsCodeIndent => Indent >= CodeIndent;
	/// <summary>
	/// 获取行的起始位置。
	/// </summary>
	public int Start
	{
		get
		{
			if (hasIndent)
			{
				return indentStart;
			}
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
	/// 获取行的结束位置。
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
	/// 获取行的范围。
	/// </summary>
	public TextSpan Span
	{
		get
		{
			if (hasIndent)
			{
				return new TextSpan(indentStart, End);
			}
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
	/// 获取行是否是空的。
	/// </summary>
	internal bool IsEmpty => tokens.Count == 0;

	/// <summary>
	/// 获取词法单元列表。
	/// </summary>
	internal IReadOnlyList<Token<BlockKind>> Tokens => tokens;

	/// <summary>
	/// 返回当前行是否是空的或只包含空白。
	/// </summary>
	/// <param name="start">要检查的起始词法单元索引。</param>
	public bool IsBlank(int start = 0)
	{
		int count = tokens.Count;
		for (int i = start; i < count; i++)
		{
			BlockKind kind = tokens[i].Kind;
			if (kind != BlockKind.Indent && kind != BlockKind.NewLine)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 跳过指定个数的空白。
	/// </summary>
	/// <param name="count">要跳过的空白个数。</param>
	public void SkipIndent(int count)
	{
		GetIndent();
		indentStartColumn += count;
		if (indentStartColumn >= indentEndColumn)
		{
			indentStartColumn = indentEndColumn;
			indentStart = indentEnd;
			return;
		}
		int column;
		// 由于 Tab 可能对应多列，因此需要找到首个 index 使得 column(index)≤startColumn。
		for (; indentStart < indentEnd; indentStart++)
		{
			column = locator.GetPosition(indentStart).Column;
			if (column == indentStartColumn)
			{
				return;
			}
			else if (column > indentStartColumn)
			{
				indentStart--;
				return;
			}
		}
		// 避免 end 的列位置超出 startColumn
		column = locator.GetPosition(indentStart).Column;
		if (column > indentStartColumn)
		{
			indentStart--;
		}
	}

	/// <summary>
	/// 跳过起始空白。
	/// </summary>
	public void SkipIndent()
	{
		GetIndent();
		indentStartColumn = indentEndColumn;
		indentStart = indentEnd;
	}

	/// <summary>
	/// 跳过当前行。
	/// </summary>
	public void Skip()
	{
		for (int i = 0; i < tokens.Count; i++)
		{
			tokenPool.Push(tokens[i]);
		}
		tokens.Clear();
		hasIndent = true;
		indentStart = indentOriginalStart = indentEnd = 0;
		indentText = StringView.Empty;
		indentStartColumn = indentEndColumn = 0;
	}

	/// <summary>
	/// 将当前行添加到指定块文本。
	/// </summary>
	/// <param name="text">要添加到的块文本。</param>
	/// <param name="includeIndent">是否要添加缩进文本。</param>
	public void AppendTo(BlockText text, bool includeIndent = true)
	{
		// 添加缩进。
		if (includeIndent && hasIndent && Indent > 0)
		{
			text.Add(GetIndentText(), indentStart);
		}
		// 添加剩余词法单元。
		int count = tokens.Count;
		for (int i = 0; i < count; i++)
		{
			var token = tokens[i];
			text.Add(token.Text, token.Span.Start);
		}
	}

	/// <summary>
	/// 将当前行添加到指定字符串。
	/// </summary>
	/// <param name="builder">字符串构造器。</param>
	public void AppendTo(StringBuilder builder)
	{
		// 添加缩进。
		if (hasIndent && Indent > 0)
		{
			builder.Append(GetIndentText().AsSpan());
		}
		// 添加剩余词法单元。
		int count = tokens.Count;
		for (int i = 0; i < count; i++)
		{
			builder.Append(tokens[i].Text.AsSpan());
		}
	}

	/// <summary>
	/// 清除行的数据。
	/// </summary>
	public void Clear()
	{
		for (int i = 0; i < tokens.Count; i++)
		{
			tokenPool.Push(tokens[i]);
		}
		tokens.Clear();
		hasIndent = false;
		ParagraphSkippable = true;
	}

	/// <summary>
	/// 添加新的词法单元。
	/// </summary>
	/// <param name="kind">词法单元的类型。</param>
	/// <param name="text">词法单元的文本。</param>
	/// <param name="span">词法单元的范围。</param>
	/// <param name="value">词法单元的值。</param>
	public void Add(BlockKind kind, StringView text, TextSpan span, object? value = null)
	{
		if (tokenPool.Count > 0)
		{
			var token = tokenPool.Pop();
			token.Kind = kind;
			token.Text = text;
			token.Span = span;
			token.Value = value;
			tokens.PushBack(token);
		}
		else
		{
			tokens.PushBack(new Token<BlockKind>(kind, text, span, value));
		}
	}

	/// <summary>
	/// 获取缩进信息。
	/// </summary>
	private void GetIndent()
	{
		if (hasIndent)
		{
			return;
		}
		hasIndent = true;
		if (tokens.Count == 0)
		{
			indentStart = indentOriginalStart = indentEnd = 0;
			indentText = StringView.Empty;
			indentStartColumn = indentEndColumn = 0;
			return;
		}
		Token<BlockKind> token = tokens.PeekFront();
		if (token.Kind == BlockKind.Indent)
		{
			// 是缩进，提取相关信息。
			tokenPool.Push(tokens.PopFront());
			indentStart = token.Span.Start;
			indentEnd = token.Span.End;
			indentOriginalStart = indentStart;
			indentText = token.Text;
			indentStartColumn = locator.GetPosition(indentStart).Column;
			indentEndColumn = locator.GetPosition(indentEnd).Column;
		}
		else
		{
			// 其它，使用空缩进。
			indentStart = indentOriginalStart = indentEnd = token.Span.Start;
			indentText = StringView.Empty;
			indentStartColumn = indentEndColumn = 0;
		}
	}

	/// <summary>
	/// 返回开始处的下一词法单元，但不将其消费。
	/// </summary>
	/// <returns>开始处的下一词法单元。</returns>
	internal Token<BlockKind> PeekFront()
	{
		return tokens.PeekFront();
	}

	/// <summary>
	/// 读取并返回开始处的下一词法单元。
	/// </summary>
	/// <returns>开始处的下一词法单元。</returns>
	internal Token<BlockKind> PopFront()
	{
		Token<BlockKind> token = tokens.PopFront();
		tokenPool.Push(token);
		// 读取词法单元后，需要重新检查缩进、文本和位置信息。
		hasIndent = false;
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
	/// 移除起始位置的多个字符。
	/// </summary>
	/// <param name="length">要移除的字符个数。</param>
	public void RemoveStart(int length)
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
	public void RemoveEnd(int length)
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
	public bool TrimEnd()
	{
		int diff = 0;
		while (tokens.TryPeekBack(out var token))
		{
			if (token.Kind is BlockKind.NewLine or BlockKind.Indent)
			{
				tokens.PopBack();
				continue;
			}
			StringView text = token.Text;
			int textLen = text.Length;
			text = text.TrimEnd(MarkdownUtil.WhitespaceChars);
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
	/// 返回当前对象的块文本表示形式。
	/// </summary>
	/// <param name="includeIndent">是否要添加缩进文本。</param>
	/// <returns>当前对象的块文本表示形式。</returns>
	public BlockText ToBlockText(bool includeIndent = true)
	{
		BlockText text = new();
		AppendTo(text, includeIndent);
		return text;
	}

	/// <summary>
	/// 返回当前对象的字符串视图表示形式。
	/// </summary>
	/// <returns>当前对象的字符串视图表示形式。</returns>
	public StringView ToStringView()
	{
		int count = tokens.Count;
		if (!hasIndent || Indent == 0)
		{
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
			return StringBuilderPool.GetStringAndReturn(text);
		}
		StringBuilder sb = StringBuilderPool.Rent(length);
		sb.Append(GetIndentText().AsSpan());
		for (int i = 0; i < count; i++)
		{
			sb.Append(tokens[i].Text.AsSpan());
		}
		string result = sb.ToString();
		StringBuilderPool.Return(sb);
		return result;
	}

	/// <summary>
	/// 返回剩余的缩进文本。
	/// </summary>
	/// <returns>缩进文本。</returns>
	private StringView GetIndentText()
	{
		int column = locator.GetPosition(indentStart).Column;
		if (column == indentStartColumn)
		{
			return indentText[(indentStart - indentOriginalStart)..];
		}
		else
		{
			// 当前是部分 Tab，需要使用空格补齐 column(start) 到 startColumn 的位置。
			column = locator.GetPosition(indentStart + 1).Column;
			using ValueList<char> result = new(stackalloc char[ValueList.StackallocCharSizeLimit]);
			result.Add(' ', column - indentStartColumn);
			int idx = indentStart + 1 - indentOriginalStart;
			// 存在 Tab 时，可能会出现列数超出字符数的场景。
			if (idx < indentText.Length)
			{
				result.Add(indentText.AsSpan(idx));
			}
			return result.ToString();
		}
	}
}
