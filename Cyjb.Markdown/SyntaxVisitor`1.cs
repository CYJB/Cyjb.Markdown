using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown;

/// <summary>
/// Markdown 语法节点的访问器，可以返回指定类型的结果。
/// </summary>
/// <typeparam name="TResult">Visit 方法返回值的类型。</typeparam>
public abstract class SyntaxVisitor<TResult>
{
	/// <summary>
	/// 初始化 <see cref="SyntaxVisitor{TResult}"/> 类的新实例。
	/// </summary>
	protected SyntaxVisitor() { }

	/// <summary>
	/// 访问指定的 Markdown 节点。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? Visit(Node? node)
	{
		if (node == null)
		{
			return default;
		}
		return node.Accept(this);
	}

	/// <summary>
	/// 提供默认的访问行为。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? DefaultVisit(Node node)
	{
		return default;
	}

	/// <summary>
	/// 访问指定的文档节点。
	/// </summary>
	/// <param name="node">要访问的文档节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitDocument(Document node)
	{
		return DefaultVisit(node);
	}

	#region 块节点

	/// <summary>
	/// 访问指定的分割线节点。
	/// </summary>
	/// <param name="node">要访问的分割线节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitThematicBreak(ThematicBreak node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的标题节点。
	/// </summary>
	/// <param name="node">要访问的标题节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitHeading(Heading node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的代码块节点。
	/// </summary>
	/// <param name="node">要访问的代码块节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitCodeBlock(CodeBlock node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的 HTML 块节点。
	/// </summary>
	/// <param name="node">要访问的 HTML 块节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitHtmlBlock(HtmlBlock node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的链接定义节点。
	/// </summary>
	/// <param name="node">要访问的链接定义节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitLinkDefinition(LinkDefinition node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的段落节点。
	/// </summary>
	/// <param name="node">要访问的段落节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitParagraph(Paragraph node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的引用节点。
	/// </summary>
	/// <param name="node">要访问的引用节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitBlockquote(Blockquote node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的列表项节点。
	/// </summary>
	/// <param name="node">要访问的列表项节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitListItem(ListItem node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的列表节点。
	/// </summary>
	/// <param name="node">要访问的列表节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitList(List node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表格单元格。
	/// </summary>
	/// <param name="node">要访问的表格单元格节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitTableCell(TableCell node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表格行。
	/// </summary>
	/// <param name="node">要访问的表格行节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitTableRow(TableRow node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表格。
	/// </summary>
	/// <param name="node">要访问的表格节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitTable(Table node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的数学公式块节点。
	/// </summary>
	/// <param name="node">要访问的数学公式块节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitMathBlock(MathBlock node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的脚注节点。
	/// </summary>
	/// <param name="node">要访问的脚注节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitFootnote(Footnote node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的自定义容器节点。
	/// </summary>
	/// <param name="node">要访问的自定义容器节点。</param>
	public virtual TResult? VisitCustomContainer(CustomContainer node)
	{
		return DefaultVisit(node);
	}

	#endregion // 块节点

	#region 行内节点

	/// <summary>
	/// 访问指定的行内代码段节点。
	/// </summary>
	/// <param name="node">要访问的行内代码段节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitCodeSpan(CodeSpan node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的强调节点。
	/// </summary>
	/// <param name="node">要访问的强调节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitEmphasis(Emphasis node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的加粗节点。
	/// </summary>
	/// <param name="node">要访问的加粗节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitStrong(Strong node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的删除节点。
	/// </summary>
	/// <param name="node">要访问的删除节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitStrikethrough(Strikethrough node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的链接节点。
	/// </summary>
	/// <param name="node">要访问的链接节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitLink(Link node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的行内 HTML 节点。
	/// </summary>
	/// <param name="node">要访问的行内 HTML 节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitHtml(Html node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的换行节点。
	/// </summary>
	/// <param name="node">要访问的换行节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitBreak(Break node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的表情符号节点。
	/// </summary>
	/// <param name="node">要访问的表情符号节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitEmoji(Emoji node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的行内数学公式节点。
	/// </summary>
	/// <param name="node">要访问的行内数学公式节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitMathSpan(MathSpan node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的脚注引用节点。
	/// </summary>
	/// <param name="node">要访问的脚注引用节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitFootnoteRef(FootnoteRef node)
	{
		return DefaultVisit(node);
	}

	/// <summary>
	/// 访问指定的文本节点。
	/// </summary>
	/// <param name="node">要访问的文本节点。</param>
	/// <returns>返回的结果。</returns>
	public virtual TResult? VisitLiteral(Literal node)
	{
		return DefaultVisit(node);
	}

	#endregion // 行内节点

}
