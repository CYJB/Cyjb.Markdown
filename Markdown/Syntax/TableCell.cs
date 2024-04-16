using Cyjb.Text;
using System.Diagnostics;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 表格的单元格。
/// </summary>
public sealed class TableCell : Node, INodeContainer<InlineNode>
{
	/// <summary>
	/// 前驱兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TableCell? prev;
	/// <summary>
	/// 后继兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TableCell? next;
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<InlineNode> children;

	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="TableCell"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public TableCell(TextSpan span = default) : base(MarkdownKind.TableCell)
	{
		children = new NodeList<InlineNode>(this);
		Span = span;
	}

	/// <summary>
	/// 获取所属行。
	/// </summary>
	public new TableRow? Parent => base.Parent as TableRow;
	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<InlineNode> Children => children;
	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	public override TableCell? Prev => prev;
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	public override TableCell? Next => next;

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override InlineNode? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override InlineNode? LastChild => children.LastOrDefault();

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetPrev(Node? node)
	{
		prev = node as TableCell;
	}
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetNext(Node? node)
	{
		next = node as TableCell;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitTableCell(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitTableCell(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		TableCell node = new(Span)
		{
			Locator = Locator,
		};
		if (deep)
		{
			children.CloneTo(node.children, context);
		}
		return node;
	}
}
