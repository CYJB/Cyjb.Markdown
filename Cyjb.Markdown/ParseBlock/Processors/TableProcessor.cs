using Cyjb.Markdown.ParseInline;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表格的解析器。
/// </summary>
internal sealed class TableProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 表格节点。
	/// </summary>
	private readonly Table table;
	/// <summary>
	/// 所有单元格信息。
	/// </summary>
	private readonly List<CellInfo> cellInfos = new();

	/// <summary>
	/// 使用表格的分隔符信息和标题行初始化 <see cref="TableProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="delimiters">表格分隔符。</param>
	/// <param name="heading">标题行。</param>
	private TableProcessor(StringView[] delimiters, BlockText heading)
		: base(MarkdownKind.Table)
	{
		TableRow row = ParseRow(heading.Span, heading);
		int start = heading.Start;
		table = new Table(row, new TextSpan(start, start));
		ParseDelimiters(delimiters);
	}

	/// <summary>
	/// 解析分隔符。
	/// </summary>
	/// <param name="delimiters">要解析的分隔符。</param>
	private void ParseDelimiters(StringView[] delimiters)
	{
		for (int i = 0; i < delimiters.Length; i++)
		{
			StringView cellText = delimiters[i].Trim(MarkdownUtil.WhitespaceChars);
			TableAlign align = TableAlign.None;
			if (cellText.StartsWith(':'))
			{
				align = TableAlign.Left;
			}
			if (cellText.EndsWith(':'))
			{
				if (align == TableAlign.Left)
				{
					align = TableAlign.Center;
				}
				else
				{
					align = TableAlign.Right;
				}
			}
			table.Aligns[i] = align;
		}
	}

	/// <summary>
	/// 获取是否可以延迟延伸。
	/// </summary>
	public override bool CanLazyContinuation => true;
	/// <summary>
	/// 获取是否允许尝试开始新的块。
	/// </summary>
	/// <remarks>表格允许被其它块节点中断。</remarks>
	public override bool TryBlockStarts => true;
	/// <summary>
	/// 获取当前块是否需要解析行内节点。
	/// </summary>
	public override bool NeedParseInlines => true;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		return line.IsBlank() ? BlockContinue.None : BlockContinue.Continue;
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="line">新添加的行。</param>
	public override void AddLine(BlockLine line)
	{
		table.Children.Add(ParseRow(line.Span, line.BlockText));
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		table.Span = new TextSpan(table.Span.Start, end);
		return table;
	}

	/// <summary>
	/// 解析行内节点。
	/// </summary>
	/// <param name="parser">行内节点的解析器。</param>
	public override void ParseInline(InlineParser parser)
	{
		foreach (CellInfo info in cellInfos)
		{
			info.ParseInline(parser);
		}
	}

	/// <summary>
	/// 解析指定行。
	/// </summary>
	/// <param name="rowSpan">行的文本范围。</param>
	/// <param name="text">要解析的文本。</param>
	/// <returns>解析后的表格行。</returns>
	private TableRow ParseRow(TextSpan rowSpan, BlockText text)
	{
		// 行首的空白都已当作缩进来处理，因此这里不需要 TrimStart。
		text.TrimEnd();
		List<TableCell> cells = new();
		BlockText cellText = new();
		int cellStart = text.Start;
		int cellEnd = text.End;
		bool escaped = false;
		int count = text.Tokens.Count;
		for (int i = 0; i < count; i++)
		{
			int startIdx = 0;
			var token = text.Tokens[i];
			ReadOnlySpan<char> textSpan = token.Text;
			int j = 0;
			if (i == 0 && textSpan[0] == '|')
			{
				// 首个 | 总是会当作前导竖划线看待，不计入内容。
				startIdx = 1;
				j++;
			}
			for (; j < textSpan.Length; j++)
			{
				char ch = textSpan[j];
				if (ch == '\\')
				{
					escaped = !escaped;
				}
				else if (ch == '|')
				{
					if (escaped)
					{
						// 需要特殊处理竖划线，转义后的竖划线不会产生新单元格，
						// 但在后续解析行级元素时，需要当作一个竖划线看待，不会包含前面的转义字符。
						// 特别是在 `\|` 场景，会被当作 `|` 解析。
						// 所以在拼接字符串时，将 \ 忽略掉。
						if (startIdx < j - 1)
						{
							cellText.Add(token, startIdx, j - 1 - startIdx);
						}
						startIdx = j;
						escaped = false;
					}
					else
					{
						if (j > startIdx)
						{
							cellText.Add(token, startIdx, j - startIdx);
						}
						// 单元格总是包含结束 | 的。
						int curEnd = token.Span.Start + j + 1;
						TextSpan span = new(cellStart, curEnd);
						CellInfo info = new(span, cellText);
						cells.Add(info.Cell);
						cellInfos.Add(info);
						cellStart = curEnd;
						startIdx = j + 1;
						cellText = new BlockText();
					}
				}
				else
				{
					escaped = false;
				}
			}
			// 添加当前 Token 的剩余文本。
			if (startIdx < textSpan.Length)
			{
				cellText.Add(token, startIdx, textSpan.Length - startIdx);
			}
		}
		// 添加可能的最后一个单元格。
		if (cellText.Length > 0 || cellStart < cellEnd)
		{
			CellInfo info = new(new TextSpan(cellStart, cellEnd), cellText);
			cells.Add(info.Cell);
			cellInfos.Add(info);
		}
		return new TableRow(cells, rowSpan);
	}

	/// <summary>
	/// 单元格的信息。
	/// </summary>
	private sealed class CellInfo
	{
		/// <summary>
		/// 单元格节点。
		/// </summary>
		public readonly TableCell Cell;
		/// <summary>
		/// 映射文本列表。
		/// </summary>
		private readonly BlockText text;

		/// <summary>
		/// 使用指定的文本范围和映射文本初始化 <see cref="CellInfo"/> 类的新实例。
		/// </summary>
		/// <param name="span">单元格的文本范围。</param>
		/// <param name="text">块文本。</param>
		public CellInfo(TextSpan span, BlockText text)
		{
			Cell = new TableCell(span);
			// 需要移除文本的前后空白。
			if (text.Length > 0)
			{
				text.TrimStart();
				text.TrimEnd();
			}
			this.text = text;
		}

		/// <summary>
		/// 解析当前单元格。
		/// </summary>
		/// <param name="parser">行内节点的解析器。</param>
		public void ParseInline(InlineParser parser)
		{
			if (text.Length > 0)
			{
				parser.Parse(text, Cell.Children);
			}
		}
	}

	/// <summary>
	/// 处理器工厂。
	/// </summary>
	private sealed class BlockFactory : IBlockFactory
	{
		/// <summary>
		/// 尝试开始当前块的解析。
		/// </summary>
		/// <param name="parser">块级语法分析器。</param>
		/// <param name="line">要检查的行。</param>
		/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
		/// <returns>如果能够开始当前块的解析，则返回解析器序列。否则返回空序列。</returns>
		public IEnumerable<BlockProcessor> TryStart(BlockParser parser, BlockLine line, BlockProcessor matchedProcessor)
		{
			// 要求分割行之前是段落，而且包含且只包含一行。
			BlockText? text;
			if (line.IsCodeIndent || (text = matchedProcessor.ParagraphText) == null ||
				!text.IsSingleLine())
			{
				yield break;
			}
			// 语法分析时已确保分割行的内容是有效的，这里直接 split 即可。
			// 忽略前后空白，忽略空的子字符串来忽略最外侧的竖划线。
			StringView[] delimiters = line.PeekFront().Text.Trim(MarkdownUtil.WhitespaceChars)
				.Split('|', StringSplitOptions.RemoveEmptyEntries);
			// 标题行与分割行必须具有相同的单元格数。
			if (delimiters.Length != CountCell(text))
			{
				yield break;
			}
			// 需要将之前的段落关闭。
			matchedProcessor.NeedReplace();
			// 跳过当前行。
			line.Skip();
			yield return new TableProcessor(delimiters, text);
		}

		/// <summary>
		/// 返回指定行的单元格个数。
		/// </summary>
		/// <param name="text">要解析的文本。</param>
		/// <returns>单元格的个数。</returns>
		private static int CountCell(BlockText text)
		{
			int cellCount = 0;
			bool escaped = false;
			bool hasContent = false;
			int end = text.Tokens.Count - 1;
			for (int i = 0; i <= end; i++)
			{
				ReadOnlySpan<char> textSpan = text.Tokens[i].Text;
				textSpan = textSpan.TrimEnd(MarkdownUtil.Whitespace);
				int j = 0;
				if (i == 0 && textSpan[0] == '|')
				{
					// 首个 | 总是会当作前导竖划线看待，不计入内容。
					j++;
					hasContent = true;
				}
				for (; j < textSpan.Length; j++)
				{
					char ch = textSpan[j];
					if (ch == '\\')
					{
						escaped = !escaped;
					}
					else if (ch == '|')
					{
						if (escaped)
						{
							escaped = false;
						}
						else
						{
							cellCount++;
							hasContent = false;
						}
					}
					else
					{
						escaped = false;
						hasContent = true;
					}
				}
			}
			// 添加可能的最后一个单元格。
			if (hasContent)
			{
				cellCount++;
			}
			return cellCount;
		}
	}
}
