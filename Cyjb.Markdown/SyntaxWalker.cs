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
		if (node is INodeContainer<BlockNode> blockContainer)
		{
			NodeList<BlockNode> nodes = blockContainer.Children;
			int count = nodes.Count;
			for (int i = 0; i < count; i++)
			{
				nodes[i].Accept(this);
			}
		}
		else if (node is INodeContainer<InlineNode> inlineContainer)
		{
			NodeList<InlineNode> nodes = inlineContainer.Children;
			int count = nodes.Count;
			for (int i = 0; i < count; i++)
			{
				nodes[i].Accept(this);
			}
		}
		else if (node is INodeContainer<ListItem> listContainer)
		{
			NodeList<ListItem> nodes = listContainer.Children;
			int count = nodes.Count;
			for (int i = 0; i < count; i++)
			{
				nodes[i].Accept(this);
			}
		}
	}

}
