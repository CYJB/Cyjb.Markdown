using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 空行的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#blank-lines"/>
[TestClass]
public class UnitTestBlankLines : BaseTest
{
	/// <summary>
	/// 文档前后的空行会被忽略。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.30/#example-227"/>
	[TestMethod]
	public void Test227()
	{
		AssertCommonMark("  \r\n\r\naaa\r\n  \r\n\r\n# aaa\r\n\r\n  \r\n", () =>
		{
			Paragraph(6..11, () =>
			{
				Literal(6..9, "aaa");
			});
			Heading(17..24, 1, () =>
			{
				Literal(19..22, "aaa");
			});
		});
	}
}

