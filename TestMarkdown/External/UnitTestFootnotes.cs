using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 脚注的单元测试。
/// </summary>
[TestClass]
public class UnitTestFootnotes : BaseTest
{
	/// <summary>
	/// 脚注总是按照出现顺序依次编号，脚注的引用名称仅用于生成锚点。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("Here is a simple footnote[^1].\r\n\r\nA footnote can also have multiple lines[^2].\r\n\r\nYou can also use words, to fit your writing style more closely[^note].\r\n\r\n[^1]:        My reference.\r\n[^2]: New line can prefixed with any spaces.\r\n  This allows you to have a footnote with multiple lines.\r\n[^note]:\r\n    Named footnotes will still render with numbers instead of the text but allow easier identification and linking.\r\n    This footnote also has been made with a different syntax using 4 spaces for new lines.\r\n", () =>
		{
			Paragraph(0..32, () =>
			{
				Literal(0..25, "Here is a simple footnote");
				FootnoteRef(25..29, "1");
				Literal(29..30, ".");
			});
			Paragraph(34..80, () =>
			{
				Literal(34..73, "A footnote can also have multiple lines");
				FootnoteRef(73..77, "2");
				Literal(77..78, ".");
			});
			Paragraph(82..154, () =>
			{
				Literal(82..144, "You can also use words, to fit your writing style more closely");
				FootnoteRef(144..151, "note");
				Literal(151..152, ".");
			});
			Footnote(156..184, "1", () =>
			{
				Paragraph(169..184, () =>
				{
					Literal(169..182, "My reference.");
				});
			});
			Footnote(184..289, "2", () =>
			{
				Paragraph(190..289, () =>
				{
					Literal(190..228, "New line can prefixed with any spaces.");
					SoftBreak(228..232);
					Literal(232..287, "This allows you to have a footnote with multiple lines.");
				});
			});
			Footnote(289..508, "note", () =>
			{
				Paragraph(303..508, () =>
				{
					Literal(303..414, "Named footnotes will still render with numbers instead of the text but allow easier identification and linking.");
					SoftBreak(414..420);
					Literal(420..506, "This footnote also has been made with a different syntax using 4 spaces for new lines.");
				});
			});
		});
	}

	/// <summary>
	/// 脚注内容可以以任何顺序出现在任何位置，但总是会按照引用顺序输出到文档的结尾。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("[^first]: note 1\r\n> [^second]: note 2\r\n>\r\n> foo[^second][^first].\r\n", () =>
		{
			Footnote(0..18, "first", () =>
			{
				Paragraph(10..18, () =>
				{
					Literal(10..16, "note 1");
				});
			});
			Blockquote(18..67, () =>
			{
				Footnote(20..39, "second", () =>
				{
					Paragraph(31..39, () =>
					{
						Literal(31..37, "note 2");
					});
				});
				Paragraph(44..67, () =>
				{
					Literal(44..47, "foo");
					FootnoteRef(47..56, "second");
					FootnoteRef(56..64, "first");
					Literal(64..65, ".");
				});
			});
		});
	}

	/// <summary>
	/// 会在规范化之后比较脚注的引用标签，其规范化行为与链接相同。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("normalize: [^ẞ]\r\n\r\n[^SS]: first\r\n[^ss]: second\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..11, "normalize: ");
				FootnoteRef(11..15, "SS");
			});
			Footnote(19..33, "SS", () =>
			{
				Paragraph(26..33, () =>
				{
					Literal(26..31, "first");
				});
			});
			Footnote(33..48, "ss", () =>
			{
				Paragraph(40..48, () =>
				{
					Literal(40..46, "second");
				});
			});
		});
	}

	/// <summary>
	/// 可以多次引用同一个脚注，会为每个引用生成唯一的 id。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("foo[^1] and bar[^1].\r\n\r\n[^1]: note\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..3, "foo");
				FootnoteRef(3..7, "1");
				Literal(7..15, " and bar");
				FootnoteRef(15..19, "1");
				Literal(19..20, ".");
			});
			Footnote(24..36, "1", () =>
			{
				Paragraph(30..36, () =>
				{
					Literal(30..34, "note");
				});
			});
		});
	}

	/// <summary>
	/// 脚注内容中可以包含空行，只要之后的行是被缩进的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("note[^1][^2].\r\n\r\n[^1]: line1\r\nline1-2\r\n\r\n    line2\r\n  line2-2\r\n\r\n    line3\r\n\r\n        code\r\n    line4\r\n\r\n  line5\r\n  [^2]:\r\n        code\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..4, "note");
				FootnoteRef(4..8, "1");
				FootnoteRef(8..12, "2");
				Literal(12..13, ".");
			});
			Footnote(17..103, "1", () =>
			{
				Paragraph(23..39, () =>
				{
					Literal(23..28, "line1");
					SoftBreak(28..30);
					Literal(30..37, "line1-2");
				});
				Paragraph(45..63, () =>
				{
					Literal(45..50, "line2");
					SoftBreak(50..54);
					Literal(54..61, "line2-2");
				});
				Paragraph(69..76, () =>
				{
					Literal(69..74, "line3");
				});
				CodeBlock(82..92, "code\r\n");
				Paragraph(96..103, () =>
				{
					Literal(96..101, "line4");
				});
			});
			Paragraph(107..114, () =>
			{
				Literal(107..112, "line5");
			});
			Footnote(116..137, "2", () =>
			{
				CodeBlock(127..137, "code\r\n");
			});
		});
	}

	/// <summary>
	/// 同时满足脚注和链接声明格式时，会识别为脚注。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("foo [^1][link] [link][^1] [[^1]][link].\r\n\r\n[^1]: note\r\n\r\n[link]:/url\r\n", () =>
		{
			Paragraph(0..41, () =>
			{
				Literal(0..4, "foo ");
				Link(4..14, "/url", null, () =>
				{
					Literal(5..7, "^1");
				});
				Literal(14..21, " [link]");
				FootnoteRef(21..25, "1");
				Literal(25..26, " ");
				Link(26..38, "/url", null, () =>
				{
					FootnoteRef(27..31, "1");
				});
				Literal(38..39, ".");
			});
			Footnote(43..55, "1", () =>
			{
				Paragraph(49..55, () =>
				{
					Literal(49..53, "note");
				});
			});
			LinkDefinition(57..70, "link", "/url");
		});
	}

	/// <summary>
	/// 脚注中还可以嵌套包含其它脚注或脚注引用。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("foo [link] [^1][^2].\r\n\r\n[^1]: [link]: /url\r\nnote1[^1][^2]\r\n    [^2]: note2\r\n\r\n    note1-end\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..4, "foo ");
				Link(4..10, "/url", null, () =>
				{
					Literal(5..9, "link");
				});
				Literal(10..11, " ");
				FootnoteRef(11..15, "1");
				FootnoteRef(15..19, "2");
				Literal(19..20, ".");
			});
			Footnote(24..93, "1", () =>
			{
				LinkDefinition(30..44, "link", "/url");
				Paragraph(44..59, () =>
				{
					Literal(44..49, "note1");
					FootnoteRef(49..53, "1");
					FootnoteRef(53..57, "2");
				});
				Footnote(63..76, "2", () =>
				{
					Paragraph(69..76, () =>
					{
						Literal(69..74, "note2");
					});
				});
				Paragraph(82..93, () =>
				{
					Literal(82..91, "note1-end");
				});
			});
		});
	}

	/// <summary>
	/// 脚注的内容可以是空的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-8"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("foo [^1]\r\n\r\n[^1]:\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..4, "foo ");
				FootnoteRef(4..8, "1");
			});
			Footnote(12..19, "1");
		});
	}

	/// <summary>
	/// 只有脚注最后一个节点是段落时，才会直接将反向引用添加到该段落内；否则总是会增加一个新段落。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-9"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("foo [^1][^2]\r\n\r\n[^1]: paragraph\r\n[^2]: > block\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..4, "foo ");
				FootnoteRef(4..8, "1");
				FootnoteRef(8..12, "2");
			});
			Footnote(16..33, "1", () =>
			{
				Paragraph(22..33, () =>
				{
					Literal(22..31, "paragraph");
				});
			});
			Footnote(33..48, "2", () =>
			{
				Blockquote(39..48, () =>
				{
					Paragraph(41..48, () =>
					{
						Literal(41..46, "block");
					});
				});
			});
		});
	}

	/// <summary>
	/// 脚注的标签可以出现中文、符号等。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-10"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("foo[^中文&<tag>]\r\n\r\n[^中文&<tag>]: note\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..3, "foo");
				FootnoteRef(3..14, "中文&<tag>");
			});
			Footnote(18..37, "中文&<tag>", () =>
			{
				Paragraph(31..37, () =>
				{
					Literal(31..35, "note");
				});
			});
		});
	}

	/// <summary>
	/// 脚注自引用或互相引用时，也会生成脚注。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md#example-11"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown("[^1]:note1 [^1]\r\n[^2]:note2 [^3]\r\n[^3]:note3 [^2]\r\n", () =>
		{
			Footnote(0..17, "1", () =>
			{
				Paragraph(5..17, () =>
				{
					Literal(5..11, "note1 ");
					FootnoteRef(11..15, "1");
				});
			});
			Footnote(17..34, "2", () =>
			{
				Paragraph(22..34, () =>
				{
					Literal(22..28, "note2 ");
					FootnoteRef(28..32, "3");
				});
			});
			Footnote(34..51, "3", () =>
			{
				Paragraph(39..51, () =>
				{
					Literal(39..45, "note3 ");
					FootnoteRef(45..49, "2");
				});
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持脚注。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("foo [^1]\r\n\r\n[^1]:\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..8, "foo [^1]");
			});
			Paragraph(12..19, () =>
			{
				Literal(12..17, "[^1]:");
			});
		});
	}
}

