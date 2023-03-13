using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 自动生成标识符的单元测试。
/// </summary>
[TestClass]
public class UnitTestAutoIdentifiers : BaseTest
{
	/// <summary>
	/// 自动生成标识符会根据标题内容生成。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/auto-identifiers.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("# Heading identifiers in HTML\r\n# Maître d'hôtel\r\n# 示例 :grinning:\r\n# *Dogs*?-in *my* house?\r\n# [link](/url) and `code`\r\n# --3. Applications--\r\n# ??\r\n", () =>
		{
			Heading(0..31, 1, new HtmlAttributeList() { { "id", "heading-identifiers-in-html" } }, () =>
			{
				Literal(2..29, "Heading identifiers in HTML");
			});
			Heading(31..49, 1, new HtmlAttributeList() { { "id", "maître-dhôtel" } }, () =>
			{
				Literal(33..47, "Maître d'hôtel");
			});
			Heading(49..66, 1, new HtmlAttributeList() { { "id", "示例-grinning" } }, () =>
			{
				Literal(51..54, "示例 ");
				Emoji(54..64, "grinning");
			});
			Heading(66..92, 1, new HtmlAttributeList() { { "id", "dogs-in-my-house" } }, () =>
			{
				Emphasis(68..74, () =>
				{
					Literal(69..73, "Dogs");
				});
				Literal(74..79, "?-in ");
				Emphasis(79..83, () =>
				{
					Literal(80..82, "my");
				});
				Literal(83..90, " house?");
			});
			Heading(92..119, 1, new HtmlAttributeList() { { "id", "link-and-code" } }, () =>
			{
				Link(94..106, "/url", null, () =>
				{
					Literal(95..99, "link");
				});
				Literal(106..111, " and ");
				CodeSpan(111..117, "code");
			});
			Heading(119..142, 1, new HtmlAttributeList() { { "id", "3.-applications" } }, () =>
			{
				Literal(121..140, "--3. Applications--");
			});
			Heading(142..148, 1, new HtmlAttributeList() { { "id", "section" } }, () =>
			{
				Literal(144..146, "??");
			});
		});
	}

	/// <summary>
	/// 如果多个标题生成了重复的标识符，那么会添加后缀。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/auto-identifiers.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("# Heading\r\n# Heading-1\r\n# Heading\r\n## Heading-2\r\n## Heading-1\r\n", () =>
		{
			Heading(0..11, 1, new HtmlAttributeList() { { "id", "heading" } }, () =>
			{
				Literal(2..9, "Heading");
			});
			Heading(11..24, 1, new HtmlAttributeList() { { "id", "heading-1" } }, () =>
			{
				Literal(13..22, "Heading-1");
			});
			Heading(24..35, 1, new HtmlAttributeList() { { "id", "heading-2" } }, () =>
			{
				Literal(26..33, "Heading");
			});
			Heading(35..49, 2, new HtmlAttributeList() { { "id", "heading-2-1" } }, () =>
			{
				Literal(38..47, "Heading-2");
			});
			Heading(49..63, 2, new HtmlAttributeList() { { "id", "heading-1-1" } }, () =>
			{
				Literal(52..61, "Heading-1");
			});
		});
	}

	/// <summary>
	/// 自动链接不会覆盖通过属性显示指定的标识符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/auto-identifiers.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("# Section 1\r\n# Section 2 {#sec2}\r\n", () =>
		{
			Heading(0..13, 1, new HtmlAttributeList() { { "id", "section-1" } }, () =>
			{
				Literal(2..11, "Section 1");
			});
			Heading(13..34, 1, new HtmlAttributeList() { { "id", "sec2" } }, () =>
			{
				Literal(15..24, "Section 2");
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持自动生成标识符。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("# foo\r\n", () =>
		{
			Heading(0..7, 1, () =>
			{
				Literal(2..5, "foo");
			});
		});
	}
}

