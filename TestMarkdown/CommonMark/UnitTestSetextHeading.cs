using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// Setext 标题的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#setext-headings"/>
[TestClass]
public partial class UnitTestSetextHeading : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-80"/>
	[TestMethod]
	public void Test80()
	{
		AssertCommonMark("Foo *bar*\r\n=========\r\n\r\nFoo *bar*\r\n---------\r\n", () =>
		{
			Heading(0..22, 1, () =>
			{
				Literal(0..4, "Foo ");
				Emphasis(4..9, () =>
				{
					Literal(5..8, "bar");
				});
			});
			Heading(24..46, 2, () =>
			{
				Literal(24..28, "Foo ");
				Emphasis(28..33, () =>
				{
					Literal(29..32, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 标题可以包含多行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-81"/>
	[TestMethod]
	public void Test81()
	{
		AssertCommonMark("Foo *bar\r\nbaz*\r\n====\r\n", () =>
		{
			Heading(0..22, 1, () =>
			{
				Literal(0..4, "Foo ");
				Emphasis(4..14, () =>
				{
					Literal(5..8, "bar");
					SoftBreak(8..10);
					Literal(10..13, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 标题的内容会移除前后空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-82"/>
	[TestMethod]
	public void Test82()
	{
		AssertCommonMark("  Foo *bar\r\nbaz*\t\r\n====\r\n", () =>
		{
			Heading(2..25, 1, () =>
			{
				Literal(2..6, "Foo ");
				Emphasis(6..16, () =>
				{
					Literal(7..10, "bar");
					SoftBreak(10..12);
					Literal(12..15, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 下划线可以是任意长度。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-83"/>
	[TestMethod]
	public void Test83()
	{
		AssertCommonMark("Foo\r\n-------------------------\r\n\r\nFoo\r\n=\r\n", () =>
		{
			Heading(0..32, 2, () =>
			{
				Literal(0..3, "Foo");
			});
			Heading(34..42, 1, () =>
			{
				Literal(34..37, "Foo");
			});
		});
	}
	/// <summary>
	/// 标题内容最多包含三个空格，不需要与下划线对齐。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-84"/>
	[TestMethod]
	public void Test84()
	{
		AssertCommonMark("   Foo\r\n---\r\n\r\n  Foo\r\n-----\r\n\r\n  Foo\r\n  ===\r\n", () =>
		{
			Heading(3..13, 2, () =>
			{
				Literal(3..6, "Foo");
			});
			Heading(17..29, 2, () =>
			{
				Literal(17..20, "Foo");
			});
			Heading(33..45, 1, () =>
			{
				Literal(33..36, "Foo");
			});
		});
	}
	/// <summary>
	/// 四个空格就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-85"/>
	[TestMethod]
	public void Test85()
	{
		AssertMarkdown("    Foo\r\n    ---\r\n\r\n    Foo\r\n---\r\n", () =>
		{
			CodeBlock(0..29, "Foo\r\n---\r\n\r\nFoo\r\n");
			ThematicBreak(29..34);
		});
	}
	/// <summary>
	/// 下划线可以包含最多三个行首空白，和任意个数行尾空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-86"/>
	[TestMethod]
	public void Test86()
	{
		AssertCommonMark("Foo\r\n   ----      \r\n", () =>
		{
			Heading(0..20, 2, () =>
			{
				Literal(0..3, "Foo");
			});
		});
	}
	/// <summary>
	/// 四个空格就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-87"/>
	[TestMethod]
	public void Test87()
	{
		AssertMarkdown("Foo\r\n    ----\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..9);
				Literal(9..13, "----");
			});
		});
	}
	/// <summary>
	/// 下划线不能包含内部空格或 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-88"/>
	[TestMethod]
	public void Test88()
	{
		AssertMarkdown("Foo\r\n= =\r\n\r\nFoo\r\n--- -\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				Literal(5..8, "= =");
			});
			Paragraph(12..17, () =>
			{
				Literal(12..15, "Foo");
			});
			ThematicBreak(17..24);
		});
	}
	/// <summary>
	/// 标题内容后的空格不会引起硬换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-89"/>
	[TestMethod]
	public void Test89()
	{
		AssertCommonMark("Foo  \r\n-----\r\n", () =>
		{
			Heading(0..14, 2, () =>
			{
				Literal(0..3, "Foo");
			});
		});
	}
	/// <summary>
	/// 包括结尾的 \。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-90"/>
	[TestMethod]
	public void Test90()
	{
		AssertCommonMark("Foo\\\r\n----\r\n", () =>
		{
			Heading(0..12, 2, () =>
			{
				Literal(0..4, "Foo\\");
			});
		});
	}
	/// <summary>
	/// 块结构的优先级大于行内结构。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-91"/>
	[TestMethod]
	public void Test91()
	{
		AssertCommonMark("`Foo\r\n----\r\n`\r\n\r\n<a title=\"a lot\r\n---\r\nof dashes\"/>\r\n", () =>
		{
			Heading(0..12, 2, () =>
			{
				Literal(0..4, "`Foo");
			});
			Paragraph(12..15, () =>
			{
				Literal(12..13, "`");
			});
			Heading(17..39, 2, () =>
			{
				Literal(17..32, "<a title=\"a lot");
			});
			Paragraph(39..53, () =>
			{
				Literal(39..51, "of dashes\"/>");
			});
		});
	}
	/// <summary>
	/// 标题不能是列表项或引用中的延迟延伸行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-92"/>
	[TestMethod]
	public void Test92()
	{
		AssertMarkdown("> Foo\r\n---\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "Foo");
				});
			});
			ThematicBreak(7..12);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-93"/>
	[TestMethod]
	public void Test93()
	{
		AssertMarkdown("> foo\r\nbar\r\n===\r\n", () =>
		{
			Blockquote(0..17, () =>
			{
				Paragraph(2..17, () =>
				{
					Literal(2..5, "foo");
					SoftBreak(5..7);
					Literal(7..10, "bar");
					SoftBreak(10..12);
					Literal(12..15, "===");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-94"/>
	[TestMethod]
	public void Test94()
	{
		AssertMarkdown("- Foo\r\n---\r\n", () =>
		{
			UnorderedList(0..7, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "Foo");
					});
				});
			});
			ThematicBreak(7..12);
		});
	}
	/// <summary>
	/// 段落和之后的 Setext 标题之间要有空行，否则都会被识别为标题。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-95"/>
	[TestMethod]
	public void Test95()
	{
		AssertCommonMark("Foo\r\nBar\r\n---\r\n", () =>
		{
			Heading(0..15, 2, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				Literal(5..8, "Bar");
			});
		});
	}
	/// <summary>
	/// 但是在 Setext 标题前后不需要空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-96"/>
	[TestMethod]
	public void Test96()
	{
		AssertCommonMark("---\r\nFoo\r\n---\r\nBar\r\n---\r\nBaz\r\n", () =>
		{
			ThematicBreak(0..5);
			Heading(5..15, 2, () =>
			{
				Literal(5..8, "Foo");
			});
			Heading(15..25, 2, () =>
			{
				Literal(15..18, "Bar");
			});
			Paragraph(25..30, () =>
			{
				Literal(25..28, "Baz");
			});
		});
	}
	/// <summary>
	/// Setext 标题不能为空。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-97"/>
	[TestMethod]
	public void Test97()
	{
		AssertMarkdown("\r\n====\r\n", () =>
		{
			Paragraph(2..8, () =>
			{
				Literal(2..6, "====");
			});
		});
	}
	/// <summary>
	/// Setext 标题的文本不能被解释为段落以外的其它块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-98"/>
	[TestMethod]
	public void Test98()
	{
		AssertMarkdown("---\r\n---\r\n", () =>
		{
			ThematicBreak(0..5);
			ThematicBreak(5..10);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-99"/>
	[TestMethod]
	public void Test99()
	{
		AssertMarkdown("- foo\r\n-----\r\n", () =>
		{
			UnorderedList(0..7, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
			});
			ThematicBreak(7..14);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-100"/>
	[TestMethod]
	public void Test100()
	{
		AssertMarkdown("    foo\r\n---\r\n", () =>
		{
			CodeBlock(0..9, "foo\r\n");
			ThematicBreak(9..14);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-101"/>
	[TestMethod]
	public void Test101()
	{
		AssertMarkdown("> foo\r\n-----\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "foo");
				});
			});
			ThematicBreak(7..14);
		});
	}
	/// <summary>
	/// 如果需要 <c>&gt; foo</c> 作为普通文本，需要被反斜杠转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-102"/>
	[TestMethod]
	public void Test102()
	{
		AssertCommonMark("\\> foo\r\n-----\r\n", () =>
		{
			Heading(0..15, 2, () =>
			{
				Literal(0..6, "> foo");
			});
		});
	}
	/// <summary>
	/// 需要避免多行标题，可以插入一个空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-103"/>
	[TestMethod]
	public void Test103()
	{
		AssertCommonMark("Foo\r\n\r\nbar\r\n---\r\nbaz\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "Foo");
			});
			Heading(7..17, 2, () =>
			{
				Literal(7..10, "bar");
			});
			Paragraph(17..22, () =>
			{
				Literal(17..20, "baz");
			});
		});
	}
	/// <summary>
	/// 需要解释为分隔线，可以在分隔线前后插入空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-104"/>
	[TestMethod]
	public void Test104()
	{
		AssertMarkdown("Foo\r\nbar\r\n\r\n---\r\n\r\nbaz\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				Literal(5..8, "bar");
			});
			ThematicBreak(12..17);
			Paragraph(19..24, () =>
			{
				Literal(19..22, "baz");
			});
		});
	}
	/// <summary>
	/// 或者可以使用不能当作 Setext 标题下划线的分割线。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-105"/>
	[TestMethod]
	public void Test105()
	{
		AssertMarkdown("Foo\r\nbar\r\n* * *\r\nbaz\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				Literal(5..8, "bar");
			});
			ThematicBreak(10..17);
			Paragraph(17..22, () =>
			{
				Literal(17..20, "baz");
			});
		});
	}
	/// <summary>
	/// 需要解释为段落，可以使用一个反斜杠转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-106"/>
	[TestMethod]
	public void Test106()
	{
		AssertMarkdown("Foo\r\nbar\r\n\\---\r\nbaz\r\n", () =>
		{
			Paragraph(0..21, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				Literal(5..8, "bar");
				SoftBreak(8..10);
				Literal(10..14, "---");
				SoftBreak(14..16);
				Literal(16..19, "baz");
			});
		});
	}
}

