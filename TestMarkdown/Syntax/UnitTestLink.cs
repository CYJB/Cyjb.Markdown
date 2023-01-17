using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="Link"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestLink
{
	/// <summary>
	/// 测试 Link 类。
	/// </summary>
	[TestMethod]
	public void TestLink()
	{
		Link link = new(false, string.Empty, null);
		Assert.AreEqual(MarkdownKind.Link, link.Kind);
		Assert.AreEqual("", link.URL);
		Assert.IsNull(link.Title);
		Assert.IsNull(link.Definition);

		link.URL = "url";
		Assert.AreEqual("url", link.URL);

		LinkDefinition define = new("label", "url1", "title");
		link.Definition = define;
		Assert.AreEqual("url1", link.URL);
		Assert.AreEqual("title", link.Title);
		Assert.AreEqual(define, link.Definition);

		link.URL = "url2";
		Assert.AreEqual("title", link.Title);
		Assert.IsNull(link.Definition);
	}
}

