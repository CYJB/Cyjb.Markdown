namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 检查指定文本是否是脚注标签。
	/// </summary>
	/// <param name="text">要检查的字符串。</param>
	/// <returns>如果指定文本是脚注标签，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool IsFootnotesLabel(ReadOnlySpan<char> text)
	{
		// 要求标签前是 ^。
		if (text[0] != '^')
		{
			return false;
		}
		text = text[1..];
		// 标签内不能包含空格、Tab 换行，或者未转义的 `[` 或 `]`。
		bool skip = false;
		foreach (char ch in text)
		{
			if (skip)
			{
				skip = false;
			}
			else if (ch == '\\')
			{
				skip = true;
			}
			else if (IsWhitespace(ch) || ch == '[' || ch == ']')
			{
				return false;
			}
		}
		return true;
	}
}
