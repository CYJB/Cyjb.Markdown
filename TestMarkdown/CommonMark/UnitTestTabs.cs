using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 制表符的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#tabs"/>
[TestClass]
public class UnitTestTabs : BaseTest
{
	/// <summary>
	/// Tab 可以在缩进时当作 4 个空格使用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("\tfoo\tbar\t\tbim\r\n", () =>
		{
			CodeBlock(0..15, "foo\tbar\t\tbim\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("  \tfoo\tbar\t\tbim\r\n", () =>
		{
			CodeBlock(0..17, "foo\tbar\t\tbim\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("    a\ta\r\n    ὐ\ta\r\n", () =>
		{
			CodeBlock(0..18, "a\ta\r\nὐ\ta\r\n");
		});
	}
	/// <summary>
	/// 在列表项的连续段落中，Tab 与四个空格的缩进有同样的效果。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("  - foo\r\n\r\n\tbar\r\n", () =>
		{
			UnorderedList(2..17, true, () =>
			{
				ListItem(2..17, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
					Paragraph(12..17, () =>
					{
						Literal(12..15, "bar");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("- foo\r\n\r\n\t\tbar\r\n", () =>
		{
			UnorderedList(0..16, true, () =>
			{
				ListItem(0..16, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
					CodeBlock(9..16, "  bar\r\n");
				});
			});
		});
	}
	/// <summary>
	/// 引用的起始 <c>&gt;</c> 后可以跟一个可选的空格，不会作为内容的一部分。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown(">\t\tfoo\r\n", () =>
		{
			Blockquote(0..8, () =>
			{
				CodeBlock(1..8, "  foo\r\n");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("-\t\tfoo\r\n", () =>
		{
			UnorderedList(0..8, false, () =>
			{
				ListItem(0..8, () =>
				{
					CodeBlock(1..8, "  foo\r\n");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-8"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("    foo\r\n\tbar\r\n", () =>
		{
			CodeBlock(0..15, "foo\r\nbar\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-9"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown(" - foo\r\n   - bar\r\n\t - baz\r\n", () =>
		{
			UnorderedList(1..27, false, () =>
			{
				ListItem(1..27, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
					UnorderedList(11..27, false, () =>
					{
						ListItem(11..27, () =>
						{
							Paragraph(13..18, () =>
							{
								Literal(13..16, "bar");
							});
							UnorderedList(20..27, false, () =>
							{
								ListItem(20..27, () =>
								{
									Paragraph(22..27, () =>
									{
										Literal(22..25, "baz");
									});
								});
							});
						});
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-10"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("#\tFoo\r\n", () =>
		{
			Heading(0..7, 1, () =>
			{
				Literal(2..5, "Foo");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-11"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown("*\t*\t*\t\r\n", () =>
		{
			ThematicBreak(0..8);
		});
	}
}

