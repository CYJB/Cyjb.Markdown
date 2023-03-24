using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="Node"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestNode
{
	/// <summary>
	/// 测试行定位器。
	/// </summary>
	[TestMethod]
	public void TestLineLocator()
	{
		Document doc = Document.Parse(">\tfoo\r\n", new ParseOptions() { UseLineLocator = true });
		// Document
		Assert.AreEqual(new LinePositionSpan(
			new LinePosition(1, 0, 1),
			new LinePosition(2, 0, 1)
		), doc.LinePositionSpan);
		// Document > Block
		Assert.AreEqual(new LinePositionSpan(
			new LinePosition(1, 0, 1),
			new LinePosition(2, 0, 1)
		), doc.FirstChild!.LinePositionSpan);
		// Document > Block > Paragraph
		Assert.AreEqual(new LinePositionSpan(
			new LinePosition(1, 2, 5),
			new LinePosition(2, 0, 1)
		), doc.FirstChild!.FirstChild!.LinePositionSpan);
		// Document > Block > Paragraph > Literal
		Assert.AreEqual(new LinePositionSpan(
			new LinePosition(1, 2, 5),
			new LinePosition(1, 5, 8)
		), doc.FirstChild!.FirstChild!.FirstChild!.LinePositionSpan);
	}
}
