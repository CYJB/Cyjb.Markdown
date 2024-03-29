using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 规范渲染结果的单元测试。
/// </summary>
[TestClass]
public class UnitTestRender
{
	/// <summary>
	/// 测试 CommonMark 规范。
	/// </summary>
	/// <see href="https://spec.commonmark.org/0.31.2/"/>
	[TestMethod]
	public void TestCommonMark()
	{
		SpecItem[] items = ReadSpec("CommonMark.0.31.2.spec.json");
		// 渲染结果会与 CommonMark 略有差异。
		foreach (SpecItem item in items)
		{
			// HTML 属性里的 & 可以不被编码为 &amp;
			if (item.Example == 573 || item.Example == 576 || item.Example == 577)
			{
				item.Html = "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train & tracks\" /></p>\n";
			}
			else if (item.Example == 595)
			{
				item.Html = "<p><a href=\"https://foo.bar.baz/test?q=hello&id=22&boolean\">https://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean</a></p>\n";
			}
		}
		TestRender(items, ParseOptions.CommonMark);
	}

	/// <summary>
	/// 测试 GFM 规范。
	/// </summary>
	/// <see href="http://github.github.com/gfm"/>
	[TestMethod]
	public void TestGFM()
	{
		SpecItem[] items = ReadSpec("GitHubFlavoredMarkdown.0.29.spec.json");
		Regex alignRegex = new(@"align=""(\w+)""");
		// 渲染结果会与 GFM 略有差异。
		foreach (SpecItem item in items)
		{
			// 表格对齐时不使用已过期的 align 属性，而是改为 text-align 样式。
			item.Html = alignRegex.Replace(item.Html, (Match match) =>
			{
				return $"style=\"text-align: {match.Groups[1]};\"";
			});
		}
		TestRender(items);
	}

	/// <summary>
	/// 测试额外的列表样式类型规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md"/>
	[TestMethod]
	public void TestExtraListStyleType()
	{
		SpecItem[] items = ReadSpec("ExtraListStyleType.spec.json");
		TestRender(items);
	}

	/// <summary>
	/// 测试任务列表项规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/task-list-item.md"/>
	[TestMethod]
	public void TestTaskListItem()
	{
		SpecItem[] items = ReadSpec("TaskListItem.spec.json");
		TestRender(items);
	}

	/// <summary>
	/// 测试数学公式规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md"/>
	[TestMethod]
	public void TestMathematics()
	{
		SpecItem[] items = ReadSpec("Mathematics.spec.json");
		TestRender(items, new ParseOptions() { UseAutoIdentifier = false });
	}

	/// <summary>
	/// 测试扩展自动链接规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/ext-autolink.md"/>
	[TestMethod]
	public void TestExtAutolink()
	{
		SpecItem[] items = ReadSpec("ExtAutolink.spec.json");
		TestRender(items);
	}

	/// <summary>
	/// 测试属性规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md"/>
	[TestMethod]
	public void TestAttributes()
	{
		SpecItem[] items = ReadSpec("Attributes.spec.json");
		TestRender(items, new ParseOptions() { UseAutoIdentifier = false });
	}

	/// <summary>
	/// 测试自动生成标识符规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/auto-identifier.md"/>
	[TestMethod]
	public void TestAutoIdentifier()
	{
		SpecItem[] items = ReadSpec("AutoIdentifier.spec.json");
		TestRender(items);
	}

	/// <summary>
	/// 测试标题引用规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/header-references.md"/>
	[TestMethod]
	public void TestHeaderReferences()
	{
		SpecItem[] items = ReadSpec("HeaderReferences.spec.json");
		TestRender(items);
	}

	/// <summary>
	/// 测试脚注规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md"/>
	[TestMethod]
	public void TestFootnotes()
	{
		SpecItem[] items = ReadSpec("Footnotes.spec.json");
		TestRender(items);
	}

	/// <summary>
	/// 测试自定义容器规范。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md"/>
	[TestMethod]
	public void TestCustomContainers()
	{
		SpecItem[] items = ReadSpec("CustomContainers.spec.json");
		TestRender(items, new ParseOptions() { UseAutoIdentifier = false });
	}

	/// <summary>
	/// 读取指定的规范。
	/// </summary>
	/// <param name="name">规范的名称。</param>
	/// <returns>规范的内容。</returns>
	private static SpecItem[] ReadSpec(string name)
	{
		string spec = File.ReadAllText("Spec/" + name);
		JsonSerializerOptions options = new()
		{
			PropertyNameCaseInsensitive = true,
		};
		return JsonSerializer.Deserialize<SpecItem[]>(spec, options)!;
	}

	/// <summary>
	/// 测试 HTML 渲染结果。
	/// </summary>
	/// <param name="items">规范的项。</param>
	/// <param name="options">解析选项。</param>
	/// <param name="commonMark">是否使用 CommonMark 规范。</param>
	private static void TestRender(SpecItem[] items, ParseOptions? options = null)
	{
		HtmlRenderer renderer = new();
		options ??= ParseOptions.Default;
		foreach (SpecItem item in items)
		{
			Document doc = Document.Parse(item.Markdown, options);
			renderer.Clear();
			doc.Accept(renderer);
			Assert.AreEqual(item.Html, renderer.ToString(), item.Section + " Example " + item.Example);
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
