using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 行级 HTML 的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#autolinks"/>
[TestClass]
public class UnitTestHtml : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-612"/>
	[TestMethod]
	public void Test612()
	{
		AssertMarkdown("<a><bab><c2c>", () =>
		{
			Paragraph(0..13, () =>
			{
				HtmlStartTag(0..3, "<a>");
				HtmlStartTag(3..8, "<bab>");
				HtmlStartTag(8..13, "<c2c>");
			});
		});
	}
	/// <summary>
	/// 空元素。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-613"/>
	[TestMethod]
	public void Test613()
	{
		AssertMarkdown("<a/><b2/>", () =>
		{
			Paragraph(0..9, () =>
			{
				HtmlStartTag(0..4, "<a/>");
				HtmlStartTag(4..9, "<b2/>");
			});
		});
	}
	/// <summary>
	/// 允许空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-614"/>
	[TestMethod]
	public void Test614()
	{
		AssertMarkdown("<a  /><b2\r\ndata=\"foo\" >", () =>
		{
			Paragraph(0..23, () =>
			{
				HtmlStartTag(0..6, "<a  />");
				HtmlStartTag(6..23, "<b2\r\ndata=\"foo\" >");
			});
		});
	}
	/// <summary>
	/// 带有属性。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-615"/>
	[TestMethod]
	public void Test615()
	{
		AssertMarkdown("<a foo=\"bar\" bam = 'baz <em>\"</em>'\r\n_boolean zoop:33=zoop:33 />", () =>
		{
			Paragraph(0..64, () =>
			{
				HtmlStartTag(0..64, "<a foo=\"bar\" bam = 'baz <em>\"</em>'\r\n_boolean zoop:33=zoop:33 />");
			});
		});
	}
	/// <summary>
	/// 可以使用自定义标签。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-616"/>
	[TestMethod]
	public void Test616()
	{
		AssertMarkdown("Foo <responsive-image src=\"foo.jpg\" />", () =>
		{
			Paragraph(0..38, () =>
			{
				Literal(0..4, "Foo ");
				HtmlStartTag(4..38, "<responsive-image src=\"foo.jpg\" />");
			});
		});
	}
	/// <summary>
	/// 无效的标签名。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-617"/>
	[TestMethod]
	public void Test617()
	{
		AssertMarkdown("<33> <__>", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..9, "<33> <__>");
			});
		});
	}
	/// <summary>
	/// 无效的属性名。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-618"/>
	[TestMethod]
	public void Test618()
	{
		AssertMarkdown("<a h*#ref=\"hi\">", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..15, "<a h*#ref=\"hi\">");
			});
		});
	}
	/// <summary>
	/// 无效的属性值。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-619"/>
	[TestMethod]
	public void Test619()
	{
		AssertMarkdown("<a href=\"hi'> <a href=hi'>", () =>
		{
			Paragraph(0..26, () =>
			{
				Literal(0..26, "<a href=\"hi'> <a href=hi'>");
			});
		});
	}
	/// <summary>
	/// 非法的空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-620"/>
	[TestMethod]
	public void Test620()
	{
		AssertMarkdown("< a><\r\nfoo><bar/ >\r\n<foo bar=baz\r\nbim!bop />", () =>
		{
			Paragraph(0..44, () =>
			{
				Literal(0..5, "< a><");
				SoftBreak(5..7);
				Literal(7..18, "foo><bar/ >");
				SoftBreak(18..20);
				Literal(20..32, "<foo bar=baz");
				SoftBreak(32..34);
				Literal(34..44, "bim!bop />");
			});
		});
	}
	/// <summary>
	/// 缺少的空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-621"/>
	[TestMethod]
	public void Test621()
	{
		AssertMarkdown("<a href='bar'title=title>", () =>
		{
			Paragraph(0..25, () =>
			{
				Literal(0..25, "<a href='bar'title=title>");
			});
		});
	}
	/// <summary>
	/// 关闭标签。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-622"/>
	[TestMethod]
	public void Test622()
	{
		AssertMarkdown("</a></foo >", () =>
		{
			Paragraph(0..11, () =>
			{
				HtmlEndTag(0..4, "</a>");
				HtmlEndTag(4..11, "</foo >");
			});
		});
	}
	/// <summary>
	/// 关闭标签中的无效属性。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-623"/>
	[TestMethod]
	public void Test623()
	{
		AssertMarkdown("</a href=\"foo\">", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..15, "</a href=\"foo\">");
			});
		});
	}
	/// <summary>
	/// 注释。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-624"/>
	[TestMethod]
	public void Test624()
	{
		AssertMarkdown("foo <!-- this is a\r\ncomment - with hyphen -->", () =>
		{
			Paragraph(0..45, () =>
			{
				Literal(0..4, "foo ");
				HtmlComment(4..45, "<!-- this is a\r\ncomment - with hyphen -->");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-625"/>
	[TestMethod]
	public void Test625()
	{
		AssertMarkdown("foo <!-- not a comment -- two hyphens -->", () =>
		{
			Paragraph(0..41, () =>
			{
				Literal(0..41, "foo <!-- not a comment -- two hyphens -->");
			});
		});
	}
	/// <summary>
	/// 非注释。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-626"/>
	[TestMethod]
	public void Test626_1()
	{
		AssertMarkdown("foo <!--> foo -->\r\n\r\nfoo <!-- foo--->", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..17, "foo <!--> foo -->");
			});
			Paragraph(21..37, () =>
			{
				Literal(21..37, "foo <!-- foo--->");
			});
		});
	}
	[TestMethod]
	public void Test626_2()
	{
		AssertMarkdown("foo <!--> foo -->\n\nfoo <!-- foo--->", () =>
		{
			Paragraph(0..18, () =>
			{
				Literal(0..17, "foo <!--> foo -->");
			});
			Paragraph(19..35, () =>
			{
				Literal(19..35, "foo <!-- foo--->");
			});
		});
	}
	/// <summary>
	/// 处理结构。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-627"/>
	[TestMethod]
	public void Test627()
	{
		AssertMarkdown("foo <?php echo $a; ?>", () =>
		{
			Paragraph(0..21, () =>
			{
				Literal(0..4, "foo ");
				HtmlProcessing(4..21, "<?php echo $a; ?>");
			});
		});
	}
	/// <summary>
	/// 声明。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-628"/>
	[TestMethod]
	public void Test628()
	{
		AssertMarkdown("foo <!ELEMENT br EMPTY>", () =>
		{
			Paragraph(0..23, () =>
			{
				Literal(0..4, "foo ");
				HtmlDeclaration(4..23, "<!ELEMENT br EMPTY>");
			});
		});
	}
	/// <summary>
	/// CDATA 段。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-629"/>
	[TestMethod]
	public void Test629()
	{
		AssertMarkdown("foo <![CDATA[>&<]]>", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..4, "foo ");
				HtmlCData(4..19, "<![CDATA[>&<]]>");
			});
		});
	}
	/// <summary>
	/// 实体会在 HTML 属性中保留。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-630"/>
	[TestMethod]
	public void Test630()
	{
		AssertMarkdown("foo <a href=\"&ouml;\">", () =>
		{
			Paragraph(0..21, () =>
			{
				Literal(0..4, "foo ");
				HtmlStartTag(4..21, "<a href=\"&ouml;\">");
			});
		});
	}
	/// <summary>
	/// 反斜杠转移在 HTML 属性中无效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-631"/>
	[TestMethod]
	public void Test631()
	{
		AssertMarkdown("foo <a href=\"\\*\">", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..4, "foo ");
				HtmlStartTag(4..17, "<a href=\"\\*\">");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-632"/>
	[TestMethod]
	public void Test632()
	{
		AssertMarkdown("<a href=\"\\\"\">", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..13, "<a href=\"\"\">");
			});
		});
	}
}

