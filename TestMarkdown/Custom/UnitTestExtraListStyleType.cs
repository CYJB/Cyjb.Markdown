using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Custom;

/// <summary>
/// 额外的列表样式的单元测试。
/// </summary>
[TestClass]
public class UnitTestExtraListStyleType : BaseTest
{
	/// <summary>
	/// 测试小写英文字母列表。
	/// </summary>
	[TestMethod]
	public void TestLowerAlpha()
	{
		AssertMarkdown("a. foo\r\nb. bar\r\n", () =>
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
		});
	}
	/// <summary>
	/// 测试大写英文字母列表。
	/// </summary>
	[TestMethod]
	public void TestUpperAlpha()
	{
		AssertMarkdown("C) foo\r\nE) bar\r\n", () =>
		{
			ExtraList(0..16, false, ListStyleType.OrderedUpperAlpha, 3, () =>
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
	/// 不支持两个英文字母。
	/// </summary>
	[TestMethod]
	public void TestTwoAlpha()
	{
		AssertMarkdown("aa. foo\r\n\r\nCE. bar\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "aa. foo");
			});
			Paragraph(11..20, () =>
			{
				Literal(11..18, "CE. bar");
			});
		});
	}
	/// <summary>
	/// 测试小写罗马数字列表。
	/// </summary>
	[TestMethod]
	public void TestLowerRoman()
	{
		AssertMarkdown("i. foo\r\nii. bar\r\n", () =>
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
		});
	}
	/// <summary>
	/// 测试大写罗马数字列表。
	/// </summary>
	[TestMethod]
	public void TestUpperRoman()
	{
		AssertMarkdown("IV) foo\r\nV) bar\r\n", () =>
		{
			ExtraList(0..17, false, ListStyleType.OrderedUpperRoman, 4, () =>
			{
				ListItem(0..9, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
				});
				ListItem(9..17, () =>
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
	/// 不支持的罗马数字。
	/// </summary>
	[TestMethod]
	public void TestInvalidRomain()
	{
		AssertMarkdown("iiii. foo\r\n\r\nVC. bar\r\n\r\nmmmm. baz\r\nii. boo\r\n\r\nIV. bom\r\n", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..9, "iiii. foo");
			});
			Paragraph(13..22, () =>
			{
				Literal(13..20, "VC. bar");
			});
			Paragraph(24..44, () =>
			{
				Literal(24..33, "mmmm. baz");
				SoftBreak(33..35);
				// 非从 1 开始的列表不能中断段落。
				Literal(35..42, "ii. boo");
			});
			ExtraList(46..55, false, ListStyleType.OrderedUpperRoman, 4, () =>
			{
				ListItem(46..55, () =>
				{
					Paragraph(50..55, () =>
					{
						Literal(50..53, "bom");
					});
				});
			});
		});
	}
	/// <summary>
	/// 测试小写希腊字母列表。
	/// </summary>
	[TestMethod]
	public void TestLowerGreek()
	{
		AssertMarkdown("β. foo\r\nγ. bar\r\n", () =>
		{
			ExtraList(0..16, false, ListStyleType.OrderedLowerGreek, 2, () =>
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
	/// 测试 CommonMark 不支持扩展列表样式。
	/// </summary>
	[TestMethod]
	public void TestCommonMarkNoExtraStyle()
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

