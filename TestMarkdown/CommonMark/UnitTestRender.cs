using System;
using System.IO;
using System.Text.Json;
using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.CommonMark;

/// <summary>
/// 渲染结果的单元测试。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#atx-headings"/>
[TestClass]
public class UnitTestRender
{
	/// <see href="https://spec.commonmark.org/0.30/#example-593"/>
	[TestMethod]
	public void TestRender()
	{
		string spec = File.ReadAllText("CommonMark/commonmark.0.30.spec.json");
		JsonSerializerOptions options = new()
		{
			PropertyNameCaseInsensitive = true,
		};
		SpecItem[] items = JsonSerializer.Deserialize<SpecItem[]>(spec, options)!;
		HtmlRenderer renderer = new();
		foreach (SpecItem item in items)
		{
			// HTML 属性里的 & 可以不被编码为 &amp;
			if (item.Example == 572 || item.Example == 575 || item.Example == 576)
			{
				item.Html = "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train & tracks\" /></p>\n";
			} else if (item.Example == 594)
			{
				item.Html = "<p><a href=\"http://foo.bar.baz/test?q=hello&id=22&boolean\">http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean</a></p>\n";
			}
			Document doc = Document.Parse(item.Markdown);
			renderer.Clear();
			doc.Accept(renderer);
			Assert.AreEqual(item.Html, renderer.ToString(), "{0}: Example {1}", item.Section, item.Example);
		}
	}

	/// <summary>
	/// 规范的项。
	/// </summary>
	private class SpecItem
	{
		/// <summary>
		/// Markdown 文本。
		/// </summary>
		public string Markdown { get; set; } = string.Empty;
		/// <summary>
		/// HTML 文本。
		/// </summary>
		public string Html { get; set; } = string.Empty;
		/// <summary>
		/// 示例序号。
		/// </summary>
		public int Example { get; set; }
		/// <summary>
		/// 所属文档部分。
		/// </summary>
		public string Section { get; set; } = string.Empty;
	}
}
