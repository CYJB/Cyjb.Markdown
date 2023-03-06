using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 属性的单元测试。
/// </summary>
[TestClass]
public class UnitTestAttributes : BaseTest
{
	/// <summary>
	/// 可以在标题的末尾指定属性，属性会附加到标题的元素上。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("# foo {#id .class attr=value attr2=value2}\r\n\r\nbar { .class2  #id2 .class2 .class3}\r\n---\r\n\r\n### baz {#other}{#id #id3}\r\n", () =>
		{
			Heading(0..44, 1, new HtmlAttributeList() {
				{ "id", "id" },
				{ "class", "class" },
				{ "attr", "value" },
				{ "attr2", "value2" },
			}, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(46..89, 2, new HtmlAttributeList() {
				{ "class", "class2 class2 class3" },
				{ "id", "id2" },
			}, () =>
			{
				Literal(46..49, "bar");
			});
			Heading(91..119, 3, new HtmlAttributeList() {
				{ "id", "id3" },
			}, () =>
			{
				Literal(95..107, "baz {#other}");
			});
		});
	}

	/// <summary>
	/// 允许在标题中使用多行属性。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("# foo {\r\n#id-foo\r\n   key=value   \r\n}\r\n\r\nbar\r\n{\r\n  .class\r\n}\r\n---\r\n", () =>
		{
			Heading(0..38, 1, new HtmlAttributeList() {
				{ "id", "id-foo" },
				{ "key", "value" },
			}, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(40..66, 2, new HtmlAttributeList() {
				{ "class", "class" },
			}, () =>
			{
				Literal(40..43, "bar");
				SoftBreak(43..45);
			});
		});
	}

	/// <summary>
	/// 会忽略属性后的空格或 Tab。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("# foo {key=value} \t\r\n\r\nbar { .class }  \t\r\n---\r\n", () =>
		{
			Heading(0..21, 1, new HtmlAttributeList() {
				{ "key", "value" },
			}, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(23..47, 2, new HtmlAttributeList() {
				{ "class", "class" },
			}, () =>
			{
				Literal(23..26, "bar");
			});
		});
	}

	/// <summary>
	/// 如果属性不合法，或者不在标题的末尾，那么会当成普通文本。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("# foo {\r\n#id-foo\r\n} bar\r\n\r\nbar { # }\r\n---\r\n# baz \\{#id}  \r\n# bim \\{#id} {#id2} {#id3}\r\n", () =>
		{
			Heading(0..9, 1, () =>
			{
				Literal(2..7, "foo {");
			});
			Paragraph(9..25, () =>
			{
				Literal(9..16, "#id-foo");
				SoftBreak(16..18);
				Literal(18..23, "} bar");
			});
			Heading(27..43, 2, () =>
			{
				Literal(27..36, "bar { # }");
			});
			Heading(43..59, 1, () =>
			{
				Literal(45..55, "baz {#id}");
			});
			Heading(59..87, 1, new HtmlAttributeList()
			{
				{ "id", "id3" }
			}, () =>
			{
				Literal(61..78, "bim {#id} {#id2}");
			});
		});
	}

	/// <summary>
	/// 属性前不能是未转义的 `\` 符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("# foo \\{#id}\r\n# foo \\\\{#id}\r\n", () =>
		{
			Heading(0..14, 1, () =>
			{
				Literal(2..12, "foo {#id}");
			});
			Heading(14..29, 1, new HtmlAttributeList()
			{
				{ "id", "id" }
			}, () =>
			{
				Literal(16..22, "foo \\");
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持属性。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("# foo {#id}\r\n", () =>
		{
			Heading(0..13, 1, () =>
			{
				Literal(2..11, "foo {#id}");
			});
		});
	}
}

