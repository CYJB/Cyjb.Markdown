using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的链接。
/// </summary>
public sealed class Link : InlineNode, INodeContainer<InlineNode>
{
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<InlineNode> children;
	/// <summary>
	/// 链接的 URL。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string url;
	/// <summary>
	/// 链接的标题。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? title;
	/// <summary>
	/// 链接的定义。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private LinkDefinition? definition;
	/// <summary>
	/// 链接的属性。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly HtmlAttributeList attributes = new();

	/// <summary>
	/// 使用指定的 URL、标题和文本范围初始化 <see cref="Link"/> 类的新实例。
	/// </summary>
	/// <param name="isImage">是否是图片。</param>
	/// <param name="url">链接的 URL。</param>
	/// <param name="title">链接的标题。</param>
	/// <param name="span">文本的范围。</param>
	public Link(bool isImage, string url, string? title = null, TextSpan span = default)
		: this(isImage ? MarkdownKind.Image : MarkdownKind.Link, url, title, span)
	{ }

	/// <summary>
	/// 使用指定的链接定义和文本范围初始化 <see cref="Link"/> 类的新实例。
	/// </summary>
	/// <param name="isImage">是否是图片。</param>
	/// <param name="definition">链接的定义。</param>
	/// <param name="span">文本的范围。</param>
	/// <exception cref="ArgumentNullException"><paramref name="definition"/> 为 <c>null</c>。</exception>
	public Link(bool isImage, LinkDefinition definition, TextSpan span = default)
		: base(isImage ? MarkdownKind.Image : MarkdownKind.Link)
	{
		ArgumentNullException.ThrowIfNull(definition);
		children = new NodeList<InlineNode>(this);
		this.definition = definition;
		url = definition.URL;
		Span = span;
	}

	/// <summary>
	/// 使用指定的类型、URL、标题和文本范围初始化 <see cref="Link"/> 类的新实例。
	/// </summary>
	/// <param name="kind">链接的类型。</param>
	/// <param name="url">链接的 URL。</param>
	/// <param name="title">链接的标题。</param>
	/// <param name="span">文本的范围。</param>
	private Link(MarkdownKind kind, string url, string? title, TextSpan span) : base(kind)
	{
		children = new NodeList<InlineNode>(this);
		this.url = url;
		this.title = title;
		Span = span;
	}

	/// <summary>
	/// 获取或设置链接的 URL。
	/// </summary>
	public string URL
	{
		get => definition?.URL ?? url;
		set
		{
			TransferDefinition();
			url = value ?? string.Empty;
		}
	}
	/// <summary>
	/// 获取或设置链接的标题。
	/// </summary>
	public string? Title
	{
		get => definition?.Title ?? title;
		set
		{
			TransferDefinition();
			title = value;
		}
	}
	/// <summary>
	/// 获取或设置链接的定义。
	/// </summary>
	public LinkDefinition? Definition
	{
		get => definition;
		set
		{
			if (value == null)
			{
				TransferDefinition();
			}
			definition = value;
		}
	}
	/// <summary>
	/// 获取链接的属性列表。
	/// </summary>
	public HtmlAttributeList Attributes => definition?.Attributes ?? attributes;
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
	/// 将链接定义的信息复制到当前链接，并清除关联到的链接定义。
	/// </summary>
	private void TransferDefinition()
	{
		if (definition == null)
		{
			return;
		}
		url = definition.URL;
		title = definition.Title;
		attributes.Clear();
		attributes.AddRange(definition.Attributes);
		definition = null;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitLink(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitLink(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		Link node = new(Kind, url, title, Span)
		{
			definition = definition?.CloneNode(deep, context) as LinkDefinition,
			Locator = Locator,
		};
		attributes.CloneTo(node.attributes);
		children.CloneTo(node.children, context);
		return node;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{{Kind} \"{URL}\" {Span}}}";
	}
}
