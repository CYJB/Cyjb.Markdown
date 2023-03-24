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

	/// <summary>
	/// 测试从父节点移除。
	/// </summary>
	[TestMethod]
	public void TestRemove()
	{
		Document doc = new();
		Assert.IsNull(doc.Parent);
		doc.Remove();
		Assert.IsNull(doc.Parent);

		// 测试块节点。
		foreach (BlockNode node in new BlockNode[]{
			new ThematicBreak(),
			new Heading(1),
			new CodeBlock(""),
			new HtmlBlock("<a>"),
			new LinkDefinition("link", ""),
			new Paragraph(),
			new Blockquote(),
			new List(ListStyleType.Unordered),
			new Table(new TableRow(new TableCell())),
			new MathBlock(""),
			new Footnote("footnote"),
		})
		{
			TestRemove(node, doc);
		}

		Paragraph paragraph = new();
		// 测试行级节点
		foreach (InlineNode node in new InlineNode[]{
			new CodeSpan(""),
			new Emphasis(),
			new Strong(),
			new Strikethrough(),
			new Link(false, ""),
			new Html(MarkdownKind.HtmlStartTag, "<a>"),
			new Break(false),
			Emoji.GetEmoji("ok")!,
			new MathSpan(""),
			new FootnoteRef(new Footnote("footnote")),
			new Literal(""),
		})
		{
			TestRemove(node, paragraph);
		}

		// 测试 ListItem
		TestRemove(new ListItem(), new List(ListStyleType.Unordered));
		// 测试 TableRow
		TestRemove(new TableRow(new TableCell()), new Table(new TableRow(new TableCell())), 1);
		// 测试 TableCell
		TestRemove(new TableCell(), new TableRow(new TableCell()), 1);
	}

	/// <summary>
	/// 测试指定节点从父节点移除的动作。
	/// </summary>
	private static void TestRemove<TNode>(TNode node, INodeContainer<TNode> parent, int count = 0)
		where TNode : Node
	{
		Assert.IsNull(node.Parent);
		node.Remove();
		Assert.IsNull(node.Parent);

		Assert.AreEqual(count, parent.Children.Count);
		parent.Children.Add(node);
		Assert.AreEqual(parent, node.Parent);
		Assert.AreEqual(count + 1, parent.Children.Count);
		node.Remove();
		Assert.IsNull(node.Parent);
		Assert.AreEqual(count, parent.Children.Count);
	}
}
