using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;
using System;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="Html"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestHtml
{
	/// <summary>
	/// 测试 Html 类。
	/// </summary>
	[TestMethod]
	public void TestHtml()
	{
		Html node = new(MarkdownKind.HtmlStartTag, "<a>");
		Assert.AreEqual("a", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);
		Assert.AreEqual(new Html(MarkdownKind.HtmlStartTag, "<a>"), node);

		node = new Html(MarkdownKind.HtmlStartTag, @"<a-30 a= b
_c = ""d \t'>\""	:f.s f
=
f2 f3=a/ a4="""" a5='
'>");
		Assert.AreEqual("a-30", node.Value);
		CollectionAssert.AreEquivalent(new HtmlAttribute[]{
			new HtmlAttribute("a", "b"),
			new HtmlAttribute("_c", @"d \t'>\", "\""),
			new HtmlAttribute(":f.s"),
			new HtmlAttribute("f", "f2"),
			new HtmlAttribute("f3", "a/"),
			new HtmlAttribute("a4", "", "\""),
			new HtmlAttribute("a5", "\r\n", "'"),
		}, node.Attributes);

		Html node2 = new(MarkdownKind.HtmlEndTag, "</b>");
		Assert.AreEqual("b", node2.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node2.Attributes);
		Assert.AreNotEqual(node, node2);

		node = new(MarkdownKind.HtmlComment, "<!---->");
		Assert.AreEqual("", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlComment, "<!-- c-->");
		Assert.AreEqual("c", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlCData, "<![CDATA[]]>");
		Assert.AreEqual("", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlCData, "<![CDATA[d\r\nf]]>");
		Assert.AreEqual("d\r\nf", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlDeclaration, "<!>");
		Assert.AreEqual("", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlDeclaration, "<! as  >");
		Assert.AreEqual("as", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlProcessing, "<??>");
		Assert.AreEqual("", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		node = new(MarkdownKind.HtmlProcessing, "<? foo 123 ?>");
		Assert.AreEqual("foo 123", node.Value);
		CollectionAssert.AreEquivalent(Array.Empty<HtmlAttribute>(), node.Attributes);

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Html(MarkdownKind.CodeSpan, "abc"));
	}
}

