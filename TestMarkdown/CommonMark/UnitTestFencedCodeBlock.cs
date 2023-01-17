using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 分隔符代码块的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#fenced-code-blocks"/>
[TestClass]
public class UnitTestFencedCodeBlock : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.30/#example-119"/>
	[TestMethod]
	public void Test119_1()
	{
		AssertMarkdown("```\r\n<\r\n >\r\n```\r\n", () =>
		{
			CodeBlock(0..17, "<\r\n >\r\n");
		});
	}
	[TestMethod]
	public void Test119_2()
	{
		AssertMarkdown("```\n<\n >\n```\n", () =>
		{
			CodeBlock(0..13, "<\n >\n");
		});
	}
	[TestMethod]
	public void Test119_3()
	{
		AssertMarkdown("```\r\n<\r\n >\r\n```", () =>
		{
			CodeBlock(0..15, "<\r\n >\r\n");
		});
	}
	[TestMethod]
	public void Test119_4()
	{
		AssertMarkdown("```\n<\n >\n```", () =>
		{
			CodeBlock(0..12, "<\n >\n");
		});
	}
	/// <summary>
	/// 使用 ~。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-120"/>
	[TestMethod]
	public void Test120()
	{
		AssertMarkdown("~~~\r\n<\r\n >\r\n~~~\r\n", () =>
		{
			CodeBlock(0..17, "<\r\n >\r\n");
		});
	}
	/// <summary>
	/// 分隔符不能少于三个。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-121"/>
	[TestMethod]
	public void Test121()
	{
		AssertMarkdown("``\r\nfoo\r\n``", () =>
		{
			Paragraph(0..11, () =>
			{
				CodeSpan(0..11, "foo");
			});
		});
	}
	/// <summary>
	/// 结束分隔符必须与起始分隔符相同。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-122"/>
	[TestMethod]
	public void Test122()
	{
		AssertMarkdown("```\r\naaa\r\n~~~\r\n```\r\n", () =>
		{
			CodeBlock(0..20, "aaa\r\n~~~\r\n");
		});
	}
	/// <summary>
	/// 结束分隔符必须与起始分隔符相同。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-123"/>
	[TestMethod]
	public void Test123()
	{
		AssertMarkdown("~~~\r\naaa\r\n```\r\n~~~\r\n", () =>
		{
			CodeBlock(0..20, "aaa\r\n```\r\n");
		});
	}
	/// <summary>
	/// 结束分隔符长度要至少是起始分隔符的长度。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-124"/>
	[TestMethod]
	public void Test124()
	{
		AssertMarkdown("````\r\naaa\r\n```\r\n``````\r\n", () =>
		{
			CodeBlock(0..24, "aaa\r\n```\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-125"/>
	[TestMethod]
	public void Test125()
	{
		AssertMarkdown("~~~~\r\naaa\r\n~~~\r\n~~~~\r\n", () =>
		{
			CodeBlock(0..22, "aaa\r\n~~~\r\n");
		});
	}
	/// <summary>
	/// 未闭合的代码块会在文档（或父容器块）结束时闭合。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-126"/>
	[TestMethod]
	public void Test126_1()
	{
		AssertMarkdown("```\r\n", () =>
		{
			CodeBlock(0..5, "");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-126"/>
	[TestMethod]
	public void Test126_2()
	{
		AssertMarkdown("```", () =>
		{
			CodeBlock(0..3, "");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-127"/>
	[TestMethod]
	public void Test127()
	{
		AssertMarkdown("`````\r\n\r\n```\r\naaa\r\n", () =>
		{
			CodeBlock(0..19, "\r\n```\r\naaa\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-128"/>
	[TestMethod]
	public void Test128()
	{
		AssertMarkdown("> ```\r\n> aaa\r\n\r\nbbb\r\n", () =>
		{
			Quote(0..14, () =>
			{
				CodeBlock(2..14, "aaa\r\n");
			});
			Paragraph(16..21, () =>
			{
				Literal(16..19, "bbb");
			});
		});
	}
	/// <summary>
	/// 代码块可以包含空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-129"/>
	[TestMethod]
	public void Test129()
	{
		AssertMarkdown("```\r\n\r\n  \r\n```\r\n", () =>
		{
			CodeBlock(0..16, "\r\n  \r\n");
		});
	}
	/// <summary>
	/// 代码块可以是空的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-130"/>
	[TestMethod]
	public void Test130()
	{
		AssertMarkdown("```\r\n```\r\n", () =>
		{
			CodeBlock(0..10, "");
		});
	}
	/// <summary>
	/// 分隔符可以缩进。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-131"/>
	[TestMethod]
	public void Test131()
	{
		AssertMarkdown(" ```\r\n aaa\r\naaa\r\n```\r\n", () =>
		{
			CodeBlock(0..22, "aaa\r\naaa\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-132"/>
	[TestMethod]
	public void Test132()
	{
		AssertMarkdown("  ```\r\naaa\r\n  aaa\r\naaa\r\n  ```\r\n", () =>
		{
			CodeBlock(0..31, "aaa\r\naaa\r\naaa\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-133"/>
	[TestMethod]
	public void Test133()
	{
		AssertMarkdown("   ```\r\n   aaa\r\n    aaa\r\n  aaa\r\n   ```\r\n", () =>
		{
			CodeBlock(0..40, "aaa\r\n aaa\r\naaa\r\n");
		});
	}
	/// <summary>
	/// 四个空格就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-134"/>
	[TestMethod]
	public void Test134()
	{
		AssertMarkdown("    ```\r\n    aaa\r\n    ```\r\n", () =>
		{
			CodeBlock(0..27, "```\r\naaa\r\n```\r\n");
		});
	}
	/// <summary>
	/// 结束分隔符前可以有最多三个空格，而且缩进不需要与起始分隔符匹配。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-135"/>
	[TestMethod]
	public void Test135()
	{
		AssertMarkdown("```\r\naaa\r\n  ```\r\n", () =>
		{
			CodeBlock(0..17, "aaa\r\n");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-136"/>
	[TestMethod]
	public void Test136()
	{
		AssertMarkdown("   ```\r\naaa\r\n  ```\r\n", () =>
		{
			CodeBlock(0..20, "aaa\r\n");
		});
	}
	/// <summary>
	/// 四个空格的缩进就太多了。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-137"/>
	[TestMethod]
	public void Test137()
	{
		AssertMarkdown("```\r\naaa\r\n    ```\r\n", () =>
		{
			CodeBlock(0..19, "aaa\r\n    ```\r\n");
		});
	}
	/// <summary>
	/// 分隔符内部不能包含空格或 Tab。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-138"/>
	[TestMethod]
	public void Test138()
	{
		AssertMarkdown("``` ```\r\naaa\r\n", () =>
		{
			Paragraph(0..14, () =>
			{
				CodeSpan(0..7, " ");
				SoftBreak(7..9);
				Literal(9..12, "aaa");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-139"/>
	[TestMethod]
	public void Test139()
	{
		AssertMarkdown("~~~~~~\r\naaa\r\n~~~ ~~\r\n", () =>
		{
			CodeBlock(0..21, "aaa\r\n~~~ ~~\r\n");
		});
	}
	/// <summary>
	/// 分隔符代码段可以打断段落，或者直接后跟段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-140"/>
	[TestMethod]
	public void Test140()
	{
		AssertMarkdown("foo\r\n```\r\nbar\r\n```\r\nbaz\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "foo");
			});
			CodeBlock(5..20, "bar\r\n");
			Paragraph(20..25, () =>
			{
				Literal(20..23, "baz");
			});
		});
	}
	/// <summary>
	/// 其它块也可以直接出现在分隔符代码段之前或之后。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-141"/>
	[TestMethod]
	public void Test141()
	{
		AssertMarkdown("foo\r\n---\r\n~~~\r\nbar\r\n~~~\r\n# baz\r\n", () =>
		{
			Heading(0..10, 2, () =>
			{
				Literal(0..3, "foo");
			});
			CodeBlock(10..25, "bar\r\n");
			Heading(25..32, 1, () =>
			{
				Literal(27..30, "baz");
			});
		});
	}
	/// <summary>
	/// 允许提供信息字符串。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-142"/>
	[TestMethod]
	public void Test142()
	{
		AssertMarkdown("```ruby\r\ndef foo(x)\r\n  return 3\r\nend\r\n```\r\n", () =>
		{
			CodeBlock(0..43, "def foo(x)\r\n  return 3\r\nend\r\n", "ruby");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-143"/>
	[TestMethod]
	public void Test143()
	{
		AssertMarkdown("~~~~    ruby startline=3 $%@#$\r\ndef foo(x)\r\n  return 3\r\nend\r\n~~~~~~~\r\n", () =>
		{
			CodeBlock(0..70, "def foo(x)\r\n  return 3\r\nend\r\n", "ruby startline=3 $%@#$");
		});
	}
	/// <see href="https://spec.commonmark.org/0.30/#example-144"/>
	[TestMethod]
	public void Test144()
	{
		AssertMarkdown("````;\r\n````\r\n", () =>
		{
			CodeBlock(0..13, "", ";");
		});
	}
	/// <summary>
	/// ` 之后的信息字符串不能包含 `。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-145"/>
	[TestMethod]
	public void Test145()
	{
		AssertMarkdown("``` aa ```\r\nfoo\r\n", () =>
		{
			Paragraph(0..17, () =>
			{
				CodeSpan(0..10, "aa");
				SoftBreak(10..12);
				Literal(12..15, "foo");
			});
		});
	}
	/// <summary>
	/// ~ 之后的信息字符串可以包含 ` 或 ~。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-146"/>
	[TestMethod]
	public void Test146()
	{
		AssertMarkdown("~~~ aa ``` ~~~\r\nfoo\r\n~~~\r\n", () =>
		{
			CodeBlock(0..26, "foo\r\n", "aa ``` ~~~");
		});
	}
	/// <summary>
	/// 结束分隔符不能包含信息字符串。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-147"/>
	[TestMethod]
	public void Test147()
	{
		AssertMarkdown("```\r\n``` aaa\r\n```\r\n", () =>
		{
			CodeBlock(0..19, "``` aaa\r\n");
		});
	}
}

