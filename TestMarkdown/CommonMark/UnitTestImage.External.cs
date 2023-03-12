using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

public partial class UnitTestImage : BaseTest
{
	/// <summary>
	/// 测试 Alt 文本。
	/// </summary>
	/// <remarks>保留普通文本和代码内容，换行会被替换为空格，忽略其它结构。</remarks>
	[TestMethod]
	public void TestAltText()
	{
		Document doc = Document.Parse("![*foo`b\r\nar`*<a k=v\nk2=\"v2\">](/url)\n");
		HtmlRenderer renderer = new();
		renderer.Clear();
		doc.Accept(renderer);
		Assert.AreEqual("<p><img src=\"/url\" alt=\"foob ar\" /></p>\n", renderer.ToString());
	}
}
