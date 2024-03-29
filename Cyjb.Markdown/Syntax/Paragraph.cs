using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的段落。
/// </summary>
public sealed class Paragraph : BlockNode, INodeContainer<InlineNode>
{
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<InlineNode> children;

	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="Paragraph"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public Paragraph(TextSpan span = default) : base(MarkdownKind.Paragraph)
	{
		children = new NodeList<InlineNode>(this);
		Span = span;
	}

	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<InlineNode> Children => children;

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override InlineNode? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override InlineNode? LastChild => children.LastOrDefault();

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitParagraph(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitParagraph(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		Paragraph node = new(Span)
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
