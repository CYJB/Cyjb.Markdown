using System.Text.RegularExpressions;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 HTML 的信息。
/// </summary>
internal sealed class HtmlInfo
{
	/// <summary>
	/// HTML 成对标签的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlPair = new(true,
		new Regex("</(?:script|pre|style|textarea)>", RegexOptions.IgnoreCase));
	/// <summary>
	/// HTML 注释的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlComment = new(true, new Regex("-->"));
	/// <summary>
	/// HTML 处理结构的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlProcessing = new(true, new Regex(@"\?>"));
	/// <summary>
	/// HTML 声明的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlDeclaration = new(true, new Regex(">"));
	/// <summary>
	/// HTML CDATA 段的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlCData = new(true, new Regex(@"\]\]>"));
	/// <summary>
	/// HTML 单独标签的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlSingle = new(true, null);
	/// <summary>
	/// HTML 其它标签的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlOther = new(false, null);

	/// <summary>
	/// 使用指定的 HTML 信息初始化 <see cref="HtmlInfo"/> 类的新实例。
	/// </summary>
	/// <param name="canInterruptParagraph">是否能够中断段落。</param>
	/// <param name="closeRegex">闭合的正则表达式。</param>
	private HtmlInfo(bool canInterruptParagraph, Regex? closeRegex)
	{
		CanInterruptParagraph = canInterruptParagraph;
		CloseRegex = closeRegex;
	}

	/// <summary>
	/// 获取是否能够中断段落。
	/// </summary>
	public bool CanInterruptParagraph { get; }
	/// <summary>
	/// 获取闭合的正则表达式，<c>null</c> 表示空行。
	/// </summary>
	public Regex? CloseRegex { get; }
}
