using System;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 用于单元测试的基类。
/// </summary>
public abstract partial class BaseTest
{
	/// <summary>
	/// 验证是块引用。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Blockquote(TextSpan span, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Blockquote, node.Kind);
		Assert.AreEqual(span, node.Span);

		Blockquote blockquote = (Blockquote)node!;
		if (validator == null)
		{
			Assert.AreEqual(0, blockquote.Children.Count);
		}
		else
		{
			AssertChildren(node, blockquote.Children, validator);
		}
	}

	/// <summary>
	/// 验证是段落。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Paragraph(TextSpan span, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Paragraph, node.Kind);
		Assert.AreEqual(span, node.Span);

		AssertChildren(node, ((Paragraph)node).Children, validator);
	}

	/// <summary>
	/// 验证是分割线。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	protected void ThematicBreak(TextSpan span)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.ThematicBreak, node.Kind);
		Assert.AreEqual(span, node.Span);
	}

	/// <summary>
	/// 验证是标题。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="depth">标题的深度。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Heading(TextSpan span, int depth, Action? validator = null)
	{
		Heading(span, depth, new HtmlAttributeList(), validator);
	}

	/// <summary>
	/// 验证是包含指定属性的标题。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="depth">标题的深度。</param>
	/// <param name="attrs">标题的属性。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Heading(TextSpan span, int depth, HtmlAttributeList attrs, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Heading, node.Kind);
		Assert.AreEqual(span, node.Span);

		Heading heading = (node as Heading)!;
		Assert.AreEqual(depth, heading.Depth);
		CollectionAssert.AreEquivalent(attrs, heading.Attributes);
		if (validator == null)
		{
			Assert.AreEqual(0, heading.Children.Count);
		}
		else
		{
			AssertChildren(node, heading.Children, validator);
		}
	}

	/// <summary>
	/// 验证是链接定义。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="label">预期的链接的标签。</param>
	/// <param name="url">预期的链接的 URL。</param>
	/// <param name="title">预期的链接的标题。</param>
	/// <param name="attrs">预期的链接的属性。</param>
	protected void LinkDefinition(TextSpan span, string label, string url, string? title = null,
		HtmlAttributeList? attrs = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.LinkDefinition, node.Kind);
		Assert.AreEqual(span, node.Span);

		LinkDefinition link = (LinkDefinition)node!;
		Assert.AreEqual(label, link.Label);
		Assert.AreEqual(url, link.URL);
		Assert.AreEqual(title, link.Title);
		if (attrs == null)
		{
			Assert.AreEqual(0, link.Attributes.Count);
		}
		else
		{
			CollectionAssert.AreEquivalent(attrs, link.Attributes);
		}
	}

	/// <summary>
	/// 验证是代码块。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="content">代码块的内容。</param>
	/// <param name="info">代码块的其它信息。</param>
	/// <param name="attrs">代码块的属性。</param>
	protected void CodeBlock(TextSpan span, string content, string? info = null, HtmlAttributeList? attrs = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.CodeBlock, node.Kind);
		Assert.AreEqual(span, node.Span);

		CodeBlock code = (CodeBlock)node!;
		Assert.AreEqual(content, code.Content);
		Assert.AreEqual(info, code.Info);
		if (attrs == null)
		{
			Assert.AreEqual(0, code.Attributes.Count);
		}
		else
		{
			CollectionAssert.AreEquivalent(attrs, code.Attributes);
		}
	}

	/// <summary>
	/// 验证是 HTML 块。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="content">代码块的内容。</param>
	protected void HtmlBlock(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlBlock, node.Kind);
		Assert.AreEqual(span, node.Span);

		HtmlBlock code = (HtmlBlock)node!;
		Assert.AreEqual(content, code.Content);
	}

	/// <summary>
	/// 验证是列表项。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void ListItem(TextSpan span, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.ListItem, node.Kind);
		Assert.AreEqual(span, node.Span);

		ListItem item = (node as ListItem)!;
		Assert.IsNull(item.Checked);
		if (validator == null)
		{
			Assert.AreEqual(0, item.Children.Count);
		}
		else
		{
			AssertChildren(node, item.Children, validator);
		}
	}

	/// <summary>
	/// 验证是任务列表项。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="isChecked">任务列表项是否是选中的。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void TaskListItem(TextSpan span, bool isChecked, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.ListItem, node.Kind);
		Assert.AreEqual(span, node.Span);

		ListItem item = (node as ListItem)!;
		Assert.AreEqual(isChecked, item.Checked);
		AssertChildren(node, item.Children, validator);
	}

	/// <summary>
	/// 验证是可选中的列表项。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="check">列表项的选中状态。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void ListCheckItem(TextSpan span, bool check, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.ListItem, node.Kind);
		Assert.AreEqual(span, node.Span);

		ListItem item = (node as ListItem)!;
		Assert.AreEqual(check, item.Checked);
		if (validator == null)
		{
			Assert.AreEqual(0, item.Children.Count);
		}
		else
		{
			AssertChildren(node, item.Children, validator);
		}
	}

	/// <summary>
	/// 验证是无序列表项。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="loose">列表是否是松散的。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void UnorderedList(TextSpan span, bool loose, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.List, node.Kind);
		Assert.AreEqual(span, node.Span);

		List list = (node as List)!;
		Assert.AreEqual(loose, list.Loose);
		Assert.AreEqual(ListStyleType.Unordered, list.StyleType, "{0} 应当是无序的。", node);
		AssertChildren(node, list.Children, validator);
	}

	/// <summary>
	/// 验证是有序数字列表项。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="loose">列表是否是松散的。</param>
	/// <param name="start">列表的起始值。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void OrderedNumberList(TextSpan span, bool loose, int start, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.List, node.Kind);
		Assert.AreEqual(span, node.Span);

		List list = (node as List)!;
		Assert.AreEqual(loose, list.Loose);
		Assert.AreEqual(ListStyleType.OrderedNumber, list.StyleType);
		Assert.AreEqual(start, list.Start);
		AssertChildren(node, list.Children, validator);
	}

	/// <summary>
	/// 验证是有序数字列表项。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="loose">列表是否是松散的。</param>
	/// <param name="styleType">列表的样式类型。</param>
	/// <param name="start">列表的起始值。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void ExtraList(TextSpan span, bool loose, ListStyleType styleType, int start, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.List, node.Kind);
		Assert.AreEqual(span, node.Span);

		List list = (node as List)!;
		Assert.AreEqual(loose, list.Loose);
		Assert.AreEqual(styleType, list.StyleType);
		Assert.AreEqual(start, list.Start);
		AssertChildren(node, list.Children, validator);
	}

	/// <summary>
	/// 验证是表格。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="aligns">表格的对齐。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Table(TextSpan span, TableAlign[] aligns, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Table, node.Kind);
		Assert.AreEqual(span, node.Span);

		Table table = (node as Table)!;
		CollectionAssert.AreEqual(aligns, table.Aligns);
		AssertChildren(node, table.Children, validator);
	}

	/// <summary>
	/// 验证是表格行。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void TableRow(TextSpan span, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.TableRow, node.Kind);
		Assert.AreEqual(span, node.Span);

		TableRow row = (node as TableRow)!;
		AssertChildren(node, row.Children, validator);
	}

	/// <summary>
	/// 验证是表格的单元格。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void TableCell(TextSpan span, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.TableCell, node.Kind);
		Assert.AreEqual(span, node.Span);

		TableCell cell = (node as TableCell)!;
		if (validator == null)
		{
			Assert.AreEqual(0, cell.Children.Count);
		}
		else
		{
			AssertChildren(node, cell.Children, validator);
		}
	}

	/// <summary>
	/// 验证是数学公式块。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="content">数学公式块的内容。</param>
	/// <param name="info">数学公式块的其它信息。</param>
	/// <param name="attrs">数学公式块的属性。</param>
	protected void MathBlock(TextSpan span, string content, string? info = null, HtmlAttributeList? attrs = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.MathBlock, node.Kind);
		Assert.AreEqual(span, node.Span);

		MathBlock math = (MathBlock)node!;
		if (attrs == null)
		{
			Assert.AreEqual(0, math.Attributes.Count);
		}
		else
		{
			CollectionAssert.AreEquivalent(attrs, math.Attributes);
		}
		Assert.AreEqual(content, math.Content);
		Assert.AreEqual(info, math.Info);
	}

	/// <summary>
	/// 验证是脚注。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="label">脚注的标签。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Footnote(TextSpan span, string label, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Footnote, node.Kind);
		Assert.AreEqual(span, node.Span);

		Footnote footnote = (Footnote)node!;
		Assert.AreEqual(label, footnote.Label);
		if (validator == null)
		{
			Assert.AreEqual(0, footnote.Children.Count);
		}
		else
		{
			AssertChildren(node, footnote.Children, validator);
		}
	}

	/// <summary>
	/// 验证是自定义容器。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="info">自定义容器的信息。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void CustomContainer(TextSpan span, string? info = null, Action? validator = null)
	{
		CustomContainer(span, info, new HtmlAttributeList(), validator);
	}

	/// <summary>
	/// 验证是自定义容器。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="info">自定义容器的信息。</param>
	/// <param name="attrs">自定义容器的属性。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void CustomContainer(TextSpan span, string? info, HtmlAttributeList attrs, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.CustomContainer, node.Kind);
		Assert.AreEqual(span, node.Span);

		CustomContainer container = (CustomContainer)node!;
		Assert.AreEqual(info, container.Info);
		CollectionAssert.AreEquivalent(attrs, container.Attributes);
		if (validator == null)
		{
			Assert.AreEqual(0, container.Children.Count);
		}
		else
		{
			AssertChildren(node, container.Children, validator);
		}
	}
}
