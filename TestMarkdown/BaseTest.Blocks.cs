using System;
using Cyjb.Markdown;
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
	/// 验证是引用。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Quote(TextSpan span, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Quote, node.Kind);
		Assert.AreEqual(span, node.Span);

		Quote quote = (Quote)node!;
		if (validator == null)
		{
			Assert.AreEqual(0, quote.Children.Count);
		}
		else
		{
			AssertChildren(node, quote.Children, validator);
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
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Heading, node.Kind);
		Assert.AreEqual(span, node.Span);

		Heading heading = (node as Heading)!;
		Assert.AreEqual(depth, heading.Depth);
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
	protected void LinkDefinition(TextSpan span, string label, string url, string? title = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.LinkDefinition, node.Kind);
		Assert.AreEqual(span, node.Span);

		LinkDefinition link = (LinkDefinition)node!;
		Assert.AreEqual(label, link.Label);
		Assert.AreEqual(url, link.URL);
		Assert.AreEqual(title, link.Title);
	}

	/// <summary>
	/// 验证是代码块。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="content">代码块的内容。</param>
	/// <param name="info">代码块的其它信息。</param>
	protected void CodeBlock(TextSpan span, string content, string? info = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.CodeBlock, node.Kind);
		Assert.AreEqual(span, node.Span);

		CodeBlock code = (CodeBlock)node!;
		Assert.AreEqual(content, code.Content);
		Assert.AreEqual(info, code.Info);
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
}