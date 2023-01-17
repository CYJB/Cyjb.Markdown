using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 引用块的解析器。
/// </summary>
internal sealed class QuoteProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 引用块节点。
	/// </summary>
	private readonly Quote quote;

	/// <summary>
	/// 使用引用块的起始位置初始化 <see cref="QuoteProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">引用块的起始位置。</param>
	private QuoteProcessor(int start) : base(MarkdownKind.Quote)
	{
		quote = new Quote(new TextSpan(start, start));
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
	public override void AddNode(BlockNode node)
	{
		quote.Children.Add(node);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		quote.Span = quote.Span with
		{
			End = end,
		};
		return quote;
	}

	/// <summary>
	/// 检查引用起始标记。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>如果找到了引用起始标记，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	private static bool CheckQuoteStart(LineInfo line)
	{
		if (line.IsCodeIndent || line.Peek().Kind != BlockKind.QuoteStart)
		{
			return false;
		}
		else
		{
			line.Read();
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
		/// <param name="line">要检查的行。</param>
		/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
		/// <returns>如果能够开始当前块的解析，则返回解析器序列。否则返回空序列。</returns>
		public IEnumerable<BlockProcessor> TryStart(LineInfo line, BlockProcessor matchedProcessor)
		{
			int start = line.Peek().Span.Start;
			if (CheckQuoteStart(line))
			{
				yield return new QuoteProcessor(start);
			}
		}
	}
}
