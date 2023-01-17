using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// HTML 块的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#html-blocks"/>
[TestClass]
public class UnitTestHtmlBlock : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-148"/>
	[TestMethod]
	public void Test148()
	{
		AssertMarkdown("<table><tr><td>\r\n<pre>\r\n**Hello**,\r\n\r\n_world_.\r\n</pre>\r\n</td></tr></table>\r\n", () =>
		{
			HtmlBlock(0..36, "<table><tr><td>\r\n<pre>\r\n**Hello**,\r\n");
			Paragraph(38..56, () =>
			{
				Emphasis(38..45, () =>
				{
					Literal(39..44, "world");
				});
				Literal(45..46, ".");
				SoftBreak(46..48);
				HtmlEndTag(48..54, "</pre>");
			});
			HtmlBlock(56..76, "</td></tr></table>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-149"/>
	[TestMethod]
	public void Test149()
	{
		AssertMarkdown("<table>\r\n  <tr>\r\n    <td>\r\n           hi\r\n    </td>\r\n  </tr>\r\n</table>\r\n\r\nokay.\r\n", () =>
		{
			HtmlBlock(0..72, "<table>\r\n  <tr>\r\n    <td>\r\n           hi\r\n    </td>\r\n  </tr>\r\n</table>\r\n");
			Paragraph(74..81, () =>
			{
				Literal(74..79, "okay.");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-150"/>
	[TestMethod]
	public void Test150()
	{
		AssertMarkdown(" <div>\r\n  *hello*\r\n         <foo><a>\r\n", () =>
		{
			HtmlBlock(0..38, " <div>\r\n  *hello*\r\n         <foo><a>\r\n");
		});
	}
	/// <summary>
	/// HTML 块可以以闭合标签开始。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-151"/>
	[TestMethod]
	public void Test151()
	{
		AssertMarkdown("</div>\r\n*foo*\r\n", () =>
		{
			HtmlBlock(0..15, "</div>\r\n*foo*\r\n");
		});
	}
	/// <summary>
	/// HTML 块之间可以包含普通的 Markdown 段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-152"/>
	[TestMethod]
	public void Test152()
	{
		AssertMarkdown("<DIV CLASS=\"foo\">\r\n\r\n*Markdown*\r\n\r\n</DIV>\r\n", () =>
		{
			HtmlBlock(0..19, "<DIV CLASS=\"foo\">\r\n");
			Paragraph(21..33, () =>
			{
				Emphasis(21..31, () =>
				{
					Literal(22..30, "Markdown");
				});
			});
			HtmlBlock(35..43, "</DIV>\r\n");
		});
	}
	/// <summary>
	/// 首行标签可以只包含部分，只要在应当是空白的部分分隔开即可。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-153"/>
	[TestMethod]
	public void Test153()
	{
		AssertMarkdown("<div id=\"foo\"\r\n  class=\"bar\">\r\n</div>\r\n", () =>
		{
			HtmlBlock(0..39, "<div id=\"foo\"\r\n  class=\"bar\">\r\n</div>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-154"/>
	[TestMethod]
	public void Test154()
	{
		AssertMarkdown("<div id=\"foo\" class=\"bar\r\n  baz\">\r\n</div>\r\n", () =>
		{
			HtmlBlock(0..43, "<div id=\"foo\" class=\"bar\r\n  baz\">\r\n</div>\r\n");
		});
	}
	/// <summary>
	/// 标签并不需要闭合。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-155"/>
	[TestMethod]
	public void Test155()
	{
		AssertMarkdown("<div>\r\n*foo*\r\n\r\n*bar*\r\n", () =>
		{
			HtmlBlock(0..14, "<div>\r\n*foo*\r\n");
			Paragraph(16..23, () =>
			{
				Emphasis(16..21, () =>
				{
					Literal(17..20, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 标签甚至不需要结束。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-156"/>
	[TestMethod]
	public void Test156()
	{
		AssertMarkdown("<div id=\"foo\"\r\n*hi*\r\n", () =>
		{
			HtmlBlock(0..21, "<div id=\"foo\"\r\n*hi*\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-157"/>
	[TestMethod]
	public void Test157()
	{
		AssertMarkdown("<div class\r\nfoo\r\n", () =>
		{
			HtmlBlock(0..17, "<div class\r\nfoo\r\n");
		});
	}
	/// <summary>
	/// 起始标签可以不是有效的标签。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-158"/>
	[TestMethod]
	public void Test158()
	{
		AssertMarkdown("<div *???-&&&-<---\r\n*foo*\r\n", () =>
		{
			HtmlBlock(0..27, "<div *???-&&&-<---\r\n*foo*\r\n");
		});
	}
	/// <summary>
	/// 对于 Type 6，起始标签并不需要独占一行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-159"/>
	[TestMethod]
	public void Test159()
	{
		AssertMarkdown("<div><a href=\"bar\">*foo*</a></div>\r\n", () =>
		{
			HtmlBlock(0..36, "<div><a href=\"bar\">*foo*</a></div>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-160"/>
	[TestMethod]
	public void Test160()
	{
		AssertMarkdown("<table><tr><td>\r\nfoo\r\n</td></tr></table>\r\n", () =>
		{
			HtmlBlock(0..42, "<table><tr><td>\r\nfoo\r\n</td></tr></table>\r\n");
		});
	}
	/// <summary>
	/// 在下一个空行或文档结束前，所有内容都是 HTML 块内的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-161"/>
	[TestMethod]
	public void Test161()
	{
		AssertMarkdown("<div></div>\r\n``` c\r\nint x = 33;\r\n```\r\n", () =>
		{
			HtmlBlock(0..38, "<div></div>\r\n``` c\r\nint x = 33;\r\n```\r\n");
		});
	}
	/// <summary>
	/// 如果希望使用 Type 6 中未列出的 HTML 块，必须标签必须完整且独占一行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-162"/>
	[TestMethod]
	public void Test162()
	{
		AssertMarkdown("<a href=\"foo\">\r\n*bar*\r\n</a>\r\n", () =>
		{
			HtmlBlock(0..29, "<a href=\"foo\">\r\n*bar*\r\n</a>\r\n");
		});
	}
	/// <summary>
	/// 对于 Type 7，可以使用任意标签名。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-163"/>
	[TestMethod]
	public void Test163()
	{
		AssertMarkdown("<Warning>\r\n*bar*\r\n</Warning>\r\n", () =>
		{
			HtmlBlock(0..30, "<Warning>\r\n*bar*\r\n</Warning>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-164"/>
	[TestMethod]
	public void Test164()
	{
		AssertMarkdown("<i class=\"foo\">\r\n*bar*\r\n</i>\r\n", () =>
		{
			HtmlBlock(0..30, "<i class=\"foo\">\r\n*bar*\r\n</i>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-165"/>
	[TestMethod]
	public void Test165()
	{
		AssertMarkdown("</ins>\r\n*bar*\r\n", () =>
		{
			HtmlBlock(0..15, "</ins>\r\n*bar*\r\n");
		});
	}
	/// <summary>
	/// 由于 <c>&lt;del&gt;</c> 独占了一行，因此整体是一个 HTML 块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-166"/>
	[TestMethod]
	public void Test166()
	{
		AssertMarkdown("<del>\r\n*foo*\r\n</del>\r\n", () =>
		{
			HtmlBlock(0..22, "<del>\r\n*foo*\r\n</del>\r\n");
		});
	}
	/// <summary>
	/// 由于 <c>&lt;del&gt;</c> 后存在空行，因此内容会作为 Markdown 解析。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-167"/>
	[TestMethod]
	public void Test167()
	{
		AssertMarkdown("<del>\r\n\r\n*foo*\r\n\r\n</del>\r\n", () =>
		{
			HtmlBlock(0..7, "<del>\r\n");
			Paragraph(9..16, () =>
			{
				Emphasis(9..14, () =>
				{
					Literal(10..13, "foo");
				});
			});
			HtmlBlock(18..26, "</del>\r\n");
		});
	}
	/// <summary>
	/// <c>&lt;del&gt;</c> 未独占一行，因此作为行级 HTML 解析。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-168"/>
	[TestMethod]
	public void Test168()
	{
		AssertMarkdown("<del>*foo*</del>\r\n", () =>
		{
			Paragraph(0..18, () =>
			{
				HtmlStartTag(0..5, "<del>");
				Emphasis(5..10, () =>
				{
					Literal(6..9, "foo");
				});
				HtmlEndTag(10..16, "</del>");
			});
		});
	}
	/// <summary>
	/// pre 标签（Type 1）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-169"/>
	[TestMethod]
	public void Test169()
	{
		AssertMarkdown("<pre language=\"haskell\"><code>\r\nimport Text.HTML.TagSoup\r\n\r\nmain :: IO ()\r\nmain = print $ parseTags tags\r\n</code></pre>\r\nokay\r\n", () =>
		{
			HtmlBlock(0..121, "<pre language=\"haskell\"><code>\r\nimport Text.HTML.TagSoup\r\n\r\nmain :: IO ()\r\nmain = print $ parseTags tags\r\n</code></pre>\r\n");
			Paragraph(121..127, () =>
			{
				Literal(121..125, "okay");
			});
		});
	}
	/// <summary>
	/// scirpt 标签（Type 1）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-170"/>
	[TestMethod]
	public void Test170()
	{
		AssertMarkdown("<script type=\"text/javascript\">\r\n// JavaScript example\r\n\r\ndocument.getElementById(\"demo\").innerHTML = \"Hello JavaScript!\";\r\n</script>\r\nokay\r\n", () =>
		{
			HtmlBlock(0..135, "<script type=\"text/javascript\">\r\n// JavaScript example\r\n\r\ndocument.getElementById(\"demo\").innerHTML = \"Hello JavaScript!\";\r\n</script>\r\n");
			Paragraph(135..141, () =>
			{
				Literal(135..139, "okay");
			});
		});
	}
	/// <summary>
	/// textarea 标签（Type 1）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-171"/>
	[TestMethod]
	public void Test171()
	{
		AssertMarkdown("<textarea>\r\n\r\n*foo*\r\n\r\n_bar_\r\n\r\n</textarea>\r\n", () =>
		{
			HtmlBlock(0..45, "<textarea>\r\n\r\n*foo*\r\n\r\n_bar_\r\n\r\n</textarea>\r\n");
		});
	}
	/// <summary>
	/// style 标签（Type 1）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-172"/>
	[TestMethod]
	public void Test172()
	{
		AssertMarkdown("<style\r\n  type=\"text/css\">\r\nh1 {color:red;}\r\n\r\np {color:blue;}\r\n</style>\r\nokay\r\n", () =>
		{
			HtmlBlock(0..74, "<style\r\n  type=\"text/css\">\r\nh1 {color:red;}\r\n\r\np {color:blue;}\r\n</style>\r\n");
			Paragraph(74..80, () =>
			{
				Literal(74..78, "okay");
			});
		});
	}
	/// <summary>
	/// 没有匹配的结束标签，会直到文档或块的结束。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-173"/>
	[TestMethod]
	public void Test173()
	{
		AssertMarkdown("<style\r\n  type=\"text/css\">\r\n\r\nfoo\r\n", () =>
		{
			HtmlBlock(0..35, "<style\r\n  type=\"text/css\">\r\n\r\nfoo\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-174"/>
	[TestMethod]
	public void Test174()
	{
		AssertMarkdown("> <div>\r\n> foo\r\n\r\nbar\r\n", () =>
		{
			Quote(0..16, () =>
			{
				HtmlBlock(2..16, "<div>\r\nfoo\r\n");
			});
			Paragraph(18..23, () =>
			{
				Literal(18..21, "bar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-175"/>
	[TestMethod]
	public void Test175()
	{
		AssertMarkdown("- <div>\r\n- foo\r\n", () =>
		{
			UnorderedList(0..16, false, () =>
			{
				ListItem(0..9, () =>
				{
					HtmlBlock(2..9, "<div>\r\n");
				});
				ListItem(9..16, () =>
				{
					Paragraph(11..16, () =>
					{
						Literal(11..14, "foo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 结束标签和起始标签可以在同一行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-176"/>
	[TestMethod]
	public void Test176()
	{
		AssertMarkdown("<style>p{color:red;}</style>\r\n*foo*\r\n", () =>
		{
			HtmlBlock(0..30, "<style>p{color:red;}</style>\r\n");
			Paragraph(30..37, () =>
			{
				Emphasis(30..35, () =>
				{
					Literal(31..34, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-177"/>
	[TestMethod]
	public void Test177()
	{
		AssertMarkdown("<!-- foo -->*bar*\r\n*baz*\r\n", () =>
		{
			HtmlBlock(0..19, "<!-- foo -->*bar*\r\n");
			Paragraph(19..26, () =>
			{
				Emphasis(19..24, () =>
				{
					Literal(20..23, "baz");
				});
			});
		});
	}
	/// <summary>
	/// 最后一行结束标签后的部分也都会计入 HTML 块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-178"/>
	[TestMethod]
	public void Test178()
	{
		AssertMarkdown("<script>\r\nfoo\r\n</script>1. *bar*\r\n", () =>
		{
			HtmlBlock(0..34, "<script>\r\nfoo\r\n</script>1. *bar*\r\n");
		});
	}
	/// <summary>
	/// 注释（Type 2）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-179"/>
	[TestMethod]
	public void Test179()
	{
		AssertMarkdown("<!-- Foo\r\n\r\nbar\r\n   baz -->\r\nokay\r\n", () =>
		{
			HtmlBlock(0..29, "<!-- Foo\r\n\r\nbar\r\n   baz -->\r\n");
			Paragraph(29..35, () =>
			{
				Literal(29..33, "okay");
			});
		});
	}
	/// <summary>
	/// 处理结构（Type 3）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-180"/>
	[TestMethod]
	public void Test180()
	{
		AssertMarkdown("<?php\r\n\r\n  echo '>';\r\n\r\n?>\r\nokay\r\n", () =>
		{
			HtmlBlock(0..28, "<?php\r\n\r\n  echo '>';\r\n\r\n?>\r\n");
			Paragraph(28..34, () =>
			{
				Literal(28..32, "okay");
			});
		});
	}
	/// <summary>
	/// 声明（Type 4）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-181"/>
	[TestMethod]
	public void Test181()
	{
		AssertMarkdown("<!DOCTYPE html>\r\n", () =>
		{
			HtmlBlock(0..17, "<!DOCTYPE html>\r\n");
		});
	}
	/// <summary>
	/// CDATA（Type 5）。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-182"/>
	[TestMethod]
	public void Test182()
	{
		AssertMarkdown("<![CDATA[\r\nfunction matchwo(a,b)\r\n{\r\n  if (a < b && a < 0) then {\r\n    return 1;\r\n\r\n  } else {\r\n\r\n    return 0;\r\n  }\r\n}\r\n]]>\r\nokay\r\n", () =>
		{
			HtmlBlock(0..126, "<![CDATA[\r\nfunction matchwo(a,b)\r\n{\r\n  if (a < b && a < 0) then {\r\n    return 1;\r\n\r\n  } else {\r\n\r\n    return 0;\r\n  }\r\n}\r\n]]>\r\n");
			Paragraph(126..132, () =>
			{
				Literal(126..130, "okay");
			});
		});
	}
	/// <summary>
	/// 起始标签前最多可以有三个空格的缩进，不能是四个。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-183"/>
	[TestMethod]
	public void Test183()
	{
		AssertMarkdown("  <!-- foo -->\r\n\r\n    <!-- foo -->\r\n", () =>
		{
			HtmlBlock(0..16, "  <!-- foo -->\r\n");
			CodeBlock(18..36, "<!-- foo -->\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-184"/>
	[TestMethod]
	public void Test184()
	{
		AssertMarkdown("  <div>\r\n\r\n    <div>\r\n", () =>
		{
			HtmlBlock(0..9, "  <div>\r\n");
			CodeBlock(11..22, "<div>\r\n");
		});
	}
	/// <summary>
	/// Type 1 - 6 可以中断段落，并且之前不需要空行分割。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-185"/>
	[TestMethod]
	public void Test185()
	{
		AssertMarkdown("Foo\r\n<div>\r\nbar\r\n</div>\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "Foo");
			});
			HtmlBlock(5..25, "<div>\r\nbar\r\n</div>\r\n");
		});
	}
	/// <summary>
	/// 但是之后必须有空行分隔，除了文档结束和 Type 1 - 5。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-186"/>
	[TestMethod]
	public void Test186()
	{
		AssertMarkdown("<div>\r\nbar\r\n</div>\r\n*foo*\r\n", () =>
		{
			HtmlBlock(0..27, "<div>\r\nbar\r\n</div>\r\n*foo*\r\n");
		});
	}
	/// <summary>
	/// Type 7 不能中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-187"/>
	[TestMethod]
	public void Test187()
	{
		AssertMarkdown("Foo\r\n<a href=\"bar\">\r\nbaz\r\n", () =>
		{
			Paragraph(0..26, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..5);
				HtmlStartTag(5..19, "<a href=\"bar\">");
				SoftBreak(19..21);
				Literal(21..24, "baz");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-188"/>
	[TestMethod]
	public void Test188()
	{
		AssertMarkdown("<div>\r\n\r\n*Emphasized* text.\r\n\r\n</div>\r\n", () =>
		{
			HtmlBlock(0..7, "<div>\r\n");
			Paragraph(9..29, () =>
			{
				Emphasis(9..21, () =>
				{
					Literal(10..20, "Emphasized");
				});
				Literal(21..27, " text.");
			});
			HtmlBlock(31..39, "</div>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-189"/>
	[TestMethod]
	public void Test189()
	{
		AssertMarkdown("<div>\r\n*Emphasized* text.\r\n</div>\r\n", () =>
		{
			HtmlBlock(0..35, "<div>\r\n*Emphasized* text.\r\n</div>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-190"/>
	[TestMethod]
	public void Test190()
	{
		AssertMarkdown("<table>\r\n\r\n<tr>\r\n\r\n<td>\r\nHi\r\n</td>\r\n\r\n</tr>\r\n\r\n</table>\r\n", () =>
		{
			HtmlBlock(0..9, "<table>\r\n");
			HtmlBlock(11..17, "<tr>\r\n");
			HtmlBlock(19..36, "<td>\r\nHi\r\n</td>\r\n");
			HtmlBlock(38..45, "</tr>\r\n");
			HtmlBlock(47..57, "</table>\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-191"/>
	[TestMethod]
	public void Test191()
	{
		AssertMarkdown("<table>\r\n\r\n  <tr>\r\n\r\n    <td>\r\n      Hi\r\n    </td>\r\n\r\n  </tr>\r\n\r\n</table>\r\n", () =>
		{
			HtmlBlock(0..9, "<table>\r\n");
			HtmlBlock(11..19, "  <tr>\r\n");
			CodeBlock(21..52, "<td>\r\n  Hi\r\n</td>\r\n");
			HtmlBlock(54..63, "  </tr>\r\n");
			HtmlBlock(65..75, "</table>\r\n");
		});
	}
}

