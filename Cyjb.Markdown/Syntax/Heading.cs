using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的标题。
/// </summary>
public sealed class Heading : BlockNode, INodeContainer<InlineNode>
{
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<InlineNode> children;
	/// <summary>
	/// 标题的深度。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int depth;
	/// <summary>
	/// 标题的属性。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly HtmlAttributeList attributes = new();

	/// <summary>
	/// 使用指定的深度和文本范围初始化 <see cref="Heading"/> 类的新实例。
	/// </summary>
	/// <param name="depth">标题的深度。</param>
	/// <param name="span">标题的文本范围。</param>
	public Heading(int depth, TextSpan span = default) : base(MarkdownKind.Heading)
	{
		children = new NodeList<InlineNode>(this);
		Depth = depth;
		Span = span;
	}

	/// <summary>
	/// 获取或设置标题的深度（1-6）。
	/// </summary>
	public int Depth
	{
		get => depth;
		set
		{
			if (value < 1 || value > 6)
			{
				throw CommonExceptions.ArgumentOutOfRange(value, 1, 6);
			}
			depth = value;
		}
	}

	/// <summary>
	/// 获取标题的属性列表。
	/// </summary>
	public HtmlAttributeList Attributes => attributes;

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
		visitor.VisitHeading(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitHeading(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		Heading node = new(depth, Span)
		{
			Locator = Locator,
		};
		attributes.CloneTo(node.attributes);
		if (deep)
		{
			children.CloneTo(node.children, context);
		}
		return node;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{Heading #{Depth} {Span}}}";
	}
}
