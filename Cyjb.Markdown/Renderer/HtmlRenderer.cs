using System.Text;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.Renderer;

/// <summary>
/// Markdown 的 HTML 渲染器。
/// </summary>
public class HtmlRenderer : BaseRenderer
{
	/// <summary>
	/// 需要被转义的字符。
	/// </summary>
	private static readonly char[] EscapedChars = new char[] { '&', '<', '>', '\"' };

	/// <summary>
	/// HTML 属性修改器。
	/// </summary>
	private readonly List<IAttributeModifier> attributeModifiers = new();
	/// <summary>
	/// HTML 属性修改器的个数。
	/// </summary>
	private int attributeModifierCount = 0;
	/// <summary>
	/// 文本构造器。
	/// </summary>
	private readonly StringBuilder text = new();
	/// <summary>
	/// 临时的 HTML 属性列表。
	/// </summary>
	private readonly HtmlAttributeList tempHtmlAttributeList = new();

	/// <summary>
	/// 初始化 <see cref="HtmlRenderer"/> 类的新实例。
	/// </summary>
	public HtmlRenderer() { }

	/// <summary>
	/// 代码块的语言前缀，默认为 <c>language-</c>。
	/// </summary>
	public string CodeBlockLanguagePrefix { get; set; } = "language-";
	/// <summary>
	/// 获取或设置未选中状态的任务列表项 HTML。
	/// </summary>
	public string UncheckedTaskListItem { get; set; } = "<input type=\"checkbox\" disabled>";
	/// <summary>
	/// 获取或设置选中状态的任务列表项 HTML。
	/// </summary>
	public string CheckedTaskListItem { get; set; } = "<input type=\"checkbox\" disabled checked>";

	/// <summary>
	/// 添加指定的 HTML 属性修改器。
	/// </summary>
	/// <param name="modifier">要添加的属性修改器。</param>
	public void AddAttributeModifier(IAttributeModifier modifier)
	{
		if (modifier != null)
		{
			attributeModifiers.Add(modifier);
			attributeModifierCount++;
		}
	}

	/// <summary>
	/// 清除已生成的 HTML 文本。
	/// </summary>
	public override void Clear()
	{
		base.Clear();
		text.Clear();
	}

	#region 块节点

	/// <summary>
	/// 访问指定的分割线节点。
	/// </summary>
	/// <param name="node">要访问的分割线节点。</param>
	public override void VisitThematicBreak(ThematicBreak node)
	{
		WriteLine();
		WriteStartTag(node, "hr", null, true);
		WriteLine();
	}

	/// <summary>
	/// 访问指定的标题节点。
	/// </summary>
	/// <param name="node">要访问的标题节点。</param>
	public override void VisitHeading(Heading node)
	{
		string tagName = $"h{node.Depth}";
		WriteLine();
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.AddRange(node.Attributes);
		WriteStartTag(node, tagName, attrs);
		DefaultVisit(node);
		WriteEndTag(tagName);
		WriteLine();
	}

	/// <summary>
	/// 访问指定的代码块节点。
	/// </summary>
	/// <param name="node">要访问的代码块节点。</param>
	public override void VisitCodeBlock(CodeBlock node)
	{
		WriteLine();
		WriteStartTag(node, "pre");

		HtmlAttributeList attrs = GetHtmlAttributeList();
		string? lang = GetInfoFirstWord(node.Info);
		if (lang != null)
		{
			attrs.AddClass(CodeBlockLanguagePrefix + lang);
		}
		attrs.AddRange(node.Attributes);

		WriteStartTag(node, "code", attrs);
		Write(node.Content);
		WriteEndTag("code");
		WriteEndTag("pre");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的 HTML 块节点。
	/// </summary>
	/// <param name="node">要访问的 HTML 块节点。</param>
	public override void VisitHtmlBlock(HtmlBlock node)
	{
		WriteLine();
		WriteRaw(node.Content);
		WriteLine();
	}

	/// <summary>
	/// 访问指定的段落节点。
	/// </summary>
	/// <param name="node">要访问的段落节点。</param>
	public override void VisitParagraph(Paragraph node)
	{
		bool loose = node.Parent?.Parent is not List list || list.Loose;
		if (loose)
		{
			WriteLine();
			WriteStartTag(node, "p");
		}
		DefaultVisit(node);
		if (loose)
		{
			WriteEndTag("p");
			WriteLine();
		}
	}

	/// <summary>
	/// 访问指定的引用节点。
	/// </summary>
	/// <param name="node">要访问的引用节点。</param>
	public override void VisitBlockquote(Blockquote node)
	{
		WriteLine();
		WriteStartTag(node, "blockquote");
		WriteLine();
		DefaultVisit(node);
		WriteLine();
		WriteEndTag("blockquote");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的列表项节点。
	/// </summary>
	/// <param name="node">要访问的列表项节点。</param>
	public override void VisitListItem(ListItem node)
	{
		WriteStartTag(node, "li");
		if (node.Checked.HasValue)
		{
			if (node.Checked.Value)
			{
				WriteRaw(CheckedTaskListItem);
			}
			else
			{
				WriteRaw(UncheckedTaskListItem);
			}
		}
		DefaultVisit(node);
		WriteEndTag("li");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的列表节点。
	/// </summary>
	/// <param name="node">要访问的列表节点。</param>
	public override void VisitList(List node)
	{
		string tagName;
		HtmlAttributeList attrs = GetHtmlAttributeList();
		if (node.StyleType == ListStyleType.Unordered)
		{
			tagName = "ul";
		}
		else
		{
			tagName = "ol";
			switch (node.StyleType)
			{
				case ListStyleType.OrderedLowerAlpha:
					attrs["type"] = "a";
					break;
				case ListStyleType.OrderedUpperAlpha:
					attrs["type"] = "A";
					break;
				case ListStyleType.OrderedLowerRoman:
					attrs["type"] = "i";
					break;
				case ListStyleType.OrderedUpperRoman:
					attrs["type"] = "I";
					break;
				case ListStyleType.OrderedLowerGreek:
					attrs["style"] = "list-style-type: lower-greek;";
					break;
			}
			if (node.Start != 1)
			{
				attrs["start"] = node.Start.ToString();
			}
		}
		WriteLine();
		WriteStartTag(node, tagName, attrs);
		WriteLine();
		DefaultVisit(node);
		WriteLine();
		WriteEndTag(tagName);
		WriteLine();
	}


	/// <summary>
	/// 访问指定的表格单元格。
	/// </summary>
	/// <param name="node">要访问的表格单元格节点。</param>
	public override void VisitTableCell(TableCell node)
	{
		string tagName;
		if (isTableHeading)
		{
			tagName = "th";
		}
		else
		{
			tagName = "td";
		}
		HtmlAttributeList attrs = GetHtmlAttributeList();
		if (tableAlign != TableAlign.None)
		{
			// 添加对齐的样式。
			switch (tableAlign)
			{
				case TableAlign.Left:
					attrs["style"] = "text-align: left;";
					break;
				case TableAlign.Center:
					attrs["style"] = "text-align: center;";
					break;
				case TableAlign.Right:
					attrs["style"] = "text-align: right;";
					break;
			}
		}
		WriteStartTag(node, tagName, attrs);
		DefaultVisit(node);
		WriteEndTag(tagName);
		WriteLine();
	}

	/// <summary>
	/// 访问指定的表格行。
	/// </summary>
	/// <param name="node">要访问的表格行节点。</param>
	public override void VisitTableRow(TableRow node)
	{
		WriteStartTag(node, "tr");
		WriteLine();
		base.VisitTableRow(node);
		WriteEndTag("tr");
		WriteLine();
	}

	/// <summary>
	/// 写入空的表格单元格，用于补齐表格行缺少的单元格。
	/// </summary>
	/// <param name="row">缺少单元格的表格行。</param>
	protected override void WriteEmptyTableCell(TableRow row)
	{
		WriteStartTag(row, "td");
		WriteEndTag("td");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的表格。
	/// </summary>
	/// <param name="node">要访问的表格节点。</param>
	public override void VisitTable(Table node)
	{
		WriteStartTag(node, "table");
		WriteLine();
		base.VisitTable(node);
		WriteEndTag("table");
		WriteLine();
	}

	/// <summary>
	/// 写入表格的头。
	/// </summary>
	/// <param name="table">表格节点。</param>
	/// <param name="heading">表格的行。</param>
	protected override void WriteTableHead(Table table, TableRow heading)
	{
		WriteStartTag(heading, "thead");
		WriteLine();
		base.WriteTableHead(table, heading);
		WriteEndTag("thead");
		WriteLine();
	}

	/// <summary>
	/// 写入表格的内容。
	/// </summary>
	/// <param name="table">表格节点。</param>
	/// <param name="rows">表格的行。</param>
	protected override void WriteTableBody(Table table, IEnumerable<TableRow> rows)
	{
		WriteStartTag(table, "tbody");
		WriteLine();
		base.WriteTableBody(table, rows);
		WriteEndTag("tbody");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的数学公式块节点。
	/// </summary>
	/// <param name="node">要访问的数学公式块节点。</param>
	public override void VisitMathBlock(MathBlock node)
	{
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.AddClass("math");
		if (node.Attributes != null)
		{
			attrs.AddRange(node.Attributes);
		}
		WriteStartTag(node, "div", attrs);
		Write("\\[");
		Write(node.Content);
		Write("\\]");
		WriteEndTag("div");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的自定义容器节点。
	/// </summary>
	/// <param name="node">要访问的自定义容器节点。</param>
	public override void VisitCustomContainer(CustomContainer node)
	{
		HtmlAttributeList attrs = GetHtmlAttributeList();
		string? type = GetInfoFirstWord(node.Info);
		if (type != null)
		{
			attrs.AddClass(type);
		}
		if (node.Attributes != null)
		{
			attrs.AddRange(node.Attributes);
		}
		WriteLine();
		WriteStartTag(node, "div", attrs);
		WriteLine();
		DefaultVisit(node);
		WriteEndTag("div");
		WriteLine();
	}

	#endregion // 块节点

	#region 行内节点

	/// <summary>
	/// 访问指定的行内代码段节点。
	/// </summary>
	/// <param name="node">要访问的行内代码段节点。</param>
	public override void VisitCodeSpan(CodeSpan node)
	{
		WriteStartTag(node, "code");
		Write(node.Content);
		WriteEndTag("code");
	}

	/// <summary>
	/// 访问指定的强调节点。
	/// </summary>
	/// <param name="node">要访问的强调节点。</param>
	public override void VisitEmphasis(Emphasis node)
	{
		WriteStartTag(node, "em");
		DefaultVisit(node);
		WriteEndTag("em");
	}

	/// <summary>
	/// 访问指定的加粗节点。
	/// </summary>
	/// <param name="node">要访问的加粗节点。</param>
	public override void VisitStrong(Strong node)
	{
		WriteStartTag(node, "strong");
		DefaultVisit(node);
		WriteEndTag("strong");
	}

	/// <summary>
	/// 访问指定的删除节点。
	/// </summary>
	/// <param name="node">要访问的删除节点。</param>
	public override void VisitStrikethrough(Strikethrough node)
	{
		WriteStartTag(node, "del");
		DefaultVisit(node);
		WriteEndTag("del");
	}

	/// <summary>
	/// 访问指定的链接节点。
	/// </summary>
	/// <param name="node">要访问的链接节点。</param>
	public override void VisitLink(Link node)
	{
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.AddRange(node.Attributes);
		if (node.Kind == MarkdownKind.Link)
		{
			attrs["href"] = LinkUtil.EncodeURL(node.URL);
			if (node.Title != null)
			{
				attrs["title"] = node.Title;
			}
			WriteStartTag(node, "a", attrs);
			DefaultVisit(node);
			WriteEndTag("a");
		}
		else
		{
			attrs["src"] = LinkUtil.EncodeURL(node.URL);
			attrs["alt"] = GetAltText(node);
			if (node.Title != null)
			{
				attrs["title"] = node.Title;
			}
			WriteStartTag(node, "img", attrs, true);
		}
	}

	/// <summary>
	/// 访问指定的行内 HTML 节点。
	/// </summary>
	/// <param name="node">要访问的行内 HTML 节点。</param>
	public override void VisitHtml(Html node)
	{
		WriteRaw(node.Content);
	}

	/// <summary>
	/// 访问指定的换行节点。
	/// </summary>
	/// <param name="node">要访问的换行节点。</param>
	public override void VisitBreak(Break node)
	{
		if (node.Kind == MarkdownKind.SoftBreak)
		{
			WriteRaw(SoftBreak);
		}
		else
		{
			WriteStartTag(node, "br", null, true);
			WriteLine();
		}
	}

	/// <summary>
	/// 访问指定的表情符号节点。
	/// </summary>
	/// <param name="node">要访问的表情符号节点。</param>
	public override void VisitEmoji(Emoji node)
	{
		if (node.IsUnicode)
		{
			// Unicode Emoji 直接输出。
			Write(node.Text);
		}
		else if (node.FallbackUrl != null)
		{
			// GitHub 自定义 Emoji 转换为 img。
			HtmlAttributeList attrs = GetHtmlAttributeList();
			attrs.Add("src", node.FallbackUrl);
			WriteStartTag(node, "img", attrs, true);
		}
	}

	/// <summary>
	/// 访问指定的行内数学公式节点。
	/// </summary>
	/// <param name="node">要访问的行内数学公式节点。</param>
	public override void VisitMathSpan(MathSpan node)
	{
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.AddClass("math");
		WriteStartTag(node, "span", attrs);
		Write("\\(");
		Write(node.Content);
		Write("\\)");
		WriteEndTag("span");
	}

	/// <summary>
	/// 访问指定的脚注引用节点。
	/// </summary>
	/// <param name="node">要访问的脚注引用节点。</param>
	/// <param name="backref">反向引用信息。</param>
	protected override void VisitFootnoteRef(FootnoteRef node, FootnoteBackref backref)
	{
		WriteStartTag(node, "sup");

		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.Add("href", "#" + LinkUtil.EncodeURL(backref.Info.Id));
		attrs.Id = LinkUtil.EncodeURL(backref.Identifier);

		WriteStartTag(node, "a", attrs);
		Write(backref.Info.Index);
		WriteEndTag("a");
		WriteEndTag("sup");
	}

	/// <summary>
	/// 访问脚注的反向引用。
	/// </summary>
	/// <param name="node">要访问的脚注反向引用节点。</param>
	protected override void VisitFootnoteBackRef(FootnoteBackref node)
	{
		// 如果是段落内的首个节点，那么不输出空白。
		if (node.Parent != null && node.Parent.FirstChild != node)
		{
			Write(" ");
		}
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.Add("href", "#" + LinkUtil.EncodeURL(node.Identifier));
		WriteStartTag(node, "a", attrs);
		Write("↩");
		WriteEndTag("a");
	}

	/// <summary>
	/// 访问指定的文本节点。
	/// </summary>
	/// <param name="node">要访问的文本节点。</param>
	public override void VisitLiteral(Literal node)
	{
		Write(node.Content);
	}

	#endregion // 行内节点

	/// <summary>
	/// 返回信息字符串的首个单词。
	/// </summary>
	/// <param name="info">信息字符串。</param>
	/// <returns>信息字符串的首个单词，或者返回 <c>null</c> 如果不存在信息字符串。</returns>
	private static string? GetInfoFirstWord(string? info)
	{
		if (info.IsNullOrEmpty())
		{
			return null;
		}
		int idx = info.IndexOf(" ");
		if (idx < 0)
		{
			return info;
		}
		else
		{
			return info[0..idx];
		}
	}

	/// <summary>
	/// 写入脚注段。
	/// </summary>
	/// <param name="doc">文档节点。</param>
	/// <param name="footnotes">要写入的脚注列表。</param>
	protected override void WriteFootnotes(Document doc, List<Footnote> footnotes)
	{
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.Id = "footnotes";
		WriteStartTag(doc, "section", attrs);
		WriteLine();
		WriteStartTag(doc, "ol");
		WriteLine();
		base.WriteFootnotes(doc, footnotes);
		WriteEndTag("ol");
		WriteLine();
		WriteEndTag("section");
		WriteLine();
	}

	/// <summary>
	/// 写入脚注节点。
	/// </summary>
	/// <param name="node">脚注节点。</param>
	/// <param name="info">脚注的信息。</param>
	protected override void WriteFootnote(Footnote node, FootnoteInfo info)
	{
		HtmlAttributeList attrs = GetHtmlAttributeList();
		attrs.Id = LinkUtil.EncodeURL(info.Id);
		WriteStartTag(node, "li", attrs);
		WriteLine();
		base.WriteFootnote(node, info);
		WriteEndTag("li");
		WriteLine();
	}

	/// <summary>
	/// 返回临时的 HTML 属性列表。
	/// </summary>
	/// <returns>临时的 HTML 属性列表。</returns>
	private HtmlAttributeList GetHtmlAttributeList()
	{
		tempHtmlAttributeList.Clear();
		return tempHtmlAttributeList;
	}

	#region 写入 HTML

	/// <summary>
	/// 写入指定的文本。
	/// </summary>
	/// <param name="str">要写入的文本。</param>
	protected void Write(string? str)
	{
		if (str == null)
		{
			return;
		}
		int start = 0;
		int idx = str.IndexOfAny(EscapedChars, start);
		while (idx >= 0)
		{
			text.Append(str, start, idx - start);
			switch (str[idx])
			{
				case '&':
					text.Append("&amp;");
					break;
				case '<':
					text.Append("&lt;");
					break;
				case '>':
					text.Append("&gt;");
					break;
				case '\"':
					text.Append("&quot;");
					break;
			}
			start = idx + 1;
			idx = str.IndexOfAny(EscapedChars, start);
		}
		text.Append(str, start, str.Length - start);
	}

	/// <summary>
	/// 写入指定的文本（不做 HTML 转义）。
	/// </summary>
	/// <param name="str">要写入的文本。</param>
	protected void WriteRaw(string? str)
	{
		if (str != null)
		{
			text.Append(str);
		}
	}

	/// <summary>
	/// 写入一个换行，不会添加空行。
	/// </summary>
	protected void WriteLine()
	{
		if (text.Length > 0 && text[^1] != '\n')
		{
			text.Append('\n');
		}
	}

	/// <summary>
	/// 写入指定起始标签。
	/// </summary>
	/// <param name="node">当前节点。</param>
	/// <param name="tagName">标签名。</param>
	/// <param name="attrs">标签的属性。</param>
	/// <param name="closed">标签是否是闭合的。</param>
	protected void WriteStartTag(Node node, string tagName,
		HtmlAttributeList? attrs = null, bool closed = false)
	{
		text.Append('<');
		text.Append(tagName);
		attrs ??= GetHtmlAttributeList();
		for (int i = 0; i < attributeModifierCount; i++)
		{
			attributeModifiers[i].UpdateAttributes(node, tagName, attrs);
		}
		if (attrs.Count > 0)
		{
			text.Append(' ');
			attrs.AppendTo(text);
		}
		if (closed)
		{
			text.Append(" /");
		}
		text.Append('>');
	}

	/// <summary>
	/// 写入指定结束标签。
	/// </summary>
	/// <param name="tagName">标签名。</param>
	protected void WriteEndTag(string tagName)
	{
		text.Append("</");
		text.Append(tagName);
		text.Append('>');
	}

	#endregion // 写入 HTML

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return text.ToString();
	}
}
