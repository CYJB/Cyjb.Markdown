using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

public partial class UnitTestHtml : BaseTest
{
	/// <summary>
	/// 属性值中可以包含换行，但不能包含空白行。
	/// </summary>
	[TestMethod]
	public void TestAttributeValueMultiline1()
	{
		AssertMarkdown("<a key=\"v\r\n  a\r\n  l\r\n  ue\" />\r\n\r\n<a key=\"v\r\n  \r\nalue\" />\r\n", () =>
		{
			Paragraph(0..31, () =>
			{
				HtmlStartTag(0..29, "<a key=\"v\r\n  a\r\n  l\r\n  ue\" />");
			});
			Paragraph(33..44, () =>
			{
				Literal(33..42, "<a key=\"v");
			});
			Paragraph(48..58, () =>
			{
				Literal(48..56, "alue\" />");
			});
		});
	}
	[TestMethod]
	public void TestAttributeValueMultiline2()
	{
		AssertMarkdown("<a key='v\r\n  a\r\n  l\r\n  ue' />\r\n\r\n<a key='v\r\n  \r\nalue' />\r\n", () =>
		{
			Paragraph(0..31, () =>
			{
				HtmlStartTag(0..29, "<a key='v\r\n  a\r\n  l\r\n  ue' />");
			});
			Paragraph(33..44, () =>
			{
				Literal(33..42, "<a key='v");
			});
			Paragraph(48..58, () =>
			{
				Literal(48..56, "alue' />");
			});
		});
	}
	[TestMethod]
	public void TestAttributeValueMultiline3()
	{
		AssertMarkdown("<a key=v\r\n  a\r\n  l\r\n  ue />\r\n\r\n<a key=v\r\n  \r\nalue />\r\n", () =>
		{
			Paragraph(0..29, () =>
			{
				// 注意这里会包含 4 个属性：key=v, a, l, ue
				HtmlStartTag(0..27, "<a key=v\r\n  a\r\n  l\r\n  ue />");
			});
			Paragraph(31..41, () =>
			{
				Literal(31..39, "<a key=v");
			});
			Paragraph(45..54, () =>
			{
				Literal(45..52, "alue />");
			});
		});
	}
}

