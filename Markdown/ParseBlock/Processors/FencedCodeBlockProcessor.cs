using System.Text;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 分隔符代码块的解析器。
/// </summary>
internal class FencedCodeBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 代码的文本。
	/// </summary>
	private readonly StringBuilder builder = StringBuilderPool.Rent(64);
	/// <summary>
	/// 代码块。
	/// </summary>
	private readonly CodeBlock code;
	/// <summary>
	/// 代码块的分割字符。
	/// </summary>
	private readonly char fence;
	/// <summary>
	/// 代码块的分割符长度。
	/// </summary>
	private readonly int fenceLength;
	/// <summary>
	/// 代码块的缩进。
	/// </summary>
	private readonly int indent;

	/// <summary>
	/// 初始化 <see cref="FencedCodeBlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="info">代码块的信息。</param>
	/// <param name="indent">代码块的缩进。</param>
	private FencedCodeBlockProcessor(BlockFenceInfo<CodeBlock> info, int indent)
		: base(MarkdownKind.CodeBlock)
	{
		fence = info.Fence;
		fenceLength = info.FenceLength;
		this.indent = indent;
		code = info.Node;
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		if (!line.IsCodeIndent)
		{
			Token<BlockKind> token = line.PeekFront();
			if (token.Kind == BlockKind.CodeFence && token.Text[0] == fence &&
				MarkdownUtil.GetFenceLength(token.Text) >= fenceLength)
			{
				return BlockContinue.Closed;
			}
		}
		// 允许跳过部分空白。
		line.SkipIndent(indent);
		return BlockContinue.Continue;
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="line">新添加的行。</param>
	public override void AddLine(BlockLine line)
	{
		line.AppendTo(builder);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		code.Span = code.Span with { End = end };
		code.Content = builder.ToString();
		StringBuilderPool.Return(builder);
		return code;
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
			// 跳过空白部分。
			int indent = line.Indent;
			line.SkipIndent();
			// 解析自定义容器的信息。
			Token<BlockKind> token = line.PeekFront();
			var codeFenceInfo = (token.Value as BlockFenceInfo<CodeBlock>)!;
			string? info = token.Text.AsSpan(codeFenceInfo.FenceLength).Trim(MarkdownUtil.Whitespace).Unescape();
			if (info.Length > 0)
			{
				codeFenceInfo.Node.Info = info;
			}
			codeFenceInfo.Node.Attributes.AddPrefix(parser.Options.AttributesPrefix);

			// 标记当前行已处理完毕。
			line.Skip();
			processors.Add(new FencedCodeBlockProcessor(codeFenceInfo, indent));
		}
	}
}
