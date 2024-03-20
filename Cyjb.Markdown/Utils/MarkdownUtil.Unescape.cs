using System.Globalization;
using Cyjb.Collections;
using Cyjb.Markdown.ParseBlock;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 允许被转义的字符。
	/// </summary>
	private static readonly CharSet Escapable = new("!\"#$%&'()*+,./:;<=>?@[\\]^_`{|}~-");

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
	/// 返回指定块文本中，指定索引的字符是否是被转义的。
	/// </summary>
	/// <param name="text">要检查的块文本。</param>
	/// <param name="idx">要检查的字符位置。</param>
	/// <param name="start">要检查的起始索引。</param>
	/// <returns>如果指定索引的字符是被转义的，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool IsEscaped(this BlockText text, int idx, int start = 0)
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
	/// <param name="text">要反转义的字符串，可能会修改字符串的内容。</param>
	/// <return>反转义后的字符串。</return>
	public static string Unescape(this Span<char> text)
	{
		int idx = text.IndexOfAny("\\&\0");
		if (idx < 0)
		{
			return text.ToString();
		}
		int dst = idx;
		// 注意这要保证 idx 之后至少有一个有效字符。
		int end = text.Length - 1;
		for (; idx < end; idx++)
		{
			char ch = text[idx];
			if (ch == '\0')
			{
				// \0 要被转换成 \uFFFD
				text[dst++] = '\uFFFD';
				continue;
			}
			else if (ch == '\\')
			{
				// 反斜杠转义。
				if (Escapable.Contains(text[idx + 1]))
				{
					text[dst++] = text[idx + 1];
					idx++;
					continue;
				}
			}
			else if (ch == '&')
			{
				if (text[idx + 1] == '#')
				{
					// 查找 HTML 数字转义。
					string? value = ParseNumericCharacter(text.Slice(idx + 2), ref idx);
					if (value != null)
					{
						// 跳过 # 字符。
						idx++;
						value.CopyTo(text.Slice(dst));
						dst += value.Length;
						continue;
					}
				}
				else
				{
					// 查找 HTML 实体转义。
					if (HtmlEntity.Entities.TryMatchShortest(text.Slice(idx + 1), out var pair))
					{
						idx += pair.Key.Length;
						pair.Value.CopyTo(text.Slice(dst));
						dst += pair.Value.Length;
						continue;
					}
				}
			}
			text[dst++] = ch;
		}
		// 复制最后的字符。
		if (idx < text.Length)
		{
			text[dst++] = text[idx];
		}
		return text.Slice(0, dst).ToString();
	}

	/// <summary>
	/// 反转义指定字符串，并返回其结果。
	/// </summary>
	/// <param name="text">要反转义的字符串。</param>
	/// <return>反转义后的字符串。</return>
	public static string Unescape(this ReadOnlySpan<char> text)
	{
		int idx = text.IndexOfAny("\\&\0");
		if (idx < 0)
		{
			return text.ToString();
		}
		using ValueList<char> chars = text.Length <= ValueList.StackallocCharSizeLimit
			? new ValueList<char>(stackalloc char[text.Length])
			: new ValueList<char>(text.Length);
		chars.Add(text.Slice(0, idx));
		// 注意这要保证 idx 之后至少有一个有效字符。
		int end = text.Length - 1;
		for (; idx < end; idx++)
		{
			char ch = text[idx];
			if (ch == '\0')
			{
				// \0 要被转换成 \uFFFD
				chars.Add('\uFFFD');
				continue;
			}
			else if (ch == '\\')
			{
				// 反斜杠转义。
				if (Escapable.Contains(text[idx + 1]))
				{
					chars.Add(text[idx + 1]);
					idx++;
					continue;
				}
			}
			else if (ch == '&')
			{
				if (text[idx + 1] == '#')
				{
					// 查找 HTML 数字转义。
					string? value = ParseNumericCharacter(text.Slice(idx + 2), ref idx);
					if (value != null)
					{
						// 跳过 # 字符。
						idx++;
						chars.Add(value);
						continue;
					}
				}
				else
				{
					// 查找 HTML 实体转义。
					if (HtmlEntity.Entities.TryMatchShortest(text.Slice(idx + 1), out var pair))
					{
						idx += pair.Key.Length;
						chars.Add(pair.Value);
						continue;
					}
				}
			}
			chars.Add(ch);
		}
		if (idx < text.Length)
		{
			chars.Add(text[idx]);

		}
		return chars.ToString();
	}

	/// <summary>
	/// 尝试解析指定的数字字符。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="srcIdx">文本的起始索引。</param>
	/// <returns>解析结果，如果未能成功解析则为 <c>null</c>。</returns>
	private static string? ParseNumericCharacter(ReadOnlySpan<char> text, ref int srcIdx)
	{
		int idx = text.IndexOf(';');
		// 长度最多为 7。
		if (idx <= 0 || idx > 7)
		{
			return null;
		}
		text = text.Slice(0, idx);
		int value;
		if (text[0] == 'X' || text[0] == 'x')
		{
			text = text.Slice(1);
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
		else if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value))
		{
			return null;
		}
		// 这里注意要将 ; 一起跳过。
		srcIdx += idx + 1;
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
