using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 块引用的解析器。
/// </summary>
internal sealed class BlockquoteProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 块引用节点。
	/// </summary>
	private readonly Blockquote blockquote;

	/// <summary>
	/// 使用块引用的起始位置初始化 <see cref="BlockquoteProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">块引用的起始位置。</param>
	private BlockquoteProcessor(int start) : base(MarkdownKind.Blockquote)
	{
		blockquote = new Blockquote(new TextSpan(start, start));
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
	public override BlockContinue TryContinue(BlockLine line)
	{
		return CheckQuoteStart(line) ? BlockContinue.Continue : BlockContinue.None;
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
		blockquote.Children.Add((BlockNode)node);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		blockquote.Span = blockquote.Span with
		{
			End = end,
		};
		return blockquote;
	}

	/// <summary>
	/// 检查块引用起始标记。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>如果找到了块引用起始标记，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	private static bool CheckQuoteStart(BlockLine line)
	{
		if (line.IsCodeIndent || line.PeekFront().Kind != BlockKind.QuoteStart)
		{
			return false;
		}
		else
		{
			line.PopFront();
			// > 之后允许跳过一个空格。
			line.SkipIndent(1);
			return true;
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
		/// <param name="processors">要添加到的处理器列表。</param>
		public void TryStart(BlockParser parser, BlockLine line, BlockProcessor matchedProcessor, List<BlockProcessor> processors)
		{
			int start = line.PeekFront().Span.Start;
			if (CheckQuoteStart(line))
			{
				processors.Add(new BlockquoteProcessor(start));
			}
		}
	}
}
