using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 扩展自动链接的单元测试。
/// </summary>
[TestClass]
public class UnitTestExtAutolink : BaseTest
{
	/// <summary>
	/// URL 自动链接。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("http://www.commonmark.org\r\n\r\nhttps://www.commonmark.org foo\r\n\r\nwww.commonmark.org<bar\r\n\r\nVisit www.commonmark.org/help for more information.\r\n\r\nmy-http://www.commonmark.org\r\n", () =>
		{
			Paragraph(0..27, () =>
			{
				Link(0..25, "http://www.commonmark.org", null, () =>
				{
					Literal(0..25, "http://www.commonmark.org");
				});
			});
			Paragraph(29..61, () =>
			{
				Link(29..55, "https://www.commonmark.org", null, () =>
				{
					Literal(29..55, "https://www.commonmark.org");
				});
				Literal(55..59, " foo");
			});
			Paragraph(63..87, () =>
			{
				Link(63..81, "http://www.commonmark.org", null, () =>
				{
					Literal(63..81, "www.commonmark.org");
				});
				Literal(81..85, "<bar");
			});
			Paragraph(89..142, () =>
			{
				Literal(89..95, "Visit ");
				Link(95..118, "http://www.commonmark.org/help", null, () =>
				{
					Literal(95..118, "www.commonmark.org/help");
				});
				Literal(118..140, " for more information.");
			});
			Paragraph(144..174, () =>
			{
				Literal(144..172, "my-http://www.commonmark.org");
			});
		});
	}

	/// <summary>
	/// URL 必须包含有效的域名。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("http://commonmark.org\r\n\r\nhttp://www.commonmark._org\r\n\r\nhttp://www\r\n", () =>
		{
			Paragraph(0..23, () =>
			{
				Link(0..21, "http://commonmark.org", null, () =>
				{
					Literal(0..21, "http://commonmark.org");
				});
			});
			Paragraph(25..53, () =>
			{
				Literal(25..51, "http://www.commonmark._org");
			});
			Paragraph(55..67, () =>
			{
				Literal(55..65, "http://www");
			});
		});
	}

	/// <summary>
	/// URL 没有包含协议时，默认使用 <c>http://</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("Visit www.commonmark.org/help for more information.\r\n", () =>
		{
			Paragraph(0..53, () =>
			{
				Literal(0..6, "Visit ");
				Link(6..29, "http://www.commonmark.org/help", null, () =>
				{
					Literal(6..29, "www.commonmark.org/help");
				});
				Literal(29..51, " for more information.");
			});
		});
	}

	/// <summary>
	/// URL 末尾的标点符号不计入自动链接之内，但可以出现在 URL 内部。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("Visit www.commonmark.org.\r\n\r\nVisit www.commonmark.org/a.b.\r\n\r\n访问http://www.commonmark.org/?p=你好,世界。\r\n\r\nhttp://commonmark.org/`bar`\r\n", () =>
		{
			Paragraph(0..27, () =>
			{
				Literal(0..6, "Visit ");
				Link(6..24, "http://www.commonmark.org", null, () =>
				{
					Literal(6..24, "www.commonmark.org");
				});
				Literal(24..25, ".");
			});
			Paragraph(29..60, () =>
			{
				Literal(29..35, "Visit ");
				Link(35..57, "http://www.commonmark.org/a.b", null, () =>
				{
					Literal(35..57, "www.commonmark.org/a.b");
				});
				Literal(57..58, ".");
			});
			Paragraph(62..101, () =>
			{
				Literal(62..64, "访问");
				Link(64..98, "http://www.commonmark.org/?p=你好,世界", null, () =>
				{
					Literal(64..98, "http://www.commonmark.org/?p=你好,世界");
				});
				Literal(98..99, "。");
			});
			Paragraph(103..132, () =>
			{
				Link(103..130, "http://commonmark.org/`bar`", null, () =>
				{
					Literal(103..130, "http://commonmark.org/`bar`");
				});
			});
		});
	}

	/// <summary>
	/// 检查成对的标点符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("www.google.com/search?q=Markup+(business)\r\n\r\nwww.google.com/search?q=([Markup])+(business))]\r\n\r\n(www.google.com/search?q=Markup+(business))\r\n\r\n(www.google.com/search?q=Markup+(business)\r\n\r\nwww.google.com/search?q=Markup+[business]]\r\n\r\n（www.google.com/search?q=Markup+【business】）\r\n", () =>
		{
			Paragraph(0..43, () =>
			{
				Link(0..41, "http://www.google.com/search?q=Markup+(business)", null, () =>
				{
					Literal(0..41, "www.google.com/search?q=Markup+(business)");
				});
			});
			Paragraph(45..94, () =>
			{
				Link(45..90, "http://www.google.com/search?q=([Markup])+(business)", null, () =>
				{
					Literal(45..90, "www.google.com/search?q=([Markup])+(business)");
				});
				Literal(90..92, ")]");
			});
			Paragraph(96..141, () =>
			{
				Literal(96..97, "(");
				Link(97..138, "http://www.google.com/search?q=Markup+(business)", null, () =>
				{
					//               0         1         2         3         4         5         6         7         8         9         10
					//               01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
					Literal(97..138, "www.google.com/search?q=Markup+(business)");
				});
				Literal(138..139, ")");
			});
			Paragraph(143..187, () =>
			{
				Literal(143..144, "(");
				Link(144..185, "http://www.google.com/search?q=Markup+(business)", null, () =>
				{
					//               0         1         2         3         4         5         6         7         8         9         10
					//               01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
					Literal(144..185, "www.google.com/search?q=Markup+(business)");
				});
			});
			Paragraph(189..233, () =>
			{
				Link(189..230, "http://www.google.com/search?q=Markup+[business]", null, () =>
				{
					//                 0         1         2         3         4         5         6         7         8         9         10
					//                 01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
					Literal(189..230, "www.google.com/search?q=Markup+[business]");
				});
				Literal(230..231, "]");
			});
			Paragraph(235..280, () =>
			{
				Literal(235..236, "（");
				Link(236..277, "http://www.google.com/search?q=Markup+【business】", null, () =>
				{
					//                0         1         2         3         4         5         6         7         8         9         10
					//                01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
					Literal(236..277, "www.google.com/search?q=Markup+【business】");
				});
				Literal(277..278, "）");
			});
		});
	}

	/// <summary>
	/// 如果 URL 以分号（<c>;</c>）结尾，会检查它是否可能是 HTML 实体的一部分。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("www.google.com/search?q=commonmark&hl=en\r\n\r\nwww.google.com/search?q=commonmark&hl;\r\n\r\nwww.google.com/search?q=commonmark&;\r\n", () =>
		{
			Paragraph(0..42, () =>
			{
				Link(0..40, "http://www.google.com/search?q=commonmark&hl=en", null, () =>
				{
					Literal(0..40, "www.google.com/search?q=commonmark&hl=en");
				});
			});
			Paragraph(44..84, () =>
			{
				Link(44..78, "http://www.google.com/search?q=commonmark", null, () =>
				{
					Literal(44..78, "www.google.com/search?q=commonmark");
				});
				Literal(78..82, "&hl;");
			});
			Paragraph(86..124, () =>
			{
				Link(86..122, "http://www.google.com/search?q=commonmark&;", null, () =>
				{
					Literal(86..122, "www.google.com/search?q=commonmark&;");
				});
			});
		});
	}

	/// <summary>
	/// 不检查未出现在结尾的标点符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("www.google.com/search?q=(business))+ok\r\n\r\nwww.google.com/search?q=Markup]+(business))\r\n", () =>
		{
			Paragraph(0..40, () =>
			{
				Link(0..38, "http://www.google.com/search?q=(business))+ok", null, () =>
				{
					Literal(0..38, "www.google.com/search?q=(business))+ok");
				});
			});
			Paragraph(42..87, () =>
			{
				Link(42..84, "http://www.google.com/search?q=Markup]+(business)", null, () =>
				{
					Literal(42..84, "www.google.com/search?q=Markup]+(business)");
				});
				Literal(84..85, ")");
			});
		});
	}

	/// <summary>
	/// 会递归的检查标点符号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-8"/>
	[TestMethod]
	public void Test8()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("Visit www.commonmark.org/a.b.,,\r\n\r\nwww.google.com/search?q=Markup+(business)])\r\n\r\nwww.google.com/search?q=Markup+[business]].)&amp;\r\n", () =>
		{
			Paragraph(0..33, () =>
			{
				Literal(0..6, "Visit ");
				Link(6..28, "http://www.commonmark.org/a.b", null, () =>
				{
					Literal(6..28, "www.commonmark.org/a.b");
				});
				Literal(28..31, ".,,");
			});
			Paragraph(35..80, () =>
			{
				Link(35..76, "http://www.google.com/search?q=Markup+(business)", null, () =>
				{
					Literal(35..76, "www.google.com/search?q=Markup+(business)");
				});
				Literal(76..78, "])");
			});
			Paragraph(82..133, () =>
			{
				Link(82..123, "http://www.google.com/search?q=Markup+[business]", null, () =>
				{
					Literal(82..123, "www.google.com/search?q=Markup+[business]");
				});
				Literal(123..131, "].)&");
			});
		});
	}

	/// <summary>
	/// <c>&lt;</c> 会立即结束 URL。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-9"/>
	[TestMethod]
	public void Test9()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("www.commonmark.org/he<lp\r\n", () =>
		{
			Paragraph(0..26, () =>
			{
				Link(0..21, "http://www.commonmark.org/he", null, () =>
				{
					Literal(0..21, "www.commonmark.org/he");
				});
				Literal(21..24, "<lp");
			});
		});
	}

	/// <summary>
	/// 自动链接可以包含 <c>*</c> 等分隔符，除非它们出现在 URL 的末尾。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-10"/>
	[TestMethod]
	public void Test10()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("*http://commonmark.org/*/*\r\n", () =>
		{
			Paragraph(0..28, () =>
			{
				Emphasis(0..26, () =>
				{
					Link(1..25, "http://commonmark.org/*/", null, () =>
					{
						Literal(1..25, "http://commonmark.org/*/");
					});
				});
			});
		});
	}

	/// <summary>
	/// 自动链接不会出现在其它链接内部。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-11"/>
	[TestMethod]
	public void Test11()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("<http://commonmark.org/>\r\n\r\n<a href=\"http://commonmark.org/\">\r\n\r\n[http://commonmark.org/](http://commonmark.org/)\r\n\r\n[foo http://commonmark.org/`bar`\r\n\r\n[foo http://commonmark.org/`bar`](http://commonmark.org/\r\n\r\n[foo [bar](/uri) http://commonmark.org/](/uri)\r\n", () =>
		{
			Paragraph(0..26, () =>
			{
				Link(0..24, "http://commonmark.org/", null, () =>
				{
					Literal(1..23, "http://commonmark.org/");
				});
			});
			HtmlBlock(28..63, "<a href=\"http://commonmark.org/\">\r\n");
			Paragraph(65..115, () =>
			{
				Link(65..113, "http://commonmark.org/", null, () =>
				{
					Literal(66..88, "http://commonmark.org/");
				});
			});
			Paragraph(117..151, () =>
			{
				Literal(117..144, "[foo http://commonmark.org/");
				CodeSpan(144..149, "bar");
			});
			Paragraph(153..211, () =>
			{
				Literal(153..180, "[foo http://commonmark.org/");
				CodeSpan(180..185, "bar");
				Literal(185..187, "](");
				Link(187..209, "http://commonmark.org/", null, () =>
				{
					Literal(187..209, "http://commonmark.org/");
				});
			});
			Paragraph(213..261, () =>
			{
				Literal(213..218, "[foo ");
				Link(218..229, "/uri", null, () =>
				{
					Literal(219..222, "bar");
				});
				Literal(229..230, " ");
				Link(230..259, "http://commonmark.org/](/uri)", null, () =>
				{
					Literal(230..259, "http://commonmark.org/](/uri)");
				});
			});
		});
	}

	/// <summary>
	/// <c>http://</c> 和 <c>https://</c> 链接会保持它们的协议。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-12"/>
	[TestMethod]
	public void Test12()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("http://commonmark.org\r\n\r\n(Visit https://encrypted.google.com/search?q=Markup+(business))\r\n", () =>
		{
			Paragraph(0..23, () =>
			{
				Link(0..21, "http://commonmark.org", null, () =>
				{
					Literal(0..21, "http://commonmark.org");
				});
			});
			Paragraph(25..90, () =>
			{
				Literal(25..32, "(Visit ");
				Link(32..87, "https://encrypted.google.com/search?q=Markup+(business)", null, () =>
				{
					Literal(32..87, "https://encrypted.google.com/search?q=Markup+(business)");
				});
				Literal(87..88, ")");
			});
		});
	}

	/// <summary>
	/// 邮件自动链接。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-13"/>
	[TestMethod]
	public void Test13()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("foo@bar.baz\r\n\r\nmailto:foo.bar@b-a-z.bim\r\n\r\nfoo@bar\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Link(0..11, "mailto:foo@bar.baz", null, () =>
				{
					Literal(0..11, "foo@bar.baz");
				});
			});
			Paragraph(15..41, () =>
			{
				Link(15..39, "mailto:foo.bar@b-a-z.bim", null, () =>
				{
					Literal(15..39, "mailto:foo.bar@b-a-z.bim");
				});
			});
			Paragraph(43..52, () =>
			{
				Literal(43..50, "foo@bar");
			});
		});
	}

	/// <summary>
	/// <c>+</c> 只能出现在 <c>@</c> 之前。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-14"/>
	[TestMethod]
	public void Test14()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("hello@mail+xyz.example isn't valid, but hello+xyz@mail.example is.\r\n", () =>
		{
			Paragraph(0..68, () =>
			{
				Literal(0..40, "hello@mail+xyz.example isn't valid, but ");
				Link(40..62, "mailto:hello+xyz@mail.example", null, () =>
				{
					Literal(40..62, "hello+xyz@mail.example");
				});
				Literal(62..66, " is.");
			});
		});
	}

	/// <summary>
	/// 邮件链接可以包含 <c>.</c>、<c>-</c> 和 <c>_</c> 等符号，会忽略链接末尾的其它字符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-15"/>
	[TestMethod]
	public void Test15()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("a.b-c_d@a.b\r\n\r\na.b-c_d@a.b.\r\n\r\na.b-c_d@a.b-\r\n\r\na.b-c_d@a.b_\r\n\r\na.b-c_d@a.b其它\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Link(0..11, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(0..11, "a.b-c_d@a.b");
				});
			});
			Paragraph(15..29, () =>
			{
				Link(15..26, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(15..26, "a.b-c_d@a.b");
				});
				Literal(26..27, ".");
			});
			Paragraph(31..45, () =>
			{
				Literal(31..43, "a.b-c_d@a.b-");
			});
			Paragraph(47..61, () =>
			{
				Link(47..58, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(47..58, "a.b-c_d@a.b");
				});
				Literal(58..59, "_");
			});
			Paragraph(63..78, () =>
			{
				Link(63..74, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(63..74, "a.b-c_d@a.b");
				});
				Literal(74..76, "其它");
			});
		});
	}

	/// <summary>
	/// 允许包含 <c>mailto:</c> 协议，但不支持 <c>xmpp:</c> 协议。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-16"/>
	[TestMethod]
	public void Test16()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("mailto:foo@bar.baz\r\n\r\nmailto:a.b-c_d@a.b\r\n\r\nmailto:a.b-c_d@a.b.\r\n\r\nmailto:a.b-c_d@a.b/\r\n\r\nmailto:a.b-c_d@a.b-\r\n\r\nmailto:a.b-c_d@a.b_\r\n\r\nxmpp:foo@bar.baz\r\n", () =>
		{
			Paragraph(0..20, () =>
			{
				Link(0..18, "mailto:foo@bar.baz", null, () =>
				{
					Literal(0..18, "mailto:foo@bar.baz");
				});
			});
			Paragraph(22..42, () =>
			{
				Link(22..40, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(22..40, "mailto:a.b-c_d@a.b");
				});
			});
			Paragraph(44..65, () =>
			{
				Link(44..62, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(44..62, "mailto:a.b-c_d@a.b");
				});
				Literal(62..63, ".");
			});
			Paragraph(67..88, () =>
			{
				Link(67..85, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(67..85, "mailto:a.b-c_d@a.b");
				});
				Literal(85..86, "/");
			});
			Paragraph(90..111, () =>
			{
				Literal(90..109, "mailto:a.b-c_d@a.b-");
			});
			Paragraph(113..134, () =>
			{
				Link(113..131, "mailto:a.b-c_d@a.b", null, () =>
				{
					Literal(113..131, "mailto:a.b-c_d@a.b");
				});
				Literal(131..132, "_");
			});
			Paragraph(136..154, () =>
			{
				Literal(136..152, "xmpp:foo@bar.baz");
			});
		});
	}

	/// <summary>
	/// 邮件自动链接不会出现在其它链接内部。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md#example-17"/>
	[TestMethod]
	public void Test17()
	{
		//              0         1         2         3         4         5         6         7         8         9         10
		//              01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
		AssertMarkdown("<foo@bar.baz>\r\n\r\n<a href=\"mailto:foo@bar.baz\">\r\n\r\n[foo@bar.baz](mailto:biz@boo.bim)\r\n\r\n[foo foo@bar.baz`bar`\r\n\r\n[foo foo@bar.baz`bar`](mailto:biz@boo.bim\r\n\r\n[foo [bar](/uri) foo@bar.baz](/uri)\r\n", () =>
		{
			Paragraph(0..15, () =>
			{
				Link(0..13, "mailto:foo@bar.baz", null, () =>
				{
					Literal(1..12, "foo@bar.baz");
				});
			});
			HtmlBlock(17..48, "<a href=\"mailto:foo@bar.baz\">\r\n");
			Paragraph(50..85, () =>
			{
				Link(50..83, "mailto:biz@boo.bim", null, () =>
				{
					Literal(51..62, "foo@bar.baz");
				});
			});
			Paragraph(87..110, () =>
			{
				Literal(87..103, "[foo foo@bar.baz");
				CodeSpan(103..108, "bar");
			});
			Paragraph(112..155, () =>
			{
				Literal(112..128, "[foo foo@bar.baz");
				CodeSpan(128..133, "bar");
				Literal(133..135, "](");
				Link(135..153, "mailto:biz@boo.bim", null, () =>
				{
					Literal(135..153, "mailto:biz@boo.bim");
				});
			});
			Paragraph(157..194, () =>
			{
				Literal(157..162, "[foo ");
				Link(162..173, "/uri", null, () =>
				{
					Literal(163..166, "bar");
				});
				Literal(173..174, " ");
				Link(174..185, "mailto:foo@bar.baz", null, () =>
				{
					Literal(174..185, "foo@bar.baz");
				});
				Literal(185..192, "](/uri)");
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持扩展自动链接。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("http://www.commonmark.org\r\nfoo@bar.baz\r\n", () =>
		{
			Paragraph(0..40, () =>
			{
				Literal(0..25, "http://www.commonmark.org");
				SoftBreak(25..27);
				Literal(27..38, "foo@bar.baz");
			});
		});
	}
}

