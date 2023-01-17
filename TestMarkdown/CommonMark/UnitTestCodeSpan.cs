using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 行内代码片段的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#backslash-escapes"/>
[TestClass]
public class UnitTestCodeSpan : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-327"/>
	[TestMethod]
	public void Test327()
	{
		AssertMarkdown("`hi`lo`", () =>
		{
			Paragraph(0..7, () =>
			{
				CodeSpan(0..4, "hi");
				Literal(4..7, "lo`");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-328"/>
	[TestMethod]
	public void Test328()
	{
		AssertMarkdown("`foo`", () =>
		{
			Paragraph(0..5, () =>
			{
				CodeSpan(0..5, "foo");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-329"/>
	[TestMethod]
	public void Test329()
	{
		AssertMarkdown("`` foo ` bar ``", () =>
		{
			Paragraph(0..15, () =>
			{
				CodeSpan(0..15, "foo ` bar");
			});
		});
	}
	/// <summary>
	/// 移除前后空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-330"/>
	[TestMethod]
	public void Test330()
	{
		AssertMarkdown("` `` `", () =>
		{
			Paragraph(0..6, () =>
			{
				CodeSpan(0..6, "``");
			});
		});
	}
	/// <summary>
	/// 只有一个空格会被移除。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-331"/>
	[TestMethod]
	public void Test331()
	{
		AssertMarkdown("`  ``  `", () =>
		{
			Paragraph(0..8, () =>
			{
				CodeSpan(0..8, " `` ");
			});
		});
	}
	/// <summary>
	/// 只会在两端都有空格时才会移除。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-332"/>
	[TestMethod]
	public void Test332()
	{
		AssertMarkdown("` a`", () =>
		{
			Paragraph(0..4, () =>
			{
				CodeSpan(0..4, " a");
			});
		});
	}
	/// <summary>
	/// 只移除空格，不移除 Unicode 空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-333"/>
	[TestMethod]
	public void Test333()
	{
		AssertMarkdown("` b `", () =>
		{
			Paragraph(0..5, () =>
			{
				CodeSpan(0..5, " b ");
			});
		});
	}
	/// <summary>
	/// 代码中只包含空格时也不移除。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-334"/>
	[TestMethod]
	public void Test334()
	{
		AssertMarkdown("` `\n`  `", () =>
		{
			Paragraph(0..8, () =>
			{
				CodeSpan(0..3, " ");
				SoftBreak(3..4);
				CodeSpan(4..8, "  ");
			});
		});
		AssertMarkdown("` `\r\n`  `", () =>
		{
			Paragraph(0..9, () =>
			{
				CodeSpan(0..3, " ");
				SoftBreak(3..5);
				CodeSpan(5..9, "  ");
			});
		});
	}
	/// <summary>
	/// 换行被当作空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-335"/>
	[TestMethod]
	public void Test335()
	{
		AssertMarkdown("``\nfoo\nbar  \nbaz\n``", () =>
		{
			Paragraph(0..19, () =>
			{
				CodeSpan(0..19, "foo bar   baz");
			});
		});
		AssertMarkdown("``\r\nfoo\r\nbar  \r\nbaz\r\n``", () =>
		{
			Paragraph(0..23, () =>
			{
				CodeSpan(0..23, "foo bar   baz");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-336"/>
	[TestMethod]
	public void Test336()
	{
		AssertMarkdown("``\nfoo \n``", () =>
		{
			Paragraph(0..10, () =>
			{
				CodeSpan(0..10, "foo ");
			});
		});
		AssertMarkdown("``\r\nfoo \r\n``", () =>
		{
			Paragraph(0..12, () =>
			{
				CodeSpan(0..12, "foo ");
			});
		});
	}
	/// <summary>
	/// 内部空白不会被折叠。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-337"/>
	[TestMethod]
	public void Test337()
	{
		AssertMarkdown("`foo   bar \nbaz`", () =>
		{
			Paragraph(0..16, () =>
			{
				CodeSpan(0..16, "foo   bar  baz");
			});
		});
		AssertMarkdown("`foo   bar \r\nbaz`", () =>
		{
			Paragraph(0..17, () =>
			{
				CodeSpan(0..17, "foo   bar  baz");
			});
		});
	}
	/// <summary>
	/// 会忽略内部的反斜杠转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-338"/>
	[TestMethod]
	public void Test338()
	{
		AssertMarkdown(@"`foo\`bar`", () =>
		{
			Paragraph(0..10, () =>
			{
				CodeSpan(0..6, @"foo\");
				Literal(6..10, "bar`");
			});
		});
	}
	/// <summary>
	/// 使用不同的反斜杠个数。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-339"/>
	[TestMethod]
	public void Test339()
	{
		AssertMarkdown(@"``foo`bar``", () =>
		{
			Paragraph(0..11, () =>
			{
				CodeSpan(0..11, @"foo`bar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-340"/>
	[TestMethod]
	public void Test340()
	{
		AssertMarkdown(@"` foo `` bar `", () =>
		{
			Paragraph(0..14, () =>
			{
				CodeSpan(0..14, @"foo `` bar");
			});
		});
	}
	/// <summary>
	/// 优先级高于强调符号。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-341"/>
	[TestMethod]
	public void Test341()
	{
		AssertMarkdown(@"*foo`*`", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..4, "*foo");
				CodeSpan(4..7, "*");
			});
		});
	}
	/// <summary>
	/// 优先级高于链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-342"/>
	[TestMethod]
	public void Test342()
	{
		AssertMarkdown(@"[not a `link](/foo`)", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..7, "[not a ");
				CodeSpan(7..19, "link](/foo");
				Literal(19..20, ")");
			});
		});
	}
	/// <summary>
	/// 优先级与 HTML 标签、自动链接一致。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-343"/>
	[TestMethod]
	public void Test343()
	{
		AssertMarkdown("`<a href=\"`\">`", () =>
		{
			Paragraph(0..14, () =>
			{
				CodeSpan(0..11, "<a href=\"");
				Literal(11..14, "\">`");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-344"/>
	[TestMethod]
	public void Test344()
	{
		AssertMarkdown("<a href=\"`\">`", () =>
		{
			Paragraph(0..13, () =>
			{
				HtmlStartTag(0..12, "<a href=\"`\">");
				Literal(12..13, "`");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-345"/>
	[TestMethod]
	public void Test345()
	{
		AssertMarkdown("`<http://foo.bar.`baz>`", () =>
		{
			Paragraph(0..23, () =>
			{
				CodeSpan(0..18, "<http://foo.bar.");
				Literal(18..23, "baz>`");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-346"/>
	[TestMethod]
	public void Test346()
	{
		AssertMarkdown("<http://foo.bar.`baz>`", () =>
		{
			Paragraph(0..22, () =>
			{
				Link(0..21, "http://foo.bar.`baz", null, () =>
				{
					Literal(1..20, "http://foo.bar.`baz");
				});
				Literal(21..22, "`");
			});
		});
	}
	/// <summary>
	/// 未被匹配关闭的直接当作普通文本。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-347"/>
	[TestMethod]
	public void Test347()
	{
		AssertMarkdown("```foo``", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "```foo``");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-348"/>
	[TestMethod]
	public void Test348()
	{
		AssertMarkdown("`foo", () =>
		{
			Paragraph(0..4, () =>
			{
				Literal(0..4, "`foo");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-349"/>
	[TestMethod]
	public void Test349()
	{
		AssertMarkdown("`foo``bar``", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..4, "`foo");
				CodeSpan(4..11, "bar");
			});
		});
	}
}

