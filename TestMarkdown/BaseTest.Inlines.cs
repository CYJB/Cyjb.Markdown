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
	/// 验证是文本。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	/// <param name="span">预期的文本范围。</param>
	protected void Literal(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Literal, node.Kind);
		Assert.AreEqual(span, node.Span);

		Literal textNode = (Literal)node!;
		Assert.AreEqual(content, textNode.Content);
	}

	/// <summary>
	/// 验证是硬换行。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	protected void HardBreak(TextSpan span)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HardBreak, node.Kind);
		Assert.AreEqual(span, node.Span);
	}

	/// <summary>
	/// 验证是软换行。
	/// </summary>
	/// <param name="span">节点的文本范围。</param>
	protected void SoftBreak(TextSpan span)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.SoftBreak, node.Kind);
		Assert.AreEqual(span, node.Span);
	}

	/// <summary>
	/// 验证是行内代码片段。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="content">预期的代码内容。</param>
	protected void CodeSpan(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.CodeSpan, node.Kind);
		Assert.AreEqual(span, node.Span);

		CodeSpan codeSnap = (CodeSpan)node!;
		Assert.AreEqual(content, codeSnap.Content);
	}

	/// <summary>
	/// 验证是链接。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="url">预期的链接 URL。</param>
	/// <param name="title">预期的链接标题。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Link(TextSpan span, string url, string? title, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Link, node.Kind);
		Assert.AreEqual(span, node.Span);

		Link link = (Link)node!;
		Assert.AreEqual(url, link.URL);
		Assert.AreEqual(title, link.Title);
		if (validator == null)
		{
			Assert.AreEqual(0, link.Children.Count);
		}
		else
		{
			AssertChildren(node, link.Children, validator);
		}
	}

	/// <summary>
	/// 验证是图片。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="url">预期的链接 URL。</param>
	/// <param name="title">预期的链接标题。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Image(TextSpan span, string? url, string? title, Action? validator = null)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Image, node.Kind);
		Assert.AreEqual(span, node.Span);

		Link link = (Link)node!;
		Assert.AreEqual(url, link.URL);
		Assert.AreEqual(title, link.Title);
		if (validator == null)
		{
			Assert.AreEqual(0, link.Children.Count);
		}
		else
		{
			AssertChildren(node, link.Children, validator);
		}
	}

	/// <summary>
	/// 验证是 HTML 起始标签。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="fullText">预期的 HTML 标签完整文本。</param>
	protected void HtmlStartTag(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlStartTag, node.Kind);
		Assert.AreEqual(span, node.Span);

		Html html = (Html)node!;
		Assert.AreEqual(content, html.Content);
	}

	/// <summary>
	/// 验证是 HTML 结束标签。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="fullText">预期的 HTML 标签完整文本。</param>
	protected void HtmlEndTag(TextSpan span, string fullText)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlEndTag, node.Kind);
		Assert.AreEqual(span, node.Span);

		Html html = (Html)node!;
		Assert.AreEqual(fullText, html.Content);
	}

	/// <summary>
	/// 验证是 HTML 注释。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="fullText">预期的 HTML 标签完整文本。</param>
	protected void HtmlComment(TextSpan span, string fullText)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlComment, node.Kind);
		Assert.AreEqual(span, node.Span);

		Html html = (Html)node!;
		Assert.AreEqual(fullText, html.Content);
	}

	/// <summary>
	/// 验证是 HTML 处理结构。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="fullText">预期的 HTML 标签完整文本。</param>
	protected void HtmlProcessing(TextSpan span, string fullText)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlProcessing, node.Kind);
		Assert.AreEqual(span, node.Span);

		Html html = (Html)node!;
		Assert.AreEqual(fullText, html.Content);
	}

	/// <summary>
	/// 验证是 HTML 声明。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="fullText">预期的 HTML 标签完整文本。</param>
	protected void HtmlDeclaration(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlDeclaration, node.Kind);
		Assert.AreEqual(span, node.Span);

		Html html = (Html)node!;
		Assert.AreEqual(content, html.Content);
	}

	/// <summary>
	/// 验证是 HTML CDATA 片段。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="fullText">预期的 HTML 标签完整文本。</param>
	protected void HtmlCData(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.HtmlCData, node.Kind);
		Assert.AreEqual(span, node.Span);

		Html html = (Html)node!;
		Assert.AreEqual(content, html.Content);
	}

	/// <summary>
	/// 验证是强调。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Emphasis(TextSpan span, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Emphasis, node.Kind);
		Assert.AreEqual(span, node.Span);

		AssertChildren(node, ((Emphasis)node).Children, validator);
	}

	/// <summary>
	/// 验证是加粗。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Strong(TextSpan span, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Strong, node.Kind);
		Assert.AreEqual(span, node.Span);

		AssertChildren(node, ((Strong)node).Children, validator);
	}

	/// <summary>
	/// 验证是删除。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void Strikethrough(TextSpan span, Action validator)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Strikethrough, node.Kind);
		Assert.AreEqual(span, node.Span);

		AssertChildren(node, ((Strikethrough)node).Children, validator);
	}

	/// <summary>
	/// 验证是表情符号。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="code">表情符号代码。</param>
	protected void Emoji(TextSpan span, string code)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.Emoji, node.Kind);
		Assert.AreEqual(span, node.Span);

		Emoji emoji = (Emoji)node!;
		Assert.AreEqual(code, emoji.Code);
	}

	/// <summary>
	/// 验证是行内数学公式。
	/// </summary>
	/// <param name="span">预期的文本范围。</param>
	/// <param name="content">预期的数学公式内容。</param>
	protected void MathSpan(TextSpan span, string content)
	{
		Node node = Next();
		Assert.AreEqual(MarkdownKind.MathSpan, node.Kind);
		Assert.AreEqual(span, node.Span);

		MathSpan mathSnap = (MathSpan)node!;
		Assert.AreEqual(content, mathSnap.Content);
	}
}
