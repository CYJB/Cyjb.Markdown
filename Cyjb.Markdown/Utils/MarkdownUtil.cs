namespace Cyjb.Markdown.Utils;

/// <summary>
/// Markdown 的工具类。
/// </summary>
internal static class MarkdownUtil
{
	/// <summary>
	/// Markdown 的空白字符。
	/// </summary>
	private const string Whitespace = " \t\r\n";

	/// <summary>
	/// 返回指定字符是否表示 Markdown 空白。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns><paramref name="ch"/> 是否表示 Markdown 空白。</returns>
	/// <remarks>Markdown 的空白只包含空格、Tab、\r 和 \n。</remarks>
	public static bool IsWhitespace(char ch)
	{
		return ch is ' ' or '\t' or '\r' or '\n';
	}

	/// <summary>
	/// 移除指定文本的起始空白。
	/// </summary>
	/// <param name="text">要移除起始空白的文本。</param>
	/// <returns>如果移除了任何起始空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TrimStart(ref ReadOnlySpan<char> text)
	{
		int len = text.Length;
		text = text.TrimStart(Whitespace);
		return text.Length < len;
	}

	/// <summary>
	/// 移除指定文本末尾的空白。
	/// </summary>
	/// <param name="text">要移除末尾空白的文本。</param>
	/// <returns>如果移除了任何末尾空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TrimEnd(ref ReadOnlySpan<char> text)
	{
		int len = text.Length;
		text = text.TrimEnd(Whitespace);
		return text.Length < len;
	}

	/// <summary>
	/// 获取当前文本是否是空白的（只包含空格、Tab、\r 或 \n）。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>如果当前文本是空白的，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool IsBlank(this ReadOnlySpan<char> text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (!IsWhitespace(text[i]))
			{
				return false;
			}
		}
		return true;
	}
}