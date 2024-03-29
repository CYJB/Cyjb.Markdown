using System.Text;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// HTML 块的解析器。
/// </summary>
internal class HtmlBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// HTML 文本。
	/// </summary>
	private readonly StringBuilder builder = StringBuilderPool.Rent(16);
	/// <summary>
	/// 块的起始位置。
	/// </summary>
	private readonly int start;
	/// <summary>
	/// HTML 的信息。
	/// </summary>
	private readonly HtmlInfo info;
	/// <summary>
	/// 是否已完成 HTML 块的识别。
	/// </summary>
	private bool finished = false;

	/// <summary>
	/// 初始化 <see cref="HtmlBlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">块的起始索引。</param>
	/// <param name="info">HTML 的信息。</param>
	private HtmlBlockProcessor(int start, HtmlInfo info)
		: base(MarkdownKind.CodeBlock)
	{
		this.start = start;
		this.info = info;
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		if (finished)
		{
			return BlockContinue.None;
		}
		if (line.IsBlank() && info.CloseByBlankLine)
		{
			// 遇到空行结束。
			return BlockContinue.None;
		}
		else
		{
			return BlockContinue.Continue;
		}
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="line">新添加的行。</param>
	public override void AddLine(BlockLine line)
	{
		int start = builder.Length;
		line.AppendTo(builder);
		finished = info.IsClosed(builder, start);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		HtmlBlock node = new(builder.ToString(), new TextSpan(start, end));
		StringBuilderPool.Return(builder);
		return node;
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
			HtmlInfo info = (HtmlInfo)line.PeekFront().Value!;
			if (!info.CanInterruptParagraph && (parser.ActivatedProcessor.Kind == MarkdownKind.Paragraph ||
				parser.ActivatedProcessor.CanLazyContinuation))
			{
				// 不中断段落。
				return;
			}
			// HTML 块的起始位置包含前面的空白。
			processors.Add(new HtmlBlockProcessor(line.Start, info));
		}
	}
}
