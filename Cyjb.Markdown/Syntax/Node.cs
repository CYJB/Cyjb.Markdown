using System.Diagnostics;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的节点。
/// </summary>
[DebuggerDisplay("{ToString()}")]
public abstract class Node
{
	/// <summary>
	/// 节点的类型。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly MarkdownKind kind;

	/// <summary>
	/// 使用指定的节点类型初始化 <see cref="Node"/> 类的新实例。
	/// </summary>
	/// <param name="kind">节点的类型。</param>
	protected Node(MarkdownKind kind)
	{
		this.kind = kind;
	}

	/// <summary>
	/// 获取节点的类型。
	/// </summary>
	public MarkdownKind Kind => kind;
	/// <summary>
	/// 获取节点是否是 HTML 节点。
	/// </summary>
	public bool IsHtml => kind is MarkdownKind.HtmlBlock || kind.IsInlineHtml();
	/// <summary>
	/// 获取或设置节点的文本范围。
	/// </summary>
	public TextSpan Span { get; set; }
	/// <summary>
	/// 获取节点的行列位置范围。
	/// </summary>
	/// <value>节点的行列位置范围。</value>
	public LinePositionSpan LinePositionSpan => Locator.GetSpan(Span);

	/// <summary>
	/// 获取所属父节点。
	/// </summary>
	public Node? Parent { get; internal set; }
	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	public abstract Node? Prev { get; }
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	public abstract Node? Next { get; }
	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public virtual Node? FirstChild => null;
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public virtual Node? LastChild => null;
	/// <summary>
	/// 获取或设置关联到的行定位器。
	/// </summary>
	public LineLocator? Locator { get; set; }

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal abstract void SetPrev(Node? node);
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal abstract void SetNext(Node? node);

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public abstract void Accept(SyntaxVisitor visitor);

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public abstract TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor);

	/// <summary>
	/// 将当前节点从父节点中移除。
	/// </summary>
	public void Remove()
	{
		if (Parent == null)
		{
			return;
		}
		if (this is BlockNode block)
		{
			((INodeContainer<BlockNode>)Parent!).Children.Remove(block);
		}
		else if (this is InlineNode inline)
		{
			((INodeContainer<InlineNode>)Parent!).Children.Remove(inline);
		}
		else if (this is ListItem listItem)
		{
			((INodeContainer<ListItem>)Parent!).Children.Remove(listItem);
		}
		else if (this is TableRow tableRow)
		{
			((INodeContainer<TableRow>)Parent!).Children.Remove(tableRow);
		}
		else if (this is TableCell tableCell)
		{
			((INodeContainer<TableCell>)Parent!).Children.Remove(tableCell);
		}
		else
		{
			throw CommonExceptions.Unreachable();
		}
	}

	/// <summary>
	/// 重置与其它节点的连接。
	/// </summary>
	/// <param name="fixSibling">是否修复兄弟节点的链接。</param>
	internal void Unlink(bool fixSibling = true)
	{
		Parent = null;
		if (fixSibling)
		{
			Prev?.SetNext(Next);
			Next?.SetPrev(Prev);
		}
		SetPrev(null);
		SetNext(null);
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{{kind} {Span}}}";
	}
}
