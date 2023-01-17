using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 优先级的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#precedence"/>
[TestClass]
public class UnitTestPrecedence : BaseTest
{
	/// <summary>
	/// 块级结构的优先级高于行级结构。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-42"/>
	[TestMethod]
	public void Test42()
	{
		AssertMarkdown("- `one\r\n- two`\r\n", () =>
		{
			UnorderedList(0..16, false, () =>
			{
				ListItem(0..8, () =>
				{
					Paragraph(2..8, () =>
					{
						Literal(2..6, "`one");
					});
				});
				ListItem(8..16, () =>
				{
					Paragraph(10..16, () =>
					{
						Literal(10..14, "two`");
					});
				});
			});
		});
	}
}

