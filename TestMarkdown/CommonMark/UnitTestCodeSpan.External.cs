using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

public partial class UnitTestCodeSpan
{
	/// <summary>
	/// 空的 <c>``</c> 不会被识别为代码段。
	/// </summary>
	[TestMethod]
	public void TestEmptyCodeSpan()
	{
		AssertMarkdown("``\r\n", () =>
		{
			Paragraph(0..4, () =>
			{
				Literal(0..2, "``");
			});
		});
	}
}
