using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 任务列表项的单元测试。
/// </summary>
/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md"/>
[TestClass]
public class UnitTestTaskListItem : BaseTest
{
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-1"/>
	[TestMethod]
	public void Test1()
	{
		AssertMarkdown("- [ ] foo\r\n- [x] bar\r\n", () =>
		{
			UnorderedList(0..22, false, () =>
			{
				TaskListItem(0..11, false, () =>
				{
					Paragraph(6..11, () =>
					{
						Literal(6..9, "foo");
					});
				});
				TaskListItem(11..22, true, () =>
				{
					Paragraph(17..22, () =>
					{
						Literal(17..20, "bar");
					});
				});
			});
		});
	}
	/// <summary>
	/// 任务列表项可以是嵌套的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-2"/>
	[TestMethod]
	public void Test2()
	{
		AssertMarkdown("- [x] foo\r\n  - [ ] bar\r\n  - [x] baz\r\n- [ ] bim\r\n", () =>
		{
			UnorderedList(0..48, false, () =>
			{
				TaskListItem(0..37, true, () =>
				{
					Paragraph(6..11, () =>
					{
						Literal(6..9, "foo");
					});
					UnorderedList(13..37, false, () =>
					{
						TaskListItem(13..24, false, () =>
						{
							Paragraph(19..24, () =>
							{
								Literal(19..22, "bar");
							});
						});
						TaskListItem(26..37, true, () =>
						{
							Paragraph(32..37, () =>
							{
								Literal(32..35, "baz");
							});
						});
					});
				});
				TaskListItem(37..48, false, () =>
				{
					Paragraph(43..48, () =>
					{
						Literal(43..46, "bim");
					});
				});
			});
		});
	}
	/// <summary>
	/// 任务列表项可以与有序列表一起使用，或者是不同的列表项标志。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-3"/>
	[TestMethod]
	public void Test3()
	{
		AssertMarkdown("1. [ ] one\r\n2. [x] two\r\n3. [ ] three\r\n   - [ ] foo\r\n   * [X] bar\r\n     + [X] baz\r\n", () =>
		{
			OrderedNumberList(0..82, false, 1, () =>
			{
				TaskListItem(0..12, false, () =>
				{
					Paragraph(7..12, () =>
					{
						Literal(7..10, "one");
					});
				});
				TaskListItem(12..24, true, () =>
				{
					Paragraph(19..24, () =>
					{
						Literal(19..22, "two");
					});
				});
				TaskListItem(24..82, false, () =>
				{
					Paragraph(31..38, () =>
					{
						Literal(31..36, "three");
					});
					UnorderedList(41..52, false, () =>
					{
						TaskListItem(41..52, false, () =>
						{
							Paragraph(47..52, () =>
							{
								Literal(47..50, "foo");
							});
						});
					});
					UnorderedList(55..82, false, () =>
					{
						TaskListItem(55..82, true, () =>
						{
							Paragraph(61..66, () =>
							{
								Literal(61..64, "bar");
							});
							UnorderedList(71..82, false, () =>
							{
								TaskListItem(71..82, true, () =>
								{
									Paragraph(77..82, () =>
									{
										Literal(77..80, "baz");
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
	/// 任务列表项只能用于列表。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-4"/>
	[TestMethod]
	public void Test4()
	{
		AssertMarkdown("[ ] foo\r\n[x] bar\r\n [ ] baz\r\n [X] bim\r\n", () =>
		{
			Paragraph(0..38, () =>
			{
				Literal(0..7, "[ ] foo");
				SoftBreak(7..9);
				Literal(9..16, "[x] bar");
				SoftBreak(16..19);
				Literal(19..26, "[ ] baz");
				SoftBreak(26..29);
				Literal(29..36, "[X] bim");
			});
		});
	}
	/// <summary>
	/// 任务列表项前可以包含最多三个空白（注意列表项标记后的一个空白不计入在内）。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-5"/>
	[TestMethod]
	public void Test5()
	{
		AssertMarkdown("- [ ]  foo\r\n-  [ ] bar\r\n-   [x] baz\r\n-    [ ] bim\r\n", () =>
		{
			UnorderedList(0..51, false, () =>
			{
				TaskListItem(0..12, false, () =>
				{
					Paragraph(6..12, () =>
					{
						Literal(6..10, " foo");
					});
				});
				TaskListItem(12..24, false, () =>
				{
					Paragraph(19..24, () =>
					{
						Literal(19..22, "bar");
					});
				});
				TaskListItem(24..37, true, () =>
				{
					Paragraph(32..37, () =>
					{
						Literal(32..35, "baz");
					});
				});
				TaskListItem(37..51, false, () =>
				{
					Paragraph(46..51, () =>
					{
						Literal(46..49, "bim");
					});
				});
			});
		});
	}
	/// <summary>
	/// 四个空白就太多了，也不能包含其它字符。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-6"/>
	[TestMethod]
	public void Test6()
	{
		AssertMarkdown("-     [ ] foo\r\n- bar [ ] baz\r\n", () =>
		{
			UnorderedList(0..30, false, () =>
			{
				ListItem(0..15, () =>
				{
					CodeBlock(2..15, "[ ] foo\r\n");
				});
				ListItem(15..30, () =>
				{
					Paragraph(17..30, () =>
					{
						Literal(17..28, "bar [ ] baz");
					});
				});
			});
		});
	}
	/// <summary>
	/// 必须使用成对的方括号。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-7"/>
	[TestMethod]
	public void Test7()
	{
		AssertMarkdown("- [x one\r\n- [[x two\r\n- x] three\r\n- x]] four\r\n- (x) five\r\n- {x} six\r\n- [[x]] seven\r\n", () =>
		{
			UnorderedList(0..83, false, () =>
			{
				ListItem(0..10, () =>
				{
					Paragraph(2..10, () =>
					{
						Literal(2..8, "[x one");
					});
				});
				ListItem(10..21, () =>
				{
					Paragraph(12..21, () =>
					{
						Literal(12..19, "[[x two");
					});
				});
				ListItem(21..33, () =>
				{
					Paragraph(23..33, () =>
					{
						Literal(23..31, "x] three");
					});
				});
				ListItem(33..45, () =>
				{
					Paragraph(35..45, () =>
					{
						Literal(35..43, "x]] four");
					});
				});
				ListItem(45..57, () =>
				{
					Paragraph(47..57, () =>
					{
						Literal(47..55, "(x) five");
					});
				});
				ListItem(57..68, () =>
				{
					Paragraph(59..68, () =>
					{
						Literal(59..66, "{x} six");
					});
				});
				ListItem(68..83, () =>
				{
					Paragraph(70..83, () =>
					{
						Literal(70..81, "[[x]] seven");
					});
				});
			});
		});
	}
	/// <summary>
	/// 未选中的情况下，方括号中必须有且之后一个缩进。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-8"/>
	[TestMethod]
	public void Test8()
	{
		AssertMarkdown("- [  ] foo\r\n-  [\t] bar\r\n- [　] baz\r\n- [] bim\r\n", () =>
		{
			UnorderedList(0..45, false, () =>
			{
				ListItem(0..12, () =>
				{
					Paragraph(2..12, () =>
					{
						Literal(2..10, "[  ] foo");
					});
				});
				ListItem(12..24, () =>
				{
					Paragraph(15..24, () =>
					{
						Literal(15..22, "[\t] bar");
					});
				});
				ListItem(24..35, () =>
				{
					Paragraph(26..35, () =>
					{
						Literal(26..33, "[　] baz");
					});
				});
				ListItem(35..45, () =>
				{
					Paragraph(37..45, () =>
					{
						Literal(37..43, "[] bim");
					});
				});
			});
		});
	}
	/// <summary>
	/// 只要是一个缩进就行，这个缩进可以是由 Tab 形成的。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-9"/>
	[TestMethod]
	public void Test9()
	{
		AssertMarkdown("- [\t] foo\r\n", () =>
		{
			UnorderedList(0..11, false, () =>
			{
				TaskListItem(0..11, false, () =>
				{
					Paragraph(6..11, () =>
					{
						Literal(6..9, "foo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 选中的情况下，方括号中的 <c>x</c> 不区分大小写，但不能包含其它内容。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-10"/>
	[TestMethod]
	public void Test10()
	{
		AssertMarkdown("- [x] foo\r\n- [X] bar\r\n- [x ] baz\r\n- [xx] bim\r\n- [o] boo\r\n", () =>
		{
			UnorderedList(0..57, false, () =>
			{
				TaskListItem(0..11, true, () =>
				{
					Paragraph(6..11, () =>
					{
						Literal(6..9, "foo");
					});
				});
				TaskListItem(11..22, true, () =>
				{
					Paragraph(17..22, () =>
					{
						Literal(17..20, "bar");
					});
				});
				ListItem(22..34, () =>
				{
					Paragraph(24..34, () =>
					{
						Literal(24..32, "[x ] baz");
					});
				});
				ListItem(34..46, () =>
				{
					Paragraph(36..46, () =>
					{
						Literal(36..44, "[xx] bim");
					});
				});
				ListItem(46..57, () =>
				{
					Paragraph(48..57, () =>
					{
						Literal(48..55, "[o] boo");
					});
				});
			});
		});
	}
	/// <summary>
	/// 方括号后必须包含至少一个空白的缩进，且这个空白不会包含在内容中。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-11"/>
	[TestMethod]
	public void Test11()
	{
		AssertMarkdown("- [x]   foo\r\n- [ ]\tbar\r\n- [ ]baz\r\n", () =>
		{
			UnorderedList(0..34, false, () =>
			{
				TaskListItem(0..13, true, () =>
				{
					Paragraph(6..13, () =>
					{
						Literal(6..11, "  foo");
					});
				});
				TaskListItem(13..24, false, () =>
				{
					Paragraph(18..24, () =>
					{
						Literal(18..22, "  bar");
					});
				});
				ListItem(24..34, () =>
				{
					Paragraph(26..34, () =>
					{
						Literal(26..32, "[ ]baz");
					});
				});
			});
		});
	}
	/// <summary>
	/// 方括号后必须包含至少一个空白的缩进，且这个空白不会包含在内容中。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-12"/>
	[TestMethod]
	public void Test12()
	{
		AssertMarkdown("- [ ]\r\n- [x]\r\n- [ ]  \r\n- [X]\t\r\n", () =>
		{
			UnorderedList(0..31, false, () =>
			{
				ListItem(0..7, () =>
				{
					Paragraph(2..7, () =>
					{
						Literal(2..5, "[ ]");
					});
				});
				ListItem(7..14, () =>
				{
					Paragraph(9..14, () =>
					{
						Literal(9..12, "[x]");
					});
				});
				ListItem(14..23, () =>
				{
					Paragraph(16..23, () =>
					{
						Literal(16..19, "[ ]");
					});
				});
				ListItem(23..31, () =>
				{
					Paragraph(25..31, () =>
					{
						Literal(25..28, "[X]");
					});
				});
			});
		});
	}
	/// <summary>
	/// 方括号后总是被识别为段落。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-items.md#example-13"/>
	[TestMethod]
	public void Test13()
	{
		AssertMarkdown("- [ ] > foo\r\n- [x] - bar\r\n- [ ]     baz\r\n- [ ] [x] bim\r\n- [x] - [x] boo\r\n- [ ] one\r\n  [ ] two\r\n  > [ ] three\r\n", () =>
		{
			UnorderedList(0..110, false, () =>
			{
				TaskListItem(0..13, false, () =>
				{
					Paragraph(6..13, () =>
					{
						Literal(6..11, "> foo");
					});
				});
				TaskListItem(13..26, true, () =>
				{
					Paragraph(19..26, () =>
					{
						Literal(19..24, "- bar");
					});
				});
				TaskListItem(26..41, false, () =>
				{
					Paragraph(32..41, () =>
					{
						Literal(32..39, "    baz");
					});
				});
				TaskListItem(41..56, false, () =>
				{
					Paragraph(47..56, () =>
					{
						Literal(47..54, "[x] bim");
					});
				});
				TaskListItem(56..73, true, () =>
				{
					Paragraph(62..73, () =>
					{
						Literal(62..71, "- [x] boo");
					});
				});
				TaskListItem(73..110, false, () =>
				{
					Paragraph(79..95, () =>
					{
						Literal(79..82, "one");
						SoftBreak(82..86);
						Literal(86..93, "[ ] two");
					});
					Quote(97..110, () =>
					{
						Paragraph(99..110, () =>
						{
							Literal(99..108, "[ ] three");
						});
					});
				});
			});
		});
	}
	/// <summary>
	/// 测试 CommonMark 不支持任务列表项。
	/// </summary>
	[TestMethod]
	public void TestCommonMark()
	{
		AssertCommonMark("- [ ] foo\r\n- [x] bar\r\n", () =>
		{
			UnorderedList(0..22, false, () =>
			{
				ListItem(0..11, () =>
				{
					Paragraph(2..11, () =>
					{
						Literal(2..9, "[ ] foo");
					});
				});
				ListItem(11..22, () =>
				{
					Paragraph(13..22, () =>
					{
						Literal(13..20, "[x] bar");
					});
				});
			});
		});
	}
}

