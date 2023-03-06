using System;
using Cyjb.Collections;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.Parse;

internal static partial class ParseUtil
{
	/// <summary>
	/// 找到属性的起始 <c>{</c> 字符。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>起始 <c>{</c> 字符的索引；如果未找到则返回 <c>-1</c>；
	/// 如果找到了 <c>{</c> 字符但不能用作属性起始，则返回 <c>-2</c>。</returns>
	public static int FindAttributeStart(ReadOnlySpan<char> text)
	{
		for (int i = text.Length - 1; i >= 0; i--)
		{
			char ch = text[i];
			if (ch == '{')
			{
				// 要求 { 是未转义的。
				if (text.IsEscaped(i))
				{
					return -2;
				}
				else
				{
					return i;
				}
			}
			else if (ch == '"' || ch == '\'')
			{
				for (i--; i >= 0 && text[i] != ch; i--) ;
			}
		}
		return -1;
	}
	/// <summary>
	/// 尝试从文本中解析属性。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <param name="attrs">保存属性的列表。</param>
	/// <param name="checkChar">是否检查字符是否满足属性的要求。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseAttributes(ref ReadOnlySpan<char> text, HtmlAttributeList attrs
		, bool checkChar)
	{
		// 找到属性的起始索引。
		int idx = ParseUtil.FindAttributeStart(text);
		if (idx < 0)
		{
			return false;
		}
		int start = idx;
		ReadOnlySpan<char> attrText = text[(start + 1)..];
		MarkdownUtil.TrimEnd(ref attrText);
		// 要求最后一个非空白字符是 }。
		if (attrText.IsEmpty || attrText[^1] != '}')
		{
			// 未找到最后一个 }。
			return false;
		}
		attrText = attrText[0..^1];
		MarkdownUtil.Trim(ref attrText);
		while (!attrText.IsEmpty)
		{
			if (attrText[0] == '#' || attrText[0] == '.')
			{
				ReadOnlySpan<char> value;
				idx = MarkdownUtil.IndexOfWhitespace(attrText);
				if (idx < 0)
				{
					idx = attrText.Length;
				}
				value = attrText[1..idx];
				if (checkChar && IsInvalidAttributeValue(value))
				{
					return false;
				}
				if (attrText[0] == '#')
				{
					attrs["id"] = value.ToString();
				}
				else
				{
					attrs.AddClass(value.ToString());
				}
			}
			else
			{
				int idx2 = attrText.IndexOf('=');
				if (idx2 < 0)
				{
					idx = MarkdownUtil.IndexOfWhitespace(attrText);
					if (idx < 0)
					{
						idx = attrText.Length;
					}
					ReadOnlySpan<char> key = attrText[0..idx];
					if (checkChar && IsInvalidAttributeKey(key))
					{
						return false;
					}
					attrs[key.ToString()] = string.Empty;
				}
				else
				{
					ReadOnlySpan<char> key = attrText[0..idx2];
					if (checkChar && IsInvalidAttributeKey(key))
					{
						return false;
					}
					idx2++;
					ReadOnlySpan<char> value;
					if (attrText[idx2] == '"' || attrText[idx2] == '\'')
					{
						idx = attrText.IndexOf(attrText[idx2], idx2 + 1);
						if (idx < 0)
						{
							// 未找到匹配的结束引号。
							return false;
						}
						if (idx + 1 < attrText.Length && !MarkdownUtil.IsWhitespace(attrText[idx + 1]))
						{
							// 结束引号后不是空白或文本结束。
							return false;
						}
						value = attrText[(idx2 + 1)..idx];
						idx++;
					}
					else
					{
						idx = MarkdownUtil.IndexOfWhitespace(attrText);
						if (idx < 0)
						{
							idx = attrText.Length;
						}
						value = attrText[idx2..idx];
						if (checkChar && IsInvalidAttributeValue(value))
						{
							return false;
						}
					}
					attrs[key.ToString()] = value.ToString();
				}
			}
			attrText = attrText[idx..];
			MarkdownUtil.TrimStart(ref attrText);
		}
		text = text[0..start];
		return true;
	}

	/// <summary>
	/// 有效的属性键的首字符。
	/// </summary>
	/// <remarks>包含 ASCII 字母、<c>_</c> 或 <c>:</c>。</remarks>
	private static readonly CharSet ValidAttributeFirstChar = CharSet.FromRange("azAZ::__");
	/// <summary>
	/// 有效的属性键的其它字符。
	/// </summary>
	/// <remarks>包含 ASCII 字母、数字、<c>_</c>、<c>.</c>、<c>:</c> 或 <c>-</c>。</remarks>
	private static readonly CharSet ValidAttributeOtherChar = CharSet.FromRange("0:azAZ__--");
	/// <summary>
	/// 无效的属性值的字符。
	/// </summary>
	private static readonly HashSet<char> InvalidAttributeValueChars = new(" \t\r\n\"'=<>`{}");

	/// <summary>
	/// 检查是否是非法的属性键。
	/// </summary>
	/// <param name="text">要检查的属性键。</param>
	/// <returns>如果是非法的属性键，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	private static bool IsInvalidAttributeKey(ReadOnlySpan<char> text)
	{
		if (text.IsEmpty)
		{
			return true;
		}
		bool first = true;
		foreach (char ch in text)
		{
			if (first)
			{
				if (!ValidAttributeFirstChar.Contains(ch))
				{
					return true;
				}
				first = false;
			}
			else if (!ValidAttributeOtherChar.Contains(ch))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 检查是否是非法的属性值。
	/// </summary>
	/// <param name="text">要检查的属性值。</param>
	/// <returns>如果是非法的属性值，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	private static bool IsInvalidAttributeValue(ReadOnlySpan<char> text)
	{
		if (text.IsEmpty)
		{
			return true;
		}
		foreach (char ch in text)
		{
			if (InvalidAttributeValueChars.Contains(ch))
			{
				return true;
			}
		}
		return false;
	}

	//private static int FindAttribute()
}
