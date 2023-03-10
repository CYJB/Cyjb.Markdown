using System.Buffers;
using System.Globalization;
using Cyjb.Collections;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 允许被转义的字符。
	/// </summary>
	private static readonly CharSet Escapable = new("!\"#$%&'()*+,./:;<=>?@[\\]^_`{|}~-");

	/// <summary>
	/// 返回当前字符是否可以被转义。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>如果当前字符可以被转义，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool IsEscapable(this char ch)
	{
		return Escapable.Contains(ch);
	}

	/// <summary>
	/// 返回指定字符序列中，指定字符首次出现（非转义）的索引。
	/// </summary>
	/// <param name="text">要检查的字符序列。</param>
	/// <param name="target">要检查的字符。</param>
	/// <param name="start">要检查的起始索引。</param>
	/// <returns><paramref name="target"/> 首次出现（非转义）的索引。</returns>
	public static int IndexOfUnescaped(this ReadOnlySpan<char> text, char target, int start = 0)
	{
		for (int i = start; i < text.Length; i++)
		{
			char ch = text[i];
			if (ch == '\\')
			{
				if (i + 1 >= text.Length)
				{
					return -1;
				}
				else if (Escapable.Contains(text[i + 1]))
				{
					i++;
				}
			}
			else if (ch == target)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定字符序列中，指定索引的字符是否是被转义的。
	/// </summary>
	/// <param name="text">要检查的字符序列。</param>
	/// <param name="idx">要检查的字符位置。</param>
	/// <param name="start">要检查的起始索引。</param>
	/// <returns>如果指定索引的字符是被转义的，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool IsEscaped(this ReadOnlySpan<char> text, int idx, int start = 0)
	{
		int slashCount = 0;
		for (int i = idx - 1; i >= start; i--)
		{
			if (text[i] == '\\')
			{
				slashCount++;
			}
			else
			{
				break;
			}
		}
		return (slashCount & 1) == 1;
	}

	/// <summary>
	/// 返回指定字符序列中，指定字符首次出现（非转义）的索引。
	/// </summary>
	/// <param name="text">要检查的字符序列。</param>
	/// <param name="target">要检查的字符。</param>
	/// <param name="fail">不允许出现的字符（非转义）。</param>
	/// <param name="start">要检查的起始索引。</param>
	/// <returns><paramref name="target"/> 首次出现（非转义）的索引。如果
	/// <paramref name="fail"/> 先于 <paramref name="target"/> 出现，那么会返回 <c>-2</c>。</returns>
	public static int IndexOfUnescaped(this ReadOnlySpan<char> text, char target, char fail, int start = 0)
	{
		for (int i = start; i < text.Length; i++)
		{
			char ch = text[i];
			if (ch == '\\')
			{
				if (i + 1 >= text.Length)
				{
					return -1;
				}
				else if (Escapable.Contains(text[i + 1]))
				{
					i++;
				}
			}
			else if (ch == target)
			{
				return i;
			}
			else if (ch == fail)
			{
				return -2;
			}
		}
		return -1;
	}

	/// <summary>
	/// 反转义指定字符串，并返回其结果。
	/// </summary>
	/// <param name="text">要反转义的字符串。</param>
	/// <return>反转义后的字符串。</return>
	public static string Unescape(this ReadOnlySpan<char> text)
	{
		if (text.IndexOfAny("\\&\0") < 0)
		{
			return text.ToString();
		}
		char[] chars = ArrayPool<char>.Shared.Rent(text.Length);
		int i = 0;
		int idx = 0;
		// 注意这要保证 i 之后至少有一个有效字符。
		for (; i < text.Length - 1; i++)
		{
			char ch = text[i];
			if (ch == '\0')
			{
				// \0 要被转换成 \uFFFD
				chars[idx++] = '\uFFFD';
				continue;
			}
			else if (ch == '\\')
			{
				// 反斜杠转义。
				if (Escapable.Contains(text[i + 1]))
				{
					chars[idx++] = text[i + 1];
					i++;
					continue;
				}
			}
			else if (ch == '&')
			{
				int end = text.IndexOf(';', i + 1);
				if (end >= 0 && i + 2 < end)
				{
					string? str;
					if (text[i + 1] == '#')
					{
						// HTML 数字转义
						str = ParseNumericCharacter(text[(i + 2)..end]);
					}
					else
					{
						// 实体转义
						HtmlEntity.Entities.TryGetValue(text[(i + 1)..end].ToString(), out str);
					}
					if (str != null)
					{
						str.CopyTo(chars.AsSpan(idx));
						idx += str.Length;
						i = end;
						continue;
					}
				}
			}
			chars[idx++] = ch;
		}
		if (i < text.Length)
		{
			chars[idx++] = text[i];
		}
		string result = new(chars, 0, idx);
		ArrayPool<char>.Shared.Return(chars);
		return result;
	}

	/// <summary>
	/// 解析指定的数字字符。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <returns>解析结果，如果未能成功解析则为 <c>null</c>。</returns>
	private static string? ParseNumericCharacter(ReadOnlySpan<char> text)
	{
		// 长度最多为 7。
		if (text.IsEmpty || text.Length > 7)
		{
			return null;
		}
		int value;
		if (text[0] == 'X' || text[0] == 'x')
		{
			text = text[1..];
			// 长度不足。
			if (text.IsEmpty)
			{
				return null;
			}
			if (!int.TryParse(text, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out value))
			{
				return null;
			}
		}
		else
		{
			if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value))
			{
				return null;
			}
		}
		if (value == 0)
		{
			// U+0000 需要被替换为 U+FFFD
			return "\uFFFD";
		}
		try
		{
			return char.ConvertFromUtf32(value);
		}
		catch (ArgumentOutOfRangeException)
		{
			return "\uFFFD";
		}
	}
}
