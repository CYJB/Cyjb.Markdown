using Cyjb.Collections;

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
	/// Markdown 的空白字符。
	/// </summary>
	public static readonly char[] WhitespaceChars = new char[] { ' ', '\t', '\r', '\n' };

	/// <summary>
	/// Markdown 的空白字符。
	/// </summary>
	public static readonly char[] WhitespaceCharsWithoutNewLine = new char[] { ' ', '\t' };

	/// <summary>
	/// Unicode 标点符号的集合。
	/// </summary>
	private static readonly ReadOnlyCharSet UnicodePunctuations = GetUnicodePunctuations();

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
		return UnicodePunctuations.Contains(ch);
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
	/// 移除指定文本的起始空白。
	/// </summary>
	/// <param name="text">要移除起始空白的文本。</param>
	/// <returns>如果移除了任何起始空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TrimStart(ref StringView text)
	{
		int len = text.Length;
		text = text.TrimStart(WhitespaceChars);
		return text.Length < len;
	}

	/// <summary>
	/// 返回 Unicode 标点符号的集合。
	/// </summary>
	/// <remarks>包含 Unicode P 和 S 类别。</remarks>
	private static ReadOnlyCharSet GetUnicodePunctuations()
	{
		CharSet set = new();
		for (int i = 0; i <= char.MaxValue; i++)
		{
			char ch = (char)i;
			if (char.IsPunctuation(ch) || char.IsSymbol(ch))
			{
				set.Add(ch);
			}
		}
		return set.AsReadOnly();
	}
}
