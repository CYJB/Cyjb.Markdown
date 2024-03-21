using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 图片的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#images"/>
[TestClass]
public partial class UnitTestImage : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-572"/>
	[TestMethod]
	public void Test572()
	{
		AssertMarkdown("![foo](/url \"title\")", () =>
		{
			Paragraph(0..20, () =>
			{
				Image(0..20, "/url", "title", () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-573"/>
	[TestMethod]
	public void Test573()
	{
		AssertMarkdown("![foo *bar*]\r\n\r\n[foo *bar*]: train.jpg \"train & tracks\"", () =>
		{
			Paragraph(0..14, () =>
			{
				Image(0..12, "train.jpg", "train & tracks", () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..11, () =>
					{
						Literal(7..10, "bar");
					});
				});
			});
			LinkDefinition(16..55, "foo *bar*", "train.jpg", "train & tracks");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-574"/>
	[TestMethod]
	public void Test574()
	{
		AssertMarkdown("![foo ![bar](/url)](/url2)", () =>
		{
			Paragraph(0..26, () =>
			{
				Image(0..26, "/url2", null, () =>
				{
					Literal(2..6, "foo ");
					Image(6..18, "/url", null, () =>
					{
						Literal(8..11, "bar");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-575"/>
	[TestMethod]
	public void Test575()
	{
		AssertMarkdown("![foo [bar](/url)](/url2)", () =>
		{
			Paragraph(0..25, () =>
			{
				Image(0..25, "/url2", null, () =>
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-576"/>
	[TestMethod]
	public void Test576()
	{
		AssertMarkdown("![foo *bar*][]\r\n\r\n[foo *bar*]: train.jpg \"train & tracks\"", () =>
		{
			Paragraph(0..16, () =>
			{
				Image(0..14, "train.jpg", "train & tracks", () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..11, () =>
					{
						Literal(7..10, "bar");
					});
				});
			});
			LinkDefinition(18..57, "foo *bar*", "train.jpg", "train & tracks");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-577"/>
	[TestMethod]
	public void Test577()
	{
		AssertMarkdown("![foo *bar*][foobar]\r\n\r\n[FOOBAR]: train.jpg \"train & tracks\"", () =>
		{
			Paragraph(0..22, () =>
			{
				Image(0..20, "train.jpg", "train & tracks", () =>
				{
					Literal(2..6, "foo ");
					Emphasis(6..11, () =>
					{
						Literal(7..10, "bar");
					});
				});
			});
			LinkDefinition(24..60, "FOOBAR", "train.jpg", "train & tracks");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-578"/>
	[TestMethod]
	public void Test578()
	{
		AssertMarkdown("![foo](train.jpg)", () =>
		{
			Paragraph(0..17, () =>
			{
				Image(0..17, "train.jpg", null, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-579"/>
	[TestMethod]
	public void Test579()
	{
		AssertMarkdown("My ![foo bar](/path/to/train.jpg  \"title\"   )", () =>
		{
			Paragraph(0..45, () =>
			{
				Literal(0..3, "My ");
				Image(3..45, "/path/to/train.jpg", "title", () =>
				{
					Literal(5..12, "foo bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-580"/>
	[TestMethod]
	public void Test580()
	{
		AssertMarkdown("![foo](<url>)", () =>
		{
			Paragraph(0..13, () =>
			{
				Image(0..13, "url", null, () =>
				{
					Literal(2..5, "foo");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-581"/>
	[TestMethod]
	public void Test581()
	{
		AssertMarkdown("![](/url)", () =>
		{
			Paragraph(0..9, () =>
			{
				Image(0..9, "/url", null);
			});
		});
	}
	/// <summary>
	/// 图片引用。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-582"/>
	[TestMethod]
	public void Test582()
	{
		AssertMarkdown("![foo][bar]\r\n\r\n[bar]: /url", () =>
		{
			Paragraph(0..13, () =>
			{
				Image(0..11, "/url", null, () =>
				{
					Literal(2..5, "foo");
				});
			});
			LinkDefinition(15..26, "bar", "/url", null);
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-583"/>
	[TestMethod]
	public void Test583()
	{
		AssertMarkdown("![foo][bar]\r\n\r\n[BAR]: /url", () =>
		{
			Paragraph(0..13, () =>
			{
				Image(0..11, "/url", null, () =>
				{
					Literal(2..5, "foo");
				});
			});
			LinkDefinition(15..26, "BAR", "/url", null);
		});
	}
	/// <summary>
	/// 折叠。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-584"/>
	[TestMethod]
	public void Test584()
	{
		AssertMarkdown("![foo][]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..10, () =>
			{
				Image(0..8, "/url", "title", () =>
				{
					Literal(2..5, "foo");
				});
			});
			LinkDefinition(12..31, "foo", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-585"/>
	[TestMethod]
	public void Test585()
	{
		AssertMarkdown("![*foo* bar][]\r\n\r\n[*foo* bar]: /url \"title\"", () =>
		{
			Paragraph(0..16, () =>
			{
				Image(0..14, "/url", "title", () =>
				{
					Emphasis(2..7, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(7..11, " bar");
				});
			});
			LinkDefinition(18..43, "*foo* bar", "/url", "title");
		});
	}
	/// <summary>
	/// 链接标签是忽略大小写的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-586"/>
	[TestMethod]
	public void Test586()
	{
		AssertMarkdown("![Foo][]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..10, () =>
			{
				Image(0..8, "/url", "title", () =>
				{
					Literal(2..5, "Foo");
				});
			});
			LinkDefinition(12..31, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 方括号间不能存在空格、Tab 或换行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-587"/>
	[TestMethod]
	public void Test587()
	{
		AssertMarkdown("![foo] \r\n[]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..13, () =>
			{
				Image(0..6, "/url", "title", () =>
				{
					Literal(2..5, "foo");
				});
				SoftBreak(6..9);
				Literal(9..11, "[]");
			});
			LinkDefinition(15..34, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 简写。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-588"/>
	[TestMethod]
	public void Test588()
	{
		AssertMarkdown("![foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..8, () =>
			{
				Image(0..6, "/url", "title", () =>
				{
					Literal(2..5, "foo");
				});
			});
			LinkDefinition(10..29, "foo", "/url", "title");
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-589"/>
	[TestMethod]
	public void Test589()
	{
		AssertMarkdown("![*foo* bar]\r\n\r\n[*foo* bar]: /url \"title\"", () =>
		{
			Paragraph(0..14, () =>
			{
				Image(0..12, "/url", "title", () =>
				{
					Emphasis(2..7, () =>
					{
						Literal(3..6, "foo");
					});
					Literal(7..11, " bar");
				});
			});
			LinkDefinition(16..41, "*foo* bar", "/url", "title");
		});
	}
	/// <summary>
	/// 连接标签不能包含未转义的方括号。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-590"/>
	[TestMethod]
	public void Test590()
	{
		AssertMarkdown("![[foo]]\r\n\r\n[[foo]]: /url \"title\"", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..8, "![[foo]]");
			});
			Paragraph(12..33, () =>
			{
				Literal(12..33, "[[foo]]: /url \"title\"");
			});
		});
	}
	/// <summary>
	/// 链接标签是忽略大小写的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-591"/>
	[TestMethod]
	public void Test591()
	{
		AssertMarkdown("![Foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..8, () =>
			{
				Image(0..6, "/url", "title", () =>
				{
					Literal(2..5, "Foo");
				});
			});
			LinkDefinition(10..29, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 转义起始 [。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-592"/>
	[TestMethod]
	public void Test592()
	{
		AssertMarkdown("!\\[foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..7, "![foo]");
			});
			LinkDefinition(11..30, "foo", "/url", "title");
		});
	}
	/// <summary>
	/// 转义 !。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-593"/>
	[TestMethod]
	public void Test593()
	{
		AssertMarkdown("\\![foo]\r\n\r\n[foo]: /url \"title\"", () =>
		{
			Paragraph(0..9, () =>
			{
				Literal(0..2, "!");
				Link(2..7, "/url", "title", () =>
				{
					Literal(3..6, "foo");
				});
			});
			LinkDefinition(11..30, "foo", "/url", "title");
		});
	}
}
