using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 转义字符串的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#backslash-escapes"/>
[TestClass]
public class UnitTestEscapes : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-12"/>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown(@"\!\""\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~", () =>
		{
			Paragraph(0..64, () =>
			{
				Literal(0..64, @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-13"/>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown(@"\→\A\a\ \3\φ\«", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..14, @"\→\A\a\ \3\φ\«");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-14"/>
	[TestMethod]
	public void Test14()
	{
		AssertMarkdown(@"\*not emphasized*
\<br/> not a tag
\[not a link](/foo)
\`not code`
1\. not a list
\* not a list
\# not a heading
\[foo]: /url ""not a reference""
\&ouml; not a character entity", () =>
		{
			Paragraph(0..182, () =>
			{
				Literal(0..17, @"*not emphasized*");
				SoftBreak(17..19);
				Literal(19..35, @"<br/> not a tag");
				SoftBreak(35..37);
				Literal(37..56, @"[not a link](/foo)");
				SoftBreak(56..58);
				Literal(58..69, @"`not code`");
				SoftBreak(69..71);
				Literal(71..85, @"1. not a list");
				SoftBreak(85..87);
				Literal(87..100, @"* not a list");
				SoftBreak(100..102);
				Literal(102..118, @"# not a heading");
				SoftBreak(118..120);
				Literal(120..150, @"[foo]: /url ""not a reference""");
				SoftBreak(150..152);
				Literal(152..182, @"&ouml; not a character entity");
			});
		});
	}
	/// <summary>
	/// 如果反斜杠本身被转义了，之后的字符不会被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-15"/>
	[TestMethod]
	public void Test15()
	{
		AssertMarkdown("\\\\*emphasis*\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..2, "\\");
				Emphasis(2..12, () =>
				{
					Literal(3..11, "emphasis");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-16"/>
	[TestMethod]
	public void Test16_1()
	{
		AssertMarkdown("foo\\\nbar", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, @"foo");
				HardBreak(3..5);
				Literal(5..8, "bar");
			});
		});
	}
	[TestMethod]
	public void Test16_2()
	{
		AssertMarkdown("foo\\\r\nbar", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..3, @"foo");
				HardBreak(3..6);
				Literal(6..9, "bar");
			});
		});
	}
	/// <summary>
	/// 反斜杠转义在代码块、代码段、自动链接和 HTML 中不生效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-17"/>
	[TestMethod]
	public void Test17()
	{
		AssertMarkdown("`` \\[\\` ``\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				CodeSpan(0..10, "\\[\\`");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-18"/>
	[TestMethod]
	public void Test18()
	{
		AssertMarkdown("    \\[\\]\r\n", () =>
		{
			CodeBlock(0..10, "\\[\\]\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-19"/>
	[TestMethod]
	public void Test19()
	{
		AssertMarkdown("~~~\r\n\\[\\]\r\n~~~\r\n", () =>
		{
			CodeBlock(0..16, "\\[\\]\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-20"/>
	[TestMethod]
	public void Test20()
	{
		AssertMarkdown("<https://example.com?find=\\*>\r\n", () =>
		{
			Paragraph(0..31, () =>
			{
				Link(0..29, "https://example.com?find=\\*", null, () =>
				{
					Literal(1..28, "https://example.com?find=\\*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-21"/>
	[TestMethod]
	public void Test21()
	{
		AssertMarkdown("<a href=\"/bar\\/)\">\r\n", () =>
		{
			HtmlBlock(0..20, "<a href=\"/bar\\/)\">\r\n");
		});
	}
	/// <summary>
	/// 但是在其他上下文中都能够生效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-22"/>
	[TestMethod]
	public void Test22()
	{
		AssertMarkdown("[foo](/bar\\* \"ti\\*tle\")\r\n", () =>
		{
			Paragraph(0..25, () =>
			{
				Link(0..23, "/bar*", "ti*tle", () =>
				{
					Literal(1..4, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-23"/>
	[TestMethod]
	public void Test23()
	{
		AssertMarkdown("[foo]\r\n\r\n[foo]: /bar\\* \"ti\\*tle\"\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "/bar*", "ti*tle", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(9..34, "foo", "/bar*", "ti*tle");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-24"/>
	[TestMethod]
	public void Test24()
	{
		AssertMarkdown("``` foo\\+bar\r\nfoo\r\n```\r\n", () =>
		{
			CodeBlock(0..24, "foo\r\n", "foo+bar");
		});
	}
}

