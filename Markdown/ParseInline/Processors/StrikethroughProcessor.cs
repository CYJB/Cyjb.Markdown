using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 删除线分隔符的处理器。
/// </summary>
internal sealed class StrikethroughProcessor : DelimiterProcessor
{
	/// <summary>
	/// 初始化 <see cref="StrikethroughProcessor"/> 类的新实例。
	/// </summary>
	public StrikethroughProcessor() : base(1, 2) { }

	/// <summary>
	/// 处理指定的分隔符对。
	/// </summary>
	/// <param name="opening">起始分隔符。</param>
	/// <param name="closing">结束分隔符。</param>
	/// <returns>容器节点，如果分隔符不匹配则返回 <c>null</c>。</returns>
	public override INodeContainer<InlineNode>? Process(DelimiterInfo opening, DelimiterInfo closing)
	{
		if (opening.Length == closing.Length)
		{
			// 将分隔符消费掉。
			opening.Length = 0;
			closing.Length = 0;
			TextSpan span = TextSpan.Combine(opening.Node.Span, closing.Node.Span);
			return new Strikethrough(span);
		}
		return null;
	}
}
