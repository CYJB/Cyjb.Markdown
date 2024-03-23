using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的链接定义。
/// </summary>
public sealed class LinkDefinition : BlockNode
{
	/// <summary>
	/// 定义的标签。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string label;
	/// <summary>
	/// 定义的 URL。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string url;
	/// <summary>
	/// 定义的 ID。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? identifier;
	/// <summary>
	/// 定义的属性。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal HtmlAttributeList? attributes;

	/// <summary>
	/// 使用指定的链接定义信息和文本范围初始化 <see cref="LinkDefinition"/> 类的新实例。
	/// </summary>
	/// <param name="label">定义的标签。</param>
	/// <param name="url">定义的 URL。</param>
	/// <param name="title">定义的标题。</param>
	/// <param name="span">定义的文本范围。</param>
	/// <exception cref="ArgumentNullException"><paramref name="label"/> 或 <paramref name="url"/>
	/// 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="label"/> 为空字符串或只包含空白字符。</exception>
	public LinkDefinition(string label, string url, string? title = null, TextSpan span = default)
		: base(MarkdownKind.LinkDefinition)
	{
		LinkUtil.CheckLabel(label);
		this.label = label;
		this.url = url ?? string.Empty;
		Title = title;
		Span = span;
	}

	/// <summary>
	/// 获取或设置链接定义的标签。
	/// </summary>
	/// <exception cref="ArgumentNullException">标签为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException">标签为空字符串或只包含空白字符。</exception>
	public string Label
	{
		get => label;
		set
		{
			LinkUtil.CheckLabel(value);
			label = value;
			identifier = null;
		}
	}
	/// <summary>
	/// 获取链接定义的 ID。
	/// </summary>
	public string Identifier
	{
		get
		{
			identifier ??= LinkUtil.NormalizeLabel(label);
			return identifier;
		}
	}
	/// <summary>
	/// 获取或设置链接定义的 URL。
	/// </summary>
	public string URL
	{
		get => url;
		set => url = value ?? string.Empty;
	}
	/// <summary>
	/// 获取或设置链接定义的标题。
	/// </summary>
	public string? Title { get; set; }
	/// <summary>
	/// 获取是否包含属性。
	/// </summary>
	[MemberNotNullWhen(true, nameof(attributes))]
	public bool HasAttribute => attributes?.Count > 0;
	/// <summary>
	/// 获取链接定义的属性列表。
	/// </summary>
	public HtmlAttributeList Attributes => attributes ??= new HtmlAttributeList();

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitLinkDefinition(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitLinkDefinition(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		// 先检查是否存在已被复制的链接定义。
		if (context.LinkDefinitions.TryGetValue(this, out var cloned))
		{
			return cloned;
		}
		LinkDefinition node = new(label, url, Title, Span)
		{
			identifier = identifier,
			Locator = Locator,
		};
		if (HasAttribute)
		{
			attributes.CloneTo(node.Attributes);
		}
		context.LinkDefinitions.Add(this, node);
		return node;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{LinkDefinition \"{Label}\" {Span}}}";
	}
}
