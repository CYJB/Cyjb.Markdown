using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Custom;

/// <summary>
/// 链接定义的单元测试。
/// </summary>
[TestClass]
public class UnitTestLinkDefinition : BaseTest
{
	/// <summary>
	/// 链接定义会忽略前后的空格。
	/// </summary>
	[TestMethod]
	public void TestSpace()
	{
		AssertMarkdown("[ link  ]\r\n[link]\r\n\r\n[  link ]: /uri", () =>
		{
			Paragraph(0..19, () =>
			{
				Link(0..9, "/uri", null, () =>
				{
					Literal(1..8, " link  ");
				});
				SoftBreak(9..11);
				Link(11..17, "/uri", null, () =>
				{
					Literal(12..16, "link");
				});
			});
			LinkDefinition(21..36, "  link ", "/uri");
		});
	}
	/// <summary>
	/// 链接定义会忽略前后的 Tab。
	/// </summary>
	[TestMethod]
	public void TestTab()
	{
		AssertMarkdown("[ link\t ]\r\n[link]\r\n\r\n[\t link ]: /uri", () =>
		{
			Paragraph(0..19, () =>
			{
				Link(0..9, "/uri", null, () =>
				{
					Literal(1..8, " link\t ");
				});
				SoftBreak(9..11);
				Link(11..17, "/uri", null, () =>
				{
					Literal(12..16, "link");
				});
			});
			LinkDefinition(21..36, "\t link ", "/uri");
		});
	}
	/// <summary>
	/// 链接定义会保留全角空格。
	/// </summary>
	[TestMethod]
	public void TestFullSpace()
	{
		AssertMarkdown("[　link　]\r\n[link]\r\n\r\n[　link　]: /uri", () =>
		{
			Paragraph(0..18, () =>
			{
				Link(0..8, "/uri", null, () =>
				{
					Literal(1..7, "　link　");
				});
				SoftBreak(8..10);
				Literal(10..16, "[link]");
			});
			LinkDefinition(20..34, "　link　", "/uri");
		});
	}
}
