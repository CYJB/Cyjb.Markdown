using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的列表项。
/// </summary>
public sealed class ListItem : Node, INodeContainer<BlockNode>
{
	/// <summary>
	/// 前驱兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ListItem? prev;
	/// <summary>
	/// 后继兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ListItem? next;
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<BlockNode> children;

	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="ListItem"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public ListItem(TextSpan span = default) : base(MarkdownKind.ListItem)
	{
		children = new NodeList<BlockNode>(this);
		Span = span;
	}

	/// <summary>
	/// 获取所属列表。
	/// </summary>
	public new List? Parent => base.Parent as List;
	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<BlockNode> Children => children;
	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	public override ListItem? Prev => prev;
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	public override ListItem? Next => next;

	/// <summary>
	/// 获取或设置列表项的选中状态。
	/// </summary>
	/// <value>使用 <c>null</c> 表示不能选中（普通列表项）；<c>true</c> 表示已选中，
	/// <c>false</c> 表示未选中。</value>
	public bool? Checked { get; set; }

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? LastChild => children.LastOrDefault();

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetPrev(Node? node)
	{
		prev = node as ListItem;
	}
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetNext(Node? node)
	{
		next = node as ListItem;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitListItem(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitListItem(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		ListItem node = new(Span)
		{
			Checked = Checked,
			Locator = Locator,
		};
		if (deep)
		{
			children.CloneTo(node.children, context);
		}
		return node;
	}
}
