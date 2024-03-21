using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// <see cref="Cyjb.Markdown.ThematicBreak"/> 的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#thematic-breaks"/>
[TestClass]
public class UnitTestThematicBreak : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-43"/>
	[TestMethod]
	public void Test43()
	{
		AssertMarkdown("***\n---\n___", () =>
		{
			ThematicBreak(0..4);
			ThematicBreak(4..8);
			ThematicBreak(8..11);
		});
		AssertMarkdown("***\r\n---\r\n___", () =>
		{
			ThematicBreak(0..5);
			ThematicBreak(5..10);
			ThematicBreak(10..13);
		});
	}

	/// <summary>
	/// 错误的字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-44"/>
	[TestMethod]
	public void Test44()
	{
		AssertMarkdown("+++", () =>
		{
			Paragraph(0..3, () =>
			{
				Literal(0..3, "+++");
			});
		});
	}

	/// <see href="https://spec.commonmark.org/0.31.2/#example-45"/>
	[TestMethod]
	public void Test45()
	{
		AssertMarkdown("===", () =>
		{
			Paragraph(0..3, () =>
			{
				Literal(0..3, "===");
			});
		});
	}

	/// <summary>
	/// 字符数不足。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-46"/>
	[TestMethod]
	public void Test46()
	{
		AssertMarkdown("--\n**\n__", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..2, "--");
				SoftBreak(2..3);
				Literal(3..5, "**");
				SoftBreak(5..6);
				Literal(6..8, "__");
			});
		});
		AssertMarkdown("--\r\n**\r\n__", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..2, "--");
				SoftBreak(2..4);
				Literal(4..6, "**");
				SoftBreak(6..8);
				Literal(8..10, "__");
			});
		});
	}

	/// <summary>
	/// 最多允许三个空格缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-47"/>
	[TestMethod]
	public void Test47()
	{
		AssertMarkdown(" ***\n  ***\n   ***\n", () =>
		{
			ThematicBreak(1..5);
			ThematicBreak(7..11);
			ThematicBreak(14..18);
		});
		AssertMarkdown(" ***\r\n  ***\r\n   ***\r\n", () =>
		{
			ThematicBreak(1..6);
			ThematicBreak(8..13);
			ThematicBreak(16..21);
		});
	}

	/// <summary>
	/// 四个空格缩进就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-48"/>
	[TestMethod]
	public void Test48()
	{
		AssertMarkdown("    ***", () =>
		{
			CodeBlock(0..7, "***");
		});
	}

	/// <see href="https://spec.commonmark.org/0.31.2/#example-49"/>
	[TestMethod]
	public void Test49()
	{
		AssertMarkdown("Foo\n    ***", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..8);
				Literal(8..11, "***");
			});
		});
		AssertMarkdown("Foo\r\n    ***", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..9);
				Literal(9..12, "***");
			});
		});
	}

	/// <summary>
	/// 也可以多于三个字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-50"/>
	[TestMethod]
	public void Test50()
	{
		AssertMarkdown("_____________________________________", () =>
		{
			ThematicBreak(0..37);
		});
	}

	/// <summary>
	/// 字符中间可以存在空格和 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-51"/>
	[TestMethod]
	public void Test51()
	{
		AssertMarkdown(" - - -", () =>
		{
			ThematicBreak(1..6);
		});
	}

	/// <see href="https://spec.commonmark.org/0.31.2/#example-52"/>
	[TestMethod]
	public void Test52()
	{
		AssertMarkdown(" **  * ** * ** * **", () =>
		{
			ThematicBreak(1..19);
		});
	}

	/// <see href="https://spec.commonmark.org/0.31.2/#example-53"/>
	[TestMethod]
	public void Test53()
	{
		AssertMarkdown("-     -      -      -", () =>
		{
			ThematicBreak(0..21);
		});
	}

	/// <summary>
	/// 允许末尾的空格和 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-54"/>
	[TestMethod]
	public void Test54()
	{
		AssertMarkdown("- - - -    ", () =>
		{
			ThematicBreak(0..11);
		});
	}

	/// <summary>
	/// 但是行内不能存在其它字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-55"/>
	[TestMethod]
	public void Test55()
	{
		AssertMarkdown("_ _ _ _ a\n\na------\n\n---a---", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..9, "_ _ _ _ a");
			});
			Paragraph(11..19, () =>
			{
				Literal(11..18, "a------");
			});
			Paragraph(20..27, () =>
			{
				Literal(20..27, "---a---");
			});
		});
		AssertMarkdown("_ _ _ _ a\r\n\r\na------\r\n\r\n---a---", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..9, "_ _ _ _ a");
			});
			Paragraph(13..22, () =>
			{
				Literal(13..20, "a------");
			});
			Paragraph(24..31, () =>
			{
				Literal(24..31, "---a---");
			});
		});
	}

	/// <summary>
	/// 空格和 Tab 之外的所有字符都必须是相同的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-56"/>
	[TestMethod]
	public void Test56()
	{
		AssertMarkdown(" *-*", () =>
		{
			Paragraph(1..4, () =>
			{
				Emphasis(1..4, () =>
				{
					Literal(2..3, "-");
				});
			});
		});
	}

	/// <summary>
	/// 分割线前后不需要空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-57"/>
	[TestMethod]
	public void Test57()
	{
		AssertMarkdown("- foo\r\n***\r\n- bar", () =>
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
			ThematicBreak(7..12);
			UnorderedList(12..17, false, () =>
			{
				ListItem(12..17, () =>
				{
					Paragraph(14..17, () =>
					{
						Literal(14..17, "bar");
					});
				});
			});
		});
	}

	/// <summary>
	/// 分割线可以中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-58"/>
	[TestMethod]
	public void Test58()
	{
		AssertMarkdown("Foo\n***\nbar", () =>
		{
			Paragraph(0..4, () =>
			{
				Literal(0..3, "Foo");
			});
			ThematicBreak(4..8);
			Paragraph(8..11, () =>
			{
				Literal(8..11, "bar");
			});
		});
		AssertMarkdown("Foo\r\n***\r\nbar", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "Foo");
			});
			ThematicBreak(5..10);
			Paragraph(10..13, () =>
			{
				Literal(10..13, "bar");
			});
		});
	}

	/// <summary>
	/// 标题之后的 --- 分割线可能被识别成 setext 头的下划线。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-59"/>
	[TestMethod]
	public void Test59()
	{
		AssertCommonMark("Foo\r\n---\r\nbar", () =>
		{
			Heading(0..10, 2, () =>
			{
				Literal(0..3, "Foo");
			});
			Paragraph(10..13, () =>
			{
				Literal(10..13, "bar");
			});
		});
	}

	/// <summary>
	/// 如果即可能是列表的一部分，有可能是分割线，那么优先被识别为分割线。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-60"/>
	[TestMethod]
	public void Test60()
	{
		AssertMarkdown("* Foo\r\n* * *\r\n* Bar", () =>
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
			ThematicBreak(7..14);
			UnorderedList(14..19, false, () =>
			{
				ListItem(14..19, () =>
				{
					Paragraph(16..19, () =>
					{
						Literal(16..19, "Bar");
					});
				});
			});
		});
	}

	/// <summary>
	/// 如果希望使用列表项中的分割线，使用不同的字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-61"/>
	[TestMethod]
	public void Test61()
	{
		AssertMarkdown("- Foo\r\n- * * *", () =>
		{
			UnorderedList(0..14, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "Foo");
					});
				});
				ListItem(7..14, () =>
				{
					ThematicBreak(9..14);
				});
			});
		});
	}
}
