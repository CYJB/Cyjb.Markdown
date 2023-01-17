using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// HTML 实体字符串的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#backslash-escapes"/>
[TestClass]
public class UnitTestEntity : BaseTest
{
	/// <summary>
	/// HTML 实体。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-25"/>
	[TestMethod]
	public void Test25()
	{
		AssertMarkdown("&nbsp; &amp; &copy; &AElig; &Dcaron;\n&frac34; &HilbertSpace; &DifferentialD;\n&ClockwiseContourIntegral; &ngE;", () =>
		{
			Paragraph(0..109, () =>
			{
				Literal(0..36, "\u00A0 \u0026 \u00A9 \u00C6 \u010E");
				SoftBreak(36..37);
				Literal(37..76, "\u00BE \u210B \u2146");
				SoftBreak(76..77);
				Literal(77..109, "\u2232 \u2267\u0338");
			});
		});
	}
	/// <summary>
	/// 十进制数字字符引用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-26"/>
	[TestMethod]
	public void Test26()
	{
		AssertMarkdown(@"&#35; &#1234; &#992; &#0;", () =>
		{
			Paragraph(0..25, () =>
			{
				Literal(0..25, "# Ӓ Ϡ \uFFFD");
			});
		});
	}
	/// <summary>
	/// 十六进制数字字符引用。
	/// https://spec.commonmark.org/0.30/#example-26
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-27"/>
	[TestMethod]
	public void Test27()
	{
		AssertMarkdown(@"&#X22; &#XD06; &#xcab;", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..22, "\u0022 ആ ಫ");
			});
		});
	}
	/// <summary>
	/// 非实体。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-28"/>
	[TestMethod]
	public void Test28()
	{
		AssertMarkdown("&nbsp &x; &#; &#x;\n&#87654321;\r\n&#abcdef0;\n&ThisIsNotDefined; &hi?;", () =>
		{
			Paragraph(0..67, () =>
			{
				Literal(0..18, "&nbsp &x; &#; &#x;");
				SoftBreak(18..19);
				Literal(19..30, "&#87654321;");
				SoftBreak(30..32);
				Literal(32..42, "&#abcdef0;");
				SoftBreak(42..43);
				Literal(43..67, "&ThisIsNotDefined; &hi?;");
			});
		});
	}
	/// <summary>
	/// 只允许后跟 ; 的实体。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-29"/>
	[TestMethod]
	public void Test29()
	{
		AssertMarkdown(@"&copy", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..5, "&copy");
			});
		});
	}
	/// <summary>
	/// 不在 HTML5 列表中的实体当作普通字符看待。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-30"/>
	[TestMethod]
	public void Test30()
	{
		AssertMarkdown(@"&MadeUpEntity;", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..14, "&MadeUpEntity;");
			});
		});
	}
	/// <summary>
	/// 除了代码，其他场景都会识别实体。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-31"/>
	[TestMethod]
	public void Test31()
	{
		AssertMarkdown("<a href=\"&ouml;&ouml;.html\">", () =>
		{
			HtmlBlock(0..28, "<a href=\"&ouml;&ouml;.html\">");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-32"/>
	[TestMethod]
	public void Test32()
	{
		AssertMarkdown("[foo](/f&ouml;&ouml; \"f&ouml;&ouml;\")", () =>
		{
			Paragraph(0..37, () =>
			{
				Link(0..37, "/föö", "föö", () =>
				{
					Literal(1..4, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-33"/>
	[TestMethod]
	public void Test33()
	{
		AssertMarkdown("[foo]\r\n\r\n[foo]: /f&ouml;&ouml; \"f&ouml;&ouml;\"", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "/föö", "föö", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(9..46, "foo", "/föö", "föö");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-34"/>
	[TestMethod]
	public void Test34()
	{
		AssertMarkdown("``` f&ouml;&ouml;\r\nfoo\r\n```", () =>
		{
			CodeBlock(0..27, "foo\r\n", "föö");
		});
	}
	/// <summary>
	/// 在代码中会被当作字面量文本。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-35"/>
	[TestMethod]
	public void Test35()
	{
		AssertMarkdown("`f&ouml;&ouml;`", () =>
		{
			Paragraph(0..15, () =>
			{
				CodeSpan(0..15, "f&ouml;&ouml;");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-36"/>
	[TestMethod]
	public void Test36_1()
	{
		AssertMarkdown("    f&ouml;f&ouml;", () =>
		{
			CodeBlock(0..18, "f&ouml;f&ouml;");
		});
	}
	[TestMethod]
	public void Test36_2()
	{
		AssertMarkdown("    f&ouml;f&ouml;\r\n", () =>
		{
			CodeBlock(0..20, "f&ouml;f&ouml;\r\n");
		});
	}
	/// <summary>
	/// 不能当作 Markdown 结构来用
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-37"/>
	[TestMethod]
	public void Test37()
	{
		AssertMarkdown("&#42;foo&#42;\r\n*foo*", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..13, "*foo*");
				SoftBreak(13..15);
				Emphasis(15..20, () =>
				{
					Literal(16..19, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-38"/>
	[TestMethod]
	public void Test38()
	{
		AssertMarkdown("&#42; foo\r\n\r\n* foo", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..9, "* foo");
			});
			UnorderedList(13..18, false, () =>
			{
				ListItem(13..18, () =>
				{
					Paragraph(15..18, () =>
					{
						Literal(15..18, "foo");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-39"/>
	[TestMethod]
	public void Test39()
	{
		AssertMarkdown("foo&#10;&#10;bar", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..16, "foo\n\nbar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-40"/>
	[TestMethod]
	public void Test40()
	{
		AssertMarkdown("&#9;foo", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "\tfoo");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-41"/>
	[TestMethod]
	public void Test41()
	{
		AssertMarkdown("[a](url &quot;tit&quot;)", () =>
		{
			Paragraph(0..24, () =>
			{
				Literal(0..24, "[a](url \"tit\")");
			});
		});
	}
}

