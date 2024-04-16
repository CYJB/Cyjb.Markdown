using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的自定义容器。
/// </summary>
public sealed class CustomContainer : BlockNode, INodeContainer<BlockNode>
{
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<BlockNode> children;
	/// <summary>
	/// 自定义容器的信息。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? info;
	/// <summary>
	/// 自定义容器的属性。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HtmlAttributeList? attributes;

	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="CustomContainer"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public CustomContainer(TextSpan span = default) : base(MarkdownKind.CustomContainer)
	{
		children = new NodeList<BlockNode>(this);
		Span = span;
	}

	/// <summary>
	/// 获取或设置自定义容器的信息。
	/// </summary>
	public string? Info
	{
		get => info;
		set
		{
			if (value.IsNullOrWhiteSpace())
			{
				info = null;
			}
			else
			{
				info = value.Trim();
			}
		}
	}

	/// <summary>
	/// 获取是否包含属性。
	/// </summary>
	[MemberNotNullWhen(true, nameof(attributes))]
	public bool HasAttribute => attributes?.Count > 0;

	/// <summary>
	/// 获取自定义容器的属性列表。
	/// </summary>
	public HtmlAttributeList Attributes => attributes ??= new HtmlAttributeList();

	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<BlockNode> Children => children;

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? LastChild => children.LastOrDefault();

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitCustomContainer(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitCustomContainer(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		CustomContainer node = new(Span)
		{
			info = info,
			Locator = Locator,
		};
		if (attributes != null && attributes.Count > 0)
		{
			attributes.CloneTo(node.Attributes);
		}
		if (deep)
		{
			children.CloneTo(node.children, context);
		}
		return node;
	}
}
