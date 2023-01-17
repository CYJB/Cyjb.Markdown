using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 段落的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#paragraphs"/>
[TestClass]
public class UnitTestParagraph : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-219"/>
	[TestMethod]
	public void Test219_1()
	{
		AssertMarkdown("aaa\r\n\r\nbbb", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "aaa");
			});
			Paragraph(7..10, () =>
			{
				Literal(7..10, "bbb");
			});
		});
	}

	[TestMethod]
	public void Test219_2()
	{
		AssertMarkdown("aaa\n\nbbb", () =>
		{
			Paragraph(0..4, () =>
			{
				Literal(0..3, "aaa");
			});
			Paragraph(5..8, () =>
			{
				Literal(5..8, "bbb");
			});
		});
	}

	/// <summary>
	/// 段落可以包含多行，但不包含空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-220"/>
	[TestMethod]
	public void Test220_1()
	{
		AssertMarkdown("aaa\r\nbbb\r\n\r\nccc\r\nddd", () =>
		{
			Paragraph(0..10, () =>
			{
				Literal(0..3, "aaa");
				SoftBreak(3..5);
				Literal(5..8, "bbb");
			});
			Paragraph(12..20, () =>
			{
				Literal(12..15, "ccc");
				SoftBreak(15..17);
				Literal(17..20, "ddd");
			});
		});
	}

	[TestMethod]
	public void Test220_2()
	{
		AssertMarkdown("aaa\nbbb\n\nccc\nddd", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..3, "aaa");
				SoftBreak(3..4);
				Literal(4..7, "bbb");
			});
			Paragraph(9..16, () =>
			{
				Literal(9..12, "ccc");
				SoftBreak(12..13);
				Literal(13..16, "ddd");
			});
		});
	}

	/// <summary>
	/// 段落间的多个空行没有影响。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-221"/>
	[TestMethod]
	public void Test221()
	{
		AssertMarkdown("aaa\r\n\r\n\r\nbbb", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "aaa");
			});
			Paragraph(9..12, () =>
			{
				Literal(9..12, "bbb");
			});
		});
	}

	/// <summary>
	/// 起始空格或 tab 会被跳过。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-222"/>
	[TestMethod]
	public void Test222()
	{
		AssertMarkdown("  aaa\r\n bbb", () =>
		{
			Paragraph(2..11, () =>
			{
				Literal(2..5, "aaa");
				SoftBreak(5..8);
				Literal(8..11, "bbb");
			});
		});
	}

	/// <summary>
	/// 段落的非首行可以包含任意缩进，因为代码段不会中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-223"/>
	[TestMethod]
	public void Test223()
	{
		AssertMarkdown("aaa\r\n             bbb\r\n                                       ccc", () =>
		{
			Paragraph(0..65, () =>
			{
				Literal(0..3, "aaa");
				SoftBreak(3..18);
				Literal(18..21, "bbb");
				SoftBreak(21..62);
				Literal(62..65, "ccc");
			});
		});
	}

	/// <summary>
	/// 段落的首行最多只能包含三个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-224"/>
	[TestMethod]
	public void Test224()
	{
		AssertMarkdown("   aaa\r\nbbb", () =>
		{
			Paragraph(3..11, () =>
			{
				Literal(3..6, "aaa");
				SoftBreak(6..8);
				Literal(8..11, "bbb");
			});
		});
	}

	/// <see href="https://spec.commonmark.org/0.30/#example-225"/>
	[TestMethod]
	public void Test225()
	{
		AssertMarkdown("    aaa\r\nbbb", () =>
		{
			CodeBlock(0..9, "aaa\r\n");
			Paragraph(9..12, () =>
			{
				Literal(9..12, "bbb");
			});
		});
	}

	/// <summary>
	/// 末尾的空格和 Tab 会在行内分析之前就去掉。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-226"/>
	[TestMethod]
	public void Test226()
	{
		AssertMarkdown("aaa     \r\nbbb     ", () =>
		{
			Paragraph(0..18, () =>
			{
				Literal(0..3, "aaa");
				HardBreak(3..10);
				Literal(10..13, "bbb");
			});
		});
	}
}

