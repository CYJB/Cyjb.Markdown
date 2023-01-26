using System;
using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="ListItem"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestListItem
{
	/// <summary>
	/// 测试 <see cref="ListItem.Parent"/> 属性。
	/// </summary>
	[TestMethod]
	public void TestParent()
	{
		List list = new(ListStyleType.Unordered);
		ListItem item = new();
		list.Children.Add(item);
		Assert.AreEqual(list, item.Parent);

		Quote quote = new();
		Assert.ThrowsException<ArgumentException>(() => quote.Children.Add(item));
	}
}

