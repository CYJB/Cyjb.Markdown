using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 表情符号的单元测试。
/// </summary>
[TestClass]
public class UnitTestEmoji : BaseTest
{
	/// <summary>
	/// 测试表情符号。
	/// </summary>
	[TestMethod]
	public void TestEmoji()
	{
		AssertMarkdown(":+1:foo:o::thumbsup:\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Emoji(0..4, "+1");
				Literal(4..7, "foo");
				Emoji(7..10, "o");
				Emoji(10..20, "thumbsup");
			});
		});
	}
	/// <summary>
	/// 测试无效的表情符号。
	/// </summary>
	[TestMethod]
	public void TestInvalidEmoji()
	{
		AssertMarkdown(":O::oo:\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, ":O::oo:");
			});
		});
	}
	/// <summary>
	/// 测试转义的冒号。
	/// </summary>
	[TestMethod]
	public void TestEscapedColon()
	{
		AssertMarkdown("foo\\:o:o:bar\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..6, "foo:o");
				Emoji(6..9, "o");
				Literal(9..12, "bar");
			});
		});
	}

	/// <summary>
	/// 测试渲染表情符号。
	/// </summary>
	[TestMethod]
	public void TestRender()
	{
		HtmlRenderer renderer = new();

		// 没有 thead 和 tbody
		renderer.Clear();
		renderer.Visit(Document.Parse(":+1:foo:o: bar :atom:"));
		Assert.AreEqual("<p>\U0001f44dfoo\u2b55 bar <img src=\"https://github.githubassets.com/images/icons/emoji/atom.png?v8\" /></p>\n", renderer.ToString());
	}

	/// <summary>
	/// 测试 CommonMark 不支持表情符号。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark(":+1:foo:o::thumbsup:\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..20, ":+1:foo:o::thumbsup:");
			});
		});
	}
}

