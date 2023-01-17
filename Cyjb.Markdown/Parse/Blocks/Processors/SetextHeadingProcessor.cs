using Cyjb.Markdown.Parse.Inlines;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// Setext 标题的解析器。
/// </summary>
internal sealed class SetextHeadingProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// Setext 标题节点。
	/// </summary>
	private readonly Heading heading;

	/// <summary>
	/// Setext 标题的文本。
	/// </summary>
	private readonly IList<MappedText> text;

	/// <summary>
	/// 使用Setext 标题的起始位置和文本初始化 <see cref="SetextHeadingProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">Setext 标题的起始位置。</param>
	/// <param name="depth">Setext 标题的深度。</param>
	/// <param name="text">Setext 标题的文本。</param>
	private SetextHeadingProcessor(int start, int depth, IList<MappedText> text) : base(MarkdownKind.Heading)
	{
		heading = new Heading(depth, new TextSpan(start, start));
		this.text = text;
	}

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
		return BlockContinue.None;
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		heading.Span = new TextSpan(heading.Span.Start, end);
		return heading;
	}

	/// <summary>
	/// 解析行内节点。
	/// </summary>
	/// <param name="parser">行内节点的解析器。</param>
	public override void ParseInline(InlineParser parser)
	{
		parser.Parse(text, heading.Children);
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
			// 要求 Setext 标签之前是段落，而且包含有效内容。
			IList<MappedText>? lines;
			if (line.IsCodeIndent || (lines = matchedProcessor.ParagraphLines) == null ||
				lines.Count == 0)
			{
				yield break;
			}
			// 需要将之前的段落关闭。
			matchedProcessor.NeedReplace();
			int depth = line.Peek().Text[0] == '=' ? 1 : 2;
			// 移除尾行后的空白。
			lines[^1].TrimEnd();
			yield return new SetextHeadingProcessor(lines[0].Span.Start, depth, lines);
		}
	}
}
