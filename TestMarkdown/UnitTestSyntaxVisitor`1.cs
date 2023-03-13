using System.Collections.Generic;
using System.Linq;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// <see cref="SyntaxVisitor{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestSyntaxVisitorT : BaseTest
{
	/// <summary>
	/// 测试接受访问器。
	/// </summary>
	[TestMethod]
	public void TestAccept()
	{
		Document doc = Document.Parse(SyntaxConstants.SyntaxMarkdown);

		TestVisitor1 visitor1 = new();
		Assert.AreEqual(null, visitor1.Visit(null));
		CollectionAssert.AreEquivalent(SyntaxConstants.SyntaxNames, doc.Accept(visitor1).ToArray());

		TestVisitor2 visitor2 = new();
		Assert.AreEqual(null, visitor2.Visit(null));
		CollectionAssert.AreEquivalent(SyntaxConstants.SyntaxNames, doc.Accept(visitor2).ToArray());
	}

	private class TestVisitor1 : SyntaxVisitor<IEnumerable<string>>
	{
		/// <summary>
		/// 提供默认的访问行为。
		/// </summary>
		/// <param name="node">要访问的节点。</param>
		/// <returns>返回的结果。</returns>
		public override IEnumerable<string> DefaultVisit(Node node)
		{
			yield return node.GetType().Name;
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
				yield break;
			}
			int count = nodes.Count;
			for (int i = 0; i < count; i++)
			{
				foreach (var child in nodes[i].Accept(this)!)
				{
					yield return child;
				}
			}
		}
	}

	private class TestVisitor2 : SyntaxVisitor<IEnumerable<string>>
	{
		/// <summary>
		/// 访问指定的文档节点。
		/// </summary>
		/// <param name="node">要访问的文档节点。</param>
		public override IEnumerable<string> VisitDocument(Document node)
		{
			yield return "Document";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		#region 块节点

		/// <summary>
		/// 访问指定的分割线节点。
		/// </summary>
		/// <param name="node">要访问的分割线节点。</param>
		public override IEnumerable<string> VisitThematicBreak(ThematicBreak node)
		{
			yield return "ThematicBreak";
		}

		/// <summary>
		/// 访问指定的标题节点。
		/// </summary>
		/// <param name="node">要访问的标题节点。</param>
		public override IEnumerable<string> VisitHeading(Heading node)
		{
			yield return "Heading";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的代码块节点。
		/// </summary>
		/// <param name="node">要访问的代码块节点。</param>
		public override IEnumerable<string> VisitCodeBlock(CodeBlock node)
		{
			yield return "CodeBlock";
		}

		/// <summary>
		/// 访问指定的 HTML 块节点。
		/// </summary>
		/// <param name="node">要访问的 HTML 块节点。</param>
		public override IEnumerable<string> VisitHtmlBlock(HtmlBlock node)
		{
			yield return "HtmlBlock";
		}

		/// <summary>
		/// 访问指定的链接定义节点。
		/// </summary>
		/// <param name="node">要访问的链接定义节点。</param>
		public override IEnumerable<string> VisitLinkDefinition(LinkDefinition node)
		{
			yield return "LinkDefinition";
		}

		/// <summary>
		/// 访问指定的段落节点。
		/// </summary>
		/// <param name="node">要访问的段落节点。</param>
		public override IEnumerable<string> VisitParagraph(Paragraph node)
		{
			yield return "Paragraph";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的引用节点。
		/// </summary>
		/// <param name="node">要访问的引用节点。</param>
		public override IEnumerable<string> VisitBlockquote(Blockquote node)
		{
			yield return "Blockquote";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的列表项节点。
		/// </summary>
		/// <param name="node">要访问的列表项节点。</param>
		public override IEnumerable<string> VisitListItem(ListItem node)
		{
			yield return "ListItem";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的列表节点。
		/// </summary>
		/// <param name="node">要访问的列表节点。</param>
		public override IEnumerable<string> VisitList(List node)
		{
			yield return "List";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的表格单元格。
		/// </summary>
		/// <param name="node">要访问的表格单元格节点。</param>
		public override IEnumerable<string> VisitTableCell(TableCell node)
		{
			yield return "TableCell";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的表格行。
		/// </summary>
		/// <param name="node">要访问的表格行节点。</param>
		public override IEnumerable<string> VisitTableRow(TableRow node)
		{
			yield return "TableRow";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的表格。
		/// </summary>
		/// <param name="node">要访问的表格节点。</param>
		public override IEnumerable<string> VisitTable(Table node)
		{
			yield return "Table";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的数学公式块节点。
		/// </summary>
		/// <param name="node">要访问的数学公式块节点。</param>
		public override IEnumerable<string> VisitMathBlock(MathBlock node)
		{
			yield return "MathBlock";
		}

		#endregion // 块节点

		#region 行内节点

		/// <summary>
		/// 访问指定的行内代码段节点。
		/// </summary>
		/// <param name="node">要访问的行内代码段节点。</param>
		public override IEnumerable<string> VisitCodeSpan(CodeSpan node)
		{
			yield return "CodeSpan";
		}

		/// <summary>
		/// 访问指定的强调节点。
		/// </summary>
		/// <param name="node">要访问的强调节点。</param>
		public override IEnumerable<string> VisitEmphasis(Emphasis node)
		{
			yield return "Emphasis";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的加粗节点。
		/// </summary>
		/// <param name="node">要访问的加粗节点。</param>
		public override IEnumerable<string> VisitStrong(Strong node)
		{
			yield return "Strong";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的删除节点。
		/// </summary>
		/// <param name="node">要访问的删除节点。</param>
		public override IEnumerable<string> VisitStrikethrough(Strikethrough node)
		{
			yield return "Strikethrough";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的链接节点。
		/// </summary>
		/// <param name="node">要访问的链接节点。</param>
		public override IEnumerable<string> VisitLink(Link node)
		{
			yield return "Link";
			foreach (var child in node.Children)
			{
				foreach (string text in child.Accept(this)!)
				{
					yield return text;
				}
			}
		}

		/// <summary>
		/// 访问指定的行内 HTML 节点。
		/// </summary>
		/// <param name="node">要访问的行内 HTML 节点。</param>
		public override IEnumerable<string> VisitHtml(Html node)
		{
			yield return "Html";
		}

		/// <summary>
		/// 访问指定的换行节点。
		/// </summary>
		/// <param name="node">要访问的换行节点。</param>
		public override IEnumerable<string> VisitBreak(Break node)
		{
			yield return "Break";
		}

		/// <summary>
		/// 访问指定的表情符号节点。
		/// </summary>
		/// <param name="node">要访问的表情符号节点。</param>
		public override IEnumerable<string> VisitEmoji(Emoji node)
		{
			yield return "Emoji";
		}

		/// <summary>
		/// 访问指定的行内数学公式节点。
		/// </summary>
		/// <param name="node">要访问的行内数学公式节点。</param>
		public override IEnumerable<string> VisitMathSpan(MathSpan node)
		{
			yield return "MathSpan";
		}

		/// <summary>
		/// 访问指定的文本节点。
		/// </summary>
		/// <param name="node">要访问的文本节点。</param>
		public override IEnumerable<string> VisitLiteral(Literal node)
		{
			yield return "Literal";
		}

		#endregion // 行内节点

	}
}

