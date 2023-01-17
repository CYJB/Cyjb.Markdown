using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 换行的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#autolinks"/>
[TestClass]
public class UnitTestLineBreak : BaseTest
{
	/// <summary>
	/// 硬换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-633"/>
	[TestMethod]
	public void Test633_1()
	{
		AssertMarkdown("foo  \nbaz", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..6);
				Literal(6..9, "baz");
			});
		});
	}

	[TestMethod]
	public void Test633_2()
	{
		AssertMarkdown("foo  \r\nbaz", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..7);
				Literal(7..10, "baz");
			});
		});
	}
	/// <summary>
	/// 使用行尾的 /。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-634"/>
	[TestMethod]
	public void Test634_1()
	{
		AssertMarkdown("foo\\\nbaz", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..5);
				Literal(5..8, "baz");
			});
		});
	}

	[TestMethod]
	public void Test634_2()
	{
		AssertMarkdown("foo\\\r\nbaz", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..6);
				Literal(6..9, "baz");
			});
		});
	}
	/// <summary>
	/// 可以多于两个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-635"/>
	[TestMethod]
	public void Test635_1()
	{
		AssertMarkdown("foo       \nbaz", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..11);
				Literal(11..14, "baz");
			});
		});
	}

	[TestMethod]
	public void Test635_2()
	{
		AssertMarkdown("foo       \r\nbaz", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..12);
				Literal(12..15, "baz");
			});
		});
	}
	/// <summary>
	/// 忽略下一行的行首空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-636"/>
	[TestMethod]
	public void Test636_1()
	{
		AssertMarkdown("foo  \n     bar", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..11);
				Literal(11..14, "bar");
			});
		});
	}

	[TestMethod]
	public void Test636_2()
	{
		AssertMarkdown("foo  \r\n     bar", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..12);
				Literal(12..15, "bar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-637"/>
	[TestMethod]
	public void Test637_1()
	{
		AssertMarkdown("foo\\\n     bar", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..10);
				Literal(10..13, "bar");
			});
		});
	}

	[TestMethod]
	public void Test637_2()
	{
		AssertMarkdown("foo\\\r\n     bar", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..3, "foo");
				HardBreak(3..11);
				Literal(11..14, "bar");
			});
		});
	}
	/// <summary>
	/// 允许出现在强调范围内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-638"/>
	[TestMethod]
	public void Test638()
	{
		AssertMarkdown("*foo  \r\nbar*", () =>
		{
			Paragraph(0..12, () =>
			{
				Emphasis(0..12, () =>
				{
					Literal(1..4, "foo");
					HardBreak(4..8);
					Literal(8..11, "bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-639"/>
	[TestMethod]
	public void Test639()
	{
		AssertMarkdown("*foo\\\r\nbar*", () =>
		{
			Paragraph(0..11, () =>
			{
				Emphasis(0..11, () =>
				{
					Literal(1..4, "foo");
					HardBreak(4..7);
					Literal(7..10, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 不能出现在代码段内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-640"/>
	[TestMethod]
	public void Test640_1()
	{
		AssertMarkdown("`code  \nspan`", () =>
		{
			Paragraph(0..13, () =>
			{
				CodeSpan(0..13, "code   span");
			});
		});
	}

	[TestMethod]
	public void Test640_2()
	{
		AssertMarkdown("`code  \r\nspan`", () =>
		{
			Paragraph(0..14, () =>
			{
				CodeSpan(0..14, "code   span");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-641"/>
	[TestMethod]
	public void Test641_1()
	{
		AssertMarkdown("`code\\\nspan`", () =>
		{
			Paragraph(0..12, () =>
			{
				CodeSpan(0..12, "code\\ span");
			});
		});
	}
	[TestMethod]
	public void Test641_2()
	{
		AssertMarkdown("`code\\\r\nspan`", () =>
		{
			Paragraph(0..13, () =>
			{
				CodeSpan(0..13, "code\\ span");
			});
		});
	}
	/// <summary>
	/// 也不能出现在 HTML 标签内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-642"/>
	[TestMethod]
	public void Test642_1()
	{
		AssertMarkdown("<a href=\"foo  \nbar\">", () =>
		{
			Paragraph(0..20, () =>
			{
				HtmlStartTag(0..20, "<a href=\"foo  \nbar\">");
			});
		});
	}
	[TestMethod]
	public void Test642_2()
	{
		AssertMarkdown("<a href=\"foo  \r\nbar\">", () =>
		{
			Paragraph(0..21, () =>
			{
				HtmlStartTag(0..21, "<a href=\"foo  \r\nbar\">");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-643"/>
	[TestMethod]
	public void Test643_1()
	{
		AssertMarkdown("<a href=\"foo\\\nbar\">", () =>
		{
			Paragraph(0..19, () =>
			{
				HtmlStartTag(0..19, "<a href=\"foo\\\nbar\">");
			});
		});
	}
	[TestMethod]
	public void Test643_2()
	{
		AssertMarkdown("<a href=\"foo\\\r\nbar\">", () =>
		{
			Paragraph(0..20, () =>
			{
				HtmlStartTag(0..20, "<a href=\"foo\\\r\nbar\">");
			});
		});
	}
	/// <summary>
	/// 在块末尾无效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-644"/>
	[TestMethod]
	public void Test644()
	{
		AssertMarkdown("foo\\", () =>
		{
			Paragraph(0..4, () =>
			{
				Literal(0..4, "foo\\");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-645"/>
	[TestMethod]
	public void Test645()
	{
		AssertMarkdown("foo  ", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "foo");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-646"/>
	[TestMethod]
	public void Test646()
	{
		AssertMarkdown("### foo\\", () =>
		{
			Heading(0..8, 3, () =>
			{
				Literal(4..8, "foo\\");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-647"/>
	[TestMethod]
	public void Test647()
	{
		AssertMarkdown("### foo  ", () =>
		{
			Heading(0..9, 3, () =>
			{
				Literal(4..7, "foo");
			});
		});
	}
	/// <summary>
	/// 软换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-648"/>
	[TestMethod]
	public void Test648_1()
	{
		AssertMarkdown("foo\nbaz", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..4);
				Literal(4..7, "baz");
			});
		});
	}
	[TestMethod]
	public void Test648_2()
	{
		AssertMarkdown("foo\r\nbaz", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..5);
				Literal(5..8, "baz");
			});
		});
	}
	/// <summary>
	/// 行末和行首的空格会被移除。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-649"/>
	[TestMethod]
	public void Test649_1()
	{
		AssertMarkdown("foo \n baz", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..6);
				Literal(6..9, "baz");
			});
		});
		AssertMarkdown("foo \r\n baz", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..7);
				Literal(7..10, "baz");
			});
		});
	}
	[TestMethod]
	public void Test649_2()
	{
		AssertMarkdown("foo \n   baz", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..8);
				Literal(8..11, "baz");
			});
		});
		AssertMarkdown("foo \r\n   baz", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..9);
				Literal(9..12, "baz");
			});
		});
	}
	/// <summary>
	/// 行末的 \t 会被保留，但下一行行首的会被全部移除。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-649"/>
	[TestMethod]
	public void Test649_3()
	{
		AssertMarkdown("foo\t \n \tbaz", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..4, "foo\t");
				SoftBreak(4..8);
				Literal(8..11, "baz");
			});
		});
		AssertMarkdown("foo\t \r\n \tbaz", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..4, "foo\t");
				SoftBreak(4..9);
				Literal(9..12, "baz");
			});
		});
	}
	[TestMethod]
	public void Test649_4()
	{
		AssertMarkdown("foo\t \n \t  \t\tbaz", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..4, "foo\t");
				SoftBreak(4..12);
				Literal(12..15, "baz");
			});
		});
		AssertMarkdown("foo\t \r\n \t  \t\tbaz", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..4, "foo\t");
				SoftBreak(4..13);
				Literal(13..16, "baz");
			});
		});
	}
}

