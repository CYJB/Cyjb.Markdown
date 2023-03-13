using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// cmark 的回归测试。
/// </summary>
/// <see href="https://github.com/commonmark/cmark/blob/master/test/regression.txt"/>
[TestClass]
public class UnitTestCMark : BaseTest
{
	/// <summary>
	/// Issue #113: EOL character weirdness on Windows
	/// (Important: first line ends with CR + CR + LF)
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/113"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("line1\r\r\nline2\r\n", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..5, "line1");
			});
			Paragraph(8..15, () =>
			{
				Literal(8..13, "line2");
			});
		});
	}
	/// <summary>
	/// Issue #114: cmark skipping first character in line
	/// (Important: the blank lines around "Repeatedly" contain a tab.)
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/114"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("By taking it apart\r\n\r\n- alternative solutions\r\n\t\r\nRepeatedly solving\r\n\t\r\n- how techniques\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..18, "By taking it apart");
			});
			UnorderedList(22..47, false, () =>
			{
				ListItem(22..47, () =>
				{
					Paragraph(24..47, () =>
					{
						Literal(24..45, "alternative solutions");
					});
				});
			});
			Paragraph(50..70, () =>
			{
				Literal(50..68, "Repeatedly solving");
			});
			UnorderedList(73..91, false, () =>
			{
				ListItem(73..91, () =>
				{
					Paragraph(75..91, () =>
					{
						Literal(75..89, "how techniques");
					});
				});
			});
		});
	}
	/// <summary>
	/// Issue jgm/CommonMark#430:  h2..h6 not recognized as block tags.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/430"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("<h1>lorem</h1>\r\n\r\n<h2>lorem</h2>\r\n\r\n<h3>lorem</h3>\r\n\r\n<h4>lorem</h4>\r\n\r\n<h5>lorem</h5>\r\n\r\n<h6>lorem</h6>\r\n", () =>
		{
			HtmlBlock(0..16, "<h1>lorem</h1>\r\n");
			HtmlBlock(18..34, "<h2>lorem</h2>\r\n");
			HtmlBlock(36..52, "<h3>lorem</h3>\r\n");
			HtmlBlock(54..70, "<h4>lorem</h4>\r\n");
			HtmlBlock(72..88, "<h5>lorem</h5>\r\n");
			HtmlBlock(90..106, "<h6>lorem</h6>\r\n");
		});
	}
	/// <summary>
	/// Issue jgm/commonmark.js#109 - tabs after setext header line
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/109"/>
	[TestMethod]
	public void Test4()
	{
		AssertCommonMark("hi\r\n--\t\r\n", () =>
		{
			Heading(0..9, 2, () =>
			{
				Literal(0..2, "hi");
			});
		});
	}
	/// <summary>
	/// Issue #177 - incorrect emphasis parsing
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/177"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("a***b* c*\r\n", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..2, "a*");
				Emphasis(2..9, () =>
				{
					Emphasis(3..6, () =>
					{
						Literal(4..5, "b");
					});
					Literal(6..8, " c");
				});
			});
		});
	}
	/// <summary>
	/// Issue #193 - unescaped left angle brackets in link destination
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/193"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("[a]\r\n\r\n[a]: <te<st>\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "[a]");
			});
			Paragraph(7..21, () =>
			{
				Literal(7..15, "[a]: <te");
				HtmlStartTag(15..19, "<st>");
			});
		});
	}
	/// <summary>
	/// Issue #192 - escaped spaces in link destination
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/192"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("[a](te\\ st)\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..11, "[a](te\\ st)");
			});
		});
	}
	/// <summary>
	/// Issue #527 - meta tags in inline contexts
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/527"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("City:\r\n<span itemprop=\"contentLocation\" itemscope itemtype=\"https://schema.org/City\">\r\n  <meta itemprop=\"name\" content=\"Springfield\">\r\n</span>\r\n", () =>
		{
			Paragraph(0..144, () =>
			{
				Literal(0..5, "City:");
				SoftBreak(5..7);
				HtmlStartTag(7..85, "<span itemprop=\"contentLocation\" itemscope itemtype=\"https://schema.org/City\">");
				SoftBreak(85..89);
				HtmlStartTag(89..133, "<meta itemprop=\"name\" content=\"Springfield\">");
				SoftBreak(133..135);
				HtmlEndTag(135..142, "</span>");
			});
		});
	}
	/// <summary>
	/// Issue #530 - link parsing corner cases
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/530"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("[a](\\ b)\r\n\r\n[a](<<b)\r\n\r\n[a](<b\r\n)\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..8, "[a](\\ b)");
			});
			Paragraph(12..22, () =>
			{
				Literal(12..20, "[a](<<b)");
			});
			Paragraph(24..35, () =>
			{
				Literal(24..30, "[a](<b");
				SoftBreak(30..32);
				Literal(32..33, ")");
			});
		});
	}
	/// <summary>
	/// Issue commonmark#526 - unescaped ( in link title
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/526"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("[link](url ((title))\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..20, "[link](url ((title))");
			});
		});
	}
	/// <summary>
	/// Issue commonamrk#517 - script, pre, style close tag without opener.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/517"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown("</script>\r\n\r\n</pre>\r\n\r\n</style>\r\n", () =>
		{
			HtmlBlock(0..11, "</script>\r\n");
			HtmlBlock(13..21, "</pre>\r\n");
			HtmlBlock(23..33, "</style>\r\n");
		});
	}
	/// <summary>
	/// Issue #289.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/289"/>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown("[a](<b) c>\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..10, "[a](<b) c>");
			});
		});
	}
	/// <summary>
	/// Issue #334 - UTF-8 BOM
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/334"/>
	/// <remarks>这里不处理 BOM。</remarks>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown("\uFEFF# Hi\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..5, "\uFEFF# Hi");
			});
		});
	}
	/// <summary>
	/// Issue commonmark.js#213 - type 7 blocks can't interrupt paragraph
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/213"/>
	[TestMethod]
	public void Test14()
	{
		AssertMarkdown("- <script>\r\n- some text\r\nsome other text\r\n</script>\r\n", () =>
		{
			UnorderedList(0..53, false, () =>
			{
				ListItem(0..12, () =>
				{
					HtmlBlock(2..12, "<script>\r\n");
				});
				ListItem(12..53, () =>
				{
					Paragraph(14..53, () =>
					{
						Literal(14..23, "some text");
						SoftBreak(23..25);
						Literal(25..40, "some other text");
						SoftBreak(40..42);
						HtmlEndTag(42..51, "</script>");
					});
				});
			});
		});
	}
	/// <summary>
	/// Issue #383 - emphasis parsing.
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/383"/>
	[TestMethod]
	public void Test15()
	{
		AssertMarkdown("*****Hello*world****\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..2, "**");
				Emphasis(2..20, () =>
				{
					Strong(3..19, () =>
					{
						Literal(5..10, "Hello");
						Emphasis(10..17, () =>
						{
							Literal(11..16, "world");
						});
					});
				});
			});
		});
	}
}

