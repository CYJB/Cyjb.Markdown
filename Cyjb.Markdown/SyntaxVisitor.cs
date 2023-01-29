using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown;

/// <summary>
/// Markdown 语法节点的访问器。
/// </summary>
public abstract class SyntaxVisitor
{
	/// <summary>
	/// 初始化 <see cref="SyntaxVisitor"/> 类的新实例。
	/// </summary>
	protected SyntaxVisitor() { }

	/// <summary>
	/// 访问指定的 Markdown 节点。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	public virtual void Visit(Node? node)
	{
		if (node == null)
		{
			return;
		}
		node.Accept(this);
	}

	/// <summary>
	/// 提供默认的访问行为。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	public virtual void DefaultVisit(Node node) { }

	/// <summary>
	/// 访问指定的文档节点。
	/// </summary>
	/// <param name="node">要访问的文档节点。</param>
	public virtual void VisitDocument(Document node)
	{
		DefaultVisit(node);
	}

	#region 块节点

	/// <summary>
	/// 访问指定的分割线节点。
	/// </summary>
	/// <param name="node">要访问的分割线节点。</param>
	public virtual void VisitThematicBreak(ThematicBreak node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的标题节点。
	/// </summary>
	/// <param name="node">要访问的标题节点。</param>
	public virtual void VisitHeading(Heading node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的代码块节点。
	/// </summary>
	/// <param name="node">要访问的代码块节点。</param>
	public virtual void VisitCodeBlock(CodeBlock node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的 HTML 块节点。
	/// </summary>
	/// <param name="node">要访问的 HTML 块节点。</param>
	public virtual void VisitHtmlBlock(HtmlBlock node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的链接定义节点。
	/// </summary>
	/// <param name="node">要访问的链接定义节点。</param>
	public virtual void VisitLinkDefinition(LinkDefinition node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的段落节点。
	/// </summary>
	/// <param name="node">要访问的段落节点。</param>
	public virtual void VisitParagraph(Paragraph node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的引用节点。
	/// </summary>
	/// <param name="node">要访问的引用节点。</param>
	public virtual void VisitQuote(Quote node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的列表项节点。
	/// </summary>
	/// <param name="node">要访问的列表项节点。</param>
	public virtual void VisitListItem(ListItem node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的列表节点。
	/// </summary>
	/// <param name="node">要访问的列表节点。</param>
	public virtual void VisitList(List node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表格单元格。
	/// </summary>
	/// <param name="node">要访问的表格单元格节点。</param>
	public virtual void VisitTableCell(TableCell node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表格行。
	/// </summary>
	/// <param name="node">要访问的表格行节点。</param>
	public virtual void VisitTableRow(TableRow node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表格。
	/// </summary>
	/// <param name="node">要访问的表格节点。</param>
	public virtual void VisitTable(Table node)
	{
		DefaultVisit(node);
	}

	#endregion // 块节点

	#region 行内节点

	/// <summary>
	/// 访问指定的行内代码段节点。
	/// </summary>
	/// <param name="node">要访问的行内代码段节点。</param>
	public virtual void VisitCodeSpan(CodeSpan node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的强调节点。
	/// </summary>
	/// <param name="node">要访问的强调节点。</param>
	public virtual void VisitEmphasis(Emphasis node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的加粗节点。
	/// </summary>
	/// <param name="node">要访问的加粗节点。</param>
	public virtual void VisitStrong(Strong node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的删除节点。
	/// </summary>
	/// <param name="node">要访问的删除节点。</param>
	public virtual void VisitStrikethrough(Strikethrough node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的链接节点。
	/// </summary>
	/// <param name="node">要访问的链接节点。</param>
	public virtual void VisitLink(Link node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的行内 HTML 节点。
	/// </summary>
	/// <param name="node">要访问的行内 HTML 节点。</param>
	public virtual void VisitHtml(Html node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的换行节点。
	/// </summary>
	/// <param name="node">要访问的换行节点。</param>
	public virtual void VisitBreak(Break node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表情符号节点。
	/// </summary>
	/// <param name="node">要访问的表情符号节点。</param>
	public virtual void VisitEmoji(Emoji node)
	{
		DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的文本节点。
	/// </summary>
	/// <param name="node">要访问的文本节点。</param>
	public virtual void VisitLiteral(Literal node)
	{
		DefaultVisit(node);
	}

	#endregion // 行内节点

}
