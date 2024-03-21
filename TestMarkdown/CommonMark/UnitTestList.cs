using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 列表的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#list"/>
[TestClass]
public class UnitTestList : BaseTest
{
	/// <summary>
	/// 改变列表分隔符会开始一个新的列表。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-301"/>
	[TestMethod]
	public void Test301()
	{
		AssertMarkdown("- foo\r\n- bar\r\n+ baz\r\n", () =>
		{
			UnorderedList(0..14, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
				ListItem(7..14, () =>
				{
					Paragraph(9..14, () =>
					{
						Literal(9..12, "bar");
					});
				});
			});
			UnorderedList(14..21, false, () =>
			{
				ListItem(14..21, () =>
				{
					Paragraph(16..21, () =>
					{
						Literal(16..19, "baz");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-302"/>
	[TestMethod]
	public void Test302()
	{
		AssertMarkdown("1. foo\r\n2. bar\r\n3) baz\r\n", () =>
		{
			OrderedNumberList(0..16, false, 1, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(3..8, () =>
					{
						Literal(3..6, "foo");
					});
				});
				ListItem(8..16, () =>
				{
					Paragraph(11..16, () =>
					{
						Literal(11..14, "bar");
					});
				});
			});
			OrderedNumberList(16..24, false, 3, () =>
			{
				ListItem(16..24, () =>
				{
					Paragraph(19..24, () =>
					{
						Literal(19..22, "baz");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表可以中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-303"/>
	[TestMethod]
	public void Test303()
	{
		AssertMarkdown("Foo\r\n- bar\r\n- baz\r\n", () =>
		{
			Paragraph(0..5, () =>
			{
				Literal(0..3, "Foo");
			});
			UnorderedList(5..19, false, () =>
			{
				ListItem(5..12, () =>
				{
					Paragraph(7..12, () =>
					{
						Literal(7..10, "bar");
					});
				});
				ListItem(12..19, () =>
				{
					Paragraph(14..19, () =>
					{
						Literal(14..17, "baz");
					});
				});
			});
		});
	}
	/// <summary>
	/// 只允许从 1 开始的列表中断段落。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-304"/>
	[TestMethod]
	public void Test304()
	{
		AssertMarkdown("The number of windows in my house is\r\n14.  The number of doors is 6.\r\n", () =>
		{
			Paragraph(0..70, () =>
			{
				Literal(0..36, "The number of windows in my house is");
				SoftBreak(36..38);
				Literal(38..68, "14.  The number of doors is 6.");
			});
		});
	}
	/// <summary>
	/// 下面的场景就会识别成列表。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-305"/>
	[TestMethod]
	public void Test305()
	{
		AssertMarkdown("The number of windows in my house is\r\n1.  The number of doors is 6.\r\n", () =>
		{
			Paragraph(0..38, () =>
			{
				Literal(0..36, "The number of windows in my house is");
			});
			OrderedNumberList(38..69, false, 1, () =>
			{
				ListItem(38..69, () =>
				{
					Paragraph(42..69, () =>
					{
						Literal(42..67, "The number of doors is 6.");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项之间可以包含任意空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-306"/>
	[TestMethod]
	public void Test306()
	{
		AssertMarkdown("- foo\r\n\r\n- bar\r\n\r\n\r\n- baz\r\n", () =>
		{
			UnorderedList(0..27, true, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
				ListItem(9..16, () =>
				{
					Paragraph(11..16, () =>
					{
						Literal(11..14, "bar");
					});
				});
				ListItem(20..27, () =>
				{
					Paragraph(22..27, () =>
					{
						Literal(22..25, "baz");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-307"/>
	[TestMethod]
	public void Test307()
	{
		AssertMarkdown("- foo\r\n  - bar\r\n    - baz\r\n\r\n\r\n      bim\r\n", () =>
		{
			UnorderedList(0..42, false, () =>
			{
				ListItem(0..42, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
					UnorderedList(9..42, false, () =>
					{
						ListItem(9..42, () =>
						{
							Paragraph(11..16, () =>
							{
								Literal(11..14, "bar");
							});
							UnorderedList(20..42, true, () =>
							{
								ListItem(20..42, () =>
								{
									Paragraph(22..27, () =>
									{
										Literal(22..25, "baz");
									});
									Paragraph(37..42, () =>
									{
										Literal(37..40, "bim");
									});
								});
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 为了分割相同序号类型的列表，可以插入空的 HTML 注释。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-308"/>
	[TestMethod]
	public void Test308()
	{
		AssertMarkdown("- foo\r\n- bar\r\n\r\n<!-- -->\r\n\r\n- baz\r\n- bim\r\n", () =>
		{
			UnorderedList(0..14, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
				});
				ListItem(7..14, () =>
				{
					Paragraph(9..14, () =>
					{
						Literal(9..12, "bar");
					});
				});
			});
			HtmlBlock(16..26, "<!-- -->\r\n");
			UnorderedList(28..42, false, () =>
			{
				ListItem(28..35, () =>
				{
					Paragraph(30..35, () =>
					{
						Literal(30..33, "baz");
					});
				});
				ListItem(35..42, () =>
				{
					Paragraph(37..42, () =>
					{
						Literal(37..40, "bim");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-309"/>
	[TestMethod]
	public void Test309()
	{
		AssertMarkdown("-   foo\r\n\r\n    notcode\r\n\r\n-   foo\r\n\r\n<!-- -->\r\n\r\n    code\r\n", () =>
		{
			UnorderedList(0..35, true, () =>
			{
				ListItem(0..24, () =>
				{
					Paragraph(4..9, () =>
					{
						Literal(4..7, "foo");
					});
					Paragraph(15..24, () =>
					{
						Literal(15..22, "notcode");
					});
				});
				ListItem(26..35, () =>
				{
					Paragraph(30..35, () =>
					{
						Literal(30..33, "foo");
					});
				});
			});
			HtmlBlock(37..47, "<!-- -->\r\n");
			CodeBlock(49..59, "code\r\n");
		});
	}
	/// <summary>
	/// 列表项不需要同样的缩进级别。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-310"/>
	[TestMethod]
	public void Test310()
	{
		AssertMarkdown("- a\r\n - b\r\n  - c\r\n   - d\r\n  - e\r\n - f\r\n- g\r\n", () =>
		{
			UnorderedList(0..44, false, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(6..11, () =>
				{
					Paragraph(8..11, () =>
					{
						Literal(8..9, "b");
					});
				});
				ListItem(13..18, () =>
				{
					Paragraph(15..18, () =>
					{
						Literal(15..16, "c");
					});
				});
				ListItem(21..26, () =>
				{
					Paragraph(23..26, () =>
					{
						Literal(23..24, "d");
					});
				});
				ListItem(28..33, () =>
				{
					Paragraph(30..33, () =>
					{
						Literal(30..31, "e");
					});
				});
				ListItem(34..39, () =>
				{
					Paragraph(36..39, () =>
					{
						Literal(36..37, "f");
					});
				});
				ListItem(39..44, () =>
				{
					Paragraph(41..44, () =>
					{
						Literal(41..42, "g");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-311"/>
	[TestMethod]
	public void Test311()
	{
		AssertMarkdown("1. a\r\n\r\n  2. b\r\n\r\n   3. c\r\n", () =>
		{
			OrderedNumberList(0..27, true, 1, () =>
			{
				ListItem(0..6, () =>
				{
					Paragraph(3..6, () =>
					{
						Literal(3..4, "a");
					});
				});
				ListItem(10..16, () =>
				{
					Paragraph(13..16, () =>
					{
						Literal(13..14, "b");
					});
				});
				ListItem(21..27, () =>
				{
					Paragraph(24..27, () =>
					{
						Literal(24..25, "c");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项前不能有 4 个或更多空白。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-312"/>
	[TestMethod]
	public void Test312()
	{
		AssertMarkdown("- a\r\n - b\r\n  - c\r\n   - d\r\n    - e\r\n", () =>
		{
			UnorderedList(0..35, false, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(6..11, () =>
				{
					Paragraph(8..11, () =>
					{
						Literal(8..9, "b");
					});
				});
				ListItem(13..18, () =>
				{
					Paragraph(15..18, () =>
					{
						Literal(15..16, "c");
					});
				});
				ListItem(21..35, () =>
				{
					Paragraph(23..35, () =>
					{
						Literal(23..24, "d");
						SoftBreak(24..30);
						Literal(30..33, "- e");
					});
				});
			});
		});
	}
	/// <summary>
	/// 这里 <c>3. c</c> 会被识别为缩进代码块，因为之前有一个空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-313"/>
	[TestMethod]
	public void Test313()
	{
		AssertMarkdown("1. a\r\n\r\n  2. b\r\n\r\n    3. c\r\n", () =>
		{
			OrderedNumberList(0..16, true, 1, () =>
			{
				ListItem(0..6, () =>
				{
					Paragraph(3..6, () =>
					{
						Literal(3..4, "a");
					});
				});
				ListItem(10..16, () =>
				{
					Paragraph(13..16, () =>
					{
						Literal(13..14, "b");
					});
				});
			});
			CodeBlock(18..28, "3. c\r\n");
		});
	}
	/// <summary>
	/// 是松散列表，因为两个列表项间包含空行。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-314"/>
	[TestMethod]
	public void Test314()
	{
		AssertMarkdown("- a\r\n- b\r\n\r\n- c\r\n", () =>
		{
			UnorderedList(0..17, true, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(5..10, () =>
				{
					Paragraph(7..10, () =>
					{
						Literal(7..8, "b");
					});
				});
				ListItem(12..17, () =>
				{
					Paragraph(14..17, () =>
					{
						Literal(14..15, "c");
					});
				});
			});
		});
	}
	/// <summary>
	/// 第二个列表项是空的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-315"/>
	[TestMethod]
	public void Test315()
	{
		AssertMarkdown("* a\r\n*\r\n\r\n* c\r\n", () =>
		{
			UnorderedList(0..15, true, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(5..8);
				ListItem(10..15, () =>
				{
					Paragraph(12..15, () =>
					{
						Literal(12..13, "c");
					});
				});
			});
		});
	}
	/// <summary>
	/// 列表项包含了被空行分隔的两个块级节点，因此也是松散的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-316"/>
	[TestMethod]
	public void Test316()
	{
		AssertMarkdown("- a\r\n- b\r\n\r\n  c\r\n- d\r\n", () =>
		{
			UnorderedList(0..22, true, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(5..17, () =>
				{
					Paragraph(7..10, () =>
					{
						Literal(7..8, "b");
					});
					Paragraph(14..17, () =>
					{
						Literal(14..15, "c");
					});
				});
				ListItem(17..22, () =>
				{
					Paragraph(19..22, () =>
					{
						Literal(19..20, "d");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-317"/>
	[TestMethod]
	public void Test317()
	{
		AssertMarkdown("- a\r\n- b\r\n\r\n  [ref]: /url\r\n- d\r\n", () =>
		{
			UnorderedList(0..32, true, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(5..27, () =>
				{
					Paragraph(7..10, () =>
					{
						Literal(7..8, "b");
					});
					LinkDefinition(14..27, "ref", "/url");
				});
				ListItem(27..32, () =>
				{
					Paragraph(29..32, () =>
					{
						Literal(29..30, "d");
					});
				});
			});
		});
	}
	/// <summary>
	/// 非松散列表，因为空行在代码块内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-318"/>
	[TestMethod]
	public void Test318()
	{
		AssertMarkdown("- a\r\n- ```\r\n  b\r\n\r\n\r\n  ```\r\n- c\r\n", () =>
		{
			UnorderedList(0..33, false, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
				ListItem(5..28, () =>
				{
					CodeBlock(7..28, "b\r\n\r\n\r\n");
				});
				ListItem(28..33, () =>
				{
					Paragraph(30..33, () =>
					{
						Literal(30..31, "c");
					});
				});
			});
		});
	}
	/// <summary>
	/// 非松散列表，因为空行在子列表内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-319"/>
	[TestMethod]
	public void Test319()
	{
		AssertMarkdown("- a\r\n  - b\r\n\r\n    c\r\n- d\r\n", () =>
		{
			UnorderedList(0..26, false, () =>
			{
				ListItem(0..21, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
					UnorderedList(7..21, true, () =>
					{
						ListItem(7..21, () =>
						{
							Paragraph(9..12, () =>
							{
								Literal(9..10, "b");
							});
							Paragraph(18..21, () =>
							{
								Literal(18..19, "c");
							});
						});
					});
				});
				ListItem(21..26, () =>
				{
					Paragraph(23..26, () =>
					{
						Literal(23..24, "d");
					});
				});
			});
		});
	}
	/// <summary>
	/// 非松散列表，因为空行在引用内。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-320"/>
	[TestMethod]
	public void Test320()
	{
		AssertMarkdown("* a\r\n  > b\r\n  >\r\n* c\r\n", () =>
		{
			UnorderedList(0..22, false, () =>
			{
				ListItem(0..17, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
					Blockquote(7..17, () =>
					{
						Paragraph(9..12, () =>
						{
							Literal(9..10, "b");
						});
					});
				});
				ListItem(17..22, () =>
				{
					Paragraph(19..22, () =>
					{
						Literal(19..20, "c");
					});
				});
			});
		});
	}
	/// <summary>
	/// 非松散列表，因为未使用空行分割。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-321"/>
	[TestMethod]
	public void Test321()
	{
		AssertMarkdown("- a\r\n  > b\r\n  ```\r\n  c\r\n  ```\r\n- d\r\n", () =>
		{
			UnorderedList(0..36, false, () =>
			{
				ListItem(0..31, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
					Blockquote(7..12, () =>
					{
						Paragraph(9..12, () =>
						{
							Literal(9..10, "b");
						});
					});
					CodeBlock(14..31, "c\r\n");
				});
				ListItem(31..36, () =>
				{
					Paragraph(33..36, () =>
					{
						Literal(33..34, "d");
					});
				});
			});
		});
	}
	/// <summary>
	/// 只包含一个段落的列表是非松散的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-322"/>
	[TestMethod]
	public void Test322()
	{
		AssertMarkdown("- a\r\n", () =>
		{
			UnorderedList(0..5, false, () =>
			{
				ListItem(0..5, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-323"/>
	[TestMethod]
	public void Test323()
	{
		AssertMarkdown("- a\r\n  - b\r\n", () =>
		{
			UnorderedList(0..12, false, () =>
			{
				ListItem(0..12, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
					UnorderedList(7..12, false, () =>
					{
						ListItem(7..12, () =>
						{
							Paragraph(9..12, () =>
							{
								Literal(9..10, "b");
							});
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 松散列表，因为空行在列表项的两个块之间。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-324"/>
	[TestMethod]
	public void Test324()
	{
		AssertMarkdown("1. ```\r\n   foo\r\n   ```\r\n\r\n   bar\r\n", () =>
		{
			OrderedNumberList(0..34, true, 1, () =>
			{
				ListItem(0..34, () =>
				{
					CodeBlock(3..24, "foo\r\n");
					Paragraph(29..34, () =>
					{
						Literal(29..32, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 外层列表是松散的，内层是非松散的。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-325"/>
	[TestMethod]
	public void Test325()
	{
		AssertMarkdown("* foo\r\n  * bar\r\n\r\n  baz\r\n", () =>
		{
			UnorderedList(0..25, true, () =>
			{
				ListItem(0..25, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "foo");
					});
					UnorderedList(9..16, false, () =>
					{
						ListItem(9..16, () =>
						{
							Paragraph(11..16, () =>
							{
								Literal(11..14, "bar");
							});
						});
					});
					Paragraph(20..25, () =>
					{
						Literal(20..23, "baz");
					});
				});
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-326"/>
	[TestMethod]
	public void Test326()
	{
		AssertMarkdown("- a\r\n  - b\r\n  - c\r\n\r\n- d\r\n  - e\r\n  - f\r\n", () =>
		{
			UnorderedList(0..40, true, () =>
			{
				ListItem(0..19, () =>
				{
					Paragraph(2..5, () =>
					{
						Literal(2..3, "a");
					});
					UnorderedList(7..19, false, () =>
					{
						ListItem(7..12, () =>
						{
							Paragraph(9..12, () =>
							{
								Literal(9..10, "b");
							});
						});
						ListItem(14..19, () =>
						{
							Paragraph(16..19, () =>
							{
								Literal(16..17, "c");
							});
						});
					});
				});
				ListItem(21..40, () =>
				{
					Paragraph(23..26, () =>
					{
						Literal(23..24, "d");
					});
					UnorderedList(28..40, false, () =>
					{
						ListItem(28..33, () =>
						{
							Paragraph(30..33, () =>
							{
								Literal(30..31, "e");
							});
						});
						ListItem(35..40, () =>
						{
							Paragraph(37..40, () =>
							{
								Literal(37..38, "f");
							});
						});
					});
				});
			});
		});
	}
}

