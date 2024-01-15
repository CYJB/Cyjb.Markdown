using Cyjb.Collections;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 空白字符或者 <c>}</c>。
	/// </summary>
	private static readonly string WhitespaceOrRightBrace = Whitespace + "}";

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
	/// <param name="text">要检查的字符串视图。</param>
	/// <param name="attrs">保存属性的列表。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseAttributes(ref StringView text, HtmlAttributeList attrs)
	{
		ReadOnlySpan<char> span = text;
		if (TryParseAttributes(ref span, attrs))
		{
			text = text[0..span.Length];
			return true;
		} else
		{
			return false;
		}
	}

	/// <summary>
	/// 尝试从文本中解析属性。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <param name="attrs">保存属性的列表。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool TryParseAttributes(ref ReadOnlySpan<char> text, HtmlAttributeList attrs)
	{
		// 找到属性的起始索引。
		int idx = FindAttributeStart(text);
		if (idx < 0)
		{
			return false;
		}
		int start = idx;
		ReadOnlySpan<char> attrText = text[(start + 1)..];
		TrimEnd(ref attrText);
		// 要求最后一个非空白字符是 }。
		if (attrText.IsEmpty || attrText[^1] != '}')
		{
			// 未找到最后一个 }。
			return false;
		}
		attrText = attrText[0..^1];
		if (!TrimAttributeSeperator(ref attrText))
		{
			// 错误的分隔符。
			return false;
		}
		while (!attrText.IsEmpty)
		{
			if (!TryParseAttribute(ref attrText, attrs).IsSuccess)
			{
				return false;
			}
			if (!TrimAttributeSeperator(ref attrText))
			{
				// 错误的分隔符。
				return false;
			}
		}
		text = text[0..start];
		return true;
	}

	/// <summary>
	/// 尝试从文本中解析属性。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <param name="attrs">保存属性的列表。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static AttributeParseResult TryParseAttribute(ref ReadOnlySpan<char> text, HtmlAttributeList attrs)
	{
		if (text.Length == 0)
		{
			return AttributeParseResult.Failed;
		}
		int idx;
		if (text[0] == '#' || text[0] == '.')
		{
			ReadOnlySpan<char> value;
			idx = text.IndexOfAny(WhitespaceOrRightBrace);
			if (idx < 0)
			{
				idx = text.Length;
			}
			value = text[1..idx];
			if (IsInvalidAttributeValue(value))
			{
				return AttributeParseResult.Failed;
			}
			if (text[0] == '#')
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
			// 避免检测到下一个属性的 =。
			idx = text.IndexOfAny(WhitespaceOrRightBrace);
			int idx2 = text.IndexOf('=');
			if (idx >= 0 && idx2 > idx)
			{
				idx2 = -1;
			}
			if (idx2 < 0)
			{
				if (idx < 0)
				{
					idx = text.Length;
				}
				ReadOnlySpan<char> key = text[0..idx];
				if (IsInvalidAttributeKey(key))
				{
					return AttributeParseResult.Failed;
				}
				attrs[key.ToString()] = string.Empty;
			}
			else
			{
				ReadOnlySpan<char> key = text[0..idx2];
				if (IsInvalidAttributeKey(key))
				{
					return AttributeParseResult.Failed;
				}
				idx2++;
				ReadOnlySpan<char> value;
				if (text[idx2] == '"' || text[idx2] == '\'')
				{
					idx = text.IndexOf(text[idx2], idx2 + 1);
					if (idx < 0)
					{
						// 未找到匹配的结束引号。
						return AttributeParseResult.GetMissingQuote(text[idx2], key.ToString(),
							text[(idx2 + 1)..].ToString());
					}
					if (idx + 1 < text.Length && !IsWhitespace(text[idx + 1]) && text[idx + 1] != '}')
					{
						// 结束引号后不是文本结束、空白或结束括号。
						return AttributeParseResult.Failed;
					}
					value = text[(idx2 + 1)..idx];
					idx++;
				}
				else
				{
					idx = text.IndexOfAny(WhitespaceOrRightBrace);
					if (idx < 0)
					{
						idx = text.Length;
					}
					value = text[idx2..idx];
					if (IsInvalidAttributeValue(value))
					{
						return AttributeParseResult.Failed;
					}
				}
				attrs[key.ToString()] = value.ToString();
			}
		}
		text = text[idx..];
		return AttributeParseResult.Success;
	}

	/// <summary>
	/// 移除指定文本的起始属性分隔符。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>如果没有分隔符，或者至多包含一个换行，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	private static bool TrimAttributeSeperator(ref ReadOnlySpan<char> text)
	{
		int lineCount = 0;
		int i = 0;
		bool isLastReturn = false;
		for (; i < text.Length && MarkdownUtil.IsWhitespace(text[i]); i++)
		{
			if (text[i] == '\r')
			{
				lineCount++;
				isLastReturn = true;
			}
			else if (text[i] == '\n')
			{
				if (!isLastReturn)
				{
					lineCount++;
				}
				isLastReturn = false;
			}
			else
			{
				isLastReturn = false;
			}
		}
		text = text[i..];
		return lineCount <= 1;
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
}
