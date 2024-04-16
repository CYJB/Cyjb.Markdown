namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的容器节点。
/// </summary>
/// <typeparam name="TNode">子节点类型。</typeparam>
public interface INodeContainer<TNode>
	where TNode : Node
{
	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	NodeList<TNode> Children { get; }

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	TNode? FirstChild { get; }
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	TNode? LastChild { get; }
}
