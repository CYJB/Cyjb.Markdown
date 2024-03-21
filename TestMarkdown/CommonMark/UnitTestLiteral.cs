using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 文本的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/#textual-content"/>
[TestClass]
public class UnitTestLiteral : BaseTest
{
	/// <see href="https://spec.commonmark.org/0.31.2/#example-650"/>
	[TestMethod]
	public void Test650()
	{
		AssertMarkdown("hello $.;'there", () =>
		{
			Paragraph(0..15, () =>
			{
				Literal(0..15, "hello $.;'there");
			});
		});
	}
	/// <see href="https://spec.commonmark.org/0.31.2/#example-651"/>
	[TestMethod]
	public void Test651()
	{
		AssertMarkdown("Foo χρῆν", () =>
		{
			Paragraph(0..8, () =>
			{
				Literal(0..8, "Foo χρῆν");
			});
		});
	}
	/// <summary>
	/// 内部空白会被原样保留。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/#example-652"/>
	[TestMethod]
	public void Test652()
	{
		AssertMarkdown("Multiple     spaces", () =>
		{
			Paragraph(0..19, () =>
			{
				Literal(0..19, "Multiple     spaces");
			});
		});
	}
}

