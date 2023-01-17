namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的行内节点。
/// </summary>
public abstract class InlineNode : Node
{
	/// <summary>
	/// 前驱兄弟节点。
	/// </summary>
	private InlineNode? prev;
	/// <summary>
	/// 后继兄弟节点。
	/// </summary>
	private InlineNode? next;

	/// <summary>
	/// 使用指定的节点类型初始化 <see cref="InlineNode"/> 类的新实例。
	/// </summary>
	/// <param name="kind">节点的类型。</param>
	protected InlineNode(MarkdownKind kind) : base(kind) { }

	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	public override InlineNode? Prev => prev;
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	public override InlineNode? Next => next;

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetPrev(Node? node)
	{
		prev = node as InlineNode;
	}
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetNext(Node? node)
	{
		next = node as InlineNode;
	}
}
