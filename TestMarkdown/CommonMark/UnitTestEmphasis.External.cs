using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

public partial class UnitTestEmphasis
{
	/// <summary>
	/// 测试分割中文。
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
		AssertMarkdown("中_文_测试", () =>
		{
			Paragraph(0..6, () =>
			{
				Literal(0..6, "中_文_测试");
			});
		});
	}
}
