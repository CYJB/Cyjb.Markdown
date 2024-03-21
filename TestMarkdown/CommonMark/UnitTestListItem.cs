using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 列表项的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#list-items"/>
[TestClass]
public class UnitTestListItem : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-253"/>
	[TestMethod]
	public void Test253()
	{
		AssertMarkdown("A paragraph\r\nwith two lines.\r\n\r\n    indented code\r\n\r\n> A block quote.\r\n", () =>
		{
			Paragraph(0..30, () =>
			{
				Literal(0..11, "A paragraph");
				SoftBreak(11..13);
				Literal(13..28, "with two lines.");
			});
			CodeBlock(32..51, "indented code\r\n");
			Blockquote(53..71, () =>
			{
				Paragraph(55..71, () =>
				{
					Literal(55..69, "A block quote.");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-254"/>
	[TestMethod]
	public void Test254()
	{
		AssertMarkdown("1.  A paragraph\r\n    with two lines.\r\n\r\n        indented code\r\n\r\n    > A block quote.\r\n", () =>
		{
			OrderedNumberList(0..87, true, 1, () =>
			{
				ListItem(0..87, () =>
				{
					Paragraph(4..38, () =>
					{
						Literal(4..15, "A paragraph");
						SoftBreak(15..21);
						Literal(21..36, "with two lines.");
					});
					CodeBlock(44..63, "indented code\r\n");
					Blockquote(69..87, () =>
					{
						Paragraph(71..87, () =>
						{
							Literal(71..85, "A block quote.");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项的内容必须包含足够的缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-255"/>
	[TestMethod]
	public void Test255()
	{
		AssertMarkdown("- one\r\n\r\n two\r\n", () =>
		{
			UnorderedList(0..7, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "one");
					});
				});
			});
			Paragraph(10..15, () =>
			{
				Literal(10..13, "two");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-256"/>
	[TestMethod]
	public void Test256()
	{
		AssertMarkdown("- one\r\n\r\n  two\r\n", () =>
		{
			UnorderedList(0..16, true, () =>
			{
				ListItem(0..16, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "one");
					});
					Paragraph(11..16, () =>
					{
						Literal(11..14, "two");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-257"/>
	[TestMethod]
	public void Test257()
	{
		AssertMarkdown(" -    one\r\n\r\n     two\r\n", () =>
		{
			UnorderedList(1..11, false, () =>
			{
				ListItem(1..11, () =>
				{
					Paragraph(6..11, () =>
					{
						Literal(6..9, "one");
					});
				});
			});
			CodeBlock(13..23, " two\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-258"/>
	[TestMethod]
	public void Test258()
	{
		AssertMarkdown(" -    one\r\n\r\n      two\r\n", () =>
		{
			UnorderedList(1..24, true, () =>
			{
				ListItem(1..24, () =>
				{
					Paragraph(6..11, () =>
					{
						Literal(6..9, "one");
					});
					Paragraph(19..24, () =>
					{
						Literal(19..22, "two");
					});
				});
			});
		});
	}
	/// <summary>
	/// 缩进是按照列计算的，会依赖列表项的父节点。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-259"/>
	[TestMethod]
	public void Test259()
	{
		AssertMarkdown("   > > 1.  one\r\n>>\r\n>>     two\r\n", () =>
		{
			Blockquote(3..32, () =>
			{
				Blockquote(5..32, () =>
				{
					OrderedNumberList(7..32, true, 1, () =>
					{
						ListItem(7..32, () =>
						{
							Paragraph(11..16, () =>
							{
								Literal(11..14, "one");
							});
							Paragraph(27..32, () =>
							{
								Literal(27..30, "two");
							});
						});
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-260"/>
	[TestMethod]
	public void Test260()
	{
		AssertMarkdown(">>- one\r\n>>\r\n  >  > two\r\n", () =>
		{
			Blockquote(0..25, () =>
			{
				Blockquote(1..25, () =>
				{
					UnorderedList(2..9, false, () =>
					{
						ListItem(2..9, () =>
						{
							Paragraph(4..9, () =>
							{
								Literal(4..7, "one");
							});
						});
					});
					Paragraph(20..25, () =>
					{
						Literal(20..23, "two");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项标志和内容之间至少需要一个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-261"/>
	[TestMethod]
	public void Test261()
	{
		AssertMarkdown("-one\r\n\r\n2.two\r\n", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..4, "-one");
			});
			Paragraph(8..15, () =>
			{
				Literal(8..13, "2.two");
			});
		});
	}
	/// <summary>
	/// 列表项可以包含由一个或多个空行分割的块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-262"/>
	[TestMethod]
	public void Test262()
	{
		AssertMarkdown("- foo\r\n\r\n\r\n  bar\r\n", () =>
		{
			UnorderedList(0..18, true, () =>
			{
				ListItem(0..18, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
					Paragraph(13..18, () =>
					{
						Literal(13..16, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项可以包含任何类型的块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-263"/>
	[TestMethod]
	public void Test263()
	{
		AssertMarkdown("1.  foo\r\n\r\n    ```\r\n    bar\r\n    ```\r\n\r\n    baz\r\n\r\n    > bam\r\n", () =>
		{
			OrderedNumberList(0..62, true, 1, () =>
			{
				ListItem(0..62, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
					CodeBlock(15..38, "bar\r\n");
					Paragraph(44..49, () =>
					{
						Literal(44..47, "baz");
					});
					Blockquote(55..62, () =>
					{
						Paragraph(57..62, () =>
						{
							Literal(57..60, "bam");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项内的缩进代码块会保留空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-264"/>
	[TestMethod]
	public void Test264()
	{
		AssertMarkdown("- Foo\r\n\r\n      bar\r\n\r\n\r\n      baz\r\n", () =>
		{
			UnorderedList(0..35, true, () =>
			{
				ListItem(0..35, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "Foo");
					});
					CodeBlock(11..35, "bar\r\n\r\n\r\nbaz\r\n");
				});
			});
		});
	}
	/// <summary>
	/// 有序列表可以包含 9 个或更少的数字。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-265"/>
	[TestMethod]
	public void Test265()
	{
		AssertMarkdown("123456789. ok\r\n", () =>
		{
			OrderedNumberList(0..15, false, 123456789, () =>
			{
				ListItem(0..15, () =>
				{
					Paragraph(11..15, () =>
					{
						Literal(11..13, "ok");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-266"/>
	[TestMethod]
	public void Test266()
	{
		AssertMarkdown("1234567890. not ok\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..18, "1234567890. not ok");
			});
		});
	}
	/// <summary>
	/// 起始序号可以从 0 开始。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-267"/>
	[TestMethod]
	public void Test267()
	{
		AssertMarkdown("0. ok\r\n", () =>
		{
			OrderedNumberList(0..7, false, 0, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(3..7, () =>
					{
						Literal(3..5, "ok");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-268"/>
	[TestMethod]
	public void Test268()
	{
		AssertMarkdown("003. ok\r\n", () =>
		{
			OrderedNumberList(0..9, false, 3, () =>
			{
				ListItem(0..9, () =>
				{
					Paragraph(5..9, () =>
					{
						Literal(5..7, "ok");
					});
				});
			});
		});
	}
	/// <summary>
	/// 起始序号不能是负数。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-269"/>
	[TestMethod]
	public void Test269()
	{
		AssertMarkdown("-1. not ok\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..10, "-1. not ok");
			});
		});
	}
	/// <summary>
	/// 缩进代码块会在列表项内容左边缘后再缩进四个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-270"/>
	[TestMethod]
	public void Test270()
	{
		AssertMarkdown("- foo\r\n\r\n      bar\r\n", () =>
		{
			UnorderedList(0..20, true, () =>
			{
				ListItem(0..20, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
					CodeBlock(11..20, "bar\r\n");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-271"/>
	[TestMethod]
	public void Test271()
	{
		AssertMarkdown("  10.  foo\r\n\r\n           bar\r\n", () =>
		{
			OrderedNumberList(2..30, true, 10, () =>
			{
				ListItem(2..30, () =>
				{
					Paragraph(7..12, () =>
					{
						Literal(7..10, "foo");
					});
					CodeBlock(21..30, "bar\r\n");
				});
			});
		});
	}
	/// <summary>
	/// 如果首个块是缩进代码块，内容会在列表标志后一个空格的位置。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-272"/>
	[TestMethod]
	public void Test272()
	{
		AssertMarkdown("    indented code\r\n\r\nparagraph\r\n\r\n    more code\r\n", () =>
		{
			CodeBlock(0..19, "indented code\r\n");
			Paragraph(21..32, () =>
			{
				Literal(21..30, "paragraph");
			});
			CodeBlock(34..49, "more code\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-273"/>
	[TestMethod]
	public void Test273()
	{
		AssertMarkdown("1.     indented code\r\n\r\n   paragraph\r\n\r\n       more code\r\n", () =>
		{
			OrderedNumberList(0..58, true, 1, () =>
			{
				ListItem(0..58, () =>
				{
					CodeBlock(3..22, "indented code\r\n");
					Paragraph(27..38, () =>
					{
						Literal(27..36, "paragraph");
					});
					CodeBlock(43..58, "more code\r\n");
				});
			});
		});
	}
	/// <summary>
	/// 注意额外的缩进会计入代码块中。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-274"/>
	[TestMethod]
	public void Test274()
	{
		AssertMarkdown("1.      indented code\r\n\r\n   paragraph\r\n\r\n       more code\r\n", () =>
		{
			OrderedNumberList(0..59, true, 1, () =>
			{
				ListItem(0..59, () =>
				{
					CodeBlock(3..23, " indented code\r\n");
					Paragraph(28..39, () =>
					{
						Literal(28..37, "paragraph");
					});
					CodeBlock(44..59, "more code\r\n");
				});
			});
		});
	}
	/// <summary>
	/// 第一个块以三个空格开头，使得第二个块的缩进不够，不能添加到列表项中。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-275"/>
	[TestMethod]
	public void Test275()
	{
		AssertMarkdown("   foo\r\n\r\nbar\r\n", () =>
		{
			Paragraph(3..8, () =>
			{
				Literal(3..6, "foo");
			});
			Paragraph(10..15, () =>
			{
				Literal(10..13, "bar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-276"/>
	[TestMethod]
	public void Test276()
	{
		AssertMarkdown("-    foo\r\n\r\n  bar\r\n", () =>
		{
			UnorderedList(0..10, false, () =>
			{
				ListItem(0..10, () =>
				{
					Paragraph(5..10, () =>
					{
						Literal(5..8, "foo");
					});
				});
			});
			Paragraph(14..19, () =>
			{
				Literal(14..17, "bar");
			});
		});
	}
	/// <summary>
	/// 第二个块有足够的缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-277"/>
	[TestMethod]
	public void Test277()
	{
		AssertMarkdown("-  foo\r\n\r\n   bar\r\n", () =>
		{
			UnorderedList(0..18, true, () =>
			{
				ListItem(0..18, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
					Paragraph(13..18, () =>
					{
						Literal(13..16, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 以空行开始的非空列表项。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-278"/>
	[TestMethod]
	public void Test278()
	{
		AssertMarkdown("-\r\n  foo\r\n-\r\n  ```\r\n  bar\r\n  ```\r\n-\r\n      baz\r\n", () =>
		{
			UnorderedList(0..48, false, () =>
			{
				ListItem(0..10, () =>
				{
					Paragraph(5..10, () =>
					{
						Literal(5..8, "foo");
					});
				});
				ListItem(10..34, () =>
				{
					CodeBlock(15..34, "bar\r\n");
				});
				ListItem(34..48, () =>
				{
					CodeBlock(39..48, "baz\r\n");
				});
			});
		});
	}
	/// <summary>
	/// 以空行开始的列表项，空格数不影响内容缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-279"/>
	[TestMethod]
	public void Test279()
	{
		AssertMarkdown("-   \r\n  foo\r\n", () =>
		{
			UnorderedList(0..13, false, () =>
			{
				ListItem(0..13, () =>
				{
					Paragraph(8..13, () =>
					{
						Literal(8..11, "foo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项最多可以以一个空行开始。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-280"/>
	[TestMethod]
	public void Test280()
	{
		AssertMarkdown("-\r\n\r\n  foo\r\n", () =>
		{
			UnorderedList(0..3, false, () =>
			{
				ListItem(0..3);
			});
			Paragraph(7..12, () =>
			{
				Literal(7..10, "foo");
			});
		});
	}
	/// <summary>
	/// 空的无序列表项。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-281"/>
	[TestMethod]
	public void Test281()
	{
		AssertMarkdown("- foo\r\n-\r\n- bar\r\n", () =>
		{
			UnorderedList(0..17, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
				ListItem(7..10);
				ListItem(10..17, () =>
				{
					Paragraph(12..17, () =>
					{
						Literal(12..15, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表标志后可以包含空格或者 Tab 。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-282"/>
	[TestMethod]
	public void Test282()
	{
		AssertMarkdown("- foo\r\n-   \r\n- bar\r\n", () =>
		{
			UnorderedList(0..20, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
				ListItem(7..13);
				ListItem(13..20, () =>
				{
					Paragraph(15..20, () =>
					{
						Literal(15..18, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 空的有序列表。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-283"/>
	[TestMethod]
	public void Test283()
	{
		AssertMarkdown("1. foo\r\n2.\r\n3. bar\r\n", () =>
		{
			OrderedNumberList(0..20, false, 1, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
				ListItem(8..12);
				ListItem(12..20, () =>
				{
					Paragraph(15..20, () =>
					{
						Literal(15..18, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表可以只包含空的列表项。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-284"/>
	[TestMethod]
	public void Test284()
	{
		AssertMarkdown("*\r\n", () =>
		{
			UnorderedList(0..3, false, () =>
			{
				ListItem(0..3);
			});
		});
	}
	/// <summary>
	/// 空列表项不能中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-285"/>
	[TestMethod]
	public void Test285()
	{
		AssertMarkdown("foo\r\n*\r\n\r\nfoo\r\n1.\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "foo");
				SoftBreak(3..5);
				Literal(5..6, "*");
			});
			Paragraph(10..19, () =>
			{
				Literal(10..13, "foo");
				SoftBreak(13..15);
				Literal(15..17, "1.");
			});
		});
	}
	/// <summary>
	/// 缩进一个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-286"/>
	[TestMethod]
	public void Test286()
	{
		AssertMarkdown(" 1.  A paragraph\r\n     with two lines.\r\n\r\n         indented code\r\n\r\n     > A block quote.\r\n", () =>
		{
			OrderedNumberList(1..91, true, 1, () =>
			{
				ListItem(1..91, () =>
				{
					Paragraph(5..40, () =>
					{
						Literal(5..16, "A paragraph");
						SoftBreak(16..23);
						Literal(23..38, "with two lines.");
					});
					CodeBlock(47..66, "indented code\r\n");
					Blockquote(73..91, () =>
					{
						Paragraph(75..91, () =>
						{
							Literal(75..89, "A block quote.");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 缩进两个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-287"/>
	[TestMethod]
	public void Test287()
	{
		AssertMarkdown("  1.  A paragraph\r\n      with two lines.\r\n\r\n          indented code\r\n\r\n      > A block quote.\r\n", () =>
		{
			OrderedNumberList(2..95, true, 1, () =>
			{
				ListItem(2..95, () =>
				{
					Paragraph(6..42, () =>
					{
						Literal(6..17, "A paragraph");
						SoftBreak(17..25);
						Literal(25..40, "with two lines.");
					});
					CodeBlock(50..69, "indented code\r\n");
					Blockquote(77..95, () =>
					{
						Paragraph(79..95, () =>
						{
							Literal(79..93, "A block quote.");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 缩进三个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-288"/>
	[TestMethod]
	public void Test288()
	{
		AssertMarkdown("   1.  A paragraph\r\n       with two lines.\r\n\r\n           indented code\r\n\r\n       > A block quote.\r\n", () =>
		{
			OrderedNumberList(3..99, true, 1, () =>
			{
				ListItem(3..99, () =>
				{
					Paragraph(7..44, () =>
					{
						Literal(7..18, "A paragraph");
						SoftBreak(18..27);
						Literal(27..42, "with two lines.");
					});
					CodeBlock(53..72, "indented code\r\n");
					Blockquote(81..99, () =>
					{
						Paragraph(83..99, () =>
						{
							Literal(83..97, "A block quote.");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 缩进四个空格就会得到代码块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-289"/>
	[TestMethod]
	public void Test289()
	{
		AssertMarkdown("    1.  A paragraph\r\n        with two lines.\r\n\r\n            indented code\r\n\r\n        > A block quote.\r\n", () =>
		{
			CodeBlock(0..103, "1.  A paragraph\r\n    with two lines.\r\n\r\n        indented code\r\n\r\n    > A block quote.\r\n");
		});
	}
	/// <summary>
	/// 列表项中的段落可以延迟延伸。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-290"/>
	[TestMethod]
	public void Test290()
	{
		AssertMarkdown("  1.  A paragraph\r\nwith two lines.\r\n\r\n          indented code\r\n\r\n      > A block quote.\r\n", () =>
		{
			OrderedNumberList(2..89, true, 1, () =>
			{
				ListItem(2..89, () =>
				{
					Paragraph(6..36, () =>
					{
						Literal(6..17, "A paragraph");
						SoftBreak(17..19);
						Literal(19..34, "with two lines.");
					});
					CodeBlock(44..63, "indented code\r\n");
					Blockquote(71..89, () =>
					{
						Paragraph(73..89, () =>
						{
							Literal(73..87, "A block quote.");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 缩进会被部分删除。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-291"/>
	[TestMethod]
	public void Test291()
	{
		AssertMarkdown("  1.  A paragraph\r\n    with two lines.\r\n", () =>
		{
			OrderedNumberList(2..40, false, 1, () =>
			{
				ListItem(2..40, () =>
				{
					Paragraph(6..40, () =>
					{
						Literal(6..17, "A paragraph");
						SoftBreak(17..23);
						Literal(23..38, "with two lines.");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-292"/>
	[TestMethod]
	public void Test292()
	{
		AssertMarkdown("> 1. > Blockquote\r\ncontinued here.\r\n", () =>
		{
			Blockquote(0..36, () =>
			{
				OrderedNumberList(2..36, false, 1, () =>
				{
					ListItem(2..36, () =>
					{
						Blockquote(5..36, () =>
						{
							Paragraph(7..36, () =>
							{
								Literal(7..17, "Blockquote");
								SoftBreak(17..19);
								Literal(19..34, "continued here.");
							});
						});
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-293"/>
	[TestMethod]
	public void Test293()
	{
		AssertMarkdown("> 1. > Blockquote\r\n> continued here.\r\n", () =>
		{
			Blockquote(0..38, () =>
			{
				OrderedNumberList(2..38, false, 1, () =>
				{
					ListItem(2..38, () =>
					{
						Blockquote(5..38, () =>
						{
							Paragraph(7..38, () =>
							{
								Literal(7..17, "Blockquote");
								SoftBreak(17..21);
								Literal(21..36, "continued here.");
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 需要两个空格的缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-294"/>
	[TestMethod]
	public void Test294()
	{
		AssertMarkdown("- foo\r\n  - bar\r\n    - baz\r\n      - boo\r\n", () =>
		{
			UnorderedList(0..40, false, () =>
			{
				ListItem(0..40, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
					UnorderedList(9..40, false, () =>
					{
						ListItem(9..40, () =>
						{
							Paragraph(11..16, () =>
							{
								Literal(11..14, "bar");
							});
							UnorderedList(20..40, false, () =>
							{
								ListItem(20..40, () =>
								{
									Paragraph(22..27, () =>
									{
										Literal(22..25, "baz");
									});
									UnorderedList(33..40, false, () =>
									{
										ListItem(33..40, () =>
										{
											Paragraph(35..40, () =>
											{
												Literal(35..38, "boo");
											});
										});
									});
								});
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 一个空格的缩进是不够的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-295"/>
	[TestMethod]
	public void Test295()
	{
		AssertMarkdown("- foo\r\n - bar\r\n  - baz\r\n   - boo\r\n", () =>
		{
			UnorderedList(0..34, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
				ListItem(8..15, () =>
				{
					Paragraph(10..15, () =>
					{
						Literal(10..13, "bar");
					});
				});
				ListItem(17..24, () =>
				{
					Paragraph(19..24, () =>
					{
						Literal(19..22, "baz");
					});
				});
				ListItem(27..34, () =>
				{
					Paragraph(29..34, () =>
					{
						Literal(29..32, "boo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表标志更宽，需要四个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-296"/>
	[TestMethod]
	public void Test296()
	{
		AssertMarkdown("10) foo\r\n    - bar\r\n", () =>
		{
			OrderedNumberList(0..20, false, 10, () =>
			{
				ListItem(0..20, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
					UnorderedList(13..20, false, () =>
					{
						ListItem(13..20, () =>
						{
							Paragraph(15..20, () =>
							{
								Literal(15..18, "bar");
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 三个空格是不够的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-297"/>
	[TestMethod]
	public void Test297()
	{
		AssertMarkdown("10) foo\r\n   - bar\r\n", () =>
		{
			OrderedNumberList(0..9, false, 10, () =>
			{
				ListItem(0..9, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
				});
			});
			UnorderedList(12..19, false, () =>
			{
				ListItem(12..19, () =>
				{
					Paragraph(14..19, () =>
					{
						Literal(14..17, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表的第一个块也可以是一个列表。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-298"/>
	[TestMethod]
	public void Test298()
	{
		AssertMarkdown("- - foo\r\n", () =>
		{
			UnorderedList(0..9, false, () =>
			{
				ListItem(0..9, () =>
				{
					UnorderedList(2..9, false, () =>
					{
						ListItem(2..9, () =>
						{
							Paragraph(4..9, () =>
							{
								Literal(4..7, "foo");
							});
						});
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-299"/>
	[TestMethod]
	public void Test299()
	{
		AssertMarkdown("1. - 2. foo\r\n", () =>
		{
			OrderedNumberList(0..13, false, 1, () =>
			{
				ListItem(0..13, () =>
				{
					UnorderedList(3..13, false, () =>
					{
						ListItem(3..13, () =>
						{
							OrderedNumberList(5..13, false, 2, () =>
							{
								ListItem(5..13, () =>
								{
									Paragraph(8..13, () =>
									{
										Literal(8..11, "foo");
									});
								});
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项可以包含标题。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-300"/>
	[TestMethod]
	public void Test300()
	{
		AssertCommonMark("- # Foo\r\n- Bar\r\n  ---\r\n  baz\r\n", () =>
		{
			UnorderedList(0..30, false, () =>
			{
				ListItem(0..9, () =>
				{
					Heading(2..9, 1, () =>
					{
						Literal(4..7, "Foo");
					});
				});
				ListItem(9..30, () =>
				{
					Heading(11..23, 2, () =>
					{
						Literal(11..14, "Bar");
					});
					Paragraph(25..30, () =>
					{
						Literal(25..28, "baz");
					});
				});
			});
		});
	}
}

