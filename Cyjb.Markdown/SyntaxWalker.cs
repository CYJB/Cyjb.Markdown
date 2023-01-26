using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown;

/// <summary>
/// 深度优先遍历所有 Markdown 语法节点的访问器。
/// </summary>
public abstract class SyntaxWalker : SyntaxVisitor
{
	/// <summary>
	/// 提供默认的访问行为。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	public override void DefaultVisit(Node node)
	{
		IReadOnlyList<Node> nodes;
		if (node is INodeContainer<BlockNode> blockContainer)
		{
			nodes = blockContainer.Children;
		}
		else if (node is INodeContainer<InlineNode> inlineContainer)
		{
			nodes = inlineContainer.Children;
		}
		else if (node is INodeContainer<ListItem> listContainer)
		{
			nodes = listContainer.Children;
		}
		else if (node is INodeContainer<TableRow> tableRowContainer)
		{
			nodes = tableRowContainer.Children;
		}
		else if (node is INodeContainer<TableCell> tableCellContainer)
		{
			nodes = tableCellContainer.Children;
		}
		else
		{
			return;
		}
		int count = nodes.Count;
		for (int i = 0; i < count; i++)
		{
			nodes[i].Accept(this);
		}
	}
}
