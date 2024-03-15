using System.Text;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 HTML 的信息。
/// </summary>
internal sealed class HtmlInfo
{
	/// <summary>
	/// HTML 成对标签的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlPair = new(true, false, (StringBuilder sb, int start) =>
	{
		int index = sb.IndexOf("</", start);
		// 忽略大小写匹配 </(script|pre|style|textarea)>
		while (index >= 0)
		{
			int endIndex = sb.IndexOf('>', index);
			switch (endIndex - index)
			{
				case 5:
					// </pre>
					return char.ToLowerInvariant(sb[index + 2]) == 'p' &&
						   char.ToLowerInvariant(sb[index + 3]) == 'r' &&
						   char.ToLowerInvariant(sb[index + 4]) == 'e';
				case 7:
					// </style>
					return char.ToLowerInvariant(sb[index + 2]) == 's' &&
						   char.ToLowerInvariant(sb[index + 3]) == 't' &&
						   char.ToLowerInvariant(sb[index + 4]) == 'y' &&
						   char.ToLowerInvariant(sb[index + 5]) == 'l' &&
						   char.ToLowerInvariant(sb[index + 6]) == 'e';
				case 8:
					// </script>
					return char.ToLowerInvariant(sb[index + 2]) == 's' &&
						   char.ToLowerInvariant(sb[index + 3]) == 'c' &&
						   char.ToLowerInvariant(sb[index + 4]) == 'r' &&
						   char.ToLowerInvariant(sb[index + 5]) == 'i' &&
						   char.ToLowerInvariant(sb[index + 6]) == 'p' &&
						   char.ToLowerInvariant(sb[index + 7]) == 't';
				case 10:
					// </textarea>
					return char.ToLowerInvariant(sb[index + 2]) == 't' &&
						   char.ToLowerInvariant(sb[index + 3]) == 'e' &&
						   char.ToLowerInvariant(sb[index + 4]) == 'x' &&
						   char.ToLowerInvariant(sb[index + 5]) == 't' &&
						   char.ToLowerInvariant(sb[index + 6]) == 'a' &&
						   char.ToLowerInvariant(sb[index + 7]) == 'r' &&
						   char.ToLowerInvariant(sb[index + 8]) == 'e' &&
						   char.ToLowerInvariant(sb[index + 9]) == 'a';
			}
			index = sb.IndexOf("</", index + 2);
		}
		return false;
	});
	/// <summary>
	/// HTML 注释的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlComment = new(true, false, (StringBuilder sb, int start) =>
		sb.IndexOf("-->", start) >= 0);
	/// <summary>
	/// HTML 处理结构的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlProcessing = new(true, false, (StringBuilder sb, int start) =>
		sb.IndexOf("?>", start) >= 0);
	/// <summary>
	/// HTML 声明的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlDeclaration = new(true, false, (StringBuilder sb, int start) =>
		sb.IndexOf('>', start) >= 0);
	/// <summary>
	/// HTML CDATA 段的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlCData = new(true, false, (StringBuilder sb, int start) =>
		sb.IndexOf("]]>", start) >= 0);
	/// <summary>
	/// HTML 单独标签的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlSingle = new(true, true, (StringBuilder sb, int start) => false);
	/// <summary>
	/// HTML 其它标签的信息。
	/// </summary>
	public static readonly HtmlInfo HtmlOther = new(false, true, (StringBuilder sb, int start) => false);

	/// <summary>
	/// 使用指定的 HTML 信息初始化 <see cref="HtmlInfo"/> 类的新实例。
	/// </summary>
	/// <param name="canInterruptParagraph">是否能够中断段落。</param>
	/// <param name="closeByBlankLine">是否在空行处闭合。</param>
	/// <param name="isClosed">检查是否闭合的回调。</param>
	private HtmlInfo(bool canInterruptParagraph, bool closeByBlankLine, Func<StringBuilder, int, bool> isClosed)
	{
		CanInterruptParagraph = canInterruptParagraph;
		CloseByBlankLine = closeByBlankLine;
		IsClosed = isClosed;
	}

	/// <summary>
	/// 获取是否能够中断段落。
	/// </summary>
	public bool CanInterruptParagraph { get; }
	/// <summary>
	/// 获取是否在空行处闭合。
	/// </summary>
	public bool CloseByBlankLine { get; }
	/// <summary>
	/// 检查 HTML 标签是否闭合的回调。
	/// </summary>
	public Func<StringBuilder, int, bool> IsClosed { get; }
}
