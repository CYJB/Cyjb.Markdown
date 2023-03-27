using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 自定义容器的解析器。
/// </summary>
internal class CustomContainerProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 自定义容器节点。
	/// </summary>
	private readonly CustomContainer container;
	/// <summary>
	/// 代码块的分割符长度。
	/// </summary>
	private readonly int fenceLength;

	/// <summary>
	/// 初始化 <see cref="CustomContainerProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">代码块的起始索引。</param>
	/// <param name="fenceLength">代码块的分隔符长度。</param>
	/// <param name="info">代码块的信息。</param>
	/// <param name="attrs">代码块的属性。</param>
	private CustomContainerProcessor(int start, int fenceLength, string? info, HtmlAttributeList? attrs)
		: base(MarkdownKind.CodeBlock)
	{
		this.fenceLength = fenceLength;
		container = new CustomContainer(new TextSpan(start, start))
		{
			Info = info
		};
		if (attrs != null)
		{
			container.Attributes.AddRange(attrs);
		}
	}


	/// <summary>
	/// 获取是否是容器节点。
	/// </summary>
	public override bool IsContainer => true;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		if (!line.IsCodeIndent)
		{
			Token<BlockKind> token = line.Peek();
			if (token.Kind == BlockKind.CustomContainerFence &&
				MarkdownUtil.GetFenceLength(token.Text) >= fenceLength)
			{
				return BlockContinue.Closed;
			}
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
		return true;
	}

	/// <summary>
	/// 添加一个新节点。
	/// </summary>
	/// <param name="node">要添加的节点。</param>
	public override void AddNode(Node node)
	{
		container.Children.Add((BlockNode)node);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		container.Span = container.Span with { End = end };
		return container;
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
			MarkdownUtil.ParseFenceStart(line, out int start, out int _,
				out char _, out int fenceLength, out string? info, out HtmlAttributeList? attrs);
			yield return new CustomContainerProcessor(start, fenceLength, info, attrs);
		}
	}
}
