using System.Runtime.CompilerServices;
using Cyjb.Markdown.ParseInline;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// ATX 标题的解析器。
/// </summary>
internal sealed class ATXHeadingProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// ATX 标题节点。
	/// </summary>
	private readonly Heading heading;

	/// <summary>
	/// ATX 标题的文本。
	/// </summary>
	private readonly BlockText text;

	/// <summary>
	/// 使用 ATX 标题和文本初始化 <see cref="ATXHeadingProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="heading">ATX 标题。</param>
	/// <param name="text">ATX 标题的文本。</param>
	private ATXHeadingProcessor(Heading heading, BlockText text)
		: base(MarkdownKind.Heading)
	{
		this.heading = heading;
		this.text = text;
	}

	/// <summary>
	/// 获取当前块是否需要解析行内节点。
	/// </summary>
	public override bool NeedParseInlines => text.Length > 0;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		return BlockContinue.None;
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		HeadingUtils.ProcessHeading(parser, heading, LinkUtil.NormalizeLabel(text));
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
		/// <param name="parser">块级语法分析器。</param>
		/// <param name="line">要检查的行。</param>
		/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
		/// <param name="processors">要添加到的处理器列表。</param>
		public void TryStart(BlockParser parser, BlockLine line, BlockProcessor matchedProcessor, List<BlockProcessor> processors)
		{
			if (line.IsCodeIndent)
			{
				return;
			}
			line.SkipIndent();
			var token = line.PeekFront();
			Heading heading = (token.Value as Heading)!;
			// 跳过标题的 # 符号。
			line.RemoveStart(heading.Depth);
			// 忽略内容前后的空白
			line.TrimStart();
			line.TrimEnd();
			// 检查闭合 #
			if (line.Tokens.Count > 0)
			{
				TrimEndingSharp(line);
			}
			processors.Add(new ATXHeadingProcessor(heading, line.ToBlockText()));
		}

		/// <summary>
		/// 移除标题的结束 # 符号。
		/// </summary>
		/// <param name="line">标题的文本。</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TrimEndingSharp(BlockLine line)
		{
			ReadOnlySpan<char> span = line.PeekBack().Text;
			int sharpCount = span.Length - span.TrimEnd('#').Length;
			if (sharpCount == 0)
			{
				return;
			}
			if (sharpCount == span.Length)
			{
				// 最后一个 Token 被全部消费。
				line.PopBack();
				line.TrimEnd();
			}
			else if (MarkdownUtil.IsWhitespace(span[span.Length - sharpCount - 1]))
			{
				// 要求闭合 # 前包含空格或 Tab。
				line.RemoveEnd(sharpCount);
				line.TrimEnd();
			}
		}
	}
}
