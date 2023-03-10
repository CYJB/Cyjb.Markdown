using System.Text;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.Renderer;

/// <summary>
/// Markdown 的 HTML 渲染器。
/// </summary>
public class HtmlRenderer : SyntaxWalker
{
	/// <summary>
	/// HTML 属性修改器。
	/// </summary>
	private readonly List<IAttributeModifier> attributeModifiers = new();
	/// <summary>
	/// 文本构造器。
	/// </summary>
	private readonly StringBuilder text = new();
	/// <summary>
	/// Alt 文本的渲染器。
	/// </summary>
	private AltTextRenderer? altTextRenderer;
	/// <summary>
	/// 当前是否正在输出表格标题。
	/// </summary>
	private bool isTableHeading = false;
	/// <summary>
	/// 当前表格对齐。
	/// </summary>
	private TableAlign tableAlign = TableAlign.None;

	/// <summary>
	/// 初始化 <see cref="HtmlRenderer"/> 类的新实例。
	/// </summary>
	public HtmlRenderer() { }

	/// <summary>
	/// 获取或设置软换行的字符，默认为 <c>\n</c>。
	/// </summary>
	public string SoftBreak { get; set; } = "\n";
	/// <summary>
	/// 是否输出空的表格标题，默认为 <c>false</c>。
	/// </summary>
	/// <remarks>如果表格标题的单元格全部是空的，那么若设置为 <c>false</c>，
	/// 不会输出 <c>&lt;thead&gt;</c>；若设置为 <c>true</c>，则会输出 <c>&lt;thead&gt;</c>。</remarks>
	public bool OutputEmptyTableHeading { get; set; } = false;
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
		}
	}

	/// <summary>
	/// 清除已生成的 HTML 文本。
	/// </summary>
	public void Clear()
	{
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
		HtmlAttributeList attrs = new();
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
		HtmlAttributeList attrs = new();
		string? info = node.Info;
		if (!info.IsNullOrEmpty())
		{
			int idx = info.IndexOf(" ");
			string lang;
			if (idx == -1)
			{
				lang = info;
			}
			else
			{
				lang = info[0..idx];
			}
			attrs.AddClass(CodeBlockLanguagePrefix + lang);
		}
		attrs.AddRange(node.Attributes);

		WriteLine();
		WriteStartTag(node, "pre");
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
	/// 访问指定的链接定义节点。
	/// </summary>
	/// <param name="node">要访问的链接定义节点。</param>
	public override void VisitLinkDefinition(LinkDefinition node) { }

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
		HtmlAttributeList attrs = new();
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
		HtmlAttributeList attrs = new();
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
		Table? table = node.Parent;
		if (table == null)
		{
			// 不存在父表格，直接依次输出。
			DefaultVisit(node);
		}
		else
		{
			// 存在父表格，设置每列的对齐，并确保每行的列数一致。
			int columnCount = table.ColumnCount;
			int len = Math.Min(node.Children.Count, columnCount);
			for (int i = 0; i < len; i++)
			{
				tableAlign = table.Aligns[i];
				node.Children[i].Accept(this);
			}
			tableAlign = TableAlign.None;
			for (int i = len; i < columnCount; i++)
			{
				WriteStartTag(node, "td");
				WriteEndTag("td");
				WriteLine();
			}
		}
		WriteEndTag("tr");
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
		// 输出 thead
		bool hasHeading = true;
		if (!OutputEmptyTableHeading)
		{
			hasHeading = node.Children[0].Children.Any(cell => cell.Children.Count > 0);
		}
		if (hasHeading)
		{
			WriteStartTag(node, "thead");
			WriteLine();
			isTableHeading = true;
			node.Children[0].Accept(this);
			isTableHeading = false;
			WriteEndTag("thead");
			WriteLine();
		}
		// 输出 tbody
		if (node.Children.Count > 1)
		{
			WriteStartTag(node, "tbody");
			WriteLine();
			foreach (TableRow row in node.Children.Skip(1))
			{
				row.Accept(this);
			}
			WriteEndTag("tbody");
			WriteLine();
		}
		WriteEndTag("table");
		WriteLine();
	}

	/// <summary>
	/// 访问指定的数学公式块节点。
	/// </summary>
	/// <param name="node">要访问的数学公式块节点。</param>
	public override void VisitMathBlock(MathBlock node)
	{
		HtmlAttributeList attrs = new() { { "class", "math" } };
		WriteStartTag(node, "div", attrs);
		Write("\\[");
		Write(node.Content);
		Write("\\]");
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
		HtmlAttributeList attrs = new();
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
			altTextRenderer ??= new AltTextRenderer();
			altTextRenderer.Clear();
			node.Accept(altTextRenderer);
			attrs["src"] = LinkUtil.EncodeURL(node.URL);
			attrs["alt"] = altTextRenderer.ToString();
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
			HtmlAttributeList attrs = new()
			{
				{ "src", node.FallbackUrl },
			};
			WriteStartTag(node, "img", attrs, true);
		}
	}

	/// <summary>
	/// 访问指定的行内数学公式节点。
	/// </summary>
	/// <param name="node">要访问的行内数学公式节点。</param>
	public override void VisitMathSpan(MathSpan node)
	{
		HtmlAttributeList attrs = new() { { "class", "math" } };
		WriteStartTag(node, "span", attrs);
		Write("\\(");
		Write(node.Content);
		Write("\\)");
		WriteEndTag("span");
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
		foreach (char ch in str)
		{
			switch (ch)
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
				default:
					text.Append(ch);
					break;
			}
		}
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
		attrs ??= new HtmlAttributeList();
		foreach (IAttributeModifier modifier in attributeModifiers)
		{
			modifier.UpdateAttributes(node, tagName, attrs);
		}
		if (attrs.Count > 0)
		{
			text.Append(' ');
			text.Append(attrs.ToString());
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
