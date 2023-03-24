using System.Diagnostics;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的脚注。
/// </summary>
public sealed class Footnote : BlockNode, INodeContainer<BlockNode>
{
	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<BlockNode> children;
	/// <summary>
	/// 脚注的标签。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string label;
	/// <summary>
	/// 脚注的 ID。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? identifier;

	/// <summary>
	/// 使用指定的脚注标签和文本范围初始化 <see cref="Footnote"/> 类的新实例。
	/// </summary>
	/// <param name="label">脚注的标签。</param>
	/// <param name="span">文本范围。</param>
	public Footnote(string label, TextSpan span = default) : base(MarkdownKind.Footnote)
	{
		children = new NodeList<BlockNode>(this);
		LinkUtil.CheckLabel(label);
		this.label = label;
		Span = span;
	}

	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<BlockNode> Children => children;
	/// <summary>
	/// 获取或设置脚注的标签。
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
		visitor.VisitFootnote(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitFootnote(this)!;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{Footnote \"{Label}\" {Span}}}";
	}
}
