using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 列表的解析器。
/// </summary>
internal sealed class ListProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();
	/// <summary>
	/// 小写的罗马数字字符。
	/// </summary>
	private static readonly HashSet<char> LowerRomain = new("ivxlcdm");
	/// <summary>
	/// 大写的罗马数字字符。
	/// </summary>
	private static readonly HashSet<char> UpperRomain = new("IVXLCDM");
	/// <summary>
	/// 罗马数字的字符映射。
	/// </summary>
	private static readonly Dictionary<char, int> RomanMap = new() {
		{ 'i', 1 }, { 'v', 5 }, { 'x', 10 }, { 'l', 50 }, { 'c', 100 }, { 'd', 500 }, { 'm', 1000 },
		{ 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 },
	};

	/// <summary>
	/// 列表的标记字符。
	/// </summary>
	private readonly char marker;
	/// <summary>
	/// 列表节点。
	/// </summary>
	private readonly List list;
	/// <summary>
	/// 列表项间是否包含了空行。
	/// </summary>
	private bool hadBlankLine;
	/// <summary>
	/// 空行的个数。
	/// </summary>
	private int blankLines;

	/// <summary>
	/// 使用列表的标记字符和节点初始化 <see cref="ListProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="marker">列表的标记字符。</param>
	/// <param name="list">列表节点。</param>
	private ListProcessor(char marker, List list) : base(MarkdownKind.List)
	{
		this.marker = marker;
		this.list = list;
	}

	/// <summary>
	/// 获取是否是容器节点。
	/// </summary>
	public override bool IsContainer => true;

	/// <summary>
	/// 将列表标记为松散的。
	/// </summary>
	public void MarkLoose()
	{
		list.Loose = true;
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		if (line.IsBlank)
		{
			hadBlankLine = true;
			blankLines = 0;
		}
		else if (hadBlankLine)
		{
			blankLines++;
		}
		return BlockContinue.Continue;
	}

	/// <summary>
	/// 返回当前节点是否可以包含指定类型的子节点。
	/// </summary>
	/// <param name="kind">要检查的节点类型。</param>
	/// <returns>如果当前节点可以包含指定类型的子节点，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool CanContains(MarkdownKind kind)
	{
		if (kind == MarkdownKind.ListItem)
		{
			if (hadBlankLine && blankLines == 1)
			{
				MarkLoose();
				hadBlankLine = false;
			}
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 添加一个新节点。
	/// </summary>
	/// <param name="node">要添加的节点。</param>
	public override void AddNode(BlockNode node)
	{
		list.Children.Add((ListItem)node);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		// 使用最后一个列表项的位置。
		list.Span = list.Span with
		{
			End = list.Children[^1].Span.End,
		};
		return list;
	}

	/// <summary>
	/// 返回当前列表是否可以继续包含指定列表标志。
	/// </summary>
	/// <param name="token">要检查的列表标志。</param>
	/// <returns>如果当前列表是否可以继续包含指定列表标志，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	private bool CanContinue(Token<BlockKind> token)
	{
		// 标志的分隔符不同，不能继续包含。
		if (marker != token.Text[^1])
		{
			return false;
		}
		ListStyleType type = (ListStyleType)token.Value!;
		// 类型相同，可以继续。
		if (list.StyleType == type)
		{
			return true;
		}
		// 特殊情况：Roman 可以允许继续包含值为 [ivxlcdm] 的英文字母列表。
		return list.StyleType switch
		{
			ListStyleType.OrderedLowerRoman => type == ListStyleType.OrderedLowerAlpha &&
				LowerRomain.Contains(token.Text[0]),
			ListStyleType.OrderedUpperRoman => type == ListStyleType.OrderedUpperAlpha &&
				UpperRomain.Contains(token.Text[0]),
			_ => false,
		};
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
			if (line.IsCodeIndent)
			{
				yield break;
			}
			Token<BlockKind> token = line.Peek();
			ListStyleType styleType = (ListStyleType)token.Value!;
			bool hasContent = line.Tokens.Skip(1).Any(LineInfo.HasContent);
			if (matchedProcessor.ParagraphLines?.Count > 0)
			{
				// 空列表不能中断段落。
				if (!hasContent)
				{
					yield break;
				}
				// 如果是有序列表，只有起始数字为 1 时才能中断段落。
				if (styleType != ListStyleType.Unordered && ParseMarkerNumber(ref styleType, token.Text) != 1)
				{
					yield break;
				}
			}
			int itemStart = token.Span.Start;
			// 找到内容相对列表项起始的缩进宽度。
			int indentAfterMarker = line.Indent + token.Text.Length;
			line.Read();
			int contentIndent = indentAfterMarker + line.Indent;
			// 如果没有内容或者是代码段，那么认为内容缩进是列表项后一个字符位置。
			if (!hasContent || contentIndent - indentAfterMarker > LineInfo.CodeIndent)
			{
				contentIndent = indentAfterMarker + 1;
				// 只跳过 marker 后的一个空白。
				line.SkipIndent(1);
			}
			else
			{
				// 跳过所有空白。
				line.SkipIndent();
			}
			// 在需要的时候插入列表处理器。
			if (matchedProcessor is not ListProcessor listProcessor || !listProcessor.CanContinue(token))
			{
				int start = 1;
				if (styleType != ListStyleType.Unordered)
				{
					// 注意这里可能会根据需要调整列表的类型。
					start = ParseMarkerNumber(ref styleType, token.Text);
				}
				List list = new(styleType, new TextSpan(itemStart, itemStart))
				{
					Start = start
				};
				listProcessor = new ListProcessor(token.Text[^1], list);
				yield return listProcessor;
			}
			ListItemProcessor itemProcessor = new(listProcessor, itemStart, contentIndent);
			// 检查任务列表项。
			if (line.Options.UseTaskListItem)
			{
				itemProcessor.Checked = CheckTaskListItem(line);
			}
			yield return itemProcessor;
		}

		/// <summary>
		/// 解析列表项的序号。
		/// </summary>
		/// <param name="styleType">列表类型。</param>
		/// <param name="text">要解析的文本。</param>
		/// <returns>解析得到的序号。</returns>
		private static int ParseMarkerNumber(ref ListStyleType styleType, ReadOnlySpan<char> text)
		{
			switch (styleType)
			{
				case ListStyleType.OrderedNumber:
					// 排除最后的 . 或 )。
					return int.Parse(text[..^1]);
				case ListStyleType.OrderedLowerAlpha:
					if (text[0] == 'i')
					{
						// 特例：i 会被解析为罗马数字。
						styleType = ListStyleType.OrderedLowerRoman;
						return 1;
					}
					return text[0] - 'a' + 1;
				case ListStyleType.OrderedUpperAlpha:
					if (text[0] == 'I')
					{
						// 特例：i 会被解析为罗马数字。
						styleType = ListStyleType.OrderedUpperRoman;
						return 1;
					}
					return text[0] - 'A' + 1;
				case ListStyleType.OrderedLowerRoman:
				case ListStyleType.OrderedUpperRoman:
					// 排除最后的 . 或 )。
					return ParseRomain(text[..^1]);
				case ListStyleType.OrderedLowerGreek:
					return text[0] - 'α' + 1;
				default: throw CommonExceptions.Unreachable();
			};
		}

		/// <summary>
		/// 解析罗马数字。
		/// </summary>
		/// <param name="text">要解析的文本。</param>
		/// <returns>解析得到的需要。</returns>
		private static int ParseRomain(ReadOnlySpan<char> text)
		{
			int result = 0;
			for (int i = 0; i < text.Length; i++)
			{
				int value = RomanMap[text[i]];
				if ((uint)(i + 1) < text.Length && value < RomanMap[text[i + 1]])
				{
					result -= value;
				}
				else
				{
					result += value;
				}
			}
			return result;
		}

		/// <summary>
		/// 检查当前行是否包含任务列表项。
		/// </summary>
		/// <param name="line">要检查的行。</param>
		/// <returns>如果当前行包含任务列表项，根据是否勾选返回 <c>true</c> 或 <c>false</c>；
		/// 如果不包含任务列表项，返回 <c>null</c>。</returns>
		private static bool? CheckTaskListItem(LineInfo line)
		{
			// 检查包含任务列表项标志
			if (line.Indent >= 4 || line.IsBlank)
			{
				return null;
			}
			Token<BlockKind> token = line.Peek();
			if (token.Kind != BlockKind.TaskListItemMarker)
			{
				return null;
			}
			// 检查任务列表项宽度为 3（处理方括号内为 Tab，但宽度为 3 的场景）。
			LinePositionSpan span = token.LinePositionSpan;
			if (span.End.Column - span.Start.Column != 3)
			{
				return null;
			}
			// 右方括号后面至少包含一个空白，也包含内容。
			if (line.Tokens.Count <= 2 || line.Tokens[1].Kind != BlockKind.Indent ||
				!line.Tokens.Skip(2).Any(LineInfo.HasContent))
			{
				return null;
			}
			// 消费掉任务列表项标志。
			line.Read();
			// 消费掉任务列表项标志后的一个空白。
			line.SkipIndent(1);
			// 后续的段落不要跳过这里的空白。
			line.ParagraphSkippable = false;
			return token.Text[1] is 'x' or 'X';
		}
	}
}
