using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 表示链接体的信息。
/// </summary>
internal class LinkBody
{
	/// <summary>
	/// 链接的 URL。
	/// </summary>
	public readonly string URL;
	/// <summary>
	/// 链接的标题。
	/// </summary>
	public readonly string? Title;
	/// <summary>
	/// 链接的属性。
	/// </summary>
	public readonly HtmlAttributeList Attributes = new();

	/// <summary>
	/// 使用指定的信息初始化 <see cref="LinkBody"/> 类的新实例。
	/// </summary>
	/// <param name="url">链接的 URL。</param>
	/// <param name="title">链接的标题。</param>
	public LinkBody(string url, string? title)
	{
		URL = url;
		Title = title;
	}
}
