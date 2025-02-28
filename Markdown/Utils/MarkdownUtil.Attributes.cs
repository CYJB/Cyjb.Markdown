using Cyjb.Collections;
using Cyjb.Markdown.ParseBlock;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 空白字符或者 <c>}</c>。
	/// </summary>
	private static readonly string WhitespaceOrRightBrace = Whitespace + "}";
	/// <summary>
	/// 空白字符或者 <c>}</c>。
	/// </summary>
	private static readonly char[] WhitespaceOrRightBraceChars = new char[] { ' ', '\t', '\r', '\n', '}' };
	/// <summary>
	/// 属性的起始搜索字符。
	/// </summary>
	private static readonly string AttributeStartSearchChars = "{\"'";
	/// <summary>
	/// 属性的分割字符。
	/// </summary>
	private const string AttributeSeperatorChars = "{'\"";

	/// <summary>
	/// 找到属性的起始 <c>{</c> 字符。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>起始 <c>{</c> 字符的索引；如果未找到则返回 <c>-1</c>；
	/// 如果找到了 <c>{</c> 字符但不能用作属性起始，则返回 <c>-2</c>。</returns>
	public static int FindAttributeStart(ReadOnlySpan<char> text)
	{
		int idx = text.LastIndexOfAny(AttributeStartSearchChars);
		while (idx >= 0)
		{
			char ch = text[idx];
			if (ch == '{')
			{
				// 要求 { 是未转义的。
				if (text.IsEscaped(idx))
				{
					return -2;
				}
				else
				{
					return idx;
				}
			}
			text = text[0..idx];
			idx = text.LastIndexOf(ch);
			if (idx < 0)
			{
				break;
			}
			text = text[0..idx];
			idx = text.LastIndexOfAny(AttributeStartSearchChars);
		}
		return -1;
	}

	/// <summary>
	/// 找到属性的起始 <c>{</c> 字符。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>起始 <c>{</c> 字符的索引；如果未找到则返回 <c>-1</c>；
	/// 如果找到了 <c>{</c> 字符但不能用作属性起始，则返回 <c>-2</c>。</returns>
	public static int FindAttributeStart(BlockText text)
	{
		var items = text.Items;
		char stringDelimiter = '\0';
		bool inString = false;
		int idx;
		for (int i = items.Count - 1; i >= 0; i--)
		{
			ReadOnlySpan<char> span = items[i].Text;
			if (inString)
			{
				// 跳过字符串。
				idx = span.LastIndexOf(stringDelimiter);
				if (idx < 0)
				{
					continue;
				}
				inString = false;
				span = span.Slice(0, idx);
			}
			idx = span.LastIndexOfAny(AttributeSeperatorChars);
			while (idx >= 0)
			{
				char ch = span[idx];
				if (ch == '{')
				{
					// 要求 { 是未转义的。
					if (span.IsEscaped(idx))
					{
						return -2;
					}
					else
					{
						for (i--; i >= 0; i--)
						{
							idx += items[i].Text.Length;
						}
						return idx;
					}
				}
				else
				{
					// 跳过字符串。
					span = span.Slice(0, idx);
					idx = span.LastIndexOf(ch);
					if (idx < 0)
					{
						stringDelimiter = ch;
						inString = true;
						break;
					}
				}
				span = span.Slice(0, idx);
				idx = span.LastIndexOfAny(AttributeSeperatorChars);
			}
		}
		return -1;
	}

	/// <summary>
	/// 从源码读取器中读取属性。
	/// </summary>
	/// <param name="source">要读取的源码读取器。</param>
	/// <param name="attrs">解析后的特性列表。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool ReadAttributes(SourceReader source, HtmlAttributeList attrs)
	{
		var tokenizer = AttributeLexer.Factory.CreateTokenizer();
		tokenizer.Load(source);
		bool hasSeperator = true;
		while (true)
		{
			var token = tokenizer.Read();
			switch (token.Kind)
			{
				case AttributeKind.Seperator:
					// 检查分隔符是否合法。
					if (!IsValidAttributeSeperator(token.Text))
					{
						goto ParseFailed;
					}
					hasSeperator = true;
					break;
				case AttributeKind.Identifier:
					if (!hasSeperator)
					{
						// 缺少分隔符。
						goto ParseFailed;
					}
					attrs.Id = token.Text.ToString();
					break;
				case AttributeKind.ClassName:
					if (!hasSeperator)
					{
						// 缺少分隔符。
						goto ParseFailed;
					}
					attrs.AddClass(token.Text.ToString());
					break;
				case AttributeKind.Common:
					if (!hasSeperator)
					{
						// 缺少分隔符。
						goto ParseFailed;
					}
					attrs.Add(token.Text.ToString(), (token.Value as string)!);
					break;
				case AttributeKind.End:
					// 解析成功。
					return true;
				default:
					// 解析失败。
					goto ParseFailed;
			}
		}
	ParseFailed:
		// 清除之前可能部分成功的属性。
		attrs.Clear();
		return false;
	}

	/// <summary>
	/// 从文本中解析属性。
	/// </summary>
	/// <param name="text">要检查的字符串视图。</param>
	/// <returns>如果成功解析属性，则返回属性列表；否则返回 <c>null</c>。</returns>
	public static HtmlAttributeList? ParseAttributes(ref StringView text)
	{
		ReadOnlySpan<char> span = text;
		HtmlAttributeList? attrs = ParseAttributes(ref span);
		if (attrs != null)
		{
			text = text[0..span.Length];
		}
		return attrs;
	}

	/// <summary>
	/// 从文本中解析属性。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>如果成功解析属性，则返回属性列表；否则返回 <c>null</c>。</returns>
	public static HtmlAttributeList? ParseAttributes(ref ReadOnlySpan<char> text)
	{
		// 找到属性的起始索引。
		int idx = FindAttributeStart(text);
		if (idx < 0)
		{
			return null;
		}
		int start = idx;
		ReadOnlySpan<char> attrText = text.Slice(start + 1).TrimEnd(Whitespace);
		// 要求最后一个非空白字符是 }。
		if (attrText.IsEmpty || attrText[^1] != '}')
		{
			// 未找到最后一个 }。
			return null;
		}
		attrText = attrText[0..^1];
		if (!TrimAttributeSeperator(ref attrText))
		{
			// 错误的分隔符。
			return null;
		}
		HtmlAttributeList? attrs = null;
		while (!attrText.IsEmpty)
		{
			if (!TryParseAttribute(ref attrText, ref attrs).IsSuccess)
			{
				return null;
			}
			if (!TrimAttributeSeperator(ref attrText))
			{
				// 错误的分隔符。
				return null;
			}
		}
		text = text[0..start];
		return attrs;
	}

	/// <summary>
	/// 尝试从文本中解析属性。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <param name="attrs">保存属性的列表。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static AttributeParseResult TryParseAttribute(ref ReadOnlySpan<char> text, ref HtmlAttributeList? attrs)
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
			attrs ??= new HtmlAttributeList();
			if (text[0] == '#')
			{
				attrs.Id = value.ToString();
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
				attrs ??= new HtmlAttributeList();
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
				attrs ??= new HtmlAttributeList();
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
		for (; i < text.Length && IsWhitespace(text[i]); i++)
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
	/// 检查是否是有效的属性分隔符。
	/// </summary>
	/// <param name="text">要检查的文本。</param>
	/// <returns>如果是有效的属性分隔符，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	private static bool IsValidAttributeSeperator(ReadOnlySpan<char> text)
	{
		int lineCount = 0;
		bool isLastReturn = false;
		foreach (char ch in text)
		{
			if (ch == '\r')
			{
				lineCount++;
				if (lineCount > 1)
				{
					return false;
				}
				isLastReturn = true;
			}
			else if (ch == '\n')
			{
				if (!isLastReturn)
				{
					lineCount++;
					if (lineCount > 1)
					{
						return false;
					}
				}
				isLastReturn = false;
			}
			else
			{
				isLastReturn = false;
			}
		}
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
}
