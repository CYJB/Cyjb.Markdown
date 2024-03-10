using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using System.Collections.Generic;
using Cyjb.Markdown.Syntax;

namespace TestMarkdown.Syntax;

/// <summary>
/// 测试复制节点。
/// </summary>
[TestClass]
public class UnitTestClone
{
	/// <summary>
	/// 测试复制节点。
	/// </summary>
	[TestMethod]
	public void TestClone()
	{
		string content = File.ReadAllText("Syntax/AllSyntax.md");
		ParseOptions options = new()
		{
			UseLineLocator = true,
		};
		Document originDoc = Document.Parse(content, options);
		HtmlRenderer renderer = new();
		originDoc.Accept(renderer);
		string originHTML = renderer.ToString();

		NodeSetVisitor visitor = new();
		originDoc.Accept(visitor);
		HashSet<Node> originNodes = visitor.GetNodes();

		Document newDoc = (Document)originDoc.Clone();

		// 复制之后内容相同。
		renderer.Clear();
		newDoc.Accept(renderer);
		string newHTML = renderer.ToString();
		Assert.AreEqual(originHTML, newHTML);

		newDoc.Accept(visitor);
		HashSet<Node> newNodes = visitor.GetNodes();

		// 复制之后节点数相同，不会复制出额外的节点。
		Assert.AreEqual(originNodes.Count, newNodes.Count);
		// 所有节点都被复制了，与旧节点无交集。
		newNodes.IntersectWith(originNodes);
		Assert.AreEqual(newNodes.Count, 0);
	}
}

