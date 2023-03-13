using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 缩进代码块的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#indented-code-blocks"/>
[TestClass]
public partial class UnitTestIndentedCodeBlock : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-107"/>
	[TestMethod]
	public void Test107_1()
	{
		AssertMarkdown("    a simple\r\n      indented code block\r\n", () =>
		{
			CodeBlock(0..41, "a simple\r\n  indented code block\r\n");
		});
	}
	[TestMethod]
	public void Test107_2()
	{
		AssertMarkdown("    a simple\n      indented code block\n", () =>
		{
			CodeBlock(0..39, "a simple\n  indented code block\n");
		});
	}
	[TestMethod]
	public void Test107_3()
	{
		AssertMarkdown("    a simple\r\n      indented code block", () =>
		{
			CodeBlock(0..39, "a simple\r\n  indented code block");
		});
	}
	[TestMethod]
	public void Test107_4()
	{
		AssertMarkdown("    a simple\n      indented code block", () =>
		{
			CodeBlock(0..38, "a simple\n  indented code block");
		});
	}
	/// <summary>
	/// 列表项优先级更高
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-108"/>
	[TestMethod]
	public void Test108()
	{
		AssertMarkdown(" - foo\r\n\r\n    bar\r\n", () =>
		{
			UnorderedList(1..19, true, () =>
			{
				ListItem(1..19, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
					Paragraph(14..19, () =>
					{
						Literal(14..17, "bar");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-109"/>
	[TestMethod]
	public void Test109()
	{
		AssertMarkdown("1.  foo\r\n\r\n    - bar", () =>
		{
			OrderedNumberList(0..20, true, 1, () =>
			{
				ListItem(0..20, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
					UnorderedList(15..20, false, () =>
					{
						ListItem(15..20, () =>
						{
							Paragraph(17..20, () =>
							{
								Literal(17..20, "bar");
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 代码块的内容是普通文本，不会按照 Markdown 解析。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-110"/>
	[TestMethod]
	public void Test110()
	{
		AssertMarkdown("    <a/>\r\n    *hi*\r\n\r\n    - one\r\n", () =>
		{
			CodeBlock(0..33, "<a/>\r\n*hi*\r\n\r\n- one\r\n");
		});
	}
	/// <summary>
	/// 三个空行分隔的块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-111"/>
	[TestMethod]
	public void Test111_1()
	{
		AssertMarkdown("    chunk1\r\n\r\n    chunk2\r\n  \r\n \r\n \r\n    chunk3\r\n", () =>
		{
			CodeBlock(0..48, "chunk1\r\n\r\nchunk2\r\n\r\n\r\n\r\nchunk3\r\n");
		});
	}
	[TestMethod]
	public void Test111_2()
	{
		AssertMarkdown("    chunk1\n\n    chunk2\n  \n \n \n    chunk3\n", () =>
		{
			CodeBlock(0..41, "chunk1\n\nchunk2\n\n\n\nchunk3\n");
		});
	}
	[TestMethod]
	public void Test111_3()
	{
		AssertMarkdown("    chunk1\r\n\r\n    chunk2\r\n  \r\n \r\n \r\n    chunk3", () =>
		{
			CodeBlock(0..46, "chunk1\r\n\r\nchunk2\r\n\r\n\r\n\r\nchunk3");
		});
	}
	[TestMethod]
	public void Test111_4()
	{
		AssertMarkdown("    chunk1\n\n    chunk2\n  \n \n \n    chunk3", () =>
		{
			CodeBlock(0..40, "chunk1\n\nchunk2\n\n\n\nchunk3");
		});
	}
	/// <summary>
	/// 4 个空格之后的部分都会包含在内容当中。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-112"/>
	[TestMethod]
	public void Test112()
	{
		AssertMarkdown("    chunk1\r\n      \r\n      chunk2\r\n", () =>
		{
			CodeBlock(0..34, "chunk1\r\n  \r\n  chunk2\r\n");
		});
	}
	/// <summary>
	/// 缩进代码块不能中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-113"/>
	[TestMethod]
	public void Test113()
	{
		AssertMarkdown("Foo\r\n    bar\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				Literal(0..3, "Foo");
				SoftBreak(3..9);
				Literal(9..12, "bar");
			});
		});
	}
	/// <summary>
	/// 任何缩进小于 4 的非空行都会立即结束代码块。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-114"/>
	[TestMethod]
	public void Test114()
	{
		AssertMarkdown("    foo\r\nbar", () =>
		{
			CodeBlock(0..9, "foo\r\n");
			Paragraph(9..12, () =>
			{
				Literal(9..12, "bar");
			});
		});
	}
	/// <summary>
	/// 缩进代码块可以出现在其它块类型的前后。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-115"/>
	[TestMethod]
	public void Test115()
	{
		AssertCommonMark("# Heading\r\n    foo\r\nHeading\r\n------\r\n    foo\r\n----", () =>
		{
			Heading(0..11, 1, () =>
			{
				Literal(2..9, "Heading");
			});
			CodeBlock(11..20, "foo\r\n");
			Heading(20..37, 2, () =>
			{
				Literal(20..27, "Heading");
			});
			CodeBlock(37..46, "foo\r\n");
			ThematicBreak(46..50);
		});
	}
	/// <summary>
	/// 首行可以包含多于 4 个空格。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-116"/>
	[TestMethod]
	public void Test116()
	{
		AssertMarkdown("        foo\r\n    bar\r\n", () =>
		{
			CodeBlock(0..22, "    foo\r\nbar\r\n");
		});
	}
	/// <summary>
	/// 缩进代码块前后的空行不会包含在内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-117"/>
	[TestMethod]
	public void Test117()
	{
		AssertMarkdown("\r\n    \r\n    foo\r\n    \r\n", () =>
		{
			CodeBlock(8..23, "foo\r\n");
		});
	}
	/// <summary>
	/// 末尾的空格或 Tab 会被包含在代码块的内容中。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-118"/>
	[TestMethod]
	public void Test118()
	{
		AssertMarkdown("    foo  \r\n", () =>
		{
			CodeBlock(0..11, "foo  \r\n");
		});
	}
}

