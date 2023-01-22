using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 强调的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#emphasis-and-strong-emphasis"/>
[TestClass]
public partial class UnitTestEmphasis : BaseTest
{
	/// <summary>
	/// 规则 1。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-350"/>
	[TestMethod]
	public void Test350()
	{
		AssertMarkdown("*foo bar*", () =>
		{
			Paragraph(0..9, () =>
			{
				Emphasis(0..9, () =>
				{
					Literal(1..8, "foo bar");
				});
			});
		});
	}
	/// <summary>
	/// 起始 * 后跟空白，因此不是强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-351"/>
	[TestMethod]
	public void Test351()
	{
		AssertMarkdown("a * foo bar*", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..12, "a * foo bar*");
			});
		});
	}
	/// <summary>
	/// 起始 * 前是字符后是后跟标点，因此不是强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-352"/>
	[TestMethod]
	public void Test352()
	{
		AssertMarkdown("a*\"foo\"*", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "a*\"foo\"*");
			});
		});
	}
	/// <summary>
	/// Unicode 空白也当作空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-353"/>
	[TestMethod]
	public void Test353()
	{
		AssertMarkdown("*\u00A0a\u00A0*", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..5, "*\u00A0a\u00A0*");
			});
		});
	}
	/// <summary>
	/// * 可以用于单词内的强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-354"/>
	[TestMethod]
	public void Test354()
	{
		AssertMarkdown("foo*bar*", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "foo");
				Emphasis(3..8, () =>
				{
					Literal(4..7, "bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-355"/>
	[TestMethod]
	public void Test355()
	{
		AssertMarkdown("5*6*78", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..1, "5");
				Emphasis(1..4, () =>
				{
					Literal(2..3, "6");
				});
				Literal(4..6, "78");
			});
		});
	}
	/// <summary>
	/// 规则 2。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-356"/>
	[TestMethod]
	public void Test356()
	{
		AssertMarkdown("_foo bar_", () =>
		{
			Paragraph(0..9, () =>
			{
				Emphasis(0..9, () =>
				{
					Literal(1..8, "foo bar");
				});
			});
		});
	}
	/// <summary>
	/// 起始 _ 后跟空白，因此不是强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-357"/>
	[TestMethod]
	public void Test357()
	{
		AssertMarkdown("_ foo bar_", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "_ foo bar_");
			});
		});
	}
	/// <summary>
	/// 起始 _ 前是字符后是后跟标点，因此不是强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-358"/>
	[TestMethod]
	public void Test358()
	{
		AssertMarkdown("a_\"foo\"_", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "a_\"foo\"_");
			});
		});
	}
	/// <summary>
	/// _ 不可以用于单词内的强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-359"/>
	[TestMethod]
	public void Test359()
	{
		AssertMarkdown("foo_bar_", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "foo_bar_");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-360"/>
	[TestMethod]
	public void Test360()
	{
		AssertMarkdown("5_6_78", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..6, "5_6_78");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-361"/>
	[TestMethod]
	public void Test361()
	{
		AssertMarkdown("пристаням_стремятся_", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..20, "пристаням_стремятся_");
			});
		});
	}
	/// <summary>
	/// 这里不是强调，因为第一个分隔符是右侧的，第二个是左侧的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-362"/>
	[TestMethod]
	public void Test362()
	{
		AssertMarkdown("aa_\"bb\"_cc", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "aa_\"bb\"_cc");
			});
		});
	}
	/// <summary>
	/// 这里是强调，起始分隔符既可以是右侧又可以是左侧。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-363"/>
	[TestMethod]
	public void Test363()
	{
		AssertMarkdown("foo-_(bar)_", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..4, "foo-");
				Emphasis(4..11, () =>
				{
					Literal(5..10, "(bar)");
				});
			});
		});
	}
	/// <summary>
	/// 规则 3。
	/// 起始和结束分隔符必须相同。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-364"/>
	[TestMethod]
	public void Test364()
	{
		AssertMarkdown("_foo*", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..5, "_foo*");
			});
		});
	}
	/// <summary>
	/// 不是强调，后一个 * 前是空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-365"/>
	[TestMethod]
	public void Test365()
	{
		AssertMarkdown("*foo bar *", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "*foo bar *");
			});
		});
	}
	/// <summary>
	/// 换行也算作空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-366"/>
	[TestMethod]
	public void Test366()
	{
		AssertMarkdown("*foo bar\r\n*", () =>
		{
			Paragraph(0..11, () =>
			{
				Literal(0..8, "*foo bar");
				SoftBreak(8..10);
				Literal(10..11, "*");
			});
		});
	}
	/// <summary>
	/// 不是强调，第二个 * 不是右侧的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-367"/>
	[TestMethod]
	public void Test367()
	{
		AssertMarkdown("*(*foo)", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "*(*foo)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-368"/>
	[TestMethod]
	public void Test368()
	{
		AssertMarkdown("*(*foo*)*", () =>
		{
			Paragraph(0..9, () =>
			{
				Emphasis(0..9, () =>
				{
					Literal(1..2, "(");
					Emphasis(2..7, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(7..8, ")");
				});
			});
		});
	}
	/// <summary>
	/// * 可以用于单词内的强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-369"/>
	[TestMethod]
	public void Test369()
	{
		AssertMarkdown("*foo*bar", () =>
		{
			Paragraph(0..8, () =>
			{
				Emphasis(0..5, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..8, "bar");
			});
		});
	}
	/// <summary>
	/// 规则 4。
	/// 不是强调，后一个 _ 前是空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-370"/>
	[TestMethod]
	public void Test370()
	{
		AssertMarkdown("_foo bar _", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "_foo bar _");
			});
		});
	}
	/// <summary>
	/// 不是强调，第二个 _ 不是右侧的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-371"/>
	[TestMethod]
	public void Test371()
	{
		AssertMarkdown("_(_foo)", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "_(_foo)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-372"/>
	[TestMethod]
	public void Test372()
	{
		AssertMarkdown("_(_foo_)_", () =>
		{
			Paragraph(0..9, () =>
			{
				Emphasis(0..9, () =>
				{
					Literal(1..2, "(");
					Emphasis(2..7, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(7..8, ")");
				});
			});
		});
	}
	/// <summary>
	/// _ 不可以用于单词内的强调。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-373"/>
	[TestMethod]
	public void Test373()
	{
		AssertMarkdown("_foo_bar", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "_foo_bar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-374"/>
	[TestMethod]
	public void Test374()
	{
		AssertMarkdown("_пристаням_стремятся", () =>
		{
			Paragraph(0..20, () =>
			{
				Literal(0..20, "_пристаням_стремятся");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-375"/>
	[TestMethod]
	public void Test375()
	{
		AssertMarkdown("_foo_bar_baz_", () =>
		{
			Paragraph(0..13, () =>
			{
				Emphasis(0..13, () =>
				{
					Literal(1..12, "foo_bar_baz");
				});
			});
		});
	}
	/// <summary>
	/// 这里是强调，结束分隔符既可以是右侧又可以是左侧。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-376"/>
	[TestMethod]
	public void Test376()
	{
		AssertMarkdown("_(bar)_.", () =>
		{
			Paragraph(0..8, () =>
			{
				Emphasis(0..7, () =>
				{
					Literal(1..6, "(bar)");
				});
				Literal(7..8, ".");
			});
		});
	}
	/// <summary>
	/// 规则 5。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-377"/>
	[TestMethod]
	public void Test377()
	{
		AssertMarkdown("**foo bar**", () =>
		{
			Paragraph(0..11, () =>
			{
				Strong(0..11, () =>
				{
					Literal(2..9, "foo bar");
				});
			});
		});
	}
	/// <summary>
	/// 起始 ** 后跟空白，因此不是加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-378"/>
	[TestMethod]
	public void Test378()
	{
		AssertMarkdown("** foo bar**", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..12, "** foo bar**");
			});
		});
	}
	/// <summary>
	/// 起始 ** 前是字符后是后跟标点，因此不是加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-379"/>
	[TestMethod]
	public void Test379()
	{
		AssertMarkdown("a**\"foo\"**", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "a**\"foo\"**");
			});
		});
	}
	/// <summary>
	/// ** 可以用于单词内的加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-380"/>
	[TestMethod]
	public void Test380()
	{
		AssertMarkdown("foo**bar**", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "foo");
				Strong(3..10, () =>
				{
					Literal(5..8, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 规则 6。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-381"/>
	[TestMethod]
	public void Test381()
	{
		AssertMarkdown("__foo bar__", () =>
		{
			Paragraph(0..11, () =>
			{
				Strong(0..11, () =>
				{
					Literal(2..9, "foo bar");
				});
			});
		});
	}
	/// <summary>
	/// 起始 __ 后跟空白，因此不是加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-382"/>
	[TestMethod]
	public void Test382()
	{
		AssertMarkdown("__ foo bar__", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..12, "__ foo bar__");
			});
		});
	}
	/// <summary>
	/// 换行被当作空白看待。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-383"/>
	[TestMethod]
	public void Test383()
	{
		AssertMarkdown("__\r\nfoo bar__", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..2, "__");
				SoftBreak(2..4);
				Literal(4..13, "foo bar__");
			});
		});
	}
	/// <summary>
	/// 起始 __ 前是字符后是后跟标点，因此不是加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-384"/>
	[TestMethod]
	public void Test384()
	{
		AssertMarkdown("a__\"foo\"__", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "a__\"foo\"__");
			});
		});
	}
	/// <summary>
	/// __ 不可以用于单词内的加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-385"/>
	[TestMethod]
	public void Test385()
	{
		AssertMarkdown("foo__bar__", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "foo__bar__");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-386"/>
	[TestMethod]
	public void Test386()
	{
		AssertMarkdown("5__6__78", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "5__6__78");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-387"/>
	[TestMethod]
	public void Test387()
	{
		AssertMarkdown("пристаням__стремятся__", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..22, "пристаням__стремятся__");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-388"/>
	[TestMethod]
	public void Test388()
	{
		AssertMarkdown("__foo, __bar__, baz__", () =>
		{
			Paragraph(0..21, () =>
			{
				Strong(0..21, () =>
				{
					Literal(2..7, "foo, ");
					Strong(7..14, () =>
					{
						Literal(9..12, "bar");
					});
					Literal(14..19, ", baz");
				});
			});
		});
	}
	/// <summary>
	/// 这里是加粗，起始分隔符既可以是右侧又可以是左侧。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-389"/>
	[TestMethod]
	public void Test389()
	{
		AssertMarkdown("foo-__(bar)__", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..4, "foo-");
				Strong(4..13, () =>
				{
					Literal(6..11, "(bar)");
				});
			});
		});
	}
	/// <summary>
	/// 规则 7。
	/// 不是加粗，后一个 ** 前是空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-390"/>
	[TestMethod]
	public void Test390()
	{
		AssertMarkdown("**foo bar **", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..12, "**foo bar **");
			});
		});
	}
	/// <summary>
	/// 不是加粗，第二个 ** 不是右侧的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-391"/>
	[TestMethod]
	public void Test391()
	{
		AssertMarkdown("**(**foo)", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..9, "**(**foo)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-392"/>
	[TestMethod]
	public void Test392()
	{
		AssertMarkdown("*(**foo**)*", () =>
		{
			Paragraph(0..11, () =>
			{
				Emphasis(0..11, () =>
				{
					Literal(1..2, "(");
					Strong(2..9, () =>
					{
						Literal(4..7, "foo");
					});
					Literal(9..10, ")");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-393"/>
	[TestMethod]
	public void Test393()
	{
		AssertMarkdown("**Gomphocarpus (*Gomphocarpus physocarpus*, syn.\r\n*Asclepias physocarpa*)**", () =>
		{
			Paragraph(0..75, () =>
			{
				Strong(0..75, () =>
				{
					Literal(2..16, "Gomphocarpus (");
					Emphasis(16..42, () =>
					{
						Literal(17..41, "Gomphocarpus physocarpus");
					});
					Literal(42..48, ", syn.");
					SoftBreak(48..50);
					Emphasis(50..72, () =>
					{
						Literal(51..71, "Asclepias physocarpa");
					});
					Literal(72..73, ")");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-394"/>
	[TestMethod]
	public void Test394()
	{
		AssertMarkdown("**foo \"*bar*\" foo**", () =>
		{
			Paragraph(0..19, () =>
			{
				Strong(0..19, () =>
				{
					Literal(2..7, "foo \"");
					Emphasis(7..12, () =>
					{
						Literal(8..11, "bar");
					});
					Literal(12..17, "\" foo");
				});
			});
		});
	}
	/// <summary>
	/// ** 可以用于单词内的加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-395"/>
	[TestMethod]
	public void Test395()
	{
		AssertMarkdown("**foo**bar", () =>
		{
			Paragraph(0..10, () =>
			{
				Strong(0..7, () =>
				{
					Literal(2..5, "foo");
				});
				Literal(7..10, "bar");
			});
		});
	}
	/// <summary>
	/// 规则 8。
	/// 不是加粗，后一个 __ 前是空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-396"/>
	[TestMethod]
	public void Test396()
	{
		AssertMarkdown("__foo bar __", () =>
		{
			Paragraph(0..12, () =>
			{
				Literal(0..12, "__foo bar __");
			});
		});
	}
	/// <summary>
	/// 不是加粗，第二个 __ 不是右侧的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-397"/>
	[TestMethod]
	public void Test397()
	{
		AssertMarkdown("__(__foo)", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..9, "__(__foo)");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-398"/>
	[TestMethod]
	public void Test398()
	{
		AssertMarkdown("_(__foo__)_", () =>
		{
			Paragraph(0..11, () =>
			{
				Emphasis(0..11, () =>
				{
					Literal(1..2, "(");
					Strong(2..9, () =>
					{
						Literal(4..7, "foo");
					});
					Literal(9..10, ")");
				});
			});
		});
	}
	/// <summary>
	/// __ 不可以用于单词内的加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-399"/>
	[TestMethod]
	public void Test399()
	{
		AssertMarkdown("__foo__bar", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..10, "__foo__bar");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-400"/>
	[TestMethod]
	public void Test400()
	{
		AssertMarkdown("__пристаням__стремятся", () =>
		{
			Paragraph(0..22, () =>
			{
				Literal(0..22, "__пристаням__стремятся");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-401"/>
	[TestMethod]
	public void Test401()
	{
		AssertMarkdown("__foo__bar__baz__", () =>
		{
			Paragraph(0..17, () =>
			{
				Strong(0..17, () =>
				{
					Literal(2..15, "foo__bar__baz");
				});
			});
		});
	}
	/// <summary>
	/// 这里是加粗，结束分隔符既可以是右侧又可以是左侧。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-402"/>
	[TestMethod]
	public void Test402()
	{
		AssertMarkdown("__(bar)__.", () =>
		{
			Paragraph(0..10, () =>
			{
				Strong(0..9, () =>
				{
					Literal(2..7, "(bar)");
				});
				Literal(9..10, ".");
			});
		});
	}
	/// <summary>
	/// 规则 9。
	/// 非空行内元素序列可以作为强调的内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-403"/>
	[TestMethod]
	public void Test403()
	{
		AssertMarkdown("*foo [bar](/url)*", () =>
		{
			Paragraph(0..17, () =>
			{
				Emphasis(0..17, () =>
				{
					Literal(1..5, "foo ");
					Link(5..16, "/url", null, () =>
					{
						Literal(6..9, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 非空行内元素序列可以作为强调的内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-404"/>
	[TestMethod]
	public void Test404()
	{
		AssertMarkdown("*foo\r\nbar*", () =>
		{
			Paragraph(0..10, () =>
			{
				Emphasis(0..10, () =>
				{
					Literal(1..4, "foo");
					SoftBreak(4..6);
					Literal(6..9, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 强调和加粗可以嵌套在强调内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-405"/>
	[TestMethod]
	public void Test405()
	{
		AssertMarkdown("_foo __bar__ baz_", () =>
		{
			Paragraph(0..17, () =>
			{
				Emphasis(0..17, () =>
				{
					Literal(1..5, "foo ");
					Strong(5..12, () =>
					{
						Literal(7..10, "bar");
					});
					Literal(12..16, " baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-406"/>
	[TestMethod]
	public void Test406()
	{
		AssertMarkdown("_foo _bar_ baz_", () =>
		{
			Paragraph(0..15, () =>
			{
				Emphasis(0..15, () =>
				{
					Literal(1..5, "foo ");
					Emphasis(5..10, () =>
					{
						Literal(6..9, "bar");
					});
					Literal(10..14, " baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-407"/>
	[TestMethod]
	public void Test407()
	{
		AssertMarkdown("__foo_ bar_", () =>
		{
			Paragraph(0..11, () =>
			{
				Emphasis(0..11, () =>
				{
					Emphasis(1..6, () =>
					{
						Literal(2..5, "foo");
					});
					Literal(6..10, " bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-408"/>
	[TestMethod]
	public void Test408()
	{
		AssertMarkdown("*foo *bar**", () =>
		{
			Paragraph(0..11, () =>
			{
				Emphasis(0..11, () =>
				{
					Literal(1..5, "foo ");
					Emphasis(5..10, () =>
					{
						Literal(6..9, "bar");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-409"/>
	[TestMethod]
	public void Test409()
	{
		AssertMarkdown("*foo **bar** baz*", () =>
		{
			Paragraph(0..17, () =>
			{
				Emphasis(0..17, () =>
				{
					Literal(1..5, "foo ");
					Strong(5..12, () =>
					{
						Literal(7..10, "bar");
					});
					Literal(12..16, " baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-410"/>
	[TestMethod]
	public void Test410()
	{
		AssertMarkdown("*foo**bar**baz*", () =>
		{
			Paragraph(0..15, () =>
			{
				Emphasis(0..15, () =>
				{
					Literal(1..4, "foo");
					Strong(4..11, () =>
					{
						Literal(6..9, "bar");
					});
					Literal(11..14, "baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-411"/>
	[TestMethod]
	public void Test411()
	{
		AssertMarkdown("*foo**bar*", () =>
		{
			Paragraph(0..10, () =>
			{
				Emphasis(0..10, () =>
				{
					Literal(1..9, "foo**bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-412"/>
	[TestMethod]
	public void Test412()
	{
		AssertMarkdown("***foo** bar*", () =>
		{
			Paragraph(0..13, () =>
			{
				Emphasis(0..13, () =>
				{
					Strong(1..8, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(8..12, " bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-413"/>
	[TestMethod]
	public void Test413()
	{
		AssertMarkdown("*foo **bar***", () =>
		{
			Paragraph(0..13, () =>
			{
				Emphasis(0..13, () =>
				{
					Literal(1..5, "foo ");
					Strong(5..12, () =>
					{
						Literal(7..10, "bar");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-414"/>
	[TestMethod]
	public void Test414()
	{
		AssertMarkdown("*foo**bar***", () =>
		{
			Paragraph(0..12, () =>
			{
				Emphasis(0..12, () =>
				{
					Literal(1..4, "foo");
					Strong(4..11, () =>
					{
						Literal(6..9, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 如果起始和结束分隔符长度都是 3 的倍数，那么它们可以互相匹配。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-415"/>
	[TestMethod]
	public void Test415()
	{
		AssertMarkdown("foo***bar***baz", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..3, "foo");
				Emphasis(3..12, () =>
				{
					Strong(4..11, () =>
					{
						Literal(6..9, "bar");
					});
				});
				Literal(12..15, "baz");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-416"/>
	[TestMethod]
	public void Test416()
	{
		AssertMarkdown("foo******bar*********baz", () =>
		{
			Paragraph(0..24, () =>
			{
				Literal(0..3, "foo");
				Strong(3..18, () =>
				{
					Strong(5..16, () =>
					{
						Strong(7..14, () =>
						{
							Literal(9..12, "bar");
						});
					});
				});
				Literal(18..24, "***baz");
			});
		});
	}
	/// <summary>
	/// 允许任意层级的嵌套。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-417"/>
	[TestMethod]
	public void Test417()
	{
		AssertMarkdown("*foo **bar *baz* bim** bop*", () =>
		{
			Paragraph(0..27, () =>
			{
				Emphasis(0..27, () =>
				{
					Literal(1..5, "foo ");
					Strong(5..22, () =>
					{
						Literal(7..11, "bar ");
						Emphasis(11..16, () =>
						{
							Literal(12..15, "baz");
						});
						Literal(16..20, " bim");
					});
					Literal(22..26, " bop");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-418"/>
	[TestMethod]
	public void Test418()
	{
		AssertMarkdown("*foo [*bar*](/url)*", () =>
		{
			Paragraph(0..19, () =>
			{
				Emphasis(0..19, () =>
				{
					Literal(1..5, "foo ");
					Link(5..18, "/url", null, () =>
					{
						Emphasis(6..11, () =>
						{
							Literal(7..10, "bar");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 不允许空的强调或加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-419"/>
	[TestMethod]
	public void Test419()
	{
		AssertMarkdown("** is not an empty emphasis", () =>
		{
			Paragraph(0..27, () =>
			{
				Literal(0..27, "** is not an empty emphasis");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-420"/>
	[TestMethod]
	public void Test420()
	{
		AssertMarkdown("**** is not an empty strong emphasis", () =>
		{
			Paragraph(0..36, () =>
			{
				Literal(0..36, "**** is not an empty strong emphasis");
			});
		});
	}
	/// <summary>
	/// 规则 10。
	/// 非空行内元素序列可以作为加粗的内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-421"/>
	[TestMethod]
	public void Test421()
	{
		AssertMarkdown("**foo [bar](/url)**", () =>
		{
			Paragraph(0..19, () =>
			{
				Strong(0..19, () =>
				{
					Literal(2..6, "foo ");
					Link(6..17, "/url", null, () =>
					{
						Literal(7..10, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 非空行内元素序列可以作为加粗的内容。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-422"/>
	[TestMethod]
	public void Test422()
	{
		AssertMarkdown("**foo\r\nbar**", () =>
		{
			Paragraph(0..12, () =>
			{
				Strong(0..12, () =>
				{
					Literal(2..5, "foo");
					SoftBreak(5..7);
					Literal(7..10, "bar");
				});
			});
		});
	}
	/// <summary>
	/// 强调和加粗可以嵌套在加粗内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-423"/>
	[TestMethod]
	public void Test423()
	{
		AssertMarkdown("__foo _bar_ baz__", () =>
		{
			Paragraph(0..17, () =>
			{
				Strong(0..17, () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..11, () =>
					{
						Literal(7..10, "bar");
					});
					Literal(11..15, " baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-424"/>
	[TestMethod]
	public void Test424()
	{
		AssertMarkdown("__foo __bar__ baz__", () =>
		{
			Paragraph(0..19, () =>
			{
				Strong(0..19, () =>
				{
					Literal(2..6, "foo ");
					Strong(6..13, () =>
					{
						Literal(8..11, "bar");
					});
					Literal(13..17, " baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-425"/>
	[TestMethod]
	public void Test425()
	{
		AssertMarkdown("____foo__ bar__", () =>
		{
			Paragraph(0..15, () =>
			{
				Strong(0..15, () =>
				{
					Strong(2..9, () =>
					{
						Literal(4..7, "foo");
					});
					Literal(9..13, " bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-426"/>
	[TestMethod]
	public void Test426()
	{
		AssertMarkdown("**foo **bar****", () =>
		{
			Paragraph(0..15, () =>
			{
				Strong(0..15, () =>
				{
					Literal(2..6, "foo ");
					Strong(6..13, () =>
					{
						Literal(8..11, "bar");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-427"/>
	[TestMethod]
	public void Test427()
	{
		AssertMarkdown("**foo *bar* baz**", () =>
		{
			Paragraph(0..17, () =>
			{
				Strong(0..17, () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..11, () =>
					{
						Literal(7..10, "bar");
					});
					Literal(11..15, " baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-428"/>
	[TestMethod]
	public void Test428()
	{
		AssertMarkdown("**foo*bar*baz**", () =>
		{
			Paragraph(0..15, () =>
			{
				Strong(0..15, () =>
				{
					Literal(2..5, "foo");
					Emphasis(5..10, () =>
					{
						Literal(6..9, "bar");
					});
					Literal(10..13, "baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-429"/>
	[TestMethod]
	public void Test429()
	{
		AssertMarkdown("***foo* bar**", () =>
		{
			Paragraph(0..13, () =>
			{
				Strong(0..13, () =>
				{
					Emphasis(2..7, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(7..11, " bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-430"/>
	[TestMethod]
	public void Test430()
	{
		AssertMarkdown("**foo *bar***", () =>
		{
			Paragraph(0..13, () =>
			{
				Strong(0..13, () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..11, () =>
					{
						Literal(7..10, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 允许任意层级的嵌套。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-431"/>
	[TestMethod]
	public void Test431()
	{
		AssertMarkdown("**foo *bar **baz**\r\nbim* bop**", () =>
		{
			Paragraph(0..30, () =>
			{
				Strong(0..30, () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..24, () =>
					{
						Literal(7..11, "bar ");
						Strong(11..18, () =>
						{
							Literal(13..16, "baz");
						});
						SoftBreak(18..20);
						Literal(20..23, "bim");
					});
					Literal(24..28, " bop");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-432"/>
	[TestMethod]
	public void Test432()
	{
		AssertMarkdown("**foo [*bar*](/url)**", () =>
		{
			Paragraph(0..21, () =>
			{
				Strong(0..21, () =>
				{
					Literal(2..6, "foo ");
					Link(6..19, "/url", null, () =>
					{
						Emphasis(7..12, () =>
						{
							Literal(8..11, "bar");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 不允许空的加粗或加粗。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-433"/>
	[TestMethod]
	public void Test433()
	{
		AssertMarkdown("__ is not an empty emphasis", () =>
		{
			Paragraph(0..27, () =>
			{
				Literal(0..27, "__ is not an empty emphasis");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-434"/>
	[TestMethod]
	public void Test434()
	{
		AssertMarkdown("____ is not an empty strong emphasis", () =>
		{
			Paragraph(0..36, () =>
			{
				Literal(0..36, "____ is not an empty strong emphasis");
			});
		});
	}
	/// <summary>
	/// 规则 11。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-435"/>
	[TestMethod]
	public void Test435()
	{
		AssertMarkdown("foo ***", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "foo ***");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-436"/>
	[TestMethod]
	public void Test436()
	{
		AssertMarkdown("foo *\\**", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..4, "foo ");
				Emphasis(4..8, () =>
				{
					Literal(5..7, "*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-437"/>
	[TestMethod]
	public void Test437()
	{
		AssertMarkdown("foo *_*", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..4, "foo ");
				Emphasis(4..7, () =>
				{
					Literal(5..6, "_");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-438"/>
	[TestMethod]
	public void Test438()
	{
		AssertMarkdown("foo *****", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..9, "foo *****");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-439"/>
	[TestMethod]
	public void Test439()
	{
		AssertMarkdown("foo **\\***", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..4, "foo ");
				Strong(4..10, () =>
				{
					Literal(6..8, "*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-440"/>
	[TestMethod]
	public void Test440()
	{
		AssertMarkdown("foo **_**", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..4, "foo ");
				Strong(4..9, () =>
				{
					Literal(6..7, "_");
				});
			});
		});
	}
	/// <summary>
	/// 在分隔符不匹配时，额外的字符会出现在靠外的位置。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-441"/>
	[TestMethod]
	public void Test441()
	{
		AssertMarkdown("**foo*", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..1, "*");
				Emphasis(1..6, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-442"/>
	[TestMethod]
	public void Test442()
	{
		AssertMarkdown("*foo**", () =>
		{
			Paragraph(0..6, () =>
			{
				Emphasis(0..5, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..6, "*");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-443"/>
	[TestMethod]
	public void Test443()
	{
		AssertMarkdown("***foo**", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..1, "*");
				Strong(1..8, () =>
				{
					Literal(3..6, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-444"/>
	[TestMethod]
	public void Test444()
	{
		AssertMarkdown("****foo*", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "***");
				Emphasis(3..8, () =>
				{
					Literal(4..7, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-445"/>
	[TestMethod]
	public void Test445()
	{
		AssertMarkdown("**foo***", () =>
		{
			Paragraph(0..8, () =>
			{
				Strong(0..7, () =>
				{
					Literal(2..5, "foo");
				});
				Literal(7..8, "*");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-446"/>
	[TestMethod]
	public void Test446()
	{
		AssertMarkdown("*foo****", () =>
		{
			Paragraph(0..8, () =>
			{
				Emphasis(0..5, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..8, "***");
			});
		});
	}
	/// <summary>
	/// 规则 12。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-447"/>
	[TestMethod]
	public void Test447()
	{
		AssertMarkdown("foo ___", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "foo ___");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-448"/>
	[TestMethod]
	public void Test448()
	{
		AssertMarkdown("foo _\\__", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..4, "foo ");
				Emphasis(4..8, () =>
				{
					Literal(5..7, "_");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-449"/>
	[TestMethod]
	public void Test449()
	{
		AssertMarkdown("foo _*_", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..4, "foo ");
				Emphasis(4..7, () =>
				{
					Literal(5..6, "*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-450"/>
	[TestMethod]
	public void Test450()
	{
		AssertMarkdown("foo _____", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..9, "foo _____");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-451"/>
	[TestMethod]
	public void Test451()
	{
		AssertMarkdown("foo __\\___", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..4, "foo ");
				Strong(4..10, () =>
				{
					Literal(6..8, "_");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-452"/>
	[TestMethod]
	public void Test452()
	{
		AssertMarkdown("foo __*__", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..4, "foo ");
				Strong(4..9, () =>
				{
					Literal(6..7, "*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-453"/>
	[TestMethod]
	public void Test453()
	{
		AssertMarkdown("__foo_", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..1, "_");
				Emphasis(1..6, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <summary>
	/// 在分隔符不匹配时，额外的字符会出现在靠外的位置。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-454"/>
	[TestMethod]
	public void Test454()
	{
		AssertMarkdown("_foo__", () =>
		{
			Paragraph(0..6, () =>
			{
				Emphasis(0..5, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..6, "_");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-455"/>
	[TestMethod]
	public void Test455()
	{
		AssertMarkdown("___foo__", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..1, "_");
				Strong(1..8, () =>
				{
					Literal(3..6, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-456"/>
	[TestMethod]
	public void Test456()
	{
		AssertMarkdown("____foo_", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "___");
				Emphasis(3..8, () =>
				{
					Literal(4..7, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-457"/>
	[TestMethod]
	public void Test457()
	{
		AssertMarkdown("__foo___", () =>
		{
			Paragraph(0..8, () =>
			{
				Strong(0..7, () =>
				{
					Literal(2..5, "foo");
				});
				Literal(7..8, "_");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-458"/>
	[TestMethod]
	public void Test458()
	{
		AssertMarkdown("_foo____", () =>
		{
			Paragraph(0..8, () =>
			{
				Emphasis(0..5, () =>
				{
					Literal(1..4, "foo");
				});
				Literal(5..8, "___");
			});
		});
	}
	/// <summary>
	/// 规则 13。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-459"/>
	[TestMethod]
	public void Test459()
	{
		AssertMarkdown("**foo**", () =>
		{
			Paragraph(0..7, () =>
			{
				Strong(0..7, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-460"/>
	[TestMethod]
	public void Test460()
	{
		AssertMarkdown("*_foo_*", () =>
		{
			Paragraph(0..7, () =>
			{
				Emphasis(0..7, () =>
				{
					Emphasis(1..6, () =>
					{
						Literal(2..5, "foo");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-461"/>
	[TestMethod]
	public void Test461()
	{
		AssertMarkdown("__foo__", () =>
		{
			Paragraph(0..7, () =>
			{
				Strong(0..7, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-462"/>
	[TestMethod]
	public void Test462()
	{
		AssertMarkdown("_*foo*_", () =>
		{
			Paragraph(0..7, () =>
			{
				Emphasis(0..7, () =>
				{
					Emphasis(1..6, () =>
					{
						Literal(2..5, "foo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 嵌套在加粗内的加粗可以不切换分隔符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-463"/>
	[TestMethod]
	public void Test463()
	{
		AssertMarkdown("****foo****", () =>
		{
			Paragraph(0..11, () =>
			{
				Strong(0..11, () =>
				{
					Strong(2..9, () =>
					{
						Literal(4..7, "foo");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-464"/>
	[TestMethod]
	public void Test464()
	{
		AssertMarkdown("____foo____", () =>
		{
			Paragraph(0..11, () =>
			{
				Strong(0..11, () =>
				{
					Strong(2..9, () =>
					{
						Literal(4..7, "foo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 还可以适用于更长的分隔符序列。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-465"/>
	[TestMethod]
	public void Test465()
	{
		AssertMarkdown("******foo******", () =>
		{
			Paragraph(0..15, () =>
			{
				Strong(0..15, () =>
				{
					Strong(2..13, () =>
					{
						Strong(4..11, () =>
						{
							Literal(6..9, "foo");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 规则 14。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-466"/>
	[TestMethod]
	public void Test466()
	{
		AssertMarkdown("***foo***", () =>
		{
			Paragraph(0..9, () =>
			{
				Emphasis(0..9, () =>
				{
					Strong(1..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-467"/>
	[TestMethod]
	public void Test467()
	{
		AssertMarkdown("_____foo_____", () =>
		{
			Paragraph(0..13, () =>
			{
				Emphasis(0..13, () =>
				{
					Strong(1..12, () =>
					{
						Strong(3..10, () =>
						{
							Literal(5..8, "foo");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 规则 15。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-468"/>
	[TestMethod]
	public void Test468()
	{
		AssertMarkdown("*foo _bar* baz_", () =>
		{
			Paragraph(0..15, () =>
			{
				Emphasis(0..10, () =>
				{
					Literal(1..9, "foo _bar");
				});
				Literal(10..15, " baz_");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-469"/>
	[TestMethod]
	public void Test469()
	{
		AssertMarkdown("*foo __bar *baz bim__ bam*", () =>
		{
			Paragraph(0..26, () =>
			{
				Emphasis(0..26, () =>
				{
					Literal(1..5, "foo ");
					Strong(5..21, () =>
					{
						Literal(7..19, "bar *baz bim");
					});
					Literal(21..25, " bam");
				});
			});
		});
	}
	/// <summary>
	/// 规则 16。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-470"/>
	[TestMethod]
	public void Test470()
	{
		AssertMarkdown("**foo **bar baz**", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..6, "**foo ");
				Strong(6..17, () =>
				{
					Literal(8..15, "bar baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-471"/>
	[TestMethod]
	public void Test471()
	{
		AssertMarkdown("*foo *bar baz*", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..5, "*foo ");
				Emphasis(5..14, () =>
				{
					Literal(6..13, "bar baz");
				});
			});
		});
	}
	/// <summary>
	/// 规则 17。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-472"/>
	[TestMethod]
	public void Test472()
	{
		AssertMarkdown("*[bar*](/url)", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..1, "*");
				Link(1..13, "/url", null, () =>
				{
					Literal(2..6, "bar*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-473"/>
	[TestMethod]
	public void Test473()
	{
		AssertMarkdown("_foo [bar_](/url)", () =>
		{
			Paragraph(0..17, () =>
			{
				Literal(0..5, "_foo ");
				Link(5..17, "/url", null, () =>
				{
					Literal(6..10, "bar_");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-474"/>
	[TestMethod]
	public void Test474()
	{
		AssertMarkdown("*<img src=\"foo\" title=\"*\"/>", () =>
		{
			Paragraph(0..27, () =>
			{
				Literal(0..1, "*");
				HtmlStartTag(1..27, "<img src=\"foo\" title=\"*\"/>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-475"/>
	[TestMethod]
	public void Test475()
	{
		AssertMarkdown("**<a href=\"**\">", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..2, "**");
				HtmlStartTag(2..15, "<a href=\"**\">");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-476"/>
	[TestMethod]
	public void Test476()
	{
		AssertMarkdown("__<a href=\"__\">", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..2, "__");
				HtmlStartTag(2..15, "<a href=\"__\">");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-477"/>
	[TestMethod]
	public void Test477()
	{
		AssertMarkdown("*a `*`*", () =>
		{
			Paragraph(0..7, () =>
			{
				Emphasis(0..7, () =>
				{
					Literal(1..3, "a ");
					CodeSpan(3..6, "*");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-478"/>
	[TestMethod]
	public void Test478()
	{
		AssertMarkdown("_a `_`_", () =>
		{
			Paragraph(0..7, () =>
			{
				Emphasis(0..7, () =>
				{
					Literal(1..3, "a ");
					CodeSpan(3..6, "_");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-479"/>
	[TestMethod]
	public void Test479()
	{
		AssertMarkdown("**a<http://foo.bar/?q=**>", () =>
		{
			Paragraph(0..25, () =>
			{
				Literal(0..3, "**a");
				Link(3..25, "http://foo.bar/?q=**", null, () =>
				{
					Literal(4..24, "http://foo.bar/?q=**");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-480"/>
	[TestMethod]
	public void Test480()
	{
		AssertMarkdown("__a<http://foo.bar/?q=__>", () =>
		{
			Paragraph(0..25, () =>
			{
				Literal(0..3, "__a");
				Link(3..25, "http://foo.bar/?q=__", null, () =>
				{
					Literal(4..24, "http://foo.bar/?q=__");
				});
			});
		});
	}
}
