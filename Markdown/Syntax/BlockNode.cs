using System.Diagnostics;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的块节点。
/// </summary>
public abstract class BlockNode : Node
{
	/// <summary>
	/// 前驱兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BlockNode? prev;
	/// <summary>
	/// 后继兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BlockNode? next;

	/// <summary>
	/// 使用指定的节点类型初始化 <see cref="BlockNode"/> 类的新实例。
	/// </summary>
	/// <param name="kind">节点的类型。</param>
	protected BlockNode(MarkdownKind kind) : base(kind) { }

	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	public override BlockNode? Prev => prev;
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	public override BlockNode? Next => next;

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetPrev(Node? node)
	{
		prev = node as BlockNode;
	}
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetNext(Node? node)
	{
		next = node as BlockNode;
	}
}
