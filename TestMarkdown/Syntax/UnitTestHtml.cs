using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	/// 测试 <see cref="Html"/> 类。
	/// </summary>
	[TestMethod]
	public void TestHtml()
	{
		Html node = new(MarkdownKind.HtmlStartTag, "<a>");
		Assert.AreEqual("a", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);
		Assert.AreEqual(new Html(MarkdownKind.HtmlStartTag, "<a>"), node);

		node = new Html(MarkdownKind.HtmlStartTag, @"<a-30 a= b
_c = ""d \t'>\""	:f.s f
=
f2 f3=a/ a4="""" a5='
'>");
		Assert.AreEqual("a-30", node.Value);
		CollectionAssert.AreEquivalent(new HtmlAttributeList(){
			{ "a", "b" },
			{ "_c", @"d \t'>\" },
			{ ":f.s", "" },
			{ "f", "f2" },
			{ "f3", "a/" },
			{ "a4", "" },
			{ "a5", "\r\n" },
		}, node.Attributes);

		node = new Html(MarkdownKind.HtmlStartTag, "<a key=\"v\r\n  a\r\n  l\r\n  ue\" />");
		Assert.AreEqual("a", node.Value);
		CollectionAssert.AreEquivalent(new HtmlAttributeList(){
			{ "key", "v\r\n  a\r\n  l\r\n  ue" },
		}, node.Attributes);

		node = new Html(MarkdownKind.HtmlStartTag, "<a key='v\r\n  a\r\n  l\r\n  ue' />");
		Assert.AreEqual("a", node.Value);
		CollectionAssert.AreEquivalent(new HtmlAttributeList(){
			{ "key", "v\r\n  a\r\n  l\r\n  ue" },
		}, node.Attributes);

		node = new Html(MarkdownKind.HtmlStartTag, "<a key=v\r\n  a\r\n  l\r\n  ue />");
		Assert.AreEqual("a", node.Value);
		CollectionAssert.AreEquivalent(new HtmlAttributeList(){
			{ "key", "v" },
			{ "a", "" },
			{ "l", "" },
			{ "ue", "" },
		}, node.Attributes);

		Html node2 = new(MarkdownKind.HtmlEndTag, "</b>");
		Assert.AreEqual("b", node2.Value);
		Assert.AreEqual(0, node2.Attributes.Count);
		Assert.AreNotEqual(node, node2);

		node = new(MarkdownKind.HtmlComment, "<!-->");
		Assert.AreEqual("", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlComment, "<!--->");
		Assert.AreEqual("", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlComment, "<!---->");
		Assert.AreEqual("", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlComment, "<!-- c-->");
		Assert.AreEqual("c", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlCData, "<![CDATA[]]>");
		Assert.AreEqual("", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlCData, "<![CDATA[d\r\nf]]>");
		Assert.AreEqual("d\r\nf", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlDeclaration, "<!>");
		Assert.AreEqual("", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlDeclaration, "<! as  >");
		Assert.AreEqual("as", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlProcessing, "<??>");
		Assert.AreEqual("", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		node = new(MarkdownKind.HtmlProcessing, "<? foo 123 ?>");
		Assert.AreEqual("foo 123", node.Value);
		Assert.AreEqual(0, node.Attributes.Count);

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Html(MarkdownKind.CodeSpan, "abc"));
	}
}

