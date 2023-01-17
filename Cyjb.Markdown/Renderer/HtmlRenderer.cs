using System.Text;
using System.Text.RegularExpressions;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.Renderer;

/// <summary>
/// Markdown 的 HTML 渲染器。
/// </summary>
public class HtmlRenderer : SyntaxWalker
{
	/// <summary>
	/// HTML 属性名称的正则表达式。
	/// </summary>
	private static readonly Regex AttributeNameRegex = new("^[a-z_:][a-z0-9_.:-]*$", RegexOptions.IgnoreCase);

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
	/// 初始化 <see cref="HtmlRenderer"/> 类的新实例。
	/// </summary>
	public HtmlRenderer() { }

	/// <summary>
	/// 获取或设置软换行的字符，默认为 <c>\n</c>。
	/// </summary>
	public string SoftBreak { get; set; } = "\n";

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
		WriteStartTag(node, tagName);
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
		Dictionary<string, string> attributes = new();
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
			attributes["class"] = $"language-{lang}";
		}
		WriteLine();
		WriteStartTag(node, "pre");
		WriteStartTag(node, "code", attributes);
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
	public override void VisitQuote(Quote node)
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
		Dictionary<string, string> attributes = new();
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
					attributes["type"] = "a";
					break;
				case ListStyleType.OrderedUpperAlpha:
					attributes["type"] = "A";
					break;
				case ListStyleType.OrderedLowerRoman:
					attributes["type"] = "i";
					break;
				case ListStyleType.OrderedUpperRoman:
					attributes["type"] = "I";
					break;
				case ListStyleType.OrderedLowerGreek:
					attributes["style"] = "list-style-type: lower-greek";
					break;
			}
			if (node.Start != 1)
			{
				attributes["start"] = node.Start.ToString();
			}
		}
		WriteLine();
		WriteStartTag(node, tagName, attributes);
		WriteLine();
		DefaultVisit(node);
		WriteLine();
		WriteEndTag(tagName);
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
		Dictionary<string, string> attributes = new();
		if (node.Kind == MarkdownKind.Link)
		{
			attributes["href"] = LinkUtil.EncodeURL(node.URL);
			if (node.Title != null)
			{
				attributes["title"] = node.Title;
			}
			WriteStartTag(node, "a", attributes);
			DefaultVisit(node);
			WriteEndTag("a");
		}
		else
		{
			altTextRenderer ??= new AltTextRenderer();
			altTextRenderer.Clear();
			node.Accept(altTextRenderer);
			attributes["src"] = LinkUtil.EncodeURL(node.URL);
			attributes["alt"] = altTextRenderer.ToString();
			if (node.Title != null)
			{
				attributes["title"] = node.Title;
			}
			WriteStartTag(node, "img", attributes, true);
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
	/// <param name="attributes">标签的属性。</param>
	/// <param name="closed">标签是否是闭合的。</param>
	protected void WriteStartTag(Node node, string tagName,
		Dictionary<string, string>? attributes = null, bool closed = false)
	{
		text.Append('<');
		text.Append(tagName);
		attributes ??= new Dictionary<string, string>();
		foreach (IAttributeModifier modifier in attributeModifiers)
		{
			modifier.UpdateAttributes(node, tagName, attributes);
		}
		foreach (KeyValuePair<string, string> attr in attributes)
		{
			// 忽略非法的属性名。
			if (!AttributeNameRegex.IsMatch(attr.Key))
			{
				continue;
			}
			text.Append(' ');
			text.Append(attr.Key);
			text.Append("=\"");
			text.Append(attr.Value.Replace("\"", "&quot;"));
			text.Append('\"');
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
