using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// commonmark.js 的回归测试。
/// </summary>
/// <see href="https://github.com/commonmark/commonmark.js/blob/master/test/regression.txt"/>
[TestClass]
public class UnitTestCommonMarkJS : BaseTest
{
	/// <summary>
	/// Eating a character after a partially consumed tab.
	/// </summary>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("* foo\r\n\tbar\r\n", () =>
		{
			UnorderedList(0..13, false, () =>
			{
				ListItem(0..13, () =>
				{
					Paragraph(2..13, () =>
					{
						Literal(2..5, "foo");
						SoftBreak(5..8);
						Literal(8..11, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// Type 7 HTML block followed by whitespace (#98).
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/98"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("<a>  \r\nx\r\n", () =>
		{
			HtmlBlock(0..10, "<a>  \r\nx\r\n");
		});
	}
	/// <summary>
	/// h2..h6 raw HTML blocks (jgm/CommonMark#430).
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
	/// Issue #109 - tabs after setext header line
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/109"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("hi\r\n--\t\r\n", () =>
		{
			Heading(0..9, 2, () =>
			{
				Literal(0..2, "hi");
			});
		});
	}
	/// <summary>
	/// Issue #108 - Chinese punctuation not recognized
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/108"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("**。**话\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "**。**话");
			});
		});
	}
	/// <summary>
	/// Issue jgm/cmark#177 - incorrect emphasis parsing
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/177"/>
	[TestMethod]
	public void Test6()
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
	/// Issue jgm/CommonMark#468 - backslash at end of link definition
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/468"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("[\\]: test\r\n", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..9, "[]: test");
			});
		});
	}
	/// <summary>
	/// Issue commonmark/commonmark.js#121 - punctuation set different
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/121"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("^_test_\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..1, "^");
				Emphasis(1..7, () =>
				{
					Literal(2..6, "test");
				});
			});
		});
	}
	/// <summary>
	/// Issue #116 - tabs before and after ATX closing heading
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/116"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("# foo\t#\t\r\n", () =>
		{
			Heading(0..10, 1, () =>
			{
				Literal(2..5, "foo");
			});
		});
	}
	/// <summary>
	/// commonmark/CommonMark#493 - escaped space not allowed in link destination.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/493"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("[link](a\\ b)\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..12, "[link](a\\ b)");
			});
		});
	}
	/// <summary>
	/// Issue #527 - meta tags in inline contexts
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/527"/>
	[TestMethod]
	public void Test11()
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
	/// Double-encoding.
	/// </summary>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown("[XSS](javascript&amp;colon;alert%28&#039;XSS&#039;%29)\r\n", () =>
		{
			Paragraph(0..56, () =>
			{
				Link(0..54, "javascript&colon;alert%28'XSS'%29", null, () =>
				{
					Literal(1..4, "XSS");
				});
			});
		});
	}
	/// <summary>
	/// PR #179
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/179"/>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown("[link](https://www.example.com/home/%25batty)\r\n", () =>
		{
			Paragraph(0..47, () =>
			{
				Link(0..45, "https://www.example.com/home/%25batty", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// Issue commonamrk#517 - script, pre, style close tag without opener.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark-spec/issues/517"/>
	[TestMethod]
	public void Test14()
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
	public void Test15()
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
	/// Issue #161.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/161"/>
	[TestMethod]
	public void Test16()
	{
		AssertMarkdown("*failed to be italic!*\\\r\ntext\r\n", () =>
		{
			Paragraph(0..31, () =>
			{
				Emphasis(0..22, () =>
				{
					Literal(1..21, "failed to be italic!");
				});
				HardBreak(22..25);
				Literal(25..29, "text");
			});
		});
	}
	/// <summary>
	/// Issue #196.
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/196"/>
	[TestMethod]
	public void Test17()
	{
		AssertMarkdown("a <?\r\n?>\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..2, "a ");
				HtmlProcessing(2..8, "<?\r\n?>");
			});
		});
	}
	/// <summary>
	/// Issue #211
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/211"/>
	[TestMethod]
	public void Test18()
	{
		AssertMarkdown("[\\\r\nfoo]: /uri\r\n\r\n[\\\r\nfoo]\r\n", () =>
		{
			LinkDefinition(0..16, "\\\r\nfoo", "/uri");
			Paragraph(18..28, () =>
			{
				Link(18..26, "/uri", null, () =>
				{
					HardBreak(19..22);
					Literal(22..25, "foo");
				});
			});
		});
	}
	/// <summary>
	/// Issue #213 - type 7 blocks can't interrupt paragraph
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/213"/>
	[TestMethod]
	public void Test19()
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
	/// Issue cmark/#383 - emphasis parsing.
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/383"/>
	[TestMethod]
	public void Test20()
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
	/// <summary>
	/// Issue reported at
	/// https://talk.commonmark.org/t/link-label-collapse-all-internal-whitespace/3919/5
	/// </summary>
	/// <see href="https://talk.commonmark.org/t/link-label-collapse-all-internal-whitespace/3919/5"/>
	[TestMethod]
	public void Test21()
	{
		AssertMarkdown("[foo][one two\r\n  three]\r\n\r\n[one two three]: /url \"title\"\r\n", () =>
		{
			Paragraph(0..25, () =>
			{
				Link(0..23, "/url", "title", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(27..58, "one two three", "/url", "title");
		});
	}
	/// <summary>
	/// Issue #258
	/// </summary>
	/// <see href="https://github.com/commonmark/commonmark.js/issues/258"/>
	[TestMethod]
	public void Test22()
	{
		AssertMarkdown("```\r\nabc\r\n```     \r\n", () =>
		{
			CodeBlock(0..20, "abc\r\n");
		});
	}
	/// <summary>
	/// <c>&lt;!doctype</c> is case-insensitive
	/// </summary>
	[TestMethod]
	public void Test23()
	{
		AssertMarkdown("<!docType html>\r\n", () =>
		{
			HtmlBlock(0..17, "<!docType html>\r\n");
		});
	}
	/// <summary>
	/// Declarations don't need spaces, according to the spec (cmark#456)
	/// </summary>
	/// <see href="https://github.com/commonmark/cmark/issues/456"/>
	[TestMethod]
	public void Test24()
	{
		AssertMarkdown("x <!A>\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..2, "x ");
				HtmlDeclaration(2..6, "<!A>");
			});
		});
	}
}

