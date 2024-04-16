namespace Cyjb.Markdown;

/// <summary>
/// 语法节点的遍历深度。
/// </summary>
public enum SyntaxWalkerDepth
{
	/// <summary>
	/// 仅遍历块节点。
	/// </summary>
	BlockNode,
	/// <summary>
	/// 遍历块节点和行内节点。
	/// </summary>
	InlineNode,
}
