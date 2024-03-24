using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="NodeList{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestNodeList
{
	/// <summary>
	/// 测试添加 <see cref="NodeList{T}"/> 节点。
	/// </summary>
	[TestMethod]
	public void TestAdd()
	{
		Paragraph parent = new Paragraph();
		NodeList<InlineNode> nodes = parent.Children;
		Assert.AreEqual(0, nodes.Count);

		Literal subNode1 = new("foo");
		nodes.Add(subNode1);
		Assert.AreEqual(1, nodes.Count);
		Assert.AreEqual(parent, subNode1.Parent);
		Assert.AreEqual(null, subNode1.Prev);
		Assert.AreEqual(null, subNode1.Next);

		Literal subNode2 = new("bar");
		nodes.Add(subNode2);
		Assert.AreEqual(2, nodes.Count);
		Assert.AreEqual(parent, subNode2.Parent);
		Assert.AreEqual(null, subNode1.Prev);
		Assert.AreEqual(subNode2, subNode1.Next);
		Assert.AreEqual(subNode1, subNode2.Prev);
		Assert.AreEqual(null, subNode2.Next);

		Literal subNode3 = new("baz");
		nodes.Insert(0, subNode3);
		Assert.AreEqual(3, nodes.Count);
		Assert.AreEqual(parent, subNode3.Parent);
		Assert.AreEqual(null, subNode3.Prev);
		Assert.AreEqual(subNode1, subNode3.Next);
		Assert.AreEqual(subNode3, subNode1.Prev);
		Assert.AreEqual(subNode2, subNode1.Next);
		Assert.AreEqual(subNode1, subNode2.Prev);
		Assert.AreEqual(null, subNode2.Next);

		Literal subNode4 = new("4");
		Literal subNode5 = new("5");
		Literal subNode6 = new("6");
		new NodeList<InlineNode>() { subNode4, subNode5, subNode6 };

		nodes.AddRange(subNode4, subNode6);
		Assert.AreEqual(5, nodes.Count);
		Assert.AreEqual(parent, subNode4.Parent);
		Assert.AreEqual(parent, subNode5.Parent);
		Assert.AreEqual(subNode4, subNode2.Next);
		Assert.AreEqual(subNode2, subNode4.Prev);
		Assert.AreEqual(subNode5, subNode4.Next);
		Assert.AreEqual(subNode4, subNode5.Prev);
		Assert.AreEqual(null, subNode5.Next);

		Assert.AreEqual(null, subNode6.Prev);
		Assert.AreEqual(null, subNode6.Next);

		nodes.Insert(3, subNode6);
		Assert.AreEqual(6, nodes.Count);
		Assert.AreEqual(parent, subNode6.Parent);
		Assert.AreEqual(subNode6, subNode2.Next);
		Assert.AreEqual(subNode2, subNode6.Prev);
		Assert.AreEqual(subNode4, subNode6.Next);
		Assert.AreEqual(subNode6, subNode4.Prev);
		CollectionAssert.AreEqual(new InlineNode[]
		{
			subNode3, subNode1, subNode2, subNode6, subNode4, subNode5,
		}, nodes);

		nodes.Insert(3, subNode3);
		Assert.AreEqual(6, nodes.Count);
		CollectionAssert.AreEqual(new InlineNode[]
		{
			subNode1, subNode2, subNode3, subNode6, subNode4, subNode5,
		}, nodes);
		Assert.AreEqual(parent, subNode3.Parent);
		Assert.AreEqual(null, subNode1.Prev);
		Assert.AreEqual(subNode3, subNode2.Next);
		Assert.AreEqual(subNode2, subNode3.Prev);
		Assert.AreEqual(subNode6, subNode3.Next);
		Assert.AreEqual(subNode3, subNode6.Prev);

		nodes.Insert(3, subNode5);
		Assert.AreEqual(6, nodes.Count);
		CollectionAssert.AreEqual(new InlineNode[]
		{
			subNode1, subNode2, subNode3, subNode5, subNode6, subNode4,
		}, nodes);
		Assert.AreEqual(parent, subNode5.Parent);
		Assert.AreEqual(subNode5, subNode3.Next);
		Assert.AreEqual(subNode3, subNode5.Prev);
		Assert.AreEqual(subNode6, subNode5.Next);
		Assert.AreEqual(subNode5, subNode6.Prev);
		Assert.AreEqual(null, subNode4.Next);

		nodes.AddRange(subNode5, subNode4);
		Assert.AreEqual(6, nodes.Count);
		CollectionAssert.AreEqual(new InlineNode[]
		{
			subNode1, subNode2, subNode3, subNode4, subNode5, subNode6,
		}, nodes);
		Assert.AreEqual(parent, subNode5.Parent);
		Assert.AreEqual(parent, subNode6.Parent);
		Assert.AreEqual(subNode4, subNode3.Next);
		Assert.AreEqual(subNode3, subNode4.Prev);
		Assert.AreEqual(subNode5, subNode4.Next);
		Assert.AreEqual(subNode4, subNode5.Prev);
		Assert.AreEqual(subNode6, subNode5.Next);
		Assert.AreEqual(subNode5, subNode6.Prev);
		Assert.AreEqual(null, subNode6.Next);

		nodes[1] = subNode5;
		Assert.AreEqual(5, nodes.Count);
		CollectionAssert.AreEqual(new InlineNode[]
		{
			subNode1, subNode5, subNode3, subNode4, subNode6,
		}, nodes);
		Assert.AreEqual(parent, subNode5.Parent);
		Assert.AreEqual(null, subNode2.Parent);
		Assert.AreEqual(subNode5, subNode1.Next);
		Assert.AreEqual(subNode1, subNode5.Prev);
		Assert.AreEqual(subNode3, subNode5.Next);
		Assert.AreEqual(subNode6, subNode4.Next);
		Assert.AreEqual(subNode4, subNode6.Prev);
		Assert.AreEqual(null, subNode2.Prev);
		Assert.AreEqual(null, subNode2.Next);
	}
}
