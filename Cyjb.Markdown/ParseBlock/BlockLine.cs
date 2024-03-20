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
	/// 行的文本。
	/// </summary>
	private readonly BlockText text = new();
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
	public bool ParagraphSkippable { get; set; }
	/// <summary>
	/// 获取是否是代码缩进。
	/// </summary>
	public bool IsCodeIndent => Indent >= CodeIndent;
	/// <summary>
	/// 获取行的起始位置。
	/// </summary>
	public int Start => hasIndent ? indentStart : text.Start;
	/// <summary>
	/// 获取行的结束位置。
	/// </summary>
	public int End => text.End;
	/// <summary>
	/// 获取行的范围。
	/// </summary>
	public TextSpan Span
	{
		get
		{
			if (hasIndent)
			{
				return new TextSpan(indentStart, text.End);
			}
			else
			{
				return text.Span;
			}
		}
	}

	public BlockText BlockText => text;

	/// <summary>
	/// 获取行是否是空的。
	/// </summary>
	internal bool IsEmpty => text.Tokens.Count == 0;

	/// <summary>
	/// 获取词法单元列表。
	/// </summary>
	internal IReadOnlyList<Token<BlockKind>> Tokens => text.Tokens;

	/// <summary>
	/// 返回当前行是否是空的或只包含空白。
	/// </summary>
	/// <param name="start">要检查的起始词法单元索引。</param>
	public bool IsBlank(int start = 0)
	{
		int count = text.Tokens.Count;
		for (int i = start; i < count; i++)
		{
			BlockKind kind = text.Tokens[i].Kind;
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
		text.Clear();
		hasIndent = true;
		indentStart = indentOriginalStart = indentEnd = 0;
		indentText = StringView.Empty;
		indentStartColumn = indentEndColumn = 0;
	}

	/// <summary>
	/// 将当前行添加到指定块文本。
	/// </summary>
	/// <param name="text">要添加到的块文本。</param>
	public void AppendTo(BlockText text)
	{
		// 添加缩进。
		if (hasIndent && Indent > 0)
		{
			text.Add(new Token<BlockKind>(BlockKind.Indent, GetIndentText(), new TextSpan(indentStart, indentEnd)));
		}
		// 添加剩余词法单元。
		this.text.AppendTo(text);
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
		text.AppendTo(builder);
	}

	/// <summary>
	/// 清除行的数据。
	/// </summary>
	public void Clear()
	{
		text.Clear();
		hasIndent = false;
		ParagraphSkippable = true;
	}

	/// <summary>
	/// 添加新的词法单元。
	/// </summary>
	/// <param name="token">要添加的词法单元。</param>
	internal void Add(Token<BlockKind> token)
	{
		text.Add(token);
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
		if (text.Tokens.Count == 0)
		{
			indentStart = indentOriginalStart = indentEnd = 0;
			indentText = StringView.Empty;
			indentStartColumn = indentEndColumn = 0;
			return;
		}
		Token<BlockKind> token = text.PeekFront();
		if (token.Kind == BlockKind.Indent)
		{
			// 是缩进，提取相关信息。
			text.PopFront();
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
		return text.PeekFront();
	}

	/// <summary>
	/// 读取并返回开始处的下一词法单元。
	/// </summary>
	/// <returns>开始处的下一词法单元。</returns>
	internal Token<BlockKind> PopFront()
	{
		Token<BlockKind> token = text.PopFront();
		// 读取词法单元后，需要重新检查缩进、文本和位置信息。
		hasIndent = false;
		return token;
	}


	/// <summary>
	/// 返回当前对象的字符串视图表示形式。
	/// </summary>
	/// <returns>当前对象的字符串视图表示形式。</returns>
	public StringView ToStringView()
	{
		if (!hasIndent || Indent == 0)
		{
			return text.ToStringView();
		}
		StringBuilder sb = StringBuilderPool.Rent(text.Length);
		sb.Append(GetIndentText().AsSpan());
		text.AppendTo(sb);
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
