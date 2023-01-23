using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 额外的列表样式的单元测试。
/// </summary>
/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md"/>
[TestClass]
public class UnitTestExtraListStyleType : BaseTest
{
	/// <summary>
	/// 使用英文字母作为序号使用时，会生成英文字母列表。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("a. foo\r\nb. bar\r\nA) baz\r\nB) boo\r\n", () =>
		{
			ExtraList(0..16, false, ListStyleType.OrderedLowerAlpha, 1, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
				ListItem(8..16, () =>
				{
					Paragraph(11..16, () =>
					{
						Literal(11..14, "bar");
					});
				});
			});
			ExtraList(16..32, false, ListStyleType.OrderedUpperAlpha, 1, () =>
			{
				ListItem(16..24, () =>
				{
					Paragraph(19..24, () =>
					{
						Literal(19..22, "baz");
					});
				});
				ListItem(24..32, () =>
				{
					Paragraph(27..32, () =>
					{
						Literal(27..30, "boo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 只支持一个英文字母作为序号，不支持两个或更多字母。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("aa. foo\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "aa. foo");
			});
		});
	}
	/// <summary>
	/// 使用罗马数字作为序号使用时，会生成罗马数字列表。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("i. foo\r\nii. bar\r\nI) baz\r\nII) boo\r\n", () =>
		{
			ExtraList(0..17, false, ListStyleType.OrderedLowerRoman, 1, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
				ListItem(8..17, () =>
				{
					Paragraph(12..17, () =>
					{
						Literal(12..15, "bar");
					});
				});
			});
			ExtraList(17..34, false, ListStyleType.OrderedUpperRoman, 1, () =>
			{
				ListItem(17..25, () =>
				{
					Paragraph(20..25, () =>
					{
						Literal(20..23, "baz");
					});
				});
				ListItem(25..34, () =>
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
	/// 对于列表中的首个列表项，只有 i 和长度大于等于 2 的序号才会被识别成罗马数字。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("v. foo\r\nix. bar\r\nx. baz\r\n", () =>
		{
			ExtraList(0..8, false, ListStyleType.OrderedLowerAlpha, 22, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
			});
			ExtraList(8..25, false, ListStyleType.OrderedLowerRoman, 9, () =>
			{
				ListItem(8..17, () =>
				{
					Paragraph(12..17, () =>
					{
						Literal(12..15, "bar");
					});
				});
				ListItem(17..25, () =>
				{
					Paragraph(20..25, () =>
					{
						Literal(20..23, "baz");
					});
				});
			});
		});
	}
	/// <summary>
	/// 罗马数字的长度没有限制，如果首个列表项不是有效的罗马数字或者大于 4000，那么该列表不会被识别为罗马数字。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("VC. foo\r\n\r\nX. bar\r\n\r\niiii. baz\r\n\r\nmmmm. boo\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "VC. foo");
			});
			ExtraList(11..19, false, ListStyleType.OrderedUpperAlpha, 24, () =>
			{
				ListItem(11..19, () =>
				{
					Paragraph(14..19, () =>
					{
						Literal(14..17, "bar");
					});
				});
			});
			Paragraph(21..32, () =>
			{
				Literal(21..30, "iiii. baz");
			});
			Paragraph(34..45, () =>
			{
				Literal(34..43, "mmmm. boo");
			});
		});
	}
	/// <summary>
	/// 使用希腊字母作为序号使用时，会生成希腊字母列表。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("α. foo\r\nβ. bar\r\n", () =>
		{
			ExtraList(0..16, false, ListStyleType.OrderedLowerGreek, 1, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
				ListItem(8..16, () =>
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
	/// 使用希腊字母作为序号使用时，会生成希腊字母列表。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("αβ. foo\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "αβ. foo");
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持扩展列表样式。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("a. foo\r\n\r\nB. bar\r\n\r\ni. baz\r\n\r\nIV. boo\r\n\r\nα. bom\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "a. foo");
			});
			Paragraph(10..18, () =>
			{
				Literal(10..16, "B. bar");
			});
			Paragraph(20..28, () =>
			{
				Literal(20..26, "i. baz");
			});
			Paragraph(30..39, () =>
			{
				Literal(30..37, "IV. boo");
			});
			Paragraph(41..49, () =>
			{
				Literal(41..47, "α. bom");
			});
		});
	}
}

