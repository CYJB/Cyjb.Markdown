using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// ATX 标题的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#atx-headings"/>
[TestClass]
public class UnitTestATXHeading : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-62"/>
	[TestMethod]
	public void Test62()
	{
		AssertCommonMark("# foo\r\n## foo\r\n### foo\r\n#### foo\r\n##### foo\r\n###### foo\r\n", () =>
		{
			Heading(0..7, 1, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(7..15, 2, () =>
			{
				Literal(10..13, "foo");
			});
			Heading(15..24, 3, () =>
			{
				Literal(19..22, "foo");
			});
			Heading(24..34, 4, () =>
			{
				Literal(29..32, "foo");
			});
			Heading(34..45, 5, () =>
			{
				Literal(40..43, "foo");
			});
			Heading(45..57, 6, () =>
			{
				Literal(52..55, "foo");
			});
		});
	}
	/// <summary>
	/// 多于 6 个 # 不是标题。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-63"/>
	[TestMethod]
	public void Test63()
	{
		AssertMarkdown("####### foo\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..11, "####### foo");
			});
		});
	}
	/// <summary>
	/// # 后至少需要一个空格或 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-64"/>
	[TestMethod]
	public void Test64()
	{
		AssertMarkdown("#5 bolt\r\n\r\n#hashtag\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "#5 bolt");
			});
			Paragraph(11..21, () =>
			{
				Literal(11..19, "#hashtag");
			});
		});
	}
	/// <summary>
	/// 首个 # 被转义，不是标题。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-65"/>
	[TestMethod]
	public void Test65()
	{
		AssertMarkdown("\\## foo\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "## foo");
			});
		});
	}
	/// <summary>
	/// 标题的内容作为行级节点解析。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-66"/>
	[TestMethod]
	public void Test66()
	{
		AssertCommonMark("# foo *bar* \\*baz\\*\r\n", () =>
		{
			Heading(0..21, 1, () =>
			{
				Literal(2..6, "foo ");
				Emphasis(6..11, () =>
				{
					Literal(7..10, "bar");
				});
				Literal(11..19, " *baz*");
			});
		});
	}
	/// <summary>
	/// 起始和结束空格会被忽略。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-67"/>
	[TestMethod]
	public void Test67()
	{
		AssertCommonMark("#                  foo                     \r\n", () =>
		{
			Heading(0..45, 1, () =>
			{
				Literal(19..22, "foo");
			});
		});
	}
	/// <summary>
	/// 最多允许三个空格的缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-68"/>
	[TestMethod]
	public void Test68()
	{
		AssertCommonMark(" ### foo\r\n  ## foo\r\n   # foo\r\n", () =>
		{
			Heading(1..10, 3, () =>
			{
				Literal(5..8, "foo");
			});
			Heading(12..20, 2, () =>
			{
				Literal(15..18, "foo");
			});
			Heading(23..30, 1, () =>
			{
				Literal(25..28, "foo");
			});
		});
	}
	/// <summary>
	/// 四个空格就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-69"/>
	[TestMethod]
	public void Test69()
	{
		AssertMarkdown("    # foo\r\n", () =>
		{
			CodeBlock(0..11, "# foo\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-70"/>
	[TestMethod]
	public void Test70()
	{
		AssertMarkdown("foo\r\n    # bar\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..9);
				Literal(9..14, "# bar");
			});
		});
	}
	/// <summary>
	/// 闭合的 # 是可选的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-71"/>
	[TestMethod]
	public void Test71()
	{
		AssertCommonMark("## foo ##\r\n  ###   bar    ###\r\n", () =>
		{
			Heading(0..11, 2, () =>
			{
				Literal(3..6, "foo");
			});
			Heading(13..31, 3, () =>
			{
				Literal(19..22, "bar");
			});
		});
	}
	/// <summary>
	/// 闭合的 # 个数不需要与起始 # 个数相同。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-72"/>
	[TestMethod]
	public void Test72()
	{
		AssertCommonMark("# foo ##################################\r\n##### foo ##\r\n", () =>
		{
			Heading(0..42, 1, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(42..56, 5, () =>
			{
				Literal(48..51, "foo");
			});
		});
	}
	/// <summary>
	/// 闭合 # 后允许出现空格或 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-73"/>
	[TestMethod]
	public void Test73()
	{
		AssertCommonMark("### foo ###     \r\n", () =>
		{
			Heading(0..18, 3, () =>
			{
				Literal(4..7, "foo");
			});
		});
	}
	/// <summary>
	/// 闭合 # 后不允许出现非空白字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-74"/>
	[TestMethod]
	public void Test74()
	{
		AssertCommonMark("### foo ### b\r\n", () =>
		{
			Heading(0..15, 3, () =>
			{
				Literal(4..13, "foo ### b");
			});
		});
	}
	/// <summary>
	/// 闭合 # 前必须包含空格或 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-75"/>
	[TestMethod]
	public void Test75()
	{
		AssertCommonMark("# foo#\r\n", () =>
		{
			Heading(0..8, 1, () =>
			{
				Literal(2..6, "foo#");
			});
		});
	}
	/// <summary>
	/// 被转义的 # 不计入闭合序列中。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-76"/>
	[TestMethod]
	public void Test76()
	{
		AssertCommonMark("### foo \\###\r\n## foo #\\##\r\n# foo \\#\r\n", () =>
		{
			Heading(0..14, 3, () =>
			{
				Literal(4..12, "foo ###");
			});
			Heading(14..27, 2, () =>
			{
				Literal(17..25, "foo ###");
			});
			Heading(27..37, 1, () =>
			{
				Literal(29..35, "foo #");
			});
		});
	}
	/// <summary>
	/// ATX 标题不需要使用空行分割，并且可以中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-77"/>
	[TestMethod]
	public void Test77()
	{
		AssertCommonMark("****\r\n## foo\r\n****\r\n", () =>
		{
			ThematicBreak(0..6);
			Heading(6..14, 2, () =>
			{
				Literal(9..12, "foo");
			});
			ThematicBreak(14..20);
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-78"/>
	[TestMethod]
	public void Test78()
	{
		AssertCommonMark("Foo bar\r\n# baz\r\nBar foo\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "Foo bar");
			});
			Heading(9..16, 1, () =>
			{
				Literal(11..14, "baz");
			});
			Paragraph(16..25, () =>
			{
				Literal(16..23, "Bar foo");
			});
		});
	}
	/// <summary>
	/// ATX 标题可以是空的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-79"/>
	[TestMethod]
	public void Test79()
	{
		AssertCommonMark("## \r\n#\r\n### ###\r\n", () =>
		{
			Heading(0..5, 2);
			Heading(5..8, 1);
			Heading(8..17, 3);
		});
	}
}

