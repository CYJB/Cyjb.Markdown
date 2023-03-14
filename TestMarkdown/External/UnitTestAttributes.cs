using Cyjb.Markdown;
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
		AssertMarkdown("# foo {#id .class attr=value attr2=\"value={2}\"}\r\n\r\nbar { .class2  #id2 .class2 .class3}\r\n---\r\n\r\n### baz {#other}{#id #id3}\r\n", () =>
		{
			Heading(0..49, 1, new HtmlAttributeList() {
				{ "id", "id" },
				{ "class", "class" },
				{ "attr", "value" },
				{ "attr2", "value={2}" },
			}, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(51..94, 2, new HtmlAttributeList() {
				{ "class", "class2 class2 class3" },
				{ "id", "id2" },
			}, () =>
			{
				Literal(51..54, "bar");
			});
			Heading(96..124, 3, new HtmlAttributeList() { { "id", "id3" } }, () =>
			{
				Literal(100..112, "baz {#other}");
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
		AssertMarkdown("# foo {\r\n#id-foo\r\n   key='value'   \r\n}\r\n\r\nbar\r\n{\r\n  .class\r\n}\r\n---\r\n", () =>
		{
			Heading(0..40, 1, new HtmlAttributeList() {
				{ "id", "id-foo" },
				{ "key", "value" },
			}, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(42..68, 2, new HtmlAttributeList() {
				{ "class", "class" },
				{ "id", "bar" },
			}, () =>
			{
				Literal(42..45, "bar");
				SoftBreak(45..47);
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
				{ "key", "value" } ,
				{ "id", "foo" } ,
			}, () =>
			{
				Literal(2..5, "foo");
			});
			Heading(23..47, 2, new HtmlAttributeList() {
				{ "class", "class" },
				{ "id", "bar" } ,
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
			Heading(0..9, 1, new HtmlAttributeList() { { "id", "foo" } }, () =>
			{
				Literal(2..7, "foo {");
			});
			Paragraph(9..25, () =>
			{
				Literal(9..16, "#id-foo");
				SoftBreak(16..18);
				Literal(18..23, "} bar");
			});
			Heading(27..43, 2, new HtmlAttributeList() { { "id", "bar" } }, () =>
			{
				Literal(27..36, "bar { # }");
			});
			Heading(43..59, 1, new HtmlAttributeList() { { "id", "baz-id" } }, () =>
			{
				Literal(45..55, "baz {#id}");
			});
			Heading(59..87, 1, new HtmlAttributeList() { { "id", "id3" } }, () =>
			{
				Literal(61..78, "bim {#id} {#id2}");
			});
		});
	}

	/// <summary>
	/// 属性前不能是未转义的 <c>\</c> 符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("# foo \\{#id}\r\n# foo \\\\{#id myKey}\r\n", () =>
		{
			Heading(0..14, 1, new HtmlAttributeList() { { "id", "foo-id" } }, () =>
			{
				Literal(2..12, "foo {#id}");
			});
			Heading(14..35, 1, new HtmlAttributeList()
			{
				{ "id", "id" },
				{ "myKey", "" },
			}, () =>
			{
				Literal(16..22, "foo \\");
			});
		});
	}

	/// <summary>
	/// <c>#</c> 或 <c>.</c> 后面必须包含有效字符，<c>=</c> 前后也必须包含属性键或值。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("# foo {#}\r\n# bar { . }\r\n# baz { = }\r\n# bim { =value }\r\n# boo { k = v}\r\n", () =>
		{
			Heading(0..11, 1, new HtmlAttributeList() { { "id", "foo" } }, () =>
			{
				Literal(2..9, "foo {#}");
			});
			Heading(11..24, 1, new HtmlAttributeList() { { "id", "bar" } }, () =>
			{
				Literal(13..22, "bar { . }");
			});
			Heading(24..37, 1, new HtmlAttributeList() { { "id", "baz" } }, () =>
			{
				Literal(26..35, "baz { = }");
			});
			Heading(37..55, 1, new HtmlAttributeList() { { "id", "bim-value" } }, () =>
			{
				Literal(39..53, "bim { =value }");
			});
			Heading(55..71, 1, new HtmlAttributeList() { { "id", "boo-k-v" } }, () =>
			{
				Literal(57..69, "boo { k = v}");
			});
		});
	}

	/// <summary>
	/// <c>#</c> 或 <c>.</c> 后面必须包含有效字符，<c>=</c> 前后也必须包含属性键或值。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("# foo {\r\n\r\n  #id }\r\n# bar { key=\r\nvalue }\r\n# baz {\r\n.class\r\n\r\n}\r\n", () =>
		{
			Heading(0..9, 1, new HtmlAttributeList() { { "id", "foo" } }, () =>
			{
				Literal(2..7, "foo {");
			});
			Paragraph(13..20, () =>
			{
				Literal(13..18, "#id }");
			});
			Heading(20..34, 1, new HtmlAttributeList() { { "id", "bar-key" } }, () =>
			{
				Literal(22..32, "bar { key=");
			});
			Paragraph(34..43, () =>
			{
				Literal(34..41, "value }");
			});
			Heading(43..52, 1, new HtmlAttributeList() { { "id", "baz" } }, () =>
			{
				Literal(45..50, "baz {");
			});
			Paragraph(52..60, () =>
			{
				Literal(52..58, ".class");
			});
			Paragraph(62..65, () =>
			{
				Literal(62..63, "}");
			});
		});
	}

	/// <summary>
	/// 可以在代码块信息的末尾指定属性，属性会附加到代码块的元素上。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-8"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("``` foo {#id .class attr=value attr2=\"value={2}\"}\r\n  bar\r\n```\r\n``` baz { .class2  #id2 .class2 .class3}\r\nbim\r\n```\r\n", () =>
		{
			CodeBlock(0..63, "  bar\r\n", "foo", new HtmlAttributeList()
			{
				{ "id", "id" },
				{ "class", "class" },
				{ "attr", "value" },
				{ "attr2", "value={2}" },
			});
			CodeBlock(63..115, "bim\r\n", "baz", new HtmlAttributeList()
			{
				{ "class", "class2 class2 class3" },
				{ "id", "id2" },
			});
		});
	}

	/// <summary>
	/// 允许在代码块中使用多行属性，起始 <c>{</c> 必须与代码块的信息在一行内。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-9"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("~~~ foo {\r\n     key='value'   \r\n    }\r\nbar\r\n~~~\r\n~~~~~ baz  \r\n{   \r\n  .class\r\n    }\r\n  bim  \r\n~~~~~\r\n", () =>
		{
			CodeBlock(0..49, "bar\r\n", "foo", new HtmlAttributeList() { { "key", "value" } });
			CodeBlock(49..101, "{   \r\n  .class\r\n    }\r\n  bim  \r\n", "baz");
		});
	}

	/// <summary>
	/// 会忽略属性后的空格或 Tab。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-10"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("~~~ foo {key=value}   \r\nbar\r\n~~~\r\n", () =>
		{
			CodeBlock(0..34, "bar\r\n", "foo", new HtmlAttributeList() { { "key", "value" } });
		});
	}

	/// <summary>
	/// 如果属性不合法，或者不在代码块信息的末尾，那么会当成普通文本。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-11"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown("~~~ foo {\r\n  #id-foo\r\n} bar\r\n~~~\r\n~~~ bim { # }\r\nboo\r\n~~~\r\n~~~ b1 \\{#id}  \r\nb2\r\n~~~\r\n~~~ b3 \\{#id} {#id2} {#id3}\r\nb4\r\n~~~\r\n", () =>
		{
			CodeBlock(0..34, "  #id-foo\r\n} bar\r\n", "foo {");
			CodeBlock(34..59, "boo\r\n", "bim { # }");
			CodeBlock(59..85, "b2\r\n", "b1 {#id}");
			CodeBlock(85..123, "b4\r\n", "b3 {#id} {#id2}", new HtmlAttributeList() { { "id", "id3" } });
		});
	}

	/// <summary>
	/// 属性前不能是未转义的 <c>\</c> 符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-12"/>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown("~~~ foo \\{#id}\r\nbar\r\n~~~\r\n~~~ foo\\\\{#id myKey}\r\nbar\r\n~~~\r\n", () =>
		{
			CodeBlock(0..26, "bar\r\n", "foo {#id}");
			CodeBlock(26..58, "bar\r\n", "foo\\", new HtmlAttributeList() {
				{ "id", "id" },
				{ "myKey", "" },
			});
		});
	}

	/// <summary>
	/// <c>#</c> 或 <c>.</c> 后面必须包含有效字符，<c>=</c> 前后也必须包含属性键或值。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-13"/>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown("~~~foo{#}\r\n~~~\r\n~~~ bar{ . }\r\n~~~\r\n~~~ baz{ = }\r\n~~~\r\n~~~ bim { =value }\r\n~~~\r\n~~~ boo{k = v}\r\n~~~\r\n", () =>
		{
			CodeBlock(0..16, "", "foo{#}");
			CodeBlock(16..35, "", "bar{ . }");
			CodeBlock(35..54, "", "baz{ = }");
			CodeBlock(54..79, "", "bim { =value }");
			CodeBlock(79..100, "", "boo{k = v}");
		});
	}

	/// <summary>
	/// 属性中至多使用一个换行分割，不能包含更多换行。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-14"/>
	[TestMethod]
	public void Test14()
	{
		AssertMarkdown("~~~ foo {\r\n\r\n  #id }\r\n~~~\r\n~~~ bar { key=\r\nvalue }\r\n~~~\r\n~~~ baz {\r\n.class\r\n\r\n}\r\n", () =>
		{
			CodeBlock(0..27, "\r\n  #id }\r\n", "foo {");
			CodeBlock(27..57, "value }\r\n", "bar { key=");
			CodeBlock(57..81, ".class\r\n\r\n}\r\n", "baz {");
		});
	}

	/// <summary>
	/// 可以在链接的末尾指定属性，属性会附加到链接的元素上。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-15"/>
	[TestMethod]
	public void Test15()
	{
		AssertMarkdown("<http://foo>{#id .class attr=value attr2=\"value={2}\"}\r\n\r\n[bar](/uri){ .class2  #id2 .class2 .class3}\r\n\r\n[baz text][baz]\r\n[baz]\r\n\r\n[baz]:/uri2 {#id #id3}\r\n", () =>
		{
			Paragraph(0..55, () =>
			{
				Link(0..53, "http://foo", null, new HtmlAttributeList()
				{
					{ "id", "id" },
					{ "class", "class" },
					{ "attr", "value" },
					{ "attr2", "value={2}" },
				}, () =>
				{
					Literal(1..11, "http://foo");
				});
			});
			Paragraph(57..102, () =>
			{
				Link(57..100, "/uri", null, new HtmlAttributeList()
				{
					{ "class", "class2 class2 class3" },
					{ "id", "id2" },
				}, () =>
				{
					Literal(58..61, "bar");
				});
			});
			Paragraph(104..128, () =>
			{
				Link(104..119, "/uri2", null, new HtmlAttributeList() { { "id", "id3" } }, () =>
				{
					Literal(105..113, "baz text");
				});
				SoftBreak(119..121);
				Link(121..126, "/uri2", null, new HtmlAttributeList() { { "id", "id3" } }, () =>
				{
					Literal(122..125, "baz");
				});
			});
			LinkDefinition(130..154, "baz", "/uri2", null, new HtmlAttributeList() { { "id", "id3" } });
		});
	}

	/// <summary>
	/// 链接与属性之前不能有空白，但是在链接声明中，属性之前必须要有空白。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-16"/>
	[TestMethod]
	public void Test16()
	{
		AssertMarkdown("<http://foo> {#id}\r\n[bar](/uri)\r\n{.class}\r\n[baz]\r\n\r\n[baz]:/uri2{#id #id3}\r\n", () =>
		{
			Paragraph(0..50, () =>
			{
				Link(0..12, "http://foo", null, () =>
				{
					Literal(1..11, "http://foo");
				});
				Literal(12..18, " {#id}");
				SoftBreak(18..20);
				Link(20..31, "/uri", null, () =>
				{
					Literal(21..24, "bar");
				});
				SoftBreak(31..33);
				Literal(33..41, "{.class}");
				SoftBreak(41..43);
				Literal(43..48, "[baz]");
			});
			Paragraph(52..75, () =>
			{
				Literal(52..73, "[baz]:/uri2{#id #id3}");
			});
		});
	}

	/// <summary>
	/// 使用链接声明时，只会使用链接声明中的属性，而不会再识别附加的属性。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-17"/>
	[TestMethod]
	public void Test17()
	{
		AssertMarkdown("[baz]{#id}\r\n[baz text][baz]{#id}\r\n\r\n[baz]:/uri \"title\" {.class}\r\n", () =>
		{
			Paragraph(0..34, () =>
			{
				Link(0..5, "/uri", "title", new HtmlAttributeList() { { "class", "class" } }, () =>
				{
					Literal(1..4, "baz");
				});
				Literal(5..10, "{#id}");
				SoftBreak(10..12);
				Link(12..27, "/uri", "title", new HtmlAttributeList() { { "class", "class" } }, () =>
				{
					Literal(13..21, "baz text");
				});
				Literal(27..32, "{#id}");
			});
			LinkDefinition(36..65, "baz", "/uri", "title", new HtmlAttributeList() { { "class", "class" } });
		});
	}

	/// <summary>
	/// 允许在链接中使用多行属性。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-18"/>
	[TestMethod]
	public void Test18()
	{
		AssertMarkdown("<http://foo>{\r\n   key='value'   \r\n}\r\n[bar](/uri){.class\r\n#id}\r\n[baz]\r\n\r\n[baz]:/uri2  \r\n{\r\n  #id2 key=\"v\r\n  alue\"\r\n  .class  .class2\r\n  key2=value2\r\n}\r\n", () =>
		{
			Paragraph(0..70, () =>
			{
				Link(0..35, "http://foo", null, new HtmlAttributeList() { { "key", "value" } }, () =>
				{
					Literal(1..11, "http://foo");
				});
				SoftBreak(35..37);
				Link(37..61, "/uri", null, new HtmlAttributeList() {
					{ "class", "class" },
					{ "id", "id" },
				}, () =>
				{
					Literal(38..41, "bar");
				});
				SoftBreak(61..63);
				Link(63..68, "/uri2", null, new HtmlAttributeList() {
					{ "id", "id2" },
					{ "key", "v\r\n  alue" },
					{ "class", "class class2" },
					{ "key2", "value2" },
				}, () =>
				{
					Literal(64..67, "baz");
				});
			});
			LinkDefinition(72..151, "baz", "/uri2", null, new HtmlAttributeList() {
				{ "id", "id2" },
				{ "key", "v\r\n  alue" },
				{ "class", "class class2" },
				{ "key2", "value2" },
			 });
		});
	}

	/// <summary>
	/// 会忽略属性后的空格或 Tab。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-19"/>
	[TestMethod]
	public void Test19()
	{
		AssertMarkdown("<http://foo>{key=value} \t\r\n[bar](/uri){ .class }  text\r\n[baz]\r\n[baz3]\r\n\r\n[baz]:/uri2  \r\n  \"title\" { key=\"value{}\"}  \r\n[baz3]:/uri3  \r\n  \"title3\" {#id3} other\r\n", () =>
		{
			Paragraph(0..71, () =>
			{
				Link(0..23, "http://foo", null, new HtmlAttributeList() { { "key", "value" } }, () =>
				{
					Literal(1..11, "http://foo");
				});
				Literal(23..25, " \t");
				SoftBreak(25..27);
				Link(27..48, "/uri", null, new HtmlAttributeList() { { "class", "class" } }, () =>
				{
					Literal(28..31, "bar");
				});
				Literal(48..54, "  text");
				SoftBreak(54..56);
				Link(56..61, "/uri2", "title", new HtmlAttributeList() { { "key", "value{}" } }, () =>
				{
					Literal(57..60, "baz");
				});
				SoftBreak(61..63);
				Link(63..69, "/uri3", null, () =>
				{
					Literal(64..68, "baz3");
				});
			});
			LinkDefinition(73..118, "baz", "/uri2", "title", new HtmlAttributeList() { { "key", "value{}" } });
			LinkDefinition(118..134, "baz3", "/uri3");
			Paragraph(136..159, () =>
			{
				Literal(136..157, "\"title3\" {#id3} other");
			});
		});
	}

	/// <summary>
	/// 属性前不能是未转义的 <c>\</c> 符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-20"/>
	[TestMethod]
	public void Test20()
	{
		AssertMarkdown("[foo]\r\n\r\n[foo]:/uri \\{#id}\r\n\r\n[foo]:/uri2 \\\\{#id}\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..5, "[foo]");
			});
			Paragraph(9..28, () =>
			{
				Literal(9..26, "[foo]:/uri {#id}");
			});
			Paragraph(30..51, () =>
			{
				Literal(30..49, "[foo]:/uri2 \\{#id}");
			});
		});
	}

	/// <summary>
	/// <c>#</c> 或 <c>.</c> 后面必须包含有效字符，<c>=</c> 前后也必须包含属性键或值。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-21"/>
	[TestMethod]
	public void Test21()
	{
		AssertMarkdown("<http://foo>{#}\r\n[bar](/uri){ . }\r\n[baz]\r\n\r\n[baz]:/uri2 { = }\r\n\r\n[baz]:/uri3 { =value }\r\n\r\n[baz]:/uri3 { k = v }\r\n", () =>
		{
			Paragraph(0..42, () =>
			{
				Link(0..12, "http://foo", null, () =>
				{
					Literal(1..11, "http://foo");
				});
				Literal(12..15, "{#}");
				SoftBreak(15..17);
				Link(17..28, "/uri", null, () =>
				{
					Literal(18..21, "bar");
				});
				Literal(28..33, "{ . }");
				SoftBreak(33..35);
				Literal(35..40, "[baz]");
			});
			Paragraph(44..63, () =>
			{
				Literal(44..61, "[baz]:/uri2 { = }");
			});
			Paragraph(65..89, () =>
			{
				Literal(65..87, "[baz]:/uri3 { =value }");
			});
			Paragraph(91..114, () =>
			{
				Literal(91..112, "[baz]:/uri3 { k = v }");
			});
		});
	}

	/// <summary>
	/// 属性中至多使用一个换行分割，不能包含更多换行。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#example-22"/>
	[TestMethod]
	public void Test22()
	{
		AssertMarkdown("<http://foo>{\r\n\r\n  #id }\r\n[bar](/uri){ key=\r\nvalue }\r\n[baz]\r\n\r\n[baz]:/uri2 {\r\n.class\r\n\r\n}\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..12, "http://foo", null, () =>
				{
					Literal(1..11, "http://foo");
				});
				Literal(12..13, "{");
			});
			Paragraph(19..61, () =>
			{
				Literal(19..24, "#id }");
				SoftBreak(24..26);
				Link(26..37, "/uri", null, () =>
				{
					Literal(27..30, "bar");
				});
				Literal(37..43, "{ key=");
				SoftBreak(43..45);
				Literal(45..52, "value }");
				SoftBreak(52..54);
				Literal(54..59, "[baz]");
			});
			Paragraph(63..86, () =>
			{
				Literal(63..76, "[baz]:/uri2 {");
				SoftBreak(76..78);
				Literal(78..84, ".class");
			});
			Paragraph(88..91, () =>
			{
				Literal(88..89, "}");
			});
		});
	}

	/// <summary>
	/// 测试为属性添加前缀。
	/// </summary>
	[TestMethod]
	public void TestAttributePrefix()
	{
		ParseOptions options = new()
		{
			AttributesPrefix = "data-",
		};
		AssertMarkdown("# foo {#id key=value}\r\n``` foo {#id .class2 k=v}\r\n  bar\r\n```\r\n\r\n[baz](/uri){#id k=v}\r\n[bom]\r\n\r\n[bom]:/uri {#id k=v}\r\n", options, () =>
		{
			Heading(0..23, 1, new HtmlAttributeList() {
				{ "id", "id" },
				{ "data-key", "value" },
			}, () =>
			{
				Literal(2..5, "foo");
			});
			CodeBlock(23..62, "  bar\r\n", "foo", new HtmlAttributeList()
			{
				{ "id", "id" },
				{ "class", "class2" },
				{ "data-k", "v" },
			});
			Paragraph(64..93, () =>
			{
				Link(64..84, "/uri", null, new HtmlAttributeList()
				{
					{ "id", "id" },
					{ "data-k", "v" },
				}, () =>
				{
					Literal(65..68, "baz");
				});
				SoftBreak(84..86);
				Link(86..91, "/uri", null, new HtmlAttributeList() {
					{ "id", "id" },
					{ "data-k", "v" } ,
				}, () =>
				{
					Literal(87..90, "bom");
				});
			});
			LinkDefinition(95..117, "bom", "/uri", null, new HtmlAttributeList() {
				{ "id", "id" } ,
				{ "data-k", "v" } ,
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持属性。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("# foo {#id}\r\n``` foo {#id}\r\n  bar\r\n```\r\n\r\n[baz](/uri){#id}\r\n[bom]\r\n\r\n[bom]:/uri {#id}\r\n", () =>
		{
			Heading(0..13, 1, () =>
			{
				Literal(2..11, "foo {#id}");
			});
			CodeBlock(13..40, "  bar\r\n", "foo {#id}");
			Paragraph(42..67, () =>
			{
				Link(42..53, "/uri", null, () =>
				{
					Literal(43..46, "baz");
				});
				Literal(53..58, "{#id}");
				SoftBreak(58..60);
				Literal(60..65, "[bom]");
			});
			Paragraph(69..87, () =>
			{
				Literal(69..85, "[bom]:/uri {#id}");
			});
		});
	}
}

