using System;
using System.Text;
using Cyjb.Collections;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 行的信息。
/// </summary>
internal sealed class LineInfo
{
	/// <summary>
	/// 代码缩进长度。
	/// </summary>
	public const int CodeIndent = 4;

	/// <summary>
	/// 词法单元队列。
	/// </summary>
	private readonly ListQueue<Token<BlockKind>> tokens = new();
	/// <summary>
	/// 行定位器。
	/// </summary>
	private readonly LineLocator locator;
	/// <summary>
	/// 块解析器。
	/// </summary>
	private readonly BlockParser parser;
	/// <summary>
	/// 行的起始位置。
	/// </summary>
	private int start = -1;
	/// <summary>
	/// 行的结束位置。
	/// </summary>
	private int end;
	/// <summary>
	/// 映射的文本。
	/// </summary>
	private MappedText? text;
	/// <summary>
	/// 是否已计算缩进信息。
	/// </summary>
	private bool hasIndent;
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
	/// 使用指定的行定位器初始化 <see cref="LineInfo"/> 类的新实例。
	/// </summary>
	/// <param name="parser">块解析器。</param>
	/// <param name="locator">行定位器。</param>
	internal LineInfo(BlockParser parser, LineLocator locator)
	{
		this.parser = parser;
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
	public int Start => hasIndent ? start : start;
	/// <summary>
	/// 获取行的结束位置。
	/// </summary>
	public int End => end;
	/// <summary>
	/// 获取解析的选项。
	/// </summary>
	public ParseOptions Options => parser.Options;

	/// <summary>
	/// 获取当前行的文本。
	/// </summary>
	public MappedText Text
	{
		get
		{
			if (text == null)
			{
				List<StringView> texts = new();
				int length = 0;
				int lastLength = 0;
				int lastMappedIndex = 0;
				TextSpanBuilder spanBuilder = new();
				List<Tuple<int, int>> maps = new();
				// 添加缩进。
				if (hasIndent && Indent > 0)
				{
					lastMappedIndex = start;
					maps.Add(new Tuple<int, int>(0, lastMappedIndex));
					spanBuilder.Add(lastMappedIndex);
					StringView text = GetIndentText();
					lastLength = text.Length;
					length += lastLength;
					texts.Add(text);
				}
				// 添加剩余词法单元。
				bool isFirst = true;
				int count = tokens.Count;
				for (int i = 0; i < count; i++)
				{
					var token = tokens[i];
					// 首个缩进可能会存在 Tab 部分替换为空格的情况，因此之后的词法单元也需要添加索引。
					if (isFirst)
					{
						int offset = token.Span.Start - lastMappedIndex;
						if (lastLength != offset)
						{
							maps.Add(new Tuple<int, int>(lastLength, offset));
						}
						isFirst = false;
					}
					lastMappedIndex = token.Span.Start;
					lastLength = token.Text.Length;
					length += lastLength;
					texts.Add(token.Text);
					spanBuilder.Add(token.Span);
				}
				text = new MappedText(texts, length, spanBuilder.GetSpan(), maps);
			}
			return text;
		}
	}

	/// <summary>
	/// 获取当前激活的节点处理器。
	/// </summary>
	public BlockProcessor ActivatedProcessor => parser.ActivatedProcessor;

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
			SkipIndent();
			return;
		}
		int column;
		// 由于 Tab 可能对应多列，因此需要找到首个 index 使得 column(index)≤startColumn。
		for (; start < indentEnd; start++)
		{
			column = locator.GetPosition(start).Column;
			if (column == indentStartColumn)
			{
				return;
			}
			else if (column > indentStartColumn)
			{
				start--;
				return;
			}
		}
		// 避免 end 的列位置超出 startColumn
		column = locator.GetPosition(start).Column;
		if (column > indentStartColumn)
		{
			start--;
		}
		text = null;
	}

	/// <summary>
	/// 跳过全部空白。
	/// </summary>
	public void SkipIndent()
	{
		GetIndent();
		indentStartColumn = indentEndColumn;
		start = indentEnd;
		text = null;
	}

	/// <summary>
	/// 跳过当前行。
	/// </summary>
	public void Skip()
	{
		tokens.Clear();
		hasIndent = true;
		start = indentOriginalStart = indentEnd = end;
		indentText = StringView.Empty;
		indentStartColumn = indentEndColumn = 0;
		text = null;
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
			if (indentStartColumn == indentEndColumn)
			{
				// 所有缩进均已消费。
				return;
			}
			int column = locator.GetPosition(start).Column;
			if (column == indentStartColumn)
			{
				builder.Append(indentText.AsSpan(start - indentOriginalStart));
			}
			else
			{
				// 当前是部分 Tab，需要使用空格补齐 column(start) 到 startColumn 的位置。
				column = locator.GetPosition(start + 1).Column;
				builder.Append(' ', column - indentStartColumn);
				int idx = start + 1 - indentOriginalStart;
				// 存在 Tab 时，可能会出现列数超出字符数的场景。
				if (idx < indentText.Length)
				{
					builder.Append(indentText.AsSpan(idx));
				}
			}
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
		tokens.Clear();
		hasIndent = false;
		text = null;
		start = -1;
		end = 0;
		ParagraphSkippable = true;
	}

	/// <summary>
	/// 添加新的词法单元。
	/// </summary>
	/// <param name="token">要添加的词法单元。</param>
	internal void AddToken(Token<BlockKind> token)
	{
		if (start < 0)
		{
			start = token.Span.Start;
		}
		end = token.Span.End;
		tokens.Enqueue(token);
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
			start = indentOriginalStart = indentEnd = end;
			indentText = StringView.Empty;
			indentStartColumn = indentEndColumn = 0;
			return;
		}
		Token<BlockKind> token = tokens.Peek();
		if (token.Kind == BlockKind.Indent)
		{
			// 是缩进，提取相关信息。
			tokens.Dequeue();
			text = null;
			start = token.Span.Start;
			indentEnd = token.Span.End;
			indentOriginalStart = start;
			indentText = token.Text;
			indentStartColumn = locator.GetPosition(start).Column;
			indentEndColumn = locator.GetPosition(indentEnd).Column;
		}
		else
		{
			// 其它，使用空缩进。
			start = indentOriginalStart = indentEnd = token.Span.Start;
			indentText = StringView.Empty;
			indentStartColumn = indentEndColumn = 0;
		}
	}

	/// <summary>
	/// 返回下一词法单元，但不将其消费。
	/// </summary>
	/// <returns>下一词法单元。</returns>
	internal Token<BlockKind> Peek()
	{
		return tokens.Peek();
	}

	/// <summary>
	/// 读取并返回下一词法单元。
	/// </summary>
	/// <returns>下一词法单元。</returns>
	internal Token<BlockKind> Read()
	{
		Token<BlockKind> token = tokens.Dequeue();
		// 读取词法单元后，需要重新检查缩进、文本和位置信息。
		hasIndent = false;
		text = null;
		start = token.Span.End;
		return token;
	}

	/// <summary>
	/// 返回剩余的缩进文本。
	/// </summary>
	/// <returns>缩进文本。</returns>
	private StringView GetIndentText()
	{
		if (indentStartColumn == indentEndColumn)
		{
			// 所有缩进均已消费。
			return StringView.Empty;
		}
		int column = locator.GetPosition(start).Column;
		if (column == indentStartColumn)
		{
			return indentText[(start - indentOriginalStart)..];
		}
		else
		{
			// 当前是部分 Tab，需要使用空格补齐 column(start) 到 startColumn 的位置。
			column = locator.GetPosition(start + 1).Column;
			using ValueList<char> result = new(stackalloc char[ValueList.StackallocCharSizeLimit]);
			result.Add(' ', column - indentStartColumn);
			int idx = start + 1 - indentOriginalStart;
			// 存在 Tab 时，可能会出现列数超出字符数的场景。
			if (idx < indentText.Length)
			{
				result.Add(indentText.AsSpan(idx));
			}
			return result.ToString();
		}
	}
}
