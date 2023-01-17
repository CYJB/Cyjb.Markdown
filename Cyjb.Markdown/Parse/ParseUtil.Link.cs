using System.Diagnostics.CodeAnalysis;
using Cyjb.Collections;

namespace Cyjb.Markdown.Parse;

internal static partial class ParseUtil
{
	/// <summary>
	/// 空白字符。
	/// </summary>
	private static readonly CharSet Whitespace = new("\x7F ");

	static ParseUtil()
	{
		Whitespace.Add('\x00', '\x1F');
	}

	/// <summary>
	/// 解析链接标签。
	/// </summary>
	/// <param name="text">要解析的字符串。</param>
	/// <param name="label">解析得到的链接标签。</param>
	/// <returns>如果解析成功，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseLinkLabel(ref ReadOnlySpan<char> text, [MaybeNullWhen(false)] out string label)
	{
		int idx = text.IndexOfUnescaped(']');
		if (idx > 1000)
		{
			// [ 和 ] 之间最多允许 999 个字符。
			label = null;
			return false;
		}
		label = text[1..idx].ToString();
		text = text[(idx + 1)..];
		return true;
	}

	/// <summary>
	/// 尝试解析链接目标。
	/// </summary>
	/// <param name="text">要解析的字符串。</param>
	/// <param name="destination">解析得到的链接目标</param>
	/// <returns>如果解析成功返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseLinkDestination(ref ReadOnlySpan<char> text,
		[MaybeNullWhen(false)] out string destination)
	{
		int idx;
		if (text[0] == '<')
		{
			// <..> 形式
			idx = text.IndexOfUnescaped('>');
			destination = text[1..idx].Trim().Unescape();
			text = text[(idx + 1)..];
			return true;
		}
		// 普通形式
		destination = null;
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
					// 括号不成对。
					return false;
				}
				openParens--;
			}
			else if (Whitespace.Contains(ch))
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
		destination = text[..idx].Trim().Unescape();
		text = text[idx..];
		return true;
	}
}
