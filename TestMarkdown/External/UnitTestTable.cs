using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 表格的单元测试。
/// </summary>
[TestClass]
public class UnitTestTable : BaseTest
{
	/// <see href="https://github.github.com/gfm/#example-198"/>
	[TestMethod]
	public void TestGFM198()
	{
		AssertMarkdown("| foo | bar |\r\n| --- | --- |\r\n| baz | bim |\r\n", () =>
		{
			Table(0..45, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..15, () =>
				{
					TableCell(0..7, () =>
					{
						Literal(2..5, "foo");
					});
					TableCell(7..13, () =>
					{
						Literal(8..11, "bar");
					});
				});
				TableRow(30..45, () =>
				{
					TableCell(30..37, () =>
					{
						Literal(32..35, "baz");
					});
					TableCell(37..43, () =>
					{
						Literal(38..41, "bim");
					});
				});
			});
		});
	}
	/// <summary>
	/// 同一列的单元格并不需要具有相同的宽度。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-199"/>
	[TestMethod]
	public void TestGFM199()
	{
		AssertMarkdown("| abc | defghi |\r\n:-: | -----------:\r\nbar | baz\r\n", () =>
		{
			Table(0..49, new TableAlign[] { TableAlign.Center, TableAlign.Right }, () =>
			{
				TableRow(0..18, () =>
				{
					TableCell(0..7, () =>
					{
						Literal(2..5, "abc");
					});
					TableCell(7..16, () =>
					{
						Literal(8..14, "defghi");
					});
				});
				TableRow(38..49, () =>
				{
					TableCell(38..43, () =>
					{
						Literal(38..41, "bar");
					});
					TableCell(43..47, () =>
					{
						Literal(44..47, "baz");
					});
				});
			});
		});
	}
	/// <summary>
	/// 要在单元格中使用竖划线，需要将它转义。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-200"/>
	[TestMethod]
	public void TestGFM200()
	{
		AssertMarkdown("| f\\|oo  |\r\n| ------ |\r\n| b `\\|` az |\r\n| b **\\|** im |\r\n", () =>
		{
			Table(0..56, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..12, () =>
				{
					TableCell(0..10, () =>
					{
						Literal(2..7, "f|oo");
					});
				});
				TableRow(24..39, () =>
				{
					TableCell(24..37, () =>
					{
						Literal(26..28, "b ");
						CodeSpan(28..32, "|");
						Literal(32..35, " az");
					});
				});
				TableRow(39..56, () =>
				{
					TableCell(39..54, () =>
					{
						Literal(41..43, "b ");
						Strong(43..49, () =>
						{
							Literal(46..47, "|");
						});
						Literal(49..52, " im");
					});
				});
			});
		});
	}
	/// <summary>
	/// 表格会被首个空行或其他块级结构中断。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-201"/>
	[TestMethod]
	public void TestGFM201()
	{
		AssertMarkdown("| abc | def |\r\n| --- | --- |\r\n| bar | baz |\r\n> bar\r\n", () =>
		{
			Table(0..45, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..15, () =>
				{
					TableCell(0..7, () =>
					{
						Literal(2..5, "abc");
					});
					TableCell(7..13, () =>
					{
						Literal(8..11, "def");
					});
				});
				TableRow(30..45, () =>
				{
					TableCell(30..37, () =>
					{
						Literal(32..35, "bar");
					});
					TableCell(37..43, () =>
					{
						Literal(38..41, "baz");
					});
				});
			});
			Quote(45..52, () =>
			{
				Paragraph(47..52, () =>
				{
					Literal(47..50, "bar");
				});
			});
		});
	}
	/// <see href="https://github.github.com/gfm/#example-202"/>
	[TestMethod]
	public void TestGFM202()
	{
		AssertMarkdown("| abc | def |\r\n| --- | --- |\r\n| bar | baz |\r\nbar\r\n\r\nbar\r\n", () =>
		{
			Table(0..50, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..15, () =>
				{
					TableCell(0..7, () =>
					{
						Literal(2..5, "abc");
					});
					TableCell(7..13, () =>
					{
						Literal(8..11, "def");
					});
				});
				TableRow(30..45, () =>
				{
					TableCell(30..37, () =>
					{
						Literal(32..35, "bar");
					});
					TableCell(37..43, () =>
					{
						Literal(38..41, "baz");
					});
				});
				TableRow(45..50, () =>
				{
					TableCell(45..48, () =>
					{
						Literal(45..48, "bar");
					});
				});
			});
			Paragraph(52..57, () =>
			{
				Literal(52..55, "bar");
			});
		});
	}
	/// <summary>
	/// 标题必须与分割行包含相同的单元格个数。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-203"/>
	[TestMethod]
	public void TestGFM203()
	{
		AssertMarkdown("| abc | def |\r\n| --- |\r\n| bar |\r\n", () =>
		{
			Paragraph(0..33, () =>
			{
				Literal(0..13, "| abc | def |");
				SoftBreak(13..15);
				Literal(15..22, "| --- |");
				SoftBreak(22..24);
				Literal(24..31, "| bar |");
			});
		});
	}
	/// <summary>
	/// 其它行可以包含不同个数的单元格。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-204"/>
	[TestMethod]
	public void TestGFM204()
	{
		AssertMarkdown("| abc | def |\r\n| --- | --- |\r\n| bar |\r\n| bar | baz | boo |\r\n", () =>
		{
			Table(0..60, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..15, () =>
				{
					TableCell(0..7, () =>
					{
						Literal(2..5, "abc");
					});
					TableCell(7..13, () =>
					{
						Literal(8..11, "def");
					});
				});
				TableRow(30..39, () =>
				{
					TableCell(30..37, () =>
					{
						Literal(32..35, "bar");
					});
				});
				TableRow(39..60, () =>
				{
					TableCell(39..46, () =>
					{
						Literal(41..44, "bar");
					});
					TableCell(46..52, () =>
					{
						Literal(47..50, "baz");
					});
					TableCell(52..58, () =>
					{
						Literal(53..56, "boo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 表格只有标题时，不会生成 <c>&lt;tbody&gt;</c>。
	/// </summary>
	/// <see href="https://github.github.com/gfm/#example-205"/>
	[TestMethod]
	public void TestGFM205()
	{
		AssertMarkdown("| abc | def |\r\n| --- | --- |\r\n", () =>
		{
			Table(0..30, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..15, () =>
				{
					TableCell(0..7, () =>
					{
						Literal(2..5, "abc");
					});
					TableCell(7..13, () =>
					{
						Literal(8..11, "def");
					});
				});
			});
		});
	}
	/// <summary>
	/// 标题可以包含至多三个空白缩进。
	/// </summary>
	[TestMethod]
	public void TestHeadingLeadingSpace()
	{
		AssertMarkdown(" abc\r\n|-\r\n", () =>
		{
			Table(1..10, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(1..6, () =>
				{
					TableCell(1..4, () =>
					{
						Literal(1..4, "abc");
					});
				});
			});
		});
		AssertMarkdown("  abc\r\n|-\r\n", () =>
		{
			Table(2..11, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(2..7, () =>
				{
					TableCell(2..5, () =>
					{
						Literal(2..5, "abc");
					});
				});
			});
		});
		AssertMarkdown("   abc\r\n|-\r\n", () =>
		{
			Table(3..12, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(3..8, () =>
				{
					TableCell(3..6, () =>
					{
						Literal(3..6, "abc");
					});
				});
			});
		});
		// 四个空格就太多了。
		AssertMarkdown("    abc\r\n|-\r\n", () =>
		{
			CodeBlock(0..9, "abc\r\n");
			Paragraph(9..13, () =>
			{
				Literal(9..11, "|-");
			});
		});
	}
	/// <summary>
	/// 标题可以包含任意尾随空白。
	/// </summary>
	[TestMethod]
	public void TestHeadingTrailingSpace()
	{
		AssertMarkdown("abc    \r\n|-\r\n", () =>
		{
			Table(0..13, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..9, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc |   \r\n|-\r\n", () =>
		{
			Table(0..14, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..10, () =>
				{
					TableCell(0..5, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
	}
	/// <summary>
	/// 标题必须在一行。
	/// </summary>
	[TestMethod]
	public void TestHeadingMustBeOneLine()
	{
		AssertMarkdown("abc\r\n|def\r\n-|-\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..3, "abc");
				SoftBreak(3..5);
				Literal(5..9, "|def");
				SoftBreak(9..11);
				Literal(11..14, "-|-");
			});
		});
	}
	/// <summary>
	/// 标题单元格可以为空。
	/// </summary>
	[TestMethod]
	public void TestHeadingCellCanEmpty()
	{
		AssertMarkdown("|\r\n|-\r\n", () =>
		{
			Table(0..7, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..3, () =>
				{
					TableCell(0..1);
				});
			});
		});
		AssertMarkdown(" | \r\n|-\r\n", () =>
		{
			Table(1..9, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(1..5, () =>
				{
					TableCell(1..2);
				});
			});
		});
		AssertMarkdown("||\r\n|-\r\n", () =>
		{
			Table(0..8, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..4, () =>
				{
					TableCell(0..2);
				});
			});
		});
		AssertMarkdown(" |  | \r\n|-\r\n", () =>
		{
			Table(1..12, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(1..8, () =>
				{
					TableCell(1..5);
				});
			});
		});
		AssertMarkdown("|||\r\n-|-\r\n", () =>
		{
			Table(0..10, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..2);
					TableCell(2..3);
				});
			});
		});
		AssertMarkdown("||center header||\r\n-|-------------|-\r\n1|      2      |3\r\n", () =>
		{
			Table(0..57, new TableAlign[] { TableAlign.None, TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..19, () =>
				{
					TableCell(0..2);
					TableCell(2..16, () =>
					{
						Literal(2..15, "center header");
					});
					TableCell(16..17);
				});
				TableRow(38..57, () =>
				{
					TableCell(38..40, () =>
					{
						Literal(38..39, "1");
					});
					TableCell(40..54, () =>
					{
						Literal(46..47, "2");
					});
					TableCell(54..55, () =>
					{
						Literal(54..55, "3");
					});
				});
			});
		});
	}
	/// <summary>
	/// 标题单元格可以有不同写法。
	/// </summary>
	[TestMethod]
	public void TestHeadingPipes()
	{
		AssertMarkdown("abc\r\n|-\r\n", () =>
		{
			Table(0..9, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("|abc\r\n|-\r\n", () =>
		{
			Table(0..10, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..6, () =>
				{
					TableCell(0..4, () =>
					{
						Literal(1..4, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc|\r\n|-\r\n", () =>
		{
			Table(0..10, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..6, () =>
				{
					TableCell(0..4, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("|abc|\r\n|-\r\n", () =>
		{
			Table(0..11, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..7, () =>
				{
					TableCell(0..5, () =>
					{
						Literal(1..4, "abc");
					});
				});
			});
		});
	}
	/// <summary>
	/// 标题内容全部为空时可以不输出 <c>&lt;thead&gt;</c>。
	/// </summary>
	[TestMethod]
	public void TestHeadingNoThead()
	{
		HtmlRenderer renderer = new();

		// 没有 thead 和 tbody
		renderer.Clear();
		renderer.Visit(Document.Parse("|\r\n|-\r\n"));
		Assert.AreEqual("<table>\n</table>\n", renderer.ToString());

		// 只有 tbody
		renderer.Clear();
		renderer.Visit(Document.Parse("|\r\n|-\r\nabc\r\n"));
		Assert.AreEqual("<table>\n<tbody>\n<tr>\n<td>abc</td>\n</tr>\n</tbody>\n</table>\n", renderer.ToString());

		// 全都有
		renderer.Clear();
		renderer.Visit(Document.Parse("||abc\r\n-|-\r\ndef\r\n"));
		Assert.AreEqual("<table>\n<thead>\n<tr>\n<th></th>\n<th>abc</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>def</td>\n<td></td>\n</tr>\n</tbody>\n</table>\n", renderer.ToString());

		// 总是输出 thead
		renderer.OutputEmptyTableHeading = true;

		renderer.Clear();
		renderer.Visit(Document.Parse("|\r\n|-\r\n"));
		Assert.AreEqual("<table>\n<thead>\n<tr>\n<th></th>\n</tr>\n</thead>\n</table>\n", renderer.ToString());

		renderer.Clear();
		renderer.Visit(Document.Parse("|\r\n|-\r\nabc\r\n"));
		Assert.AreEqual("<table>\n<thead>\n<tr>\n<th></th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>abc</td>\n</tr>\n</tbody>\n</table>\n", renderer.ToString());

		renderer.Clear();
		renderer.Visit(Document.Parse("||abc\r\n-|-\r\ndef\r\n"));
		Assert.AreEqual("<table>\n<thead>\n<tr>\n<th></th>\n<th>abc</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>def</td>\n<td></td>\n</tr>\n</tbody>\n</table>\n", renderer.ToString());
	}
	/// <summary>
	/// 分割行必须包含至少一个中划线。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowMustOneOrMore()
	{
		AssertMarkdown("abc|def\r\n-|-\r\n", () =>
		{
			Table(0..14, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..9, () =>
				{
					TableCell(0..4, () =>
					{
						Literal(0..3, "abc");
					});
					TableCell(4..7, () =>
					{
						Literal(4..7, "def");
					});
				});
			});
		});
		AssertMarkdown("abc|def\r\n--|--\r\n", () =>
		{
			Table(0..16, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..9, () =>
				{
					TableCell(0..4, () =>
					{
						Literal(0..3, "abc");
					});
					TableCell(4..7, () =>
					{
						Literal(4..7, "def");
					});
				});
			});
		});
	}
	/// <summary>
	/// 分割行不能包含无效字符。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowCanNotContainInvalidChars()
	{
		AssertMarkdown("abc|def\r\n-a|-\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..13, "-a|-");
			});
		});
		AssertMarkdown("abc|def\r\n-a-|-\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..14, "-a-|-");
			});
		});
		AssertMarkdown("abc|def\r\n-- -|-\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..15, "-- -|-");
			});
		});
		AssertMarkdown("abc|def\r\n:a-|-\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..14, ":a-|-");
			});
		});
		AssertMarkdown("abc|def\r\n: -|-\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..14, ": -|-");
			});
		});
		AssertMarkdown("abc|def\r\n-- :|-\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..15, "-- :|-");
			});
		});
	}
	/// <summary>
	/// 分割行可以有不同的写法。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowPipes()
	{
		AssertMarkdown("abc\r\n|-\r\n", () =>
		{
			Table(0..9, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n-|\r\n", () =>
		{
			Table(0..9, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-|\r\n", () =>
		{
			Table(0..10, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
	}
	/// <summary>
	/// 分割行可以包含至多三个空白缩进。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowLeadingSpace()
	{
		AssertMarkdown("abc\r\n |-\r\n", () =>
		{
			Table(0..10, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n  |-\r\n", () =>
		{
			Table(0..11, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n   |-\r\n", () =>
		{
			Table(0..12, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		// 四个空格就太多了。
		AssertMarkdown("abc\r\n    |-\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..3, "abc");
				SoftBreak(3..9);
				Literal(9..11, "|-");
			});
		});
	}
	/// <summary>
	/// 分割行可以包含任意尾随空白。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowTrailingSpace()
	{
		AssertMarkdown("abc\r\n|-   \r\n", () =>
		{
			Table(0..12, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|- |  \r\n", () =>
		{
			Table(0..13, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
		});
	}
	/// <summary>
	/// 分割行必须包含竖划线。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowNeedsPipes()
	{
		AssertMarkdown("abc|def\r\n-- -\r\n", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "abc|def");
			});
			ThematicBreak(9..15);
		});
	}
	/// <summary>
	/// 分割行不能包含连续的竖划线。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowCanNotHaveAdjacentPipes()
	{
		AssertMarkdown("abc|def\r\n-||-\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..7, "abc|def");
				SoftBreak(7..9);
				Literal(9..13, "-||-");
			});
		});
		AssertMarkdown("abc|def|ghi\r\n-||-\r\n", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..11, "abc|def|ghi");
				SoftBreak(11..13);
				Literal(13..17, "-||-");
			});
		});
	}
	/// <summary>
	/// 分割行必须与标题行具有相同的单元格数。
	/// </summary>
	[TestMethod]
	public void TestDelimiterRowMustHaveSameColumnWithHead()
	{
		// 这里标题的 | 会被解释成起始分割线
		AssertMarkdown(" | \r\n-|-\r\n", () =>
		{
			Paragraph(1..10, () =>
			{
				Literal(1..2, "|");
				SoftBreak(2..5);
				Literal(5..8, "-|-");
			});
		});
		AssertMarkdown(" |abc\r\n-|-\r\n", () =>
		{
			Paragraph(1..12, () =>
			{
				Literal(1..5, "|abc");
				SoftBreak(5..7);
				Literal(7..10, "-|-");
			});
		});
		// 这里标题的 | 会被解释成结束分割线
		AssertMarkdown("abc| \r\n-|-\r\n", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..4, "abc|");
				SoftBreak(4..7);
				Literal(7..10, "-|-");
			});
		});
		// 这里标题的 | 会被解释成起始和结束分割线
		AssertMarkdown(" | abc | \r\n-|-\r\n", () =>
		{
			Paragraph(1..16, () =>
			{
				Literal(1..8, "| abc |");
				SoftBreak(8..11);
				Literal(11..14, "-|-");
			});
		});
	}
	/// <summary>
	/// 表格体可以包含至多三个空白缩进。
	/// </summary>
	[TestMethod]
	public void TestBodyLeadingSpace()
	{
		AssertMarkdown("abc\r\n|-\r\n def\r\n", () =>
		{
			Table(0..15, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..15, () =>
				{
					TableCell(10..13, () =>
					{
						Literal(10..13, "def");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n  def\r\n", () =>
		{
			Table(0..16, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..16, () =>
				{
					TableCell(11..14, () =>
					{
						Literal(11..14, "def");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n   def\r\n", () =>
		{
			Table(0..17, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..17, () =>
				{
					TableCell(12..15, () =>
					{
						Literal(12..15, "def");
					});
				});
			});
		});
		// 四个就太多了
		AssertMarkdown("abc\r\n|-\r\n    def\r\n", () =>
		{
			Table(0..9, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
			});
			CodeBlock(9..18, "def\r\n");
		});
	}
	/// <summary>
	/// 表格体可以包含任意尾随空白。
	/// </summary>
	[TestMethod]
	public void TestBodyTrailingSpace()
	{
		AssertMarkdown("abc\r\n|-\r\ndef    \r\n", () =>
		{
			Table(0..18, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..18, () =>
				{
					TableCell(9..12, () =>
					{
						Literal(9..12, "def");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\ndef |   \r\n", () =>
		{
			Table(0..19, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..19, () =>
				{
					TableCell(9..14, () =>
					{
						Literal(9..12, "def");
					});
				});
			});
		});
	}
	/// <summary>
	/// 内容单元格可以有不同写法。
	/// </summary>
	[TestMethod]
	public void TestBodyPipes()
	{
		AssertMarkdown("abc\r\n|-\r\ndef\r\n", () =>
		{
			Table(0..14, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..14, () =>
				{
					TableCell(9..12, () =>
					{
						Literal(9..12, "def");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n|def\r\n", () =>
		{
			Table(0..15, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..15, () =>
				{
					TableCell(9..13, () =>
					{
						Literal(10..13, "def");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\ndef|\r\n", () =>
		{
			Table(0..15, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..15, () =>
				{
					TableCell(9..13, () =>
					{
						Literal(9..12, "def");
					});
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n|def|\r\n", () =>
		{
			Table(0..16, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..16, () =>
				{
					TableCell(9..14, () =>
					{
						Literal(10..13, "def");
					});
				});
			});
		});
	}
	/// <summary>
	/// 表格内容单元格可以为空。
	/// </summary>
	[TestMethod]
	public void TestBodyCellCanEmpty()
	{
		AssertMarkdown("abc\r\n|-\r\n|\r\n", () =>
		{
			Table(0..12, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..12, () =>
				{
					TableCell(9..10);
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n | \r\n", () =>
		{
			Table(0..14, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..14, () =>
				{
					TableCell(10..11);
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n||\r\n", () =>
		{
			Table(0..13, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..13, () =>
				{
					TableCell(9..11);
				});
			});
		});
		AssertMarkdown("abc\r\n|-\r\n |  | \r\n", () =>
		{
			Table(0..17, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..17, () =>
				{
					TableCell(10..14);
				});
			});
		});
	}
	/// <summary>
	/// 测试转义字符。
	/// </summary>
	[TestMethod]
	public void TestEscaped()
	{
		AssertMarkdown("abc\r\n|-\r\n1\\`2`\r\n", () =>
		{
			Table(0..16, new TableAlign[] { TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..3, () =>
					{
						Literal(0..3, "abc");
					});
				});
				TableRow(9..16, () =>
				{
					TableCell(9..14, () =>
					{
						Literal(9..14, "1`2`");
					});
				});
			});
		});
	}
	/// <summary>
	/// 测试被转义的反斜线。
	/// </summary>
	[TestMethod]
	public void TestEscapedBackslash()
	{
		// 这里的 `1\\|2` 看起来像是被转义的反斜线，后跟一个竖划线，因此被识别为两个表格。
		// 但根据 GFM，由于会先解析表格，此时会先将 `\|` 解析为被转义的竖划线，因此只有一个单元格。
		// 考虑到前一种用法更符合通常的 Markdown 转义用法（总是从左至右解析），因此选择了前一种实现。
		// 很多其它 Markdown 库也都采用了前一种实现。
		AssertMarkdown("a|b\r\n-|-\r\n1\\\\|2\r\n", () =>
		{
			Table(0..17, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..2, () =>
					{
						Literal(0..1, "a");
					});
					TableCell(2..3, () =>
					{
						Literal(2..3, "b");
					});
				});
				TableRow(10..17, () =>
				{
					TableCell(10..14, () =>
					{
						Literal(10..13, "1\\");
					});
					TableCell(14..15, () =>
					{
						Literal(14..15, "2");
					});
				});
			});
		});
	}
	/// <summary>
	/// 测试结尾的反斜线。
	/// </summary>
	[TestMethod]
	public void TestBackslashAtEnd()
	{
		// 对于单元格来说，`2\` 后并没有换行，因此不会被识别为硬换行。
		AssertMarkdown("a|b\r\n-|-\r\n1|2\\\r\n", () =>
		{
			Table(0..16, new TableAlign[] { TableAlign.None, TableAlign.None }, () =>
			{
				TableRow(0..5, () =>
				{
					TableCell(0..2, () =>
					{
						Literal(0..1, "a");
					});
					TableCell(2..3, () =>
					{
						Literal(2..3, "b");
					});
				});
				TableRow(10..16, () =>
				{
					TableCell(10..12, () =>
					{
						Literal(10..11, "1");
					});
					TableCell(12..14, () =>
					{
						Literal(12..14, "2\\");
					});
				});
			});
		});
	}
	/// <summary>
	/// 测试表格的对齐。
	/// </summary>
	[TestMethod]
	public void TestAlign()
	{
		AssertMarkdown("a|b|c|d|e\r\n:-| :---| -:|:-:|:----:  |   \r\n", () =>
		{
			Table(0..42, new TableAlign[] {
				TableAlign.Left,
				TableAlign.Left,
				TableAlign.Right,
				TableAlign.Center,
				TableAlign.Center,
			}, () =>
			{
				TableRow(0..11, () =>
				{
					TableCell(0..2, () =>
					{
						Literal(0..1, "a");
					});
					TableCell(2..4, () =>
					{
						Literal(2..3, "b");
					});
					TableCell(4..6, () =>
					{
						Literal(4..5, "c");
					});
					TableCell(6..8, () =>
					{
						Literal(6..7, "d");
					});
					TableCell(8..9, () =>
					{
						Literal(8..9, "e");
					});
				});
			});
		});
	}
	/// <summary>
	/// 测试引用中的表格。
	/// </summary>
	[TestMethod]
	public void TestInsideQueue()
	{
		AssertMarkdown("> abc\r\n> |-\r\n> def\r\nghi\r\n", () =>
		{
			Quote(0..25, () =>
			{
				Table(2..25, new TableAlign[] { TableAlign.None, }, () =>
				{
					TableRow(2..7, () =>
					{
						TableCell(2..5, () =>
						{
							Literal(2..5, "abc");
						});
					});
					TableRow(15..20, () =>
					{
						TableCell(15..18, () =>
						{
							Literal(15..18, "def");
						});
					});
					TableRow(20..25, () =>
					{
						TableCell(20..23, () =>
						{
							Literal(20..23, "ghi");
						});
					});
				});
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持表格。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("| foo | bar |\r\n| --- | --- |\r\n| baz | bim |\r\n", () =>
		{
			Paragraph(0..45, () =>
			{
				Literal(0..13, "| foo | bar |");
				SoftBreak(13..15);
				Literal(15..28, "| --- | --- |");
				SoftBreak(28..30);
				Literal(30..43, "| baz | bim |");
			});
		});
	}
}

