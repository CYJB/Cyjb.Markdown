namespace Cyjb.Markdown.Utils;

/// <summary>
/// Markdown 的工具类。
/// </summary>
internal static partial class MarkdownUtil
{
	/// <summary>
	/// Markdown 的空白字符。
	/// </summary>
	public const string Whitespace = " \t\r\n";

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
	/// 返回指定字符是否是 Markdown 的标点符号。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>指定字符是否是 Markdown 的标点符号。</returns>
	public static bool IsPunctuation(char ch)
	{
		return char.IsPunctuation(ch) ||
			ch == '$' || ch == '+' || ch == '<' || ch == '=' ||
			ch == '>' || ch == '^' || ch == '`' || ch == '|' ||
			ch == '~' || ch == '\'';
	}

	/// <summary>
	/// 返回指定字符序列中，指定字符首次出现的索引。
	/// </summary>
	/// <param name="text">要检查的字符序列。</param>
	/// <param name="ch">要检查的字符。</param>
	/// <param name="startIndex">要检查的起始索引。</param>
	/// <returns><paramref name="ch"/> 首次出现的索引。</returns>
	public static int IndexOf(this ReadOnlySpan<char> text, char ch, int startIndex)
	{
		for (int i = startIndex; i < text.Length; i++)
		{
			char cur = text[i];
			if (cur == ch)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 寻找指定文本中空白字符首次出现的位置。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>如果找到了任何空白字符，则为字符的索引；否则为 <c>-1</c>。</returns>
	public static int IndexOfWhitespace(ReadOnlySpan<char> text)
	{
		return text.IndexOfAny(Whitespace);
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
	/// 移除指定文本的起始和尾随空白。
	/// </summary>
	/// <param name="text">要移除起始和尾随空白的文本。</param>
	/// <returns>如果移除了任何起始和尾随空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool Trim(ref ReadOnlySpan<char> text)
	{
		int len = text.Length;
		text = text.TrimStart(Whitespace).TrimEnd(Whitespace);
		return text.Length < len;
	}

	/// <summary>
	/// 移除指定文本的起始和尾随空白。
	/// </summary>
	/// <param name="text">要移除起始和尾随空白的文本。</param>
	/// <returns>移除了起始和尾随空白后的文本。</returns>
	public static string Trim(string text)
	{
		ReadOnlySpan<char> span = text;
		if (Trim(ref span))
		{
			return span.ToString();
		}
		else
		{
			return text;
		}
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
