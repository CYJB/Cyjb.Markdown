using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// <see cref="LinkDestination"/> 的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#link-reference-definitions"/>
[TestClass]
public partial class UnitTestLinkDefinition : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-192"/>
	[TestMethod]
	public void Test192()
	{
		AssertMarkdown("[foo]: /url \"title\"\r\n", () =>
		{
			LinkDefinition(0..21, "foo", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-193"/>
	[TestMethod]
	public void Test193()
	{
		AssertMarkdown("   [foo]: \r\n      /url  \r\n           'the title'  \r\n\r\n[foo]\r\n", () =>
		{
			LinkDefinition(3..52, "foo", "/url", "the title");
			Paragraph(54..61, () =>
			{
				Link(54..59, "/url", "the title", () =>
				{
					Literal(55..58, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-194"/>
	[TestMethod]
	public void Test194()
	{
		AssertMarkdown("[Foo*bar\\]]:my_(url) 'title (with parens)'\r\n\r\n[Foo*bar\\]]\r\n", () =>
		{
			LinkDefinition(0..44, "Foo*bar\\]", "my_(url)", "title (with parens)");
			Paragraph(46..59, () =>
			{
				Link(46..57, "my_(url)", "title (with parens)", () =>
				{
					Literal(47..56, "Foo*bar]");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-195"/>
	[TestMethod]
	public void Test195()
	{
		AssertMarkdown("[Foo bar]:\r\n<my url>\r\n'title'\r\n\r\n[Foo bar]\r\n", () =>
		{
			LinkDefinition(0..31, "Foo bar", "my url", "title");
			Paragraph(33..44, () =>
			{
				Link(33..42, "my url", "title", () =>
				{
					Literal(34..41, "Foo bar");
				});
			});
		});
	}
	/// <summary>
	/// 标题可以跨多行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-196"/>
	[TestMethod]
	public void Test196()
	{
		AssertMarkdown("[foo]: /url '\r\ntitle\r\nline1\r\nline2\r\n'\r\n\r\n[foo]\r\n", () =>
		{
			LinkDefinition(0..39, "foo", "/url", "\r\ntitle\r\nline1\r\nline2\r\n");
			Paragraph(41..48, () =>
			{
				Link(41..46, "/url", "\r\ntitle\r\nline1\r\nline2\r\n", () =>
				{
					Literal(42..45, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 但是不能包含空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-197"/>
	[TestMethod]
	public void Test197()
	{
		AssertMarkdown("[foo]: /url 'title\r\n\r\nwith blank line'\r\n\r\n[foo]\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..18, "[foo]: /url 'title");
			});
			Paragraph(22..40, () =>
			{
				Literal(22..38, "with blank line'");
			});
			Paragraph(42..49, () =>
			{
				Literal(42..47, "[foo]");
			});
		});
	}
	/// <summary>
	/// 链接标题可以省略。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-198"/>
	[TestMethod]
	public void Test198()
	{
		AssertMarkdown("[foo]:\r\n/url\r\n\r\n[foo]\r\n", () =>
		{
			LinkDefinition(0..14, "foo", "/url");
			Paragraph(16..23, () =>
			{
				Link(16..21, "/url", null, () =>
				{
					Literal(17..20, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 链接目标不可以省略。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-199"/>
	[TestMethod]
	public void Test199()
	{
		AssertMarkdown("[foo]:\r\n\r\n[foo]\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "[foo]:");
			});
			Paragraph(10..17, () =>
			{
				Literal(10..15, "[foo]");
			});
		});
	}
	/// <summary>
	/// 但是空的链接目标可以使用尖括号括起来。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-200"/>
	[TestMethod]
	public void Test200()
	{
		AssertMarkdown("[foo]: <>\r\n\r\n[foo]\r\n", () =>
		{
			LinkDefinition(0..11, "foo", "");
			Paragraph(13..20, () =>
			{
				Link(13..18, "", null, () =>
				{
					Literal(14..17, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 标题与目标间必须使用空格或 Tab 分割。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-201"/>
	[TestMethod]
	public void Test201()
	{
		AssertMarkdown("[foo]: <bar>(baz)\r\n\r\n[foo]\r\n", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..7, "[foo]: ");
				HtmlStartTag(7..12, "<bar>");
				Literal(12..17, "(baz)");
			});
			Paragraph(21..28, () =>
			{
				Literal(21..26, "[foo]");
			});
		});
	}
	/// <summary>
	/// 目标和标题都可以被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-202"/>
	[TestMethod]
	public void Test202()
	{
		AssertMarkdown("[foo]: /url\\bar\\*baz \"foo\\\"bar\\baz\"\r\n\r\n[foo]\r\n", () =>
		{
			LinkDefinition(0..37, "foo", "/url\\bar*baz", "foo\"bar\\baz");
			Paragraph(39..46, () =>
			{
				Link(39..44, "/url\\bar*baz", "foo\"bar\\baz", () =>
				{
					Literal(40..43, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 链接可以出现在定义之前。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-203"/>
	[TestMethod]
	public void Test203()
	{
		AssertMarkdown("[foo]\r\n\r\n[foo]: url\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "url", null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(9..21, "foo", "url");
		});
	}
	/// <summary>
	/// 存在多个匹配时，使用第一个。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-204"/>
	[TestMethod]
	public void Test204()
	{
		AssertMarkdown("[foo]\r\n\r\n[foo]: first\r\n[foo]: second\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "first", null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(9..23, "foo", "first");
			LinkDefinition(23..38, "foo", "second");
		});
	}
	/// <summary>
	/// 标签的匹配是忽略大小写的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-205"/>
	[TestMethod]
	public void Test205()
	{
		AssertMarkdown("[FOO]: /url\r\n\r\n[Foo]\r\n", () =>
		{
			LinkDefinition(0..13, "FOO", "/url");
			Paragraph(15..22, () =>
			{
				Link(15..20, "/url", null, () =>
				{
					Literal(16..19, "Foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-206"/>
	[TestMethod]
	public void Test206()
	{
		AssertMarkdown("[ΑΓΩ]: /φου\r\n\r\n[αγω]\r\n", () =>
		{
			LinkDefinition(0..13, "ΑΓΩ", "/φου");
			Paragraph(15..22, () =>
			{
				Link(15..20, "/φου", null, () =>
				{
					Literal(16..19, "αγω");
				});
			});
		});
	}
	/// <summary>
	/// 链接定义不会生成可见内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-207"/>
	[TestMethod]
	public void Test207()
	{
		AssertMarkdown("[foo]: /url\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-208"/>
	[TestMethod]
	public void Test208()
	{
		AssertMarkdown("[\r\nfoo\r\n]: /url\r\nbar\r\n", () =>
		{
			LinkDefinition(0..17, "\r\nfoo\r\n", "/url");
			Paragraph(17..22, () =>
			{
				Literal(17..20, "bar");
			});
		});
	}
	/// <summary>
	/// 不是链接定义，因为在标题后存在其它内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-209"/>
	[TestMethod]
	public void Test209()
	{
		AssertMarkdown("[foo]: /url \"title\" ok\r\n", () =>
		{
			Paragraph(0..24, () =>
			{
				Literal(0..22, "[foo]: /url \"title\" ok");
			});
		});
	}
	/// <summary>
	/// 是链接定义，但没有标题。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-210"/>
	[TestMethod]
	public void Test210()
	{
		AssertMarkdown("[foo]: /url\r\n\"title\" ok\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
			Paragraph(13..25, () =>
			{
				Literal(13..23, "\"title\" ok");
			});
		});
	}
	/// <summary>
	/// 不是链接定义，因为缩进了 4 个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-211"/>
	[TestMethod]
	public void Test211()
	{
		AssertMarkdown("    [foo]: /url \"title\"\r\n\r\n[foo]\r\n", () =>
		{
			CodeBlock(0..25, "[foo]: /url \"title\"\r\n");
			Paragraph(27..34, () =>
			{
				Literal(27..32, "[foo]");
			});
		});
	}
	/// <summary>
	/// 不是链接定义，因为出现在代码块内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-212"/>
	[TestMethod]
	public void Test212()
	{
		AssertMarkdown("```\r\n[foo]: /url\r\n```\r\n\r\n[foo]\r\n", () =>
		{
			CodeBlock(0..23, "[foo]: /url\r\n");
			Paragraph(25..32, () =>
			{
				Literal(25..30, "[foo]");
			});
		});
	}
	/// <summary>
	/// 链接定义不能中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-213"/>
	[TestMethod]
	public void Test213()
	{
		AssertMarkdown("Foo\r\n[bar]: /baz\r\n\r\n[bar]\r\n", () =>
		{
			Paragraph(0..18, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				Literal(5..16, "[bar]: /baz");
			});
			Paragraph(20..27, () =>
			{
				Literal(20..25, "[bar]");
			});
		});
	}
	/// <summary>
	/// 链接定义可以直接后跟块节点。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-214"/>
	[TestMethod]
	public void Test214()
	{
		AssertCommonMark("# [Foo]\r\n[foo]: /url\r\n> bar\r\n", () =>
		{
			Heading(0..9, 1, () =>
			{
				Link(2..7, "/url", null, () =>
				{
					Literal(3..6, "Foo");
				});
			});
			LinkDefinition(9..22, "foo", "/url");
			Blockquote(22..29, () =>
			{
				Paragraph(24..29, () =>
				{
					Literal(24..27, "bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-215"/>
	[TestMethod]
	public void Test215()
	{
		AssertCommonMark("[foo]: /url\r\nbar\r\n===\r\n[foo]\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
			Heading(13..23, 1, () =>
			{
				Literal(13..16, "bar");
			});
			Paragraph(23..30, () =>
			{
				Link(23..28, "/url", null, () =>
				{
					Literal(24..27, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-216"/>
	[TestMethod]
	public void Test216()
	{
		AssertMarkdown("[foo]: /url\r\n===\r\n[foo]\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
			Paragraph(13..25, () =>
			{
				Literal(13..16, "===");
				SoftBreak(16..18);
				Link(18..23, "/url", null, () =>
				{
					Literal(19..22, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 多个链接定义可以依次紧挨着。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-217"/>
	[TestMethod]
	public void Test217()
	{
		AssertMarkdown("[foo]: /foo-url \"foo\"\r\n[bar]: /bar-url\r\n  \"bar\"\r\n[baz]: /baz-url\r\n\r\n[foo],\r\n[bar],\r\n[baz]\r\n", () =>
		{
			LinkDefinition(0..23, "foo", "/foo-url", "foo");
			LinkDefinition(23..49, "bar", "/bar-url", "bar");
			LinkDefinition(49..66, "baz", "/baz-url");
			Paragraph(68..91, () =>
			{
				Link(68..73, "/foo-url", "foo", () =>
				{
					Literal(69..72, "foo");
				});
				Literal(73..74, ",");
				SoftBreak(74..76);
				Link(76..81, "/bar-url", "bar", () =>
				{
					Literal(77..80, "bar");
				});
				Literal(81..82, ",");
				SoftBreak(82..84);
				Link(84..89, "/baz-url", null, () =>
				{
					Literal(85..88, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 链接定义可以出现在其它块容器内，但会在文档范围生效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-218"/>
	[TestMethod]
	public void Test218()
	{
		AssertMarkdown("[foo]\r\n\r\n> [foo]: /url\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "/url", null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			Blockquote(9..24, () =>
			{
				LinkDefinition(11..24, "foo", "/url");
			});
		});
	}
}

