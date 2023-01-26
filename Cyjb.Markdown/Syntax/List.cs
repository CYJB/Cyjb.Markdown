using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的列表。
/// </summary>
public sealed class List : BlockNode, INodeContainer<ListItem>
{
	/// <summary>
	/// 列表项。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<ListItem> children;

	/// <summary>
	/// 使用指定的列表样式类型和文本范围初始化 <see cref="List"/> 类的新实例。
	/// </summary>
	/// <param name="styleType">列表的样式类型。</param>
	/// <param name="span">文本范围。</param>
	public List(ListStyleType styleType, TextSpan span = default) : base(MarkdownKind.List)
	{
		children = new NodeList<ListItem>(this);
		StyleType = styleType;
		Start = 1;
		Span = span;
	}

	/// <summary>
	/// 获取列表项。
	/// </summary>
	public NodeList<ListItem> Children => children;

	/// <summary>
	/// 获取或设置列表的样式类型。
	/// </summary>
	public ListStyleType StyleType { get; set; }

	/// <summary>
	/// 获取或设置有序列表的起始序号。
	/// </summary>
	public int Start { get; set; }

	/// <summary>
	/// 获取或设置当前列表是否是松散的。
	/// </summary>
	/// <remarks>松散的列表表示任一列表项间使用了空行进行分割，
	/// 或者某个列表项包含了使用空行分割的两个块节点。一般来说，
	/// 松散列表中的段落在输出 HTML 时会使用 <c>&lt;p&gt;</c> 标签包裹，
	/// 但非松散列表不需要包裹。</remarks>
	public bool Loose { get; set; }

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override ListItem? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override ListItem? LastChild => children.LastOrDefault();

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitList(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitList(this)!;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		if (StyleType == ListStyleType.Unordered)
		{
			return $"{{UnorderedList {Span}}}";
		}
		else
		{
			return $"{{List {StyleType.ToString()[8..]}:{Start} {Span}}}";
		}
	}
}
