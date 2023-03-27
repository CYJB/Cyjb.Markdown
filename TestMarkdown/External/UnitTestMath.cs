using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 数学公式的单元测试。
/// </summary>
/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md"/>
[TestClass]
public partial class UnitTestMath : BaseTest
{
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("$foo$\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				MathSpan(0..5, "foo");
			});
		});
	}
	/// <summary>
	/// 由于内容包含 <c>$</c>，这里使用两个 <c>$</c> 作为分隔符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("$$ foo $ bar $$\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				MathSpan(0..15, "foo $ bar");
			});
		});
	}
	/// <summary>
	/// 数学公式以空格开始或结束时，必须使用两个 <c>$</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("$ $$ $\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "$ $$ $");
			});
		});
	}
	/// <summary>
	/// 使用一个 <c>$</c> 作为分隔符时，结束 <c>$</c> 后也不能是数字。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("$20,000 and $30,000\r\n", () =>
		{
			Paragraph(0..21, () =>
			{
				Literal(0..19, "$20,000 and $30,000");
			});
		});
	}
	/// <summary>
	/// 移除前后空格。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("$$$ $$ $$$\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				MathSpan(0..10, "$$");
			});
		});
	}
	/// <summary>
	/// 注意只会移除一个空格。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("$$$  $$  $$$\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				MathSpan(0..12, " $$ ");
			});
		});
	}
	/// <summary>
	/// 只有内容两端都有空格时，才会移除空格。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("$$$ a$$$\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				MathSpan(0..8, " a");
			});
		});
	}
	/// <summary>
	/// 只会移除空格，而非Unicode 空白。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-8"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("$　a　$\r\n", () =>
		{
			Paragraph(0..7, () =>
			{
				MathSpan(0..5, "　a　");
			});
		});
	}
	/// <summary>
	/// 在内容只有空格组成时，不会移除两端的空格。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-9"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("$$ $$\r\n$$  $$\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				MathSpan(0..5, " ");
				SoftBreak(5..7);
				MathSpan(7..13, "  ");
			});
		});
	}
	/// <summary>
	/// 起始和结尾的换行也会被移除，但内部的空格或换行都会被原样保留。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-10"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("a $$\r\nfoo  bar\r\nbaz  \r\nbim\r\n$$ $\r\n", () =>
		{
			Paragraph(0..34, () =>
			{
				Literal(0..2, "a ");
				MathSpan(2..30, "foo  bar\r\nbaz  \r\nbim");
				Literal(30..32, " $");
			});
		});
		AssertMarkdown("a $$\nfoo  bar\nbaz  \nbim\n$$ $\n", () =>
		{
			Paragraph(0..29, () =>
			{
				Literal(0..2, "a ");
				MathSpan(2..26, "foo  bar\nbaz  \nbim");
				Literal(26..28, " $");
			});
		});
		AssertMarkdown("a $$\rfoo  bar\rbaz  \rbim\r$$ $\r", () =>
		{
			Paragraph(0..29, () =>
			{
				Literal(0..2, "a ");
				MathSpan(2..26, "foo  bar\rbaz  \rbim");
				Literal(26..28, " $");
			});
		});
	}
	/// <summary>
	/// 在数学公式内，反斜杠转义不会生效。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-11"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown("$foo\\$bar$\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				MathSpan(0..6, @"foo\");
				Literal(6..10, "bar$");
			});
		});
	}
	/// <summary>
	/// 也可以通过增加分隔 <c>$</c> 的个数来避免转义内部 <c>$</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-12"/>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown("$$foo$bar$$\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				MathSpan(0..11, @"foo$bar");
			});
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-13"/>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown("$$$ foo $$ bar $$$\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				MathSpan(0..18, @"foo $$ bar");
			});
		});
	}
	/// <summary>
	/// 数学公式与代码段具有相同的优先级，高于 HTML 标签和自动链接之外的其它行级元素。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-14"/>
	[TestMethod]
	public void Test14()
	{
		AssertMarkdown("*foo$*$\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..4, "*foo");
				MathSpan(4..7, "*");
			});
		});
	}
	/// <summary>
	/// 这里也不会解析为链接。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-15"/>
	[TestMethod]
	public void Test15()
	{
		AssertMarkdown("[not a $link](/foo$)\r\n", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..7, "[not a ");
				MathSpan(7..19, "link](/foo");
				Literal(19..20, ")");
			});
		});
	}
	/// <summary>
	/// 与代码段具有相同优先级。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-16"/>
	[TestMethod]
	public void Test16()
	{
		AssertMarkdown("`code$foo$`\r\n$math`code`$\r\n", () =>
		{
			Paragraph(0..27, () =>
			{
				CodeSpan(0..11, "code$foo$");
				SoftBreak(11..13);
				MathSpan(13..25, "math`code`");
			});
		});
	}
	/// <summary>
	/// 与 HTML 标签具有相同优先级。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-17"/>
	[TestMethod]
	public void Test17()
	{
		AssertMarkdown("<a href=\"$\">$\r\n$<a href=\"$\">$\r\n", () =>
		{
			Paragraph(0..31, () =>
			{
				HtmlStartTag(0..12, "<a href=\"$\">");
				Literal(12..13, "$");
				SoftBreak(13..15);
				MathSpan(15..26, "<a href=\"");
				Literal(26..29, "\">$");
			});
		});
	}
	/// <summary>
	/// 与自动链接具有相同优先级。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-18"/>
	[TestMethod]
	public void Test18()
	{
		AssertMarkdown("<http://foo.bar.$baz>$\r\n$<http://foo.bar.$baz>$\r\n", () =>
		{
			Paragraph(0..49, () =>
			{
				Link(0..21, "http://foo.bar.$baz", null, () =>
				{
					Literal(1..20, "http://foo.bar.$baz");
				});
				Literal(21..22, "$");
				SoftBreak(22..24);
				MathSpan(24..42, "<http://foo.bar.");
				Literal(42..47, "baz>$");
			});
		});
	}
	/// <summary>
	/// 如果起始 <c>$</c> 并未被相同长度的 <c>$</c> 字符串闭合，直接作为普通字符串使用。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-19"/>
	[TestMethod]
	public void Test19()
	{
		AssertMarkdown("$$$foo$$\r\n", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..8, "$$$foo$$");
			});
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-20"/>
	[TestMethod]
	public void Test20()
	{
		AssertMarkdown("$foo\r\n", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..4, "$foo");
			});
		});
	}
	/// <summary>
	/// 起始和结束 <c>$</c> 必须有相同长度。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-21"/>
	[TestMethod]
	public void Test21()
	{
		AssertMarkdown("$foo$$bar$$\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..4, "$foo");
				MathSpan(4..11, "bar");
			});
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-22"/>
	[TestMethod]
	public void Test22_1()
	{
		AssertMarkdown("$$\r\n<\r\n >\r\n$$\r\n", () =>
		{
			MathBlock(0..15, "<\r\n >\r\n");
		});
	}
	[TestMethod]
	public void Test22_2()
	{
		AssertMarkdown("$$\n<\n >\n$$\n", () =>
		{
			MathBlock(0..11, "<\n >\n");
		});
	}
	[TestMethod]
	public void Test22_3()
	{
		AssertMarkdown("$$\r\n<\r\n >\r\n$$", () =>
		{
			MathBlock(0..13, "<\r\n >\r\n");
		});
	}
	[TestMethod]
	public void Test22_4()
	{
		AssertMarkdown("$$\n<\n >\n$$", () =>
		{
			MathBlock(0..10, "<\n >\n");
		});
	}
	/// <summary>
	/// <c>$</c> 的个数必须大于等于两个。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-23"/>
	[TestMethod]
	public void Test23()
	{
		AssertMarkdown("$\r\nfoo\r\n$\r\n", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..1, "$");
				SoftBreak(1..3);
				Literal(3..6, "foo");
				SoftBreak(6..8);
				Literal(8..9, "$");
			});
		});
	}
	/// <summary>
	/// 结束分隔符至少要与起始分隔符的长度相同。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-24"/>
	[TestMethod]
	public void Test24()
	{
		AssertMarkdown("$$$$\r\nfoo\r\n$$$\r\n$$$$$$\r\n", () =>
		{
			MathBlock(0..24, "foo\r\n$$$\r\n");
		});
	}
	/// <summary>
	/// 未闭合的数学公式块会在容器块（或文档）结束的位置闭合。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-25"/>
	[TestMethod]
	public void Test25_1()
	{
		AssertMarkdown("$$\r\n", () =>
		{
			MathBlock(0..4, "");
		});
	}
	[TestMethod]
	public void Test25_2()
	{
		AssertMarkdown("$$", () =>
		{
			MathBlock(0..2, "");
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-26"/>
	[TestMethod]
	public void Test26()
	{
		AssertMarkdown("$$$$$\r\n\r\n$$\r\naaa\r\n", () =>
		{
			MathBlock(0..18, "\r\n$$\r\naaa\r\n");
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-27"/>
	[TestMethod]
	public void Test27()
	{
		AssertMarkdown("> $$\r\n> aaa\r\n\r\nbbb\r\n", () =>
		{
			Blockquote(0..13, () =>
			{
				MathBlock(2..13, "aaa\r\n");
			});
			Paragraph(15..20, () =>
			{
				Literal(15..18, "bbb");
			});
		});
	}
	/// <summary>
	/// 数学公式可以只包含空行作为内容。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-28"/>
	[TestMethod]
	public void Test28()
	{
		AssertMarkdown("$$\r\n\r\n  \r\n$$\r\n", () =>
		{
			MathBlock(0..14, "\r\n  \r\n");
		});
	}
	/// <summary>
	/// 数学公式可以是空的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-29"/>
	[TestMethod]
	public void Test29()
	{
		AssertMarkdown("$$\r\n$$\r\n", () =>
		{
			MathBlock(0..8, "");
		});
	}
	/// <summary>
	/// 数学公式可以被缩进。如果起始分隔符被缩进了，那么每个内容行都会被移除等量的缩进。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-30"/>
	[TestMethod]
	public void Test30()
	{
		AssertMarkdown(" $$\r\n aaa\r\naaa\r\n$$\r\n", () =>
		{
			MathBlock(0..20, "aaa\r\naaa\r\n");
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-31"/>
	[TestMethod]
	public void Test31()
	{
		AssertMarkdown("  $$\r\naaa\r\n  aaa\r\naaa\r\n  $$\r\n", () =>
		{
			MathBlock(0..29, "aaa\r\naaa\r\naaa\r\n");
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-32"/>
	[TestMethod]
	public void Test32()
	{
		AssertMarkdown("   $$\r\n   aaa\r\n    aaa\r\n  aaa\r\n   $$\r\n", () =>
		{
			MathBlock(0..38, "aaa\r\n aaa\r\naaa\r\n");
		});
	}
	/// <summary>
	/// 四个空格的缩进就太多了。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-33"/>
	[TestMethod]
	public void Test33()
	{
		AssertMarkdown("    $$\r\n    aaa\r\n    $$\r\n", () =>
		{
			CodeBlock(0..25, "$$\r\naaa\r\n$$\r\n");
		});
	}
	/// <summary>
	/// 结束分隔符最多可以包含三个空格的缩进，缩进并不需要与起始分隔符一致。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-34"/>
	[TestMethod]
	public void Test34()
	{
		AssertMarkdown("$$\r\naaa\r\n  $$\r\n", () =>
		{
			MathBlock(0..15, "aaa\r\n");
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-35"/>
	[TestMethod]
	public void Test35()
	{
		AssertMarkdown("   $$\r\naaa\r\n  $$\r\n", () =>
		{
			MathBlock(0..18, "aaa\r\n");
		});
	}
	/// <summary>
	/// 包含四个缩进就不是结束分隔符了。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-36"/>
	[TestMethod]
	public void Test36()
	{
		AssertMarkdown("$$\r\naaa\r\n    $$\r\n", () =>
		{
			MathBlock(0..17, "aaa\r\n    $$\r\n");
		});
	}
	/// <summary>
	/// 分隔符不能包含内部空格或 Tab。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-37"/>
	[TestMethod]
	public void Test37()
	{
		AssertMarkdown("$$ $$\r\naaa\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				MathSpan(0..5, " ");
				SoftBreak(5..7);
				Literal(7..10, "aaa");
			});
		});
	}
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-38"/>
	[TestMethod]
	public void Test38()
	{
		AssertMarkdown("$$$$$$\r\naaa\r\n$$$ $$\r\n", () =>
		{
			MathBlock(0..21, "aaa\r\n$$$ $$\r\n");
		});
	}
	/// <summary>
	/// 数学公式块可以中断段落，并且可以直接后跟段落而不需要空行分割。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-39"/>
	[TestMethod]
	public void Test39()
	{
		AssertMarkdown("foo\r\n$$\r\nbar\r\n$$\r\nbaz\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "foo");
			});
			MathBlock(5..18, "bar\r\n");
			Paragraph(18..23, () =>
			{
				Literal(18..21, "baz");
			});
		});
	}
	/// <summary>
	/// 其它块也可以直接出现在数学公式块后面。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-40"/>
	[TestMethod]
	public void Test40()
	{
		AssertMarkdown("foo\r\n---\r\n$$\r\nbar\r\n$$\r\n# baz\r\n", () =>
		{
			Heading(0..10, 2, new HtmlAttributeList() { { "id", "foo" } }, () =>
			{
				Literal(0..3, "foo");
			});
			MathBlock(10..23, "bar\r\n");
			Heading(23..30, 1, new HtmlAttributeList() { { "id", "baz" } }, () =>
			{
				Literal(25..28, "baz");
			});
		});
	}
	/// <summary>
	/// 在起始分隔符后可以提供信息字符串。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-41"/>
	[TestMethod]
	public void Test41()
	{
		AssertMarkdown("$$ info startline=3 `%@#~\r\n1 + 1\r\n$$\r\n", () =>
		{
			MathBlock(0..38, "1 + 1\r\n", "info startline=3 `%@#~");
		});
	}
	/// <summary>
	/// 信息字符串不能包含 <c>$</c> 字符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-42"/>
	[TestMethod]
	public void Test42()
	{
		AssertMarkdown("$$ info $$\r\nfoo\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				MathSpan(0..10, "info");
				SoftBreak(10..12);
				Literal(12..15, "foo");
			});
		});
	}
	/// <summary>
	/// 结束分隔符不能包含信息字符串。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-43"/>
	[TestMethod]
	public void Test43()
	{
		AssertMarkdown("$$\r\n$$ aaa\r\n$$\r\n", () =>
		{
			MathBlock(0..16, "$$ aaa\r\n");
		});
	}
	/// <summary>
	/// 数学公式块同样支持自定义属性。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md#example-44"/>
	[TestMethod]
	public void Test44()
	{
		AssertMarkdown("$$ {\r\n  #my-id .class2\r\n  key=value\r\n}\r\nfoo\r\n$$\r\n", () =>
		{
			MathBlock(0..49, "foo\r\n", null, new HtmlAttributeList()
			{
				{ "id", "my-id" },
				{ "class", "class2" },
				{ "key", "value" },
			});
		});
	}
	/// <summary>
	/// 空的 <c>$$</c> 不会被识别为代码段。
	/// </summary>
	[TestMethod]
	public void TestEmptyMathSpan()
	{
		AssertMarkdown("foo $$\r\n", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..6, "foo $$");
			});
		});
	}
	/// <summary>
	/// 测试未结束的数学公式段。
	/// </summary>
	[TestMethod]
	public void TestMathSpanNoEnd()
	{
		AssertMarkdown("$$$$$},$$aaaa\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..13, "$$$$$},$$aaaa");
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持数学公式。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("$foo$\r\n$$\r\nbar\r\n$$\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..5, "$foo$");
				SoftBreak(5..7);
				Literal(7..9, "$$");
				SoftBreak(9..11);
				Literal(11..14, "bar");
				SoftBreak(14..16);
				Literal(16..18, "$$");
			});
		});
	}
}

