using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 链接的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#links"/>
[TestClass]
public class UnitTestLink : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-482"/>
	[TestMethod]
	public void Test482()
	{
		AssertMarkdown("[link](/uri \"title\")", () =>
		{
			Paragraph(0..20, () =>
			{
				Link(0..20, "/uri", "title", () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 标题、连接文本甚至 URL 都可以省略。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-483"/>
	[TestMethod]
	public void Test483()
	{
		AssertMarkdown("[link](/uri)", () =>
		{
			Paragraph(0..12, () =>
			{
				Link(0..12, "/uri", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-484"/>
	[TestMethod]
	public void Test484()
	{
		AssertMarkdown("[](./target.md)", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..15, "./target.md", null);
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-485"/>
	[TestMethod]
	public void Test485()
	{
		AssertMarkdown("[link]()", () =>
		{
			Paragraph(0..8, () =>
			{
				Link(0..8, string.Empty, null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-486"/>
	[TestMethod]
	public void Test486()
	{
		AssertMarkdown("[link](<>)", () =>
		{
			Paragraph(0..10, () =>
			{
				Link(0..10, string.Empty, null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-487"/>
	[TestMethod]
	public void Test487()
	{
		AssertMarkdown("[]()", () =>
		{
			Paragraph(0..4, () =>
			{
				Link(0..4, string.Empty, null);
			});
		});
	}
	/// <summary>
	/// 链接目标仅在使用尖括号包起来时才能使用空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-488"/>
	[TestMethod]
	public void Test488()
	{
		AssertMarkdown("[link](/my uri)", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..15, "[link](/my uri)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-489"/>
	[TestMethod]
	public void Test489()
	{
		AssertMarkdown("[link](</my uri>)", () =>
		{
			Paragraph(0..17, () =>
			{
				Link(0..17, "/my uri", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 链接目标不能包含换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-490"/>
	[TestMethod]
	public void Test490()
	{
		AssertMarkdown("[link](foo\r\nbar)", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..10, "[link](foo");
				SoftBreak(10..12);
				Literal(12..16, "bar)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-491"/>
	[TestMethod]
	public void Test491()
	{
		AssertMarkdown("[link](<foo\r\nbar>)", () =>
		{
			Paragraph(0..18, () =>
			{
				Literal(0..7, "[link](");
				HtmlStartTag(7..17, "<foo\r\nbar>");
				Literal(17..18, ")");
			});
		});
	}
	/// <summary>
	/// 链接目标被尖括号包起来时可以包含 )。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-492"/>
	[TestMethod]
	public void Test492()
	{
		AssertMarkdown("[a](<b)c>)", () =>
		{
			Paragraph(0..10, () =>
			{
				Link(0..10, "b)c", null, () =>
				{
					Literal(1..2, "a");
				});
			});
		});
	}
	/// <summary>
	/// 包裹的尖括号不能被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-493"/>
	[TestMethod]
	public void Test493()
	{
		AssertMarkdown(@"[link](<foo\>)", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..14, @"[link](<foo>)");
			});
		});
	}
	/// <summary>
	/// 链接的 ( 必须匹配。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-494"/>
	[TestMethod]
	public void Test494()
	{
		AssertMarkdown("[a](<b)c\r\n[a](<b)c>\r\n[a](<b>c)", () =>
		{
			Paragraph(0..30, () =>
			{
				Literal(0..8, "[a](<b)c");
				SoftBreak(8..10);
				Literal(10..19, "[a](<b)c>");
				SoftBreak(19..21);
				Literal(21..25, "[a](");
				HtmlStartTag(25..28, "<b>");
				Literal(28..30, "c)");
			});
		});
	}
	/// <summary>
	/// 链接目标中的括号应该被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-495"/>
	[TestMethod]
	public void Test495()
	{
		AssertMarkdown(@"[link](\(foo\))", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..15, "(foo)", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 允许个数平衡的非转义括号。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-496"/>
	[TestMethod]
	public void Test496()
	{
		AssertMarkdown(@"[link](foo(and(bar)))", () =>
		{
			Paragraph(0..21, () =>
			{
				Link(0..21, "foo(and(bar))", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 但是未平衡的括号，必须转义或者使用尖括号包起来。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-497"/>
	[TestMethod]
	public void Test497()
	{
		AssertMarkdown(@"[link](foo(and(bar))", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..20, "[link](foo(and(bar))");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-498"/>
	[TestMethod]
	public void Test498()
	{
		AssertMarkdown(@"[link](foo\(and\(bar\))", () =>
		{
			Paragraph(0..23, () =>
			{
				Link(0..23, "foo(and(bar)", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-499"/>
	[TestMethod]
	public void Test499()
	{
		AssertMarkdown(@"[link](<foo(and(bar)>)", () =>
		{
			Paragraph(0..22, () =>
			{
				Link(0..22, "foo(and(bar)", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 支持其它转义字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-500"/>
	[TestMethod]
	public void Test500()
	{
		AssertMarkdown(@"[link](foo\)\:)", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..15, "foo):", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 链接可以包含 fragment 和查询。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-501"/>
	[TestMethod]
	public void Test501()
	{
		AssertMarkdown(@"[link](#fragment)

[link](https://example.com#fragment)

[link](https://example.com?foo=3#frag)", () =>
		{
			Paragraph(0..19, () =>
			{
				Link(0..17, "#fragment", null, () =>
				{
					Literal(1..5, "link");
				});
			});

			Paragraph(21..59, () =>
			{
				Link(21..57, "https://example.com#fragment", null, () =>
				{
					Literal(22..26, "link");
				});
			});

			Paragraph(61..99, () =>
			{
				Link(61..99, "https://example.com?foo=3#frag", null, () =>
				{
					Literal(62..66, "link");
				});
			});
		});
	}
	/// <summary>
	/// 非可转义字符前的反斜杠会被原样保留。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-502"/>
	[TestMethod]
	public void Test502()
	{
		AssertMarkdown(@"[link](foo\bar)", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..15, "foo\\bar", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// URL 编码的字符会被原样保留。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-503"/>
	[TestMethod]
	public void Test503()
	{
		AssertMarkdown("[link](foo%20b&auml;)", () =>
		{
			Paragraph(0..21, () =>
			{
				Link(0..21, "foo%20bä", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 保留标题但忽略目标时，标题很可能会被解析为目标。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-504"/>
	[TestMethod]
	public void Test504()
	{
		AssertMarkdown("[link](\"title\")", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..15, "\"title\"", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 标题可以使用单引号、双引号或括号。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-505"/>
	[TestMethod]
	public void Test505()
	{
		AssertMarkdown(@"[link](/url ""title"")
[link](/url 'title')
[link](/url (title))", () =>
		{
			Paragraph(0..64, () =>
			{
				Link(0..20, "/url", "title", () =>
				{
					Literal(1..5, "link");
				});
				SoftBreak(20..22);

				Link(22..42, "/url", "title", () =>
				{
					Literal(23..27, "link");
				});
				SoftBreak(42..44);

				Link(44..64, "/url", "title", () =>
				{
					Literal(45..49, "link");
				});
			});
		});
	}
	/// <summary>
	/// 标题可以使用转义或实体。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-506"/>
	[TestMethod]
	public void Test506()
	{
		AssertMarkdown(@"[link](/url ""title \""&quot;"")", () =>
		{
			Paragraph(0..29, () =>
			{
				Link(0..29, "/url", "title \"\"", () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 必须使用空格、Tab 或最多一个换行分割，其它 Unicode 空白不会当作分割符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-507"/>
	[TestMethod]
	public void Test507()
	{
		AssertMarkdown("[link](/url\xA0\"title\")", () =>
		{
			Paragraph(0..20, () =>
			{
				Link(0..20, "/url\xA0\"title\"", null, () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 嵌套的引号必须被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-508"/>
	[TestMethod]
	public void Test508()
	{
		AssertMarkdown("[link](/url \"title \"and\" title\")", () =>
		{
			Paragraph(0..32, () =>
			{
				Literal(0..32, "[link](/url \"title \"and\" title\")");
			});
		});
	}
	/// <summary>
	/// 可以使用不同类型的引号。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-509"/>
	[TestMethod]
	public void Test509()
	{
		AssertMarkdown("[link](/url 'title \"and\" title')", () =>
		{
			Paragraph(0..32, () =>
			{
				Link(0..32, "/url", "title \"and\" title", () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 链接目标和标题前可以有空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-510"/>
	[TestMethod]
	public void Test510()
	{
		AssertMarkdown("[link](   /uri\r\n  \"title\"  )", () =>
		{
			Paragraph(0..28, () =>
			{
				Link(0..28, "/uri", "title", () =>
				{
					Literal(1..5, "link");
				});
			});
		});
	}
	/// <summary>
	/// 但链接文本和后续括号间不能有空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-511"/>
	[TestMethod]
	public void Test511()
	{
		AssertMarkdown("[link] (/uri)", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..13, "[link] (/uri)");
			});
		});
	}
	/// <summary>
	/// 链接文本可以包含平衡的括号，但不平衡的括号必须被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-512"/>
	[TestMethod]
	public void Test512()
	{
		AssertMarkdown("[link [foo [bar]]](/uri)", () =>
		{
			Paragraph(0..24, () =>
			{
				Link(0..24, "/uri", null, () =>
				{
					Literal(1..17, "link [foo [bar]]");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-513"/>
	[TestMethod]
	public void Test513()
	{
		AssertMarkdown("[link] bar](/uri)", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..17, "[link] bar](/uri)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-514"/>
	[TestMethod]
	public void Test514()
	{
		AssertMarkdown("[link [bar](/uri)", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..6, "[link ");
				Link(6..17, "/uri", null, () =>
				{
					Literal(7..10, "bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-515"/>
	[TestMethod]
	public void Test515()
	{
		AssertMarkdown(@"[link \[bar](/uri)", () =>
		{
			Paragraph(0..18, () =>
			{
				Link(0..18, "/uri", null, () =>
				{
					Literal(1..11, "link [bar");
				});
			});
		});
	}
	/// <summary>
	/// 链接文本可以包含行内元素。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-516"/>
	[TestMethod]
	public void Test516()
	{
		AssertMarkdown("[link *foo **bar** `#`*](/uri)", () =>
		{
			Paragraph(0..30, () =>
			{
				Link(0..30, "/uri", null, () =>
				{
					Literal(1..6, "link ");
					Emphasis(6..23, () =>
					{
						Literal(7..11, "foo ");
						Strong(11..18, () =>
						{
							Literal(13..16, "bar");
						});
						Literal(18..19, " ");
						CodeSpan(19..22, "#");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-517"/>
	[TestMethod]
	public void Test517()
	{
		AssertMarkdown("[![moon](moon.jpg)](/uri)", () =>
		{
			Paragraph(0..25, () =>
			{
				Link(0..25, "/uri", null, () =>
				{
					Image(1..18, "moon.jpg", null, () =>
					{
						Literal(3..7, "moon");
					});
				});
			});
		});
	}
	/// <summary>
	/// 但是链接不能包含其它链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-518"/>
	[TestMethod]
	public void Test518()
	{
		AssertMarkdown("[foo [bar](/uri)](/uri)", () =>
		{
			Paragraph(0..23, () =>
			{
				Literal(0..5, "[foo ");
				Link(5..16, "/uri", null, () =>
				{
					Literal(6..9, "bar");
				});
				Literal(16..23, "](/uri)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-519"/>
	[TestMethod]
	public void Test519()
	{
		AssertMarkdown("[foo *[bar [baz](/uri)](/uri)*](/uri)", () =>
		{
			Paragraph(0..37, () =>
			{
				Literal(0..5, "[foo ");
				Emphasis(5..30, () =>
				{
					Literal(6..11, "[bar ");
					Link(11..22, "/uri", null, () =>
					{
						Literal(12..15, "baz");
					});
					Literal(22..29, "](/uri)");
				});
				Literal(30..37, "](/uri)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-520"/>
	[TestMethod]
	public void Test520()
	{
		AssertMarkdown("![[[foo](uri1)](uri2)](uri3)", () =>
		{
			Paragraph(0..28, () =>
			{
				Image(0..28, "uri3", null, () =>
				{
					Literal(2..3, "[");
					Link(3..14, "uri1", null, () =>
					{
						Literal(4..7, "foo");
					});
					Literal(14..21, "](uri2)");
				});
			});
		});
	}
	/// <summary>
	/// 链接文本优先级高于强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-521"/>
	[TestMethod]
	public void Test521()
	{
		AssertMarkdown("*[foo*](/uri)", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..1, "*");
				Link(1..13, "/uri", null, () =>
				{
					Literal(2..6, "foo*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-522"/>
	[TestMethod]
	public void Test522()
	{
		AssertMarkdown("[foo *bar](baz*)", () =>
		{
			Paragraph(0..16, () =>
			{
				Link(0..16, "baz*", null, () =>
				{
					Literal(1..9, "foo *bar");
				});
			});
		});
	}
	/// <summary>
	/// 非链接一部分的括号没有优先级。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-523"/>
	[TestMethod]
	public void Test523()
	{
		AssertMarkdown("*foo [bar* baz]", () =>
		{
			Paragraph(0..15, () =>
			{
				Emphasis(0..10, () =>
				{
					Literal(1..9, "foo [bar");
				});
				Literal(10..15, " baz]");
			});
		});
	}
	/// <summary>
	/// HTML 标签、代码段、自动链接的优先级高于链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-524"/>
	[TestMethod]
	public void Test524()
	{
		AssertMarkdown("[foo <bar attr=\"](baz)\">", () =>
		{
			Paragraph(0..24, () =>
			{
				Literal(0..5, "[foo ");
				HtmlStartTag(5..24, "<bar attr=\"](baz)\">");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-525"/>
	[TestMethod]
	public void Test525()
	{
		AssertMarkdown("[foo`](/uri)`", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..4, "[foo");
				CodeSpan(4..13, "](/uri)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-526"/>
	[TestMethod]
	public void Test526()
	{
		AssertMarkdown("[foo<https://example.com/?search=](uri)>", () =>
		{
			Paragraph(0..40, () =>
			{
				Literal(0..4, "[foo");
				Link(4..40, "https://example.com/?search=](uri)", null, () =>
				{
					Literal(5..39, "https://example.com/?search=](uri)");
				});
			});
		});
	}
	/// <summary>
	/// 链接引用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-527"/>
	[TestMethod]
	public void Test527()
	{
		AssertMarkdown("[foo][bar]\r\n\r\n[bar]: /url \"title\"", () =>
		{
			Paragraph(0..12, () =>
			{
				Link(0..10, "/url", "title", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(14..33, "bar", "/url", "title");
		});
	}
	/// <summary>
	/// 链接文本可以包含平衡的括号，或被转义的非平衡括号。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-528"/>
	[TestMethod]
	public void Test528()
	{
		AssertMarkdown("[link [foo [bar]]][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..25, () =>
			{
				Link(0..23, "/uri", null, () =>
				{
					Literal(1..17, "link [foo [bar]]");
				});
			});
			LinkDefinition(27..38, "ref", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-529"/>
	[TestMethod]
	public void Test529()
	{
		AssertMarkdown("[link \\[bar][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..19, () =>
			{
				Link(0..17, "/uri", null, () =>
				{
					Literal(1..11, "link [bar");
				});
			});
			LinkDefinition(21..32, "ref", "/uri", null);
		});
	}
	/// <summary>
	/// 链接文本可以包含行内内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-530"/>
	[TestMethod]
	public void Test530()
	{
		AssertMarkdown("[link *foo **bar** `#`*][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..31, () =>
			{
				Link(0..29, "/uri", null, () =>
				{
					Literal(1..6, "link ");
					Emphasis(6..23, () =>
					{
						Literal(7..11, "foo ");
						Strong(11..18, () =>
						{
							Literal(13..16, "bar");
						});
						Literal(18..19, " ");
						CodeSpan(19..22, "#");
					});
				});
			});
			LinkDefinition(33..44, "ref", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-531"/>
	[TestMethod]
	public void Test531()
	{
		AssertMarkdown("[![moon](moon.jpg)][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..26, () =>
			{
				Link(0..24, "/uri", null, () =>
				{
					Image(1..18, "moon.jpg", null, () =>
					{
						Literal(3..7, "moon");
					});
				});
			});
			LinkDefinition(28..39, "ref", "/uri", null);
		});
	}
	/// <summary>
	/// 链接不能包含链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-532"/>
	[TestMethod]
	public void Test532()
	{
		AssertMarkdown("[foo [bar](/uri)][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..24, () =>
			{
				Literal(0..5, "[foo ");
				Link(5..16, "/uri", null, () =>
				{
					Literal(6..9, "bar");
				});
				Literal(16..17, "]");
				Link(17..22, "/uri", null, () =>
				{
					Literal(18..21, "ref");
				});
			});

			LinkDefinition(26..37, "ref", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-533"/>
	[TestMethod]
	public void Test533()
	{
		AssertMarkdown("[foo *bar [baz][ref]*][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..29, () =>
			{
				Literal(0..5, "[foo ");
				Emphasis(5..21, () =>
				{
					Literal(6..10, "bar ");
					Link(10..20, "/uri", null, () =>
					{
						Literal(11..14, "baz");
					});
				});
				Literal(21..22, "]");
				Link(22..27, "/uri", null, () =>
				{
					Literal(23..26, "ref");
				});
			});
			LinkDefinition(31..42, "ref", "/uri", null);
		});
	}
	/// <summary>
	/// 链接文本优先级高于强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-534"/>
	[TestMethod]
	public void Test534()
	{
		AssertMarkdown("*[foo*][ref]\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..1, "*");
				Link(1..12, "/uri", null, () =>
				{
					Literal(2..6, "foo*");
				});
			});
			LinkDefinition(16..27, "ref", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-535"/>
	[TestMethod]
	public void Test535()
	{
		AssertMarkdown("[foo *bar][ref]*\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..18, () =>
			{
				Link(0..15, "/uri", null, () =>
				{
					Literal(1..9, "foo *bar");
				});
				Literal(15..16, "*");
			});
			LinkDefinition(20..31, "ref", "/uri", null);
		});
	}
	/// <summary>
	/// HTML 标签、代码段、自动链接的优先级高于链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-536"/>
	[TestMethod]
	public void Test536()
	{
		AssertMarkdown("[foo <bar attr=\"][ref]\">\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..26, () =>
			{
				Literal(0..5, "[foo ");
				HtmlStartTag(5..24, "<bar attr=\"][ref]\">");
			});
			LinkDefinition(28..39, "ref", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-537"/>
	[TestMethod]
	public void Test537()
	{
		AssertMarkdown("[foo`][ref]`\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..4, "[foo");
				CodeSpan(4..12, "][ref]");
			});
			LinkDefinition(16..27, "ref", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-538"/>
	[TestMethod]
	public void Test538()
	{
		AssertMarkdown("[foo<https://example.com/?search=][ref]>\r\n\r\n[ref]: /uri", () =>
		{
			Paragraph(0..42, () =>
			{
				Literal(0..4, "[foo");
				Link(4..40, "https://example.com/?search=][ref]", null, () =>
				{
					Literal(5..39, "https://example.com/?search=][ref]");
				});
			});
			LinkDefinition(44..55, "ref", "/uri", null);
		});
	}
	/// <summary>
	/// 匹配是大小写无关的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-539"/>
	[TestMethod]
	public void Test539()
	{
		AssertMarkdown("[foo][BaR]\r\n\r\n[bar]: /url \"title\"", () =>
		{
			Paragraph(0..12, () =>
			{
				Link(0..10, "/url", "title", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(14..33, "bar", "/url", "title");
		});
	}
	/// <summary>
	/// 会使用 Unicode case fold。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-540"/>
	[TestMethod]
	public void Test540()
	{
		AssertMarkdown("[ẞ]\r\n\r\n[SS]: /url", () =>
		{
			Paragraph(0..5, () =>
			{
				Link(0..3, "/url", null, () =>
				{
					Literal(1..2, "ẞ");
				});
			});
			LinkDefinition(7..17, "SS", "/url", null);
		});
	}
	/// <summary>
	/// 连续的空白、Tab 和换行会当作一个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-541"/>
	[TestMethod]
	public void Test541()
	{
		AssertMarkdown("[Foo\r\n  bar]: /url\r\n\r\n[Baz][Foo bar]", () =>
		{
			LinkDefinition(0..20, "Foo\r\n  bar", "/url", null);
			Paragraph(22..36, () =>
			{
				Link(22..36, "/url", null, () =>
				{
					Literal(23..26, "Baz");
				});
			});
		});
	}
	/// <summary>
	/// 链接文本和标签间不能存在空格、Tab 或换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-542"/>
	[TestMethod]
	public void Test542()
	{
		AssertMarkdown("[foo] [bar]\r\n\r\n[bar]: /url \"title\"", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..6, "[foo] ");
				Link(6..11, "/url", "title", () =>
				{
					Literal(7..10, "bar");
				});
			});
			LinkDefinition(15..34, "bar", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-543"/>
	[TestMethod]
	public void Test543()
	{
		AssertMarkdown("[foo]\r\n[bar]\r\n\r\n[bar]: /url \"title\"", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..5, "[foo]");
				SoftBreak(5..7);
				Link(7..12, "/url", "title", () =>
				{
					Literal(8..11, "bar");
				});
			});
			LinkDefinition(16..35, "bar", "/url", "title");
		});
	}
	/// <summary>
	/// 多个链接定义时，会使用第一个。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-544"/>
	[TestMethod]
	public void Test544()
	{
		AssertMarkdown("[foo]: /url1\r\n\r\n[foo]: /url2\r\n\r\n[bar][foo]", () =>
		{
			LinkDefinition(0..14, "foo", "/url1", null);
			LinkDefinition(16..30, "foo", "/url2", null);
			Paragraph(32..42, () =>
			{
				Link(32..42, "/url1", null, () =>
				{
					Literal(33..36, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 链接标签不会解析行内元素。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-545"/>
	[TestMethod]
	public void Test545()
	{
		AssertMarkdown("[bar][foo\\!]\r\n\r\n[foo!]: /url", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..12, "[bar][foo!]");
			});
			LinkDefinition(16..28, "foo!", "/url", null);
		});
	}
	/// <summary>
	/// 链接标签不能包含方括号，除非被转义。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-546"/>
	[TestMethod]
	public void Test546()
	{
		AssertMarkdown("[foo][ref[]\r\n\r\n[ref[]: /uri", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..11, "[foo][ref[]");
			});
			Paragraph(15..27, () =>
			{
				Literal(15..27, "[ref[]: /uri");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-547"/>
	[TestMethod]
	public void Test547()
	{
		AssertMarkdown("[foo][ref[bar]]\r\n\r\n[ref[bar]]: /uri", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..15, "[foo][ref[bar]]");
			});
			Paragraph(19..35, () =>
			{
				Literal(19..35, "[ref[bar]]: /uri");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-548"/>
	[TestMethod]
	public void Test548()
	{
		AssertMarkdown("[[[foo]]]\r\n\r\n[[[foo]]]: /url", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..9, "[[[foo]]]");
			});
			Paragraph(13..28, () =>
			{
				Literal(13..28, "[[[foo]]]: /url");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-549"/>
	[TestMethod]
	public void Test549()
	{
		AssertMarkdown("[foo][ref\\[]\r\n\r\n[ref\\[]: /uri", () =>
		{
			Paragraph(0..14, () =>
			{
				Link(0..12, "/uri", null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(16..29, "ref\\[", "/uri", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-550"/>
	[TestMethod]
	public void Test550()
	{
		AssertMarkdown("[bar\\\\]: /uri\r\n\r\n[bar\\\\]", () =>
		{
			LinkDefinition(0..15, "bar\\\\", "/uri", null);
			Paragraph(17..24, () =>
			{
				Link(17..24, "/uri", null, () =>
				{
					Literal(18..23, "bar\\");
				});
			});
		});
	}
	/// <summary>
	/// 链接标签需要包含至少一个非空白字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-551"/>
	[TestMethod]
	public void Test551()
	{
		AssertMarkdown("[]\r\n\r\n[]: /uri", () =>
		{
			Paragraph(0..4, () =>
			{
				Literal(0..2, "[]");
			});
			Paragraph(6..14, () =>
			{
				Literal(6..14, "[]: /uri");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-552"/>
	[TestMethod]
	public void Test552()
	{
		AssertMarkdown("[\r\n ]\r\n\r\n[\r\n ]: /uri", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..1, "[");
				SoftBreak(1..4);
				Literal(4..5, "]");
			});
			Paragraph(9..20, () =>
			{
				Literal(9..10, "[");
				SoftBreak(10..13);
				Literal(13..20, "]: /uri");
			});
		});
	}
	/// <summary>
	/// 链接标签可以为空。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-553"/>
	[TestMethod]
	public void Test553()
	{
		AssertMarkdown("[foo][]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..9, () =>
			{
				Link(0..7, "/url", "title", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(11..30, "foo", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-554"/>
	[TestMethod]
	public void Test554()
	{
		AssertMarkdown("[*foo* bar][]\r\n\r\n[*foo* bar]: /url \"title\"", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..13, "/url", "title", () =>
				{
					Emphasis(1..6, () =>
					{
						Literal(2..5, "foo");
					});
					Literal(6..10, " bar");
				});
			});
			LinkDefinition(17..42, "*foo* bar", "/url", "title");
		});
	}
	/// <summary>
	/// 链接标签是忽略大小写的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-555"/>
	[TestMethod]
	public void Test555()
	{
		AssertMarkdown("[Foo][]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..9, () =>
			{
				Link(0..7, "/url", "title", () =>
				{
					Literal(1..4, "Foo");
				});
			});
			LinkDefinition(11..30, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 两个方括号之间也是不允许存在空格、Tab 或换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-556"/>
	[TestMethod]
	public void Test556()
	{
		AssertMarkdown("[foo] \r\n[]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..12, () =>
			{
				Link(0..5, "/url", "title", () =>
				{
					Literal(1..4, "foo");
				});
				SoftBreak(5..8);
				Literal(8..10, "[]");
			});
			LinkDefinition(14..33, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 简写的链接引用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-557"/>
	[TestMethod]
	public void Test557()
	{
		AssertMarkdown("[foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "/url", "title", () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(9..28, "foo", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-558"/>
	[TestMethod]
	public void Test558()
	{
		AssertMarkdown("[*foo* bar]\r\n\r\n[*foo* bar]: /url \"title\"", () =>
		{
			Paragraph(0..13, () =>
			{
				Link(0..11, "/url", "title", () =>
				{
					Emphasis(1..6, () =>
					{
						Literal(2..5, "foo");
					});
					Literal(6..10, " bar");
				});
			});
			LinkDefinition(15..40, "*foo* bar", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-559"/>
	[TestMethod]
	public void Test559()
	{
		AssertMarkdown("[[*foo* bar]]\r\n\r\n[*foo* bar]: /url \"title\"", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..1, "[");
				Link(1..12, "/url", "title", () =>
				{
					Emphasis(2..7, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(7..11, " bar");
				});
				Literal(12..13, "]");
			});
			LinkDefinition(17..42, "*foo* bar", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-560"/>
	[TestMethod]
	public void Test560()
	{
		AssertMarkdown("[[bar [foo]\r\n\r\n[foo]: /url", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..6, "[[bar ");
				Link(6..11, "/url", null, () =>
				{
					Literal(7..10, "foo");
				});
			});
			LinkDefinition(15..26, "foo", "/url", null);
		});
	}
	/// <summary>
	/// 链接标签忽略大小写。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-561"/>
	[TestMethod]
	public void Test561()
	{
		AssertMarkdown("[Foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..7, () =>
			{
				Link(0..5, "/url", "title", () =>
				{
					Literal(1..4, "Foo");
				});
			});
			LinkDefinition(9..28, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 链接文本后的空格应当被保留。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-562"/>
	[TestMethod]
	public void Test562()
	{
		AssertMarkdown("[foo] bar\r\n\r\n[foo]: /url", () =>
		{
			Paragraph(0..11, () =>
			{
				Link(0..5, "/url", null, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..9, " bar");
			});
			LinkDefinition(13..24, "foo", "/url", null);
		});
	}
	/// <summary>
	/// 可以通过转义避免识别为链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-563"/>
	[TestMethod]
	public void Test563()
	{
		AssertMarkdown("\\[foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "[foo]");
			});
			LinkDefinition(10..29, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 链接的优先级比强调高。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-564"/>
	[TestMethod]
	public void Test564()
	{
		AssertMarkdown("[foo*]: /url\r\n\r\n*[foo*]", () =>
		{
			LinkDefinition(0..14, "foo*", "/url", null);
			Paragraph(16..23, () =>
			{
				Literal(16..17, "*");
				Link(17..23, "/url", null, () =>
				{
					Literal(18..22, "foo*");
				});
			});
		});
	}
	/// <summary>
	/// 完整链接的优先级更高。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-565"/>
	[TestMethod]
	public void Test565()
	{
		AssertMarkdown("[foo][bar]\r\n\r\n[foo]: /url1\r\n[bar]: /url2", () =>
		{
			Paragraph(0..12, () =>
			{
				Link(0..10, "/url2", null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(14..28, "foo", "/url1", null);
			LinkDefinition(28..40, "bar", "/url2", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-566"/>
	[TestMethod]
	public void Test566()
	{
		AssertMarkdown("[foo][]\r\n\r\n[foo]: /url1", () =>
		{
			Paragraph(0..9, () =>
			{
				Link(0..7, "/url1", null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(11..23, "foo", "/url1", null);
		});
	}
	/// <summary>
	/// 行内链接的优先级更高。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-567"/>
	[TestMethod]
	public void Test567()
	{
		AssertMarkdown("[foo]()\r\n\r\n[foo]: /url1", () =>
		{
			Paragraph(0..9, () =>
			{
				Link(0..7, string.Empty, null, () =>
				{
					Literal(1..4, "foo");
				});
			});
			LinkDefinition(11..23, "foo", "/url1", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-568"/>
	[TestMethod]
	public void Test568()
	{
		AssertMarkdown("[foo](not a link)\r\n\r\n[foo]: /url1", () =>
		{
			Paragraph(0..19, () =>
			{
				Link(0..5, "/url1", null, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..17, "(not a link)");
			});
			LinkDefinition(21..33, "foo", "/url1", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-569"/>
	[TestMethod]
	public void Test569()
	{
		AssertMarkdown("[foo][bar][baz]\r\n\r\n[baz]: /url", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..5, "[foo]");
				Link(5..15, "/url", null, () =>
				{
					Literal(6..9, "bar");
				});
			});
			LinkDefinition(19..30, "baz", "/url", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-570"/>
	[TestMethod]
	public void Test570()
	{
		AssertMarkdown("[foo][bar][baz]\r\n\r\n[baz]: /url1\r\n[bar]: /url2", () =>
		{
			Paragraph(0..17, () =>
			{
				Link(0..10, "/url2", null, () =>
				{
					Literal(1..4, "foo");
				});
				Link(10..15, "/url1", null, () =>
				{
					Literal(11..14, "baz");
				});
			});
			LinkDefinition(19..33, "baz", "/url1", null);
			LinkDefinition(33..45, "bar", "/url2", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-571"/>
	[TestMethod]
	public void Test571()
	{
		AssertMarkdown("[foo][bar][baz]\r\n\r\n[baz]: /url1\r\n[foo]: /url2", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..5, "[foo]");
				Link(5..15, "/url1", null, () =>
				{
					Literal(6..9, "bar");
				});
			});
			LinkDefinition(19..33, "baz", "/url1", null);
			LinkDefinition(33..45, "foo", "/url2", null);
		});
	}
}
