using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 标题引用的单元测试。
/// </summary>
[TestClass]
public class UnitTestHeaderReferences : BaseTest
{
	/// <summary>
	/// 可以直接将标题内容作为链接标签使用，而不需要显式指定标题的链接。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/header-references.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("# Heading 1\r\n# Heading 2 {#sec2}\r\n\r\n[Heading 1]\r\n[Heading 2][]\r\n[Second Section][heading  2]\r\n", () =>
		{
			Heading(0..13, 1, new HtmlAttributeList() { { "id", "heading-1" } }, () =>
			{
				Literal(2..11, "Heading 1");
			});
			Heading(13..34, 1, new HtmlAttributeList() { { "id", "sec2" } }, () =>
			{
				Literal(15..24, "Heading 2");
			});
			Paragraph(36..94, () =>
			{
				Link(36..47, "#heading-1", null, () =>
				{
					Literal(37..46, "Heading 1");
				});
				SoftBreak(47..49);
				Link(49..62, "#sec2", null, () =>
				{
					Literal(50..59, "Heading 2");
				});
				SoftBreak(62..64);
				Link(64..92, "#sec2", null, () =>
				{
					Literal(65..79, "Second Section");
				});
			});
		});
	}

	/// <summary>
	/// 如果有多个相同内容的标题，那么总是链接到首次出现的标题。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/header-references.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("# Heading {#sec1}\r\n# Heading {#sec2}\r\n\r\n[Heading]\r\n", () =>
		{
			Heading(0..19, 1, new HtmlAttributeList() { { "id", "sec1" } }, () =>
			{
				Literal(2..9, "Heading");
			});
			Heading(19..38, 1, new HtmlAttributeList() { { "id", "sec2" } }, () =>
			{
				Literal(21..28, "Heading");
			});
			Paragraph(40..51, () =>
			{
				Link(40..49, "#sec1", null, () =>
				{
					Literal(41..48, "Heading");
				});
			});
		});
	}

	/// <summary>
	/// 标题的优先级总是低于链接声明，无论其顺序如何。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/header-references.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("# Foo\r\n\r\n[foo]: bar\r\n\r\nSee [foo]\r\n", () =>
		{
			Heading(0..7, 1, new HtmlAttributeList() { { "id", "foo" } }, () =>
			{
				Literal(2..5, "Foo");
			});
			LinkDefinition(9..21, "foo", "bar", null);
			Paragraph(23..34, () =>
			{
				Literal(23..27, "See ");
				Link(27..32, "bar", null, () =>
				{
					Literal(28..31, "foo");
				});
			});
		});
	}

	/// <summary>
	/// 标题的属性不会随着链接引用传递。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/header-references.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("# Foo {.class}\r\n\r\n[foo]\r\n", () =>
		{
			Heading(0..16, 1, new HtmlAttributeList() {
				{ "class", "class" },
				{ "id", "foo" },
			}, () =>
			{
				Literal(2..5, "Foo");
			});
			Paragraph(18..25, () =>
			{
				Link(18..23, "#foo", null, () =>
				{
					Literal(19..22, "foo");
				});
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持标题引用。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("# Heading 1\r\n\r\n[Heading 1]\r\n", () =>
		{
			Heading(0..13, 1, () =>
			{
				Literal(2..11, "Heading 1");
			});
			Paragraph(15..28, () =>
			{
				Literal(15..26, "[Heading 1]");
			});
		});
	}
}

