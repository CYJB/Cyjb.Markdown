using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cyjb.Markdown.Syntax;
using System.Collections.Generic;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="HtmlAttributeList"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestHtmlAttributeList
{
	/// <summary>
	/// 测试 <see cref="HtmlAttributeList"/> 类。
	/// </summary>
	[TestMethod]
	public void TestHtmlAttributeList()
	{
		HtmlAttributeList attrs = new();
		Assert.AreEqual(0, attrs.Count);
		Assert.AreEqual(null, attrs.Id);
		Assert.AreEqual(null, attrs["myAttr"]);

		// 键是不区分大小写的。
		attrs["ID"] = "10";
		Assert.AreEqual(1, attrs.Count);
		Assert.AreEqual(new KeyValuePair<string, string>("id", "10"), attrs[0]);
		Assert.AreEqual("10", attrs["id"]);
		Assert.AreEqual("10", attrs["Id"]);
		Assert.AreEqual("10", attrs.Id);
		Assert.AreEqual(null, attrs["myAttr"]);

		attrs.Add("kEy", "value");
		Assert.AreEqual(2, attrs.Count);
		Assert.AreEqual(new KeyValuePair<string, string>("key", "value"), attrs[1]);
		Assert.AreEqual("value", attrs["key"]);

		attrs.AddClass("class1");
		Assert.AreEqual(3, attrs.Count);
		Assert.AreEqual(new KeyValuePair<string, string>("class", "class1"), attrs[2]);
		Assert.AreEqual("class1", attrs["class"]);

		attrs.AddClass("class2");
		Assert.AreEqual(3, attrs.Count);
		Assert.AreEqual(new KeyValuePair<string, string>("class", "class1 class2"), attrs[2]);
		Assert.AreEqual("class1 class2", attrs["class"]);

		// 类名会重复添加。
		attrs.AddClass("class1");
		Assert.AreEqual(3, attrs.Count);
		Assert.AreEqual(new KeyValuePair<string, string>("class", "class1 class2 class1"), attrs[2]);
		Assert.AreEqual("class1 class2 class1", attrs["class"]);

		attrs.RemoveClass("class1");
		Assert.AreEqual(3, attrs.Count);
		Assert.AreEqual(new KeyValuePair<string, string>("class", "class2"), attrs[2]);
		Assert.AreEqual("class2", attrs["class"]);

		Assert.AreEqual("id=\"10\" key=\"value\" class=\"class2\"", attrs.ToString());
	}
}

