using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Custom;

/// <summary>
/// Setext 标题的单元测试。
/// </summary>
[TestClass]
public class UnitTestSetextHeading : BaseTest
{
	/// <summary>
	/// 链接定义不被解释为 Setext 标题。
	/// </summary>
	[TestMethod]
	public void TestLinkDefinition_1()
	{
		AssertMarkdown("[foo]: /url\r\n===\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
			Paragraph(13..18, () =>
			{
				Literal(13..16, "===");
			});
		});
	}
	/// <summary>
	/// 链接定义后的文本可以正常解释为 Setext 标题。
	/// </summary>
	[TestMethod]
	public void TestLinkDefinition_2()
	{
		AssertMarkdown("[foo]: /url\r\nbar\r\n===\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
			Heading(13..23, 1, () =>
			{
				Literal(13..16, "bar");
			});
		});
	}
	/// <summary>
	/// 将链接解释为 Setext 标题。
	/// </summary>
	[TestMethod]
	public void TestLinkDefinition_3()
	{
		AssertMarkdown("[foo]: /url\r\n[foo]\r\n===\r\n", () =>
		{
			LinkDefinition(0..13, "foo", "/url");
			Heading(13..25, 1, () =>
			{
				Link(13..18, "/url", null, () =>
				{
					Literal(14..17, "foo");
				});
			});
		});
	}
}

