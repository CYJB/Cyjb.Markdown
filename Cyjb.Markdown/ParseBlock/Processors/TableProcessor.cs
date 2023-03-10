using System.Collections.Generic;
using System.Diagnostics;
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
	/// 使用表格的对齐信息和标题行初始化 <see cref="TableProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="aligns">对齐信息。</param>
	/// <param name="heading">标题行。</param>
	private TableProcessor(List<TableAlign> aligns, MappedText heading)
		: base(MarkdownKind.Table)
	{
		TableRow row = ParseRow(heading);
		int start = heading.Span.Start;
		table = new Table(row, new TextSpan(start, start));
		for (int i = 0; i < aligns.Count; i++)
		{
			table.Aligns[i] = aligns[i];
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
	public override BlockContinue TryContinue(LineInfo line)
	{
		return line.IsBlank ? BlockContinue.None : BlockContinue.Continue;
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="text">行的文本。</param>
	public override void AddLine(MappedText text)
	{
		table.Children.Add(ParseRow(text));
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
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
	/// <param name="text">要解析的文本。</param>
	/// <returns>解析后的表格行。</returns>
	private TableRow ParseRow(MappedText text)
	{
		TextSpan rowSpan = text.Span;
		text.TrimStart();
		text.TrimEnd();
		string str = text.ToString();
		List<TableCell> cells = new();
		List<MappedText> texts = new();
		int spanStart = 0;
		int start = 0;
		int len = str.Length;
		for (int i = 0; i < len; i++)
		{
			char ch = str[i];
			switch (ch)
			{
				case '\\':
					if (i + 1 < len)
					{
						if (str[i + 1] == '|')
						{
							// 需要特殊处理竖划线，转义后的竖划线不会产生新单元格，
							// 但在后续解析行级元素时，需要当作一个竖划线看待，不会包含前面的转义字符。
							// 特别是在 `\|` 场景，会被当作 `|` 解析。
							// 所以在拼接字符串时，将 \ 忽略掉。
							if (i > start)
							{
								texts.Add(text[start..i]);
							}
							start = i + 1;
							i++;
						}
						i++;
					}
					break;
				case '|':
					if (i == 0)
					{
						// 首个 | 总是会当作前导竖划线看待，不计入内容。
						start = i + 1;
					}
					else
					{
						if (i > start)
						{
							texts.Add(text[start..i]);
						}
						// 单元格总是包含结束 | 的。
						TextSpan span = new(text.GetMappedIndex(spanStart), text.GetMappedIndex(i + 1));
						CellInfo info = new(span, texts);
						cells.Add(info.Cell);
						cellInfos.Add(info);
						spanStart = start = i + 1;
						texts = new List<MappedText>();
					}
					break;
			}
		}
		// 添加可能的最后一个单元格。
		if (texts.Count > 0 || spanStart < len)
		{
			if (start < len)
			{
				texts.Add(text[start..len]);
			}
			TextSpan span = new(text.GetMappedIndex(spanStart), text.GetMappedIndex(len));
			CellInfo info = new(span, texts);
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
		private readonly List<MappedText> text;

		/// <summary>
		/// 使用指定的文本范围和映射文本初始化 <see cref="CellInfo"/> 类的新实例。
		/// </summary>
		/// <param name="span">单元格的文本范围。</param>
		/// <param name="text">映射文本列表。</param>
		public CellInfo(TextSpan span, List<MappedText> text)
		{
			Cell = new TableCell(span);
			// 需要移除文本的前后空白。
			if (text.Count > 0)
			{
				text[0].TrimStart();
				text[^1].TrimEnd();
			}
			this.text = text;
		}

		/// <summary>
		/// 解析当前单元格。
		/// </summary>
		/// <param name="parser">行内节点的解析器。</param>
		public void ParseInline(InlineParser parser)
		{
			if (text.Count > 0)
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
		/// <param name="line">要检查的行。</param>
		/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
		/// <returns>如果能够开始当前块的解析，则返回解析器序列。否则返回空序列。</returns>
		public IEnumerable<BlockProcessor> TryStart(LineInfo line, BlockProcessor matchedProcessor)
		{
			// 要求分割行之前是段落，而且包含且只包含一行。
			IList<MappedText>? lines;
			if (line.IsCodeIndent || (lines = matchedProcessor.ParagraphLines) == null ||
				lines.Count != 1)
			{
				yield break;
			}
			MappedText heading = lines[0];
			List<TableAlign> aligns = ParseDelimiterRow(line.Peek().Text);
			// 标题行与分割行必须具有相同的单元格数。
			if (aligns.Count != CountCell(heading))
			{
				yield break;
			}
			// 需要将之前的段落关闭。
			matchedProcessor.NeedReplace();
			// 跳过当前行。
			line.Skip();
			yield return new TableProcessor(aligns, heading);
		}

		/// <summary>
		/// 返回指定行的单元格个数。
		/// </summary>
		/// <param name="text">要解析的文本。</param>
		/// <returns>单元格的个数。</returns>
		private static int CountCell(MappedText text)
		{
			int count = 0;
			ReadOnlySpan<char> str = text.ToString();
			MarkdownUtil.Trim(ref str);
			int start = 0;
			int len = str.Length;
			for (int i = 0; i < len; i++)
			{
				char ch = str[i];
				switch (ch)
				{
					case '\\':
						if (i + 1 < len)
						{
							i++;
						}
						break;
					case '|':
						if (i > 0)
						{
							// 首个 | 总是会当作前导竖划线看待，不计入内容。
							count++;
							start = i + 1;
						}
						break;
				}
			}
			// 添加可能的最后一个单元格。
			if (start < len)
			{
				count++;
			}
			return count;
		}

		/// <summary>
		/// 解析分割行。
		/// </summary>
		/// <param name="text">要解析的文本。</param>
		/// <returns>表格的对齐。</returns>
		private static List<TableAlign> ParseDelimiterRow(string text)
		{
			// 语法分析时已确保分割行的内容是有效的，这里直接 split 即可。
			// 忽略前后空白，忽略空的子字符串来忽略最外侧的竖划线。
			IEnumerable<string> cells = MarkdownUtil.Trim(text)
				.Split('|', StringSplitOptions.RemoveEmptyEntries)
				.Select(cell => MarkdownUtil.Trim(cell));
			List<TableAlign> columns = new();
			foreach (string str in cells)
			{
				TableAlign align = TableAlign.None;
				if (str.StartsWith(':'))
				{
					align = TableAlign.Left;
				}
				if (str.EndsWith(':'))
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
				columns.Add(align);
			}
			return columns;
		}
	}
}
