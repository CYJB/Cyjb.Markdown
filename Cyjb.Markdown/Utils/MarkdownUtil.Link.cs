using System.Diagnostics.CodeAnalysis;
using Cyjb.Collections;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 链接中的空白字符。
	/// </summary>
	private static readonly ReadOnlyCharSet LinkWhitespace = ReadOnlyCharSet.FromRange("\x00\x20\x7F\x7F");

	/// <summary>
	/// 尝试解析链接目标。
	/// </summary>
	/// <param name="text">要解析的字符串。</param>
	/// <param name="destination">解析得到的链接目标</param>
	/// <returns>如果解析成功返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseLinkDestination(ref ReadOnlySpan<char> text,
		[MaybeNullWhen(false)] out string destination)
	{
		destination = null;
		int idx;
		if (text[0] == '<')
		{
			// <..> 形式，不能出现未转义的 < 和 >。
			idx = text.IndexOfUnescaped('>', '<', 1);
			if (idx >= 0)
			{
				destination = text[1..idx].Trim().Unescape();
				text = text[(idx + 1)..];
				return true;
			}
			else
			{
				return false;
			}
		}
		// 普通形式
		// 需要确保括号是成对的。
		int openParens = 0;
		for (idx = 0; idx < text.Length; idx++)
		{
			char ch = text[idx];
			if (ch == '\\' && idx + 1 < text.Length && Escapable.Contains(text[idx + 1]))
			{
				idx++;
			}
			else if (ch == '(')
			{
				openParens++;
			}
			else if (ch == ')')
			{
				if (openParens < 1)
				{
					// 在不成对的括号前返回。
					break;
				}
				openParens--;
			}
			else if (LinkWhitespace.Contains(ch))
			{
				break;
			}
		}
		if (idx == 0 && text[idx] != ')')
		{
			// 链接目标不能为空。
			return false;
		}
		if (openParens != 0)
		{
			// 括号不匹配。
			return false;
		}
		destination = text[..idx].Unescape();
		text = text[idx..];
		return true;
	}

	/// <summary>
	/// 尝试解析链接标题。
	/// </summary>
	/// <param name="text">要解析的字符串。</param>
	/// <param name="title">解析得到的链接标题</param>
	/// <returns>如果解析成功返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseLinkTitle(ref ReadOnlySpan<char> text, out string? title)
	{
		title = null;
		if (text.Length == 0)
		{
			return true;
		}
		char ch = text[0];
		int idx;
		switch (ch)
		{
			case '"':
			case '\'':
				idx = text.IndexOfUnescaped(ch, 1);
				break;
			case '(':
				idx = text.IndexOfUnescaped(')', '(', 1);
				break;
			default:
				return true;
		}
		if (idx >= 0)
		{
			title = text[1..idx].Unescape();
			text = text[(idx + 1)..];
			return true;
		}
		else
		{
			return false;
		}
	}
}
