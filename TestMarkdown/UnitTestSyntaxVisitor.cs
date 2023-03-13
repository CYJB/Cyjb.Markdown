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
public class UnitTestSyntaxVisitor : BaseTest
{
	/// <summary>
	/// 测试接受访问器。
	/// </summary>
	[TestMethod]
	public void TestAccept()
	{
		Document doc = Document.Parse(@"---
# heading
```foo
bar
```
<script></script>

[foo]:/url

> - baz
> - bim

| h1 | h2 |
|:--:| -- |
| c1 | c2 |

$$
math
$$

[foo] `bar` _a **b** c_ ~~d~~ <test>
:atom: $3$");
		TestVisitor visitor = new();
		CollectionAssert.AreEquivalent(new string[]
		{
			"Document",
			"ThematicBreak",
			"Heading",
			"Literal",
			"CodeBlock",
			"HtmlBlock",
			"LinkDefinition",
			"Blockquote",
			"List",
			"ListItem",
			"Paragraph",
			"Literal",
			"ListItem",
			"Paragraph",
			"Literal",
			"Table",
			"TableRow",
			"TableCell",
			"Literal",
			"TableCell",
			"Literal",
			"TableRow",
			"TableCell",
			"Literal",
			"TableCell",
			"Literal",
			"MathBlock",
			"Paragraph",
			"Link",
			"Literal",
			"Literal",
			"CodeSpan",
			"Literal",
			"Emphasis",
			"Literal",
			"Strong",
			"Literal",
			"Literal",
			"Literal",
			"Strikethrough",
			"Literal",
			"Literal",
			"Html",
			"Break",
			"Emoji",
			"Literal",
			"MathSpan",
		}, doc.Accept(visitor).ToArray());
	}

	private class TestVisitor : SyntaxVisitor<IEnumerable<string>>
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

