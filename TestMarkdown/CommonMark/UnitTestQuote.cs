using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 引用的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#block-quotes"/>
[TestClass]
public class UnitTestQuote : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-228"/>
	[TestMethod]
	public void Test228()
	{
		AssertCommonMark("> # Foo\r\n> bar\r\n> baz\r\n", () =>
		{
			Blockquote(0..23, () =>
			{
				Heading(2..9, 1, () =>
				{
					Literal(4..7, "Foo");
				});
				Paragraph(11..23, () =>
				{
					Literal(11..14, "bar");
					SoftBreak(14..18);
					Literal(18..21, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 可以省略 > 后的空格或 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-229"/>
	[TestMethod]
	public void Test229()
	{
		AssertCommonMark("># Foo\r\n>bar\r\n> baz\r\n", () =>
		{
			Blockquote(0..21, () =>
			{
				Heading(1..8, 1, () =>
				{
					Literal(3..6, "Foo");
				});
				Paragraph(9..21, () =>
				{
					Literal(9..12, "bar");
					SoftBreak(12..16);
					Literal(16..19, "baz");
				});
			});
		});
	}
	/// <summary>
	/// > 前可以包含最多三个空格的缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-230"/>
	[TestMethod]
	public void Test230()
	{
		AssertCommonMark("   > # Foo\r\n   > bar\r\n > baz\r\n", () =>
		{
			Blockquote(3..30, () =>
			{
				Heading(5..12, 1, () =>
				{
					Literal(7..10, "Foo");
				});
				Paragraph(17..30, () =>
				{
					Literal(17..20, "bar");
					SoftBreak(20..25);
					Literal(25..28, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 四个空格就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-231"/>
	[TestMethod]
	public void Test231()
	{
		AssertMarkdown("    > # Foo\r\n    > bar\r\n    > baz\r\n", () =>
		{
			CodeBlock(0..35, "> # Foo\r\n> bar\r\n> baz\r\n");
		});
	}
	/// <summary>
	/// 懒闭合允许忽略段落延伸文本前的 >。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-232"/>
	[TestMethod]
	public void Test232()
	{
		AssertCommonMark("> # Foo\r\n> bar\r\nbaz\r\n", () =>
		{
			Blockquote(0..21, () =>
			{
				Heading(2..9, 1, () =>
				{
					Literal(4..7, "Foo");
				});
				Paragraph(11..21, () =>
				{
					Literal(11..14, "bar");
					SoftBreak(14..16);
					Literal(16..19, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 引用可以包含延迟延伸或非延迟延伸的行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-233"/>
	[TestMethod]
	public void Test233()
	{
		AssertMarkdown("> bar\r\nbaz\r\n> foo\r\n", () =>
		{
			Blockquote(0..19, () =>
			{
				Paragraph(2..19, () =>
				{
					Literal(2..5, "bar");
					SoftBreak(5..7);
					Literal(7..10, "baz");
					SoftBreak(10..14);
					Literal(14..17, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 引用的懒闭合只能用于段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-234"/>
	[TestMethod]
	public void Test234_1()
	{
		AssertCommonMark("> foo\r\n> ---\r\n", () =>
		{
			Blockquote(0..14, () =>
			{
				Heading(2..14, 2, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	[TestMethod]
	public void Test234_2()
	{
		AssertMarkdown("> foo\r\n---\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "foo");
				});
			});
			ThematicBreak(7..12);
		});
	}
	[TestMethod]
	public void Test235_1()
	{
		AssertMarkdown("> - foo\r\n> - bar\r\n", () =>
		{
			Blockquote(0..18, () =>
			{
				UnorderedList(2..18, false, () =>
				{
					ListItem(2..9, () =>
					{
						Paragraph(4..9, () =>
						{
							Literal(4..7, "foo");
						});
					});
					ListItem(11..18, () =>
					{
						Paragraph(13..18, () =>
						{
							Literal(13..16, "bar");
						});
					});
				});
			});
		});
	}
	[TestMethod]
	public void Test235_2()
	{
		AssertMarkdown("> - foo\r\n- bar\r\n", () =>
		{
			Blockquote(0..9, () =>
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
			UnorderedList(9..16, false, () =>
			{
				ListItem(9..16, () =>
				{
					Paragraph(11..16, () =>
					{
						Literal(11..14, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 不能省略代码段前的 >。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-236"/>
	[TestMethod]
	public void Test236()
	{
		AssertMarkdown(">     foo\r\n    bar\r\n", () =>
		{
			Blockquote(0..11, () =>
			{
				CodeBlock(2..11, "foo\r\n");
			});
			CodeBlock(11..20, "bar\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-237"/>
	[TestMethod]
	public void Test237()
	{
		AssertMarkdown("> ```\r\nfoo\r\n```\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				CodeBlock(2..7, "");
			});
			Paragraph(7..12, () =>
			{
				Literal(7..10, "foo");
			});
			CodeBlock(12..17, "");
		});
	}
	/// <summary>
	/// 可以有延迟延伸的行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-238"/>
	[TestMethod]
	public void Test238_1()
	{
		AssertMarkdown("> foo\r\n    - bar\r\n", () =>
		{
			Blockquote(0..18, () =>
			{
				Paragraph(2..18, () =>
				{
					Literal(2..5, "foo");
					SoftBreak(5..11);
					Literal(11..16, "- bar");
				});
			});
		});
	}
	[TestMethod]
	public void Test238_2()
	{
		AssertMarkdown("> foo\r\n>     - bar\r\n", () =>
		{
			Blockquote(0..20, () =>
			{
				Paragraph(2..20, () =>
				{
					Literal(2..5, "foo");
					SoftBreak(5..13);
					Literal(13..18, "- bar");
				});
			});
		});
	}
	/// <summary>
	/// 引用可以是空的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-239"/>
	[TestMethod]
	public void Test239_1()
	{
		AssertMarkdown(">\r\n", () =>
		{
			Blockquote(0..3);
		});
	}
	[TestMethod]
	public void Test239_2()
	{
		AssertMarkdown(">", () =>
		{
			Blockquote(0..1);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-240"/>
	[TestMethod]
	public void Test240_1()
	{
		AssertMarkdown(">\r\n>  \r\n> \r\n", () =>
		{
			Blockquote(0..12);
		});
	}
	[TestMethod]
	public void Test240_2()
	{
		AssertMarkdown(">\r\n>  \r\n> ", () =>
		{
			Blockquote(0..10);
		});
	}
	/// <summary>
	/// 引用可以包含起始或结束空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-241"/>
	[TestMethod]
	public void Test241()
	{
		AssertMarkdown(">\r\n> foo\r\n>  \r\n", () =>
		{
			Blockquote(0..15, () =>
			{
				Paragraph(5..10, () =>
				{
					Literal(5..8, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 空行总是会分割引用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-242"/>
	[TestMethod]
	public void Test242()
	{
		AssertMarkdown("> foo\r\n\r\n> bar\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "foo");
				});
			});
			Blockquote(9..16, () =>
			{
				Paragraph(11..16, () =>
				{
					Literal(11..14, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 如果将两个引用放在一起，会得到一个引用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-243"/>
	[TestMethod]
	public void Test243()
	{
		AssertMarkdown("> foo\r\n> bar\r\n", () =>
		{
			Blockquote(0..14, () =>
			{
				Paragraph(2..14, () =>
				{
					Literal(2..5, "foo");
					SoftBreak(5..9);
					Literal(9..12, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 如果想得到含有两个段落的引用，可以添加一个空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-244"/>
	[TestMethod]
	public void Test244()
	{
		AssertMarkdown("> foo\r\n>\r\n> bar\r\n", () =>
		{
			Blockquote(0..17, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "foo");
				});
				Paragraph(12..17, () =>
				{
					Literal(12..15, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 引用可以中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-245"/>
	[TestMethod]
	public void Test245()
	{
		AssertMarkdown("foo\r\n> bar\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "foo");
			});
			Blockquote(5..12, () =>
			{
				Paragraph(7..12, () =>
				{
					Literal(7..10, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 引用前后不需要空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-246"/>
	[TestMethod]
	public void Test246()
	{
		AssertMarkdown("> aaa\r\n***\r\n> bbb\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "aaa");
				});
			});
			ThematicBreak(7..12);
			Blockquote(12..19, () =>
			{
				Paragraph(14..19, () =>
				{
					Literal(14..17, "bbb");
				});
			});
		});
	}
	/// <summary>
	/// 由于延迟延伸，引用和后续段落间需要一个空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-247"/>
	[TestMethod]
	public void Test247()
	{
		AssertMarkdown("> bar\r\nbaz\r\n", () =>
		{
			Blockquote(0..12, () =>
			{
				Paragraph(2..12, () =>
				{
					Literal(2..5, "bar");
					SoftBreak(5..7);
					Literal(7..10, "baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-248"/>
	[TestMethod]
	public void Test248()
	{
		AssertMarkdown("> bar\r\n\r\nbaz\r\n", () =>
		{
			Blockquote(0..7, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "bar");
				});
			});
			Paragraph(9..14, () =>
			{
				Literal(9..12, "baz");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-249"/>
	[TestMethod]
	public void Test249()
	{
		AssertMarkdown("> bar\r\n>\r\nbaz\r\n", () =>
		{
			Blockquote(0..10, () =>
			{
				Paragraph(2..7, () =>
				{
					Literal(2..5, "bar");
				});
			});
			Paragraph(10..15, () =>
			{
				Literal(10..13, "baz");
			});
		});
	}
	/// <summary>
	/// 在延迟延伸时，会忽略多个 <c>&gt;</c>，将行添加到最内部引用中。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-250"/>
	[TestMethod]
	public void Test250()
	{
		AssertMarkdown("> > > foo\r\nbar\r\n", () =>
		{
			Blockquote(0..16, () =>
			{
				Blockquote(2..16, () =>
				{
					Blockquote(4..16, () =>
					{
						Paragraph(6..16, () =>
						{
							Literal(6..9, "foo");
							SoftBreak(9..11);
							Literal(11..14, "bar");
						});
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-251"/>
	[TestMethod]
	public void Test251()
	{
		AssertMarkdown(">>> foo\r\n> bar\r\n>>baz\r\n", () =>
		{
			Blockquote(0..23, () =>
			{
				Blockquote(1..23, () =>
				{
					Blockquote(2..23, () =>
					{
						Paragraph(4..23, () =>
						{
							Literal(4..7, "foo");
							SoftBreak(7..11);
							Literal(11..14, "bar");
							SoftBreak(14..18);
							Literal(18..21, "baz");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 在引用中包含缩进代码块时，注意引用标志会包含一个额外的空格，
	/// 因此在 <c>&gt;</c> 后需要 5 个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-252"/>
	[TestMethod]
	public void Test252()
	{
		AssertMarkdown(">     code\r\n\r\n>    not code\r\n", () =>
		{
			Blockquote(0..12, () =>
			{
				CodeBlock(2..12, "code\r\n");
			});
			Blockquote(14..29, () =>
			{
				Paragraph(19..29, () =>
				{
					Literal(19..27, "not code");
				});
			});
		});
	}
}

