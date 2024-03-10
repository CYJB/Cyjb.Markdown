namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 节点复制的上下文。
/// </summary>
internal sealed class NodeCloneContext
{
	/// <summary>
	/// 已复制的链接定义。
	/// </summary>
	public readonly Dictionary<LinkDefinition, LinkDefinition> LinkDefinitions = new();

	/// <summary>
	/// 已复制的脚注。
	/// </summary>
	public readonly Dictionary<Footnote, Footnote> Footnotes = new();
}
