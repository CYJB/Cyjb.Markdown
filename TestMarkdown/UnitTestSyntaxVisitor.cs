using System.Collections.Generic;
using System.Text;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// <see cref="SyntaxVisitor"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestSyntaxVisitor : BaseTest
{
	/// <summary>
	/// 测试接受访问器。
	/// </summary>
	[TestMethod]
	public void TestAccept()
	{
		Document doc = Document.Parse(SyntaxConstants.SyntaxMarkdown);

		TestVisitor1 visitor1 = new();
		visitor1.Visit(null);
		Assert.AreEqual("", visitor1.ToString());
		doc.Accept(visitor1);
		Assert.AreEqual(string.Join("", SyntaxConstants.SyntaxNames), visitor1.ToString());

		TestVisitor2 visitor2 = new();
		visitor2.Visit(null);
		Assert.AreEqual("", visitor2.ToString());
		doc.Accept(visitor2);
		Assert.AreEqual(string.Join("", SyntaxConstants.SyntaxNames), visitor2.ToString());
	}

	private class TestVisitor1 : SyntaxVisitor
	{
		private readonly StringBuilder text = new();

		/// <summary>
		/// 提供默认的访问行为。
		/// </summary>
		/// <param name="node">要访问的节点。</param>
		/// <returns>返回的结果。</returns>
		public override void DefaultVisit(Node node)
		{
			text.Append(node.GetType().Name);
			IReadOnlyList<Node> nodes;
			if (node is INodeContainer<BlockNode> blockContainer)
			{
				nodes = blockContainer.Children;
			}
			else if (node is INodeContainer<InlineNode> inlineContainer)
			{
				nodes = inlineContainer.Children;
			}
			else if (node is INodeContainer<ListItem> listContainer)
			{
				nodes = listContainer.Children;
			}
			else if (node is INodeContainer<TableRow> tableRowContainer)
			{
				nodes = tableRowContainer.Children;
			}
			else if (node is INodeContainer<TableCell> tableCellContainer)
			{
				nodes = tableCellContainer.Children;
			}
			else
			{
				return;
			}
			int count = nodes.Count;
			for (int i = 0; i < count; i++)
			{
				nodes[i].Accept(this);
			}
		}

		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return text.ToString();
		}
	}

	private class TestVisitor2 : SyntaxVisitor
	{
		private readonly StringBuilder text = new();

		/// <summary>
		/// 访问指定的文档节点。
		/// </summary>
		/// <param name="node">要访问的文档节点。</param>
		public override void VisitDocument(Document node)
		{
			text.Append("Document");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		#region 块节点

		/// <summary>
		/// 访问指定的分割线节点。
		/// </summary>
		/// <param name="node">要访问的分割线节点。</param>
		public override void VisitThematicBreak(ThematicBreak node)
		{
			text.Append("ThematicBreak");
		}

		/// <summary>
		/// 访问指定的标题节点。
		/// </summary>
		/// <param name="node">要访问的标题节点。</param>
		public override void VisitHeading(Heading node)
		{
			text.Append("Heading");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的代码块节点。
		/// </summary>
		/// <param name="node">要访问的代码块节点。</param>
		public override void VisitCodeBlock(CodeBlock node)
		{
			text.Append("CodeBlock");
		}

		/// <summary>
		/// 访问指定的 HTML 块节点。
		/// </summary>
		/// <param name="node">要访问的 HTML 块节点。</param>
		public override void VisitHtmlBlock(HtmlBlock node)
		{
			text.Append("HtmlBlock");
		}

		/// <summary>
		/// 访问指定的链接定义节点。
		/// </summary>
		/// <param name="node">要访问的链接定义节点。</param>
		public override void VisitLinkDefinition(LinkDefinition node)
		{
			text.Append("LinkDefinition");
		}

		/// <summary>
		/// 访问指定的段落节点。
		/// </summary>
		/// <param name="node">要访问的段落节点。</param>
		public override void VisitParagraph(Paragraph node)
		{
			text.Append("Paragraph");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的引用节点。
		/// </summary>
		/// <param name="node">要访问的引用节点。</param>
		public override void VisitBlockquote(Blockquote node)
		{
			text.Append("Blockquote");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的列表项节点。
		/// </summary>
		/// <param name="node">要访问的列表项节点。</param>
		public override void VisitListItem(ListItem node)
		{
			text.Append("ListItem");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的列表节点。
		/// </summary>
		/// <param name="node">要访问的列表节点。</param>
		public override void VisitList(List node)
		{
			text.Append("List");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的表格单元格。
		/// </summary>
		/// <param name="node">要访问的表格单元格节点。</param>
		public override void VisitTableCell(TableCell node)
		{
			text.Append("TableCell");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的表格行。
		/// </summary>
		/// <param name="node">要访问的表格行节点。</param>
		public override void VisitTableRow(TableRow node)
		{
			text.Append("TableRow");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的表格。
		/// </summary>
		/// <param name="node">要访问的表格节点。</param>
		public override void VisitTable(Table node)
		{
			text.Append("Table");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的数学公式块节点。
		/// </summary>
		/// <param name="node">要访问的数学公式块节点。</param>
		public override void VisitMathBlock(MathBlock node)
		{
			text.Append("MathBlock");
		}

		#endregion // 块节点

		#region 行内节点

		/// <summary>
		/// 访问指定的行内代码段节点。
		/// </summary>
		/// <param name="node">要访问的行内代码段节点。</param>
		public override void VisitCodeSpan(CodeSpan node)
		{
			text.Append("CodeSpan");
		}

		/// <summary>
		/// 访问指定的强调节点。
		/// </summary>
		/// <param name="node">要访问的强调节点。</param>
		public override void VisitEmphasis(Emphasis node)
		{
			text.Append("Emphasis");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的加粗节点。
		/// </summary>
		/// <param name="node">要访问的加粗节点。</param>
		public override void VisitStrong(Strong node)
		{
			text.Append("Strong");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的删除节点。
		/// </summary>
		/// <param name="node">要访问的删除节点。</param>
		public override void VisitStrikethrough(Strikethrough node)
		{
			text.Append("Strikethrough");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的链接节点。
		/// </summary>
		/// <param name="node">要访问的链接节点。</param>
		public override void VisitLink(Link node)
		{
			text.Append("Link");
			foreach (var child in node.Children)
			{
				child.Accept(this);
			}
		}

		/// <summary>
		/// 访问指定的行内 HTML 节点。
		/// </summary>
		/// <param name="node">要访问的行内 HTML 节点。</param>
		public override void VisitHtml(Html node)
		{
			text.Append("Html");
		}

		/// <summary>
		/// 访问指定的换行节点。
		/// </summary>
		/// <param name="node">要访问的换行节点。</param>
		public override void VisitBreak(Break node)
		{
			text.Append("Break");
		}

		/// <summary>
		/// 访问指定的表情符号节点。
		/// </summary>
		/// <param name="node">要访问的表情符号节点。</param>
		public override void VisitEmoji(Emoji node)
		{
			text.Append("Emoji");
		}

		/// <summary>
		/// 访问指定的行内数学公式节点。
		/// </summary>
		/// <param name="node">要访问的行内数学公式节点。</param>
		public override void VisitMathSpan(MathSpan node)
		{
			text.Append("MathSpan");
		}

		/// <summary>
		/// 访问指定的文本节点。
		/// </summary>
		/// <param name="node">要访问的文本节点。</param>
		public override void VisitLiteral(Literal node)
		{
			text.Append("Literal");
		}

		#endregion // 行内节点

		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return text.ToString();
		}
	}
}

