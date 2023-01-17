using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 自动链接的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#autolinks"/>
[TestClass]
public class UnitTestAutolink : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-593"/>
	[TestMethod]
	public void Test593()
	{
		AssertMarkdown("<http://foo.bar.baz>", () =>
		{
			Paragraph(0..20, () =>
			{
				Link(0..20, "http://foo.bar.baz", null, () =>
				{
					Literal(1..19, "http://foo.bar.baz");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-594"/>
	[TestMethod]
	public void Test594()
	{
		AssertMarkdown("<http://foo.bar.baz/test?q=hello&id=22&boolean>", () =>
		{
			Paragraph(0..47, () =>
			{
				Link(0..47, "http://foo.bar.baz/test?q=hello&id=22&boolean", null, () =>
				{
					Literal(1..46, "http://foo.bar.baz/test?q=hello&id=22&boolean");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-595"/>
	[TestMethod]
	public void Test595()
	{
		AssertMarkdown("<irc://foo.bar:2233/baz>", () =>
		{
			Paragraph(0..24, () =>
			{
				Link(0..24, "irc://foo.bar:2233/baz", null, () =>
				{
					Literal(1..23, "irc://foo.bar:2233/baz");
				});
			});
		});
	}
	/// <summary>
	/// 支持大写字符。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-596"/>
	[TestMethod]
	public void Test596()
	{
		AssertMarkdown("<MAILTO:FOO@BAR.BAZ>", () =>
		{
			Paragraph(0..20, () =>
			{
				Link(0..20, "MAILTO:FOO@BAR.BAZ", null, () =>
				{
					Literal(1..19, "MAILTO:FOO@BAR.BAZ");
				});
			});
		});
	}
	/// <summary>
	/// 不要求 URI 规范。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-597"/>
	[TestMethod]
	public void Test597()
	{
		AssertMarkdown("<a+b+c:d>", () =>
		{
			Paragraph(0..9, () =>
			{
				Link(0..9, "a+b+c:d", null, () =>
				{
					Literal(1..8, "a+b+c:d");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-598"/>
	[TestMethod]
	public void Test598()
	{
		AssertMarkdown("<made-up-scheme://foo,bar>", () =>
		{
			Paragraph(0..26, () =>
			{
				Link(0..26, "made-up-scheme://foo,bar", null, () =>
				{
					Literal(1..25, "made-up-scheme://foo,bar");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-599"/>
	[TestMethod]
	public void Test599()
	{
		AssertMarkdown("<http://../>", () =>
		{
			Paragraph(0..12, () =>
			{
				Link(0..12, "http://../", null, () =>
				{
					Literal(1..11, "http://../");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-600"/>
	[TestMethod]
	public void Test600()
	{
		AssertMarkdown("<localhost:5001/foo>", () =>
		{
			Paragraph(0..20, () =>
			{
				Link(0..20, "localhost:5001/foo", null, () =>
				{
					Literal(1..19, "localhost:5001/foo");
				});
			});
		});
	}
	/// <summary>
	/// 不允许出现空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-601"/>
	[TestMethod]
	public void Test601()
	{
		AssertMarkdown("<http://foo.bar/baz bim>", () =>
		{
			Paragraph(0..24, () =>
			{
				Literal(0..24, "<http://foo.bar/baz bim>");
			});
		});
	}
	/// <summary>
	/// 反斜杠转义不生效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-602"/>
	[TestMethod]
	public void Test602()
	{
		AssertMarkdown(@"<http://example.com/\[\>", () =>
		{
			Paragraph(0..24, () =>
			{
				Link(0..24, @"http://example.com/\[\", null, () =>
				{
					Literal(1..23, @"http://example.com/\[\");
				});
			});
		});
	}
	/// <summary>
	/// 邮件自动链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-603"/>
	[TestMethod]
	public void Test603()
	{
		AssertMarkdown("<foo@bar.example.com>", () =>
		{
			Paragraph(0..21, () =>
			{
				Link(0..21, "mailto:foo@bar.example.com", null, () =>
				{
					Literal(1..20, "foo@bar.example.com");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-604"/>
	[TestMethod]
	public void Test604()
	{
		AssertMarkdown("<foo+special@Bar.baz-bar0.com>", () =>
		{
			Paragraph(0..30, () =>
			{
				Link(0..30, "mailto:foo+special@Bar.baz-bar0.com", null, () =>
				{
					Literal(1..29, "foo+special@Bar.baz-bar0.com");
				});
			});
		});
	}
	/// <summary>
	/// 反斜杠转义不生效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-605"/>
	[TestMethod]
	public void Test605()
	{
		AssertMarkdown(@"<foo\+@bar.example.com>", () =>
		{
			Paragraph(0..23, () =>
			{
				Literal(0..23, "<foo+@bar.example.com>");
			});
		});
	}
	/// <summary>
	/// 非自动链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-606"/>
	[TestMethod]
	public void Test606()
	{
		AssertMarkdown("<>", () =>
		{
			Paragraph(0..2, () =>
			{
				Literal(0..2, "<>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-607"/>
	[TestMethod]
	public void Test607()
	{
		AssertMarkdown("< http://foo.bar >", () =>
		{
			Paragraph(0..18, () =>
			{
				Literal(0..18, "< http://foo.bar >");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-608"/>
	[TestMethod]
	public void Test608()
	{
		AssertMarkdown("<m:abc>", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "<m:abc>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-609"/>
	[TestMethod]
	public void Test609()
	{
		AssertMarkdown(@"<foo.bar.baz>", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..13, "<foo.bar.baz>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-610"/>
	[TestMethod]
	public void Test610()
	{
		AssertMarkdown("http://example.com", () =>
		{
			Paragraph(0..18, () =>
			{
				Literal(0..18, "http://example.com");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-611"/>
	[TestMethod]
	public void Test611()
	{
		AssertMarkdown("foo@bar.example.com", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..19, "foo@bar.example.com");
			});
		});
	}
}

