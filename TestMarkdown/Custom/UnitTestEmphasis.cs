using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Custom;

/// <summary>
/// 强调的单元测试。
/// </summary>
[TestClass]
public class UnitTestEmphasis : BaseTest
{
	/// <summary>
	/// 起始 * 后跟空白，因此不是强调。
	/// </summary>
	[TestMethod]
	public void TestChinese()
	{
		AssertMarkdown("中*文*测试", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..1, "中");
				Emphasis(1..4, () =>
				{
					Literal(2..3, "文");
				});
				Literal(4..6, "测试");
			});
		});
	}
}
