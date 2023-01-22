using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

public partial class UnitTestIndentedCodeBlock
{
	/// <summary>
	/// 缩进代码块前后的非缩进空行不会包含在内。
	/// </summary>
	[TestMethod]
	public void TestBlankLine()
	{
		AssertMarkdown("\r\n\r\n    foo\r\n\r\n", () =>
		{
			CodeBlock(4..13, "foo\r\n");
		});
	}
	/// <summary>
	/// 缩进代码块内部的非缩进空行会包含在内。
	/// </summary>
	[TestMethod]
	public void TestInnerBlankLine()
	{
		AssertMarkdown("\r\n\r\n    foo\r\n\r\n\n\n     bar\r\n", () =>
		{
			CodeBlock(4..27, "foo\r\n\r\n\n\n bar\r\n");
		});
	}
}

