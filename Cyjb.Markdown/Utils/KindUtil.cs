using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Utils;

/// <summary>
/// 提供 <see cref="MarkdownKind"/> 的扩展方法。
/// </summary>
internal static class KindUtil
{
	/// <summary>
	/// 返回指定类型是否表示行内 Html。
	/// </summary>
	/// <param name="kind">要检查的类型。</param>
	/// <returns>如果指定类型表示行内 Html，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool IsInlineHtml(this MarkdownKind kind)
	{
		return kind is MarkdownKind.HtmlStartTag
			or MarkdownKind.HtmlEndTag
			or MarkdownKind.HtmlComment
			or MarkdownKind.HtmlCData
			or MarkdownKind.HtmlDeclaration
			or MarkdownKind.HtmlProcessing;
	}
}
