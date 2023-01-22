using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 删除的单元测试。
/// </summary>
[TestClass]
public class UnitTestStrikethrough : BaseTest
{
	/// <summary>
	/// 删除可以包含一个或两个波浪号。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-491"/>
	[TestMethod]
	public void TestGFM491()
	{
		AssertMarkdown("~~Hi~~ Hello, ~there~ world!\r\n", () =>
		{
			Paragraph(0..30, () =>
			{
				Strikethrough(0..6, () =>
				{
					Literal(2..4, "Hi");
				});
				Literal(6..14, " Hello, ");
				Strikethrough(14..21, () =>
				{
					Literal(15..20, "there");
				});
				Literal(21..28, " world!");
			});
		});
	}
	/// <summary>
	/// 不能跨段落。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-492"/>
	[TestMethod]
	public void TestGFM492()
	{
		AssertMarkdown("This ~~has a\r\n\r\nnew paragraph~~.\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..12, "This ~~has a");
			});
			Paragraph(16..34, () =>
			{
				Literal(16..32, "new paragraph~~.");
			});
		});
	}
	/// <summary>
	/// 不会识别三个或更多波浪号。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-493"/>
	[TestMethod]
	public void TestGFM493()
	{
		AssertMarkdown("This will ~~~not~~~ strike.\r\n", () =>
		{
			Paragraph(0..29, () =>
			{
				Literal(0..27, "This will ~~~not~~~ strike.");
			});
		});
	}
	/// <summary>
	/// 不会识别四个波浪号。
	/// </summary>
	[TestMethod]
	public void TestFourTildes()
	{
		AssertMarkdown("foo ~~~~ bar\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..12, "foo ~~~~ bar");
			});
		});
	}
	/// <summary>
	/// 不会识别不匹配的波浪号个数。
	/// </summary>
	[TestMethod]
	public void TestUnmatchedTildes()
	{
		AssertMarkdown("~~foo~~~\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..8, "~~foo~~~");
			});
		});
	}
	/// <summary>
	/// 嵌套的波浪号。
	/// </summary>
	[TestMethod]
	public void TestNestedTildes_1()
	{
		AssertMarkdown("~~foo~bar~~\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Strikethrough(0..11, () =>
				{
					Literal(2..9, "foo~bar");
				});
			});
		});
	}
	[TestMethod]
	public void TestNestedTildes_2()
	{
		AssertMarkdown("~~foo~~bar~~\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Strikethrough(0..7, () =>
				{
					Literal(2..5, "foo");
				});
				Literal(7..12, "bar~~");
			});
		});
	}
	[TestMethod]
	public void TestNestedTildes_3()
	{
		AssertMarkdown("~~foo~~~bar~~\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Strikethrough(0..13, () =>
				{
					Literal(2..11, "foo~~~bar");
				});
			});
		});
	}
	[TestMethod]
	public void TestNestedTildes_4()
	{
		AssertMarkdown("~~foo~~~~bar~~\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Strikethrough(0..14, () =>
				{
					Literal(2..12, "foo~~~~bar");
				});
			});
		});
	}
	[TestMethod]
	public void TestNestedTildes_5()
	{
		AssertMarkdown("~~foo~~~~~bar~~\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				Strikethrough(0..15, () =>
				{
					Literal(2..13, "foo~~~~~bar");
				});
			});
		});
	}
	[TestMethod]
	public void TestNestedTildes_6()
	{
		AssertMarkdown("~~foo~~~~~~bar~~\r\n", () =>
		{
			Paragraph(0..18, () =>
			{
				Strikethrough(0..16, () =>
				{
					Literal(2..14, "foo~~~~~~bar");
				});
			});
		});
	}
	/// <summary>
	/// 删除的优先级与其它强调一致。
	/// </summary>
	[TestMethod]
	public void TestPriority()
	{
		AssertMarkdown("~ab*cd*ef**gh~ij**\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				Strikethrough(0..14, () =>
				{
					Literal(1..3, "ab");
					Emphasis(3..7, () =>
					{
						Literal(4..6, "cd");
					});
					Literal(7..13, "ef**gh");
				});
				Literal(14..18, "ij**");
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持删除线样式。
	/// </summary>
	[TestMethod]
	public void TestCommonMarkNoStrikethrough()
	{
		AssertCommonMark("~~Hi~~\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "~~Hi~~");
			});
		});
	}
}

