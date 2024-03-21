using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 自动链接的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#autolinks"/>
[TestClass]
public class UnitTestAutolink : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-594"/>
	[TestMethod]
	public void Test594()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-595"/>
	[TestMethod]
	public void Test595()
	{
		AssertMarkdown("<https://foo.bar.baz/test?q=hello&id=22&boolean>", () =>
		{
			Paragraph(0..48, () =>
			{
				Link(0..48, "https://foo.bar.baz/test?q=hello&id=22&boolean", null, () =>
				{
					Literal(1..47, "https://foo.bar.baz/test?q=hello&id=22&boolean");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-596"/>
	[TestMethod]
	public void Test596()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-597"/>
	[TestMethod]
	public void Test597()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-598"/>
	[TestMethod]
	public void Test598()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-599"/>
	[TestMethod]
	public void Test599()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-600"/>
	[TestMethod]
	public void Test600()
	{
		AssertMarkdown("<https://../>", () =>
		{
			Paragraph(0..13, () =>
			{
				Link(0..13, "https://../", null, () =>
				{
					Literal(1..12, "https://../");
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-601"/>
	[TestMethod]
	public void Test601()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-602"/>
	[TestMethod]
	public void Test602()
	{
		AssertCommonMark("<https://foo.bar/baz bim>", () =>
		{
			Paragraph(0..25, () =>
			{
				Literal(0..25, "<https://foo.bar/baz bim>");
			});
		});
	}
	/// <summary>
	/// 反斜杠转义不生效。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-603"/>
	[TestMethod]
	public void Test603()
	{
		AssertMarkdown(@"<https://example.com/\[\>", () =>
		{
			Paragraph(0..25, () =>
			{
				Link(0..25, @"https://example.com/\[\", null, () =>
				{
					Literal(1..24, @"https://example.com/\[\");
				});
			});
		});
	}
	/// <summary>
	/// 邮件自动链接。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-604"/>
	[TestMethod]
	public void Test604()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-605"/>
	[TestMethod]
	public void Test605()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-606"/>
	[TestMethod]
	public void Test606()
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
	/// <see href="https://spec.commonmark.org/0.31.2/#example-607"/>
	[TestMethod]
	public void Test607()
	{
		AssertMarkdown("<>", () =>
		{
			Paragraph(0..2, () =>
			{
				Literal(0..2, "<>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-608"/>
	[TestMethod]
	public void Test608()
	{
		AssertCommonMark("< https://foo.bar >", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..19, "< https://foo.bar >");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-609"/>
	[TestMethod]
	public void Test609()
	{
		AssertMarkdown("<m:abc>", () =>
		{
			Paragraph(0..7, () =>
			{
				Literal(0..7, "<m:abc>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-610"/>
	[TestMethod]
	public void Test610()
	{
		AssertMarkdown(@"<foo.bar.baz>", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..13, "<foo.bar.baz>");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-611"/>
	[TestMethod]
	public void Test611()
	{
		AssertCommonMark("https://example.com", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..19, "https://example.com");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-612"/>
	[TestMethod]
	public void Test612()
	{
		AssertCommonMark("foo@bar.example.com", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..19, "foo@bar.example.com");
			});
		});
	}
}

