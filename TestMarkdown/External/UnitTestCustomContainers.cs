using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 自定义容器的单元测试。
/// </summary>
[TestClass]
public class UnitTestCustomContainers : BaseTest
{
	/// <summary>
	/// 自定义容器可以生成 <c>&lt;div&gt;...&lt;/div&gt;</c> 结构，内部可以包含其它 Markdown 节点。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown(":::\r\nfoo *bar*\r\n\r\nbaz\r\n:::\r\n", () =>
		{
			CustomContainer(0..28, null, () =>
			{
				Paragraph(5..16, () =>
				{
					Literal(5..9, "foo ");
					Emphasis(9..14, () =>
					{
						Literal(10..13, "bar");
					});
				});
				Paragraph(18..23, () =>
				{
					Literal(18..21, "baz");
				});
			});
		});
	}

	/// <summary>
	/// 少于三个 <c>:</c> 是无效的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("::\r\nfoo\r\n::\r\n", () =>
		{
			Paragraph(0..13, () =>
			{
				Literal(0..2, "::");
				SoftBreak(2..4);
				Literal(4..7, "foo");
				SoftBreak(7..9);
				Literal(9..11, "::");
			});
		});
	}

	/// <summary>
	/// 自定义容器允许嵌套，但要求内部分隔符的长度短于外部分隔符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("::::: type-1\r\nfoo\r\n:::\r\nbar\r\n:::\r\nbaz\r\n:::::\r\n\r\n:::\r\nfoo\r\n::::::: type-2\r\nbar\r\n:::::::\r\nbaz\r\n", () =>
		{
			CustomContainer(0..46, "type-1", () =>
			{
				Paragraph(14..19, () =>
				{
					Literal(14..17, "foo");
				});
				CustomContainer(19..34, null, () =>
				{
					Paragraph(24..29, () =>
					{
						Literal(24..27, "bar");
					});
				});
				Paragraph(34..39, () =>
				{
					Literal(34..37, "baz");
				});
			});
			CustomContainer(48..88, null, () =>
			{
				Paragraph(53..58, () =>
				{
					Literal(53..56, "foo");
				});
				CustomContainer(58..79, "type-2", () =>
				{
					Paragraph(74..79, () =>
					{
						Literal(74..77, "bar");
					});
				});
			});
			Paragraph(88..93, () =>
			{
				Literal(88..91, "baz");
			});
		});
	}

	/// <summary>
	/// 未闭合的自定义容器会在文档结束（或父容器结束）时闭合。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown(":::\r\n", () =>
		{
			CustomContainer(0..5);
		});
	}

	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown(":::::\r\n\r\n:::\r\nfoo\r\n", () =>
		{
			CustomContainer(0..19, null, () =>
			{
				CustomContainer(9..19, null, () =>
				{
					Paragraph(14..19, () =>
					{
						Literal(14..17, "foo");
					});
				});
			});
		});
	}

	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("> :::::\r\n> foo\r\n:::\r\nbar\r\n", () =>
		{
			Blockquote(0..16, () =>
			{
				CustomContainer(2..16, null, () =>
				{
					Paragraph(11..16, () =>
					{
						Literal(11..14, "foo");
					});
				});
			});
			CustomContainer(16..26, null, () =>
			{
				Paragraph(21..26, () =>
				{
					Literal(21..24, "bar");
				});
			});
		});
	}

	/// <summary>
	/// 自定义容器可以只包含空白行。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown(":::\r\n\r\n  \r\n:::\r\n", () =>
		{
			CustomContainer(0..16);
		});
	}

	/// <summary>
	/// 自定义容器也可以是空的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-8"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown(":::\r\n:::\r\n", () =>
		{
			CustomContainer(0..10);
		});
	}

	/// <summary>
	/// 自定义容器的起始分隔符可以包含至多三个空格的缩进，但不会影响内部节点的解析。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-9"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("   :::\r\n    code\r\n:::\r\n", () =>
		{
			CustomContainer(0..23, null, () =>
			{
				CodeBlock(8..18, "code\r\n");
			});
		});
	}

	/// <summary>
	/// 四个空格的缩进就太多了。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-10"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("    :::\r\nfoo\r\n:::\r\n", () =>
		{
			CodeBlock(0..9, ":::\r\n");
			Paragraph(9..14, () =>
			{
				Literal(9..12, "foo");
			});
			CustomContainer(14..19);
		});
	}

	/// <summary>
	/// 结束分隔符也可以包含至多三个空白的缩进，缩进并不需要与起始分隔符一致。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-11"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown(" :::\r\nfoo\r\n  :::\r\n", () =>
		{
			CustomContainer(0..18, null, () =>
			{
				Paragraph(6..11, () =>
				{
					Literal(6..9, "foo");
				});
			});
		});
	}

	/// <summary>
	/// 由于有四个空白，因此并不会被识别成结束分隔符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-12"/>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown(":::\r\nfoo\r\n    :::\r\n", () =>
		{
			CustomContainer(0..19, null, () =>
			{
				Paragraph(5..19, () =>
				{
					Literal(5..8, "foo");
					SoftBreak(8..14);
					Literal(14..17, ":::");
				});
			});
		});
	}

	/// <summary>
	/// 结束分隔符不能包含内部的空格或 Tab。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-13"/>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown("::::::\r\nfoo\r\n::: :::\r\n", () =>
		{
			CustomContainer(0..22, null, () =>
			{
				Paragraph(8..13, () =>
				{
					Literal(8..11, "foo");
				});
				CustomContainer(13..22, ":::");
			});
		});
	}

	/// <summary>
	/// 自定义容器可以直接中断段落，或者直接后跟段落，必须要使用空行分隔。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-14"/>
	[TestMethod]
	public void Test14()
	{
		AssertMarkdown("foo\r\n:::\r\nbar\r\n:::\r\nbaz\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "foo");
			});
			CustomContainer(5..20, null, () =>
			{
				Paragraph(10..15, () =>
				{
					Literal(10..13, "bar");
				});
			});
			Paragraph(20..25, () =>
			{
				Literal(20..23, "baz");
			});
		});
	}

	/// <summary>
	/// 其它块也可以直接紧邻自定义容器。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-15"/>
	[TestMethod]
	public void Test15()
	{
		ParseOptions options = new()
		{
			UseAutoIdentifier = false,
		};
		AssertMarkdown("foo\r\n---\r\n:::\r\nbar\r\n:::\r\n# baz\r\n", options, () =>
		{
			Heading(0..10, 2, () =>
			{
				Literal(0..3, "foo");
			});
			CustomContainer(10..25, null, () =>
			{
				Paragraph(15..20, () =>
				{
					Literal(15..18, "bar");
				});
			});
			Heading(25..32, 1, () =>
			{
				Literal(27..30, "baz");
			});
		});
	}

	/// <summary>
	/// 自定义容器的起始分隔符后可包含信息，首个空格前的部分一般会作为 <c>&lt;div&gt;</c> 的 <c>class</c> 属性使用。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-16"/>
	[TestMethod]
	public void Test16()
	{
		AssertMarkdown(":::detail\r\nfoo\r\n:::\r\n", () =>
		{
			CustomContainer(0..21, "detail", () =>
			{
				Paragraph(11..16, () =>
				{
					Literal(11..14, "foo");
				});
			});
		});
	}

	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-17"/>
	[TestMethod]
	public void Test17()
	{
		AssertMarkdown("::: detail other info $%@:#$ \r\nfoo\r\n:::\r\n", () =>
		{
			CustomContainer(0..41, "detail other info $%@:#$", () =>
			{
				Paragraph(31..36, () =>
				{
					Literal(31..34, "foo");
				});
			});
		});
	}

	/// <summary>
	/// 闭合分隔符不能包含信息。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-18"/>
	[TestMethod]
	public void Test18()
	{
		AssertMarkdown(":::\r\nfoo\r\n::: detail\r\n", () =>
		{
			CustomContainer(0..22, null, () =>
			{
				Paragraph(5..10, () =>
				{
					Literal(5..8, "foo");
				});
				CustomContainer(10..22, "detail");
			});
		});
	}

	/// <summary>
	/// 与代码块类似，自定义容器同样支持自定义属性。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-19"/>
	[TestMethod]
	public void Test19()
	{
		AssertMarkdown("::: detail {\r\n  #my-id .class2\r\n  key=value\r\n}\r\nfoo\r\n:::\r\n", () =>
		{
			CustomContainer(0..58, "detail", new HtmlAttributeList() {
				{ "id", "my-id" },
				{ "class", "class2" },
				{ "key", "value" },
			}, () =>
			{
				Paragraph(48..53, () =>
				{
					Literal(48..51, "foo");
				});
			});
		});
	}

	/// <summary>
	/// 自定义容器内可以包含其它结构。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md#example-20"/>
	[TestMethod]
	public void Test20()
	{
		AssertMarkdown("- list1\r\n  ::: detail\r\n  - foo\r\n  > bar\r\n  :::\r\n- list2\r\n", () =>
		{
			UnorderedList(0..57, false, () =>
			{
				ListItem(0..48, () =>
				{
					Paragraph(2..9, () =>
					{
						Literal(2..7, "list1");
					});
					CustomContainer(11..48, "detail", () =>
					{
						UnorderedList(25..32, false, () =>
						{
							ListItem(25..32, () =>
							{
								Paragraph(27..32, () =>
								{
									Literal(27..30, "foo");
								});
							});
						});
						Blockquote(34..41, () =>
						{
							Paragraph(36..41, () =>
							{
								Literal(36..39, "bar");
							});
						});
					});
				});
				ListItem(48..57, () =>
				{
					Paragraph(50..57, () =>
					{
						Literal(50..55, "list2");
					});
				});
			});
		});
	}

	/// <summary>
	/// 测试 CommonMark 不支持自定义容器。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark(":::\r\nfoo *bar*\r\n\r\nbaz\r\n:::\r\n", () =>
		{
			Paragraph(0..16, () =>
			{
				Literal(0..3, ":::");
				SoftBreak(3..5);
				Literal(5..9, "foo ");
				Emphasis(9..14, () =>
				{
					Literal(10..13, "bar");
				});
			});
			Paragraph(18..28, () =>
			{
				Literal(18..21, "baz");
				SoftBreak(21..23);
				Literal(23..26, ":::");
			});
		});
	}
}

