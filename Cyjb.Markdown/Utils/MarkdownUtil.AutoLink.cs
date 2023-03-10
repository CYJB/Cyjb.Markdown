using System.Globalization;
using System.Text.RegularExpressions;
using Cyjb.Collections;
using Cyjb.Globalization;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 有效域名的正则表达式。
	/// </summary>
	private static readonly Regex ValidDomainRegex = new(@"^https?://([a-z0-9_-]+\.)*[a-z0-9-]+\.[a-z0-9-]+(/.*)?$", RegexOptions.IgnoreCase);
	/// <summary>
	/// ASCII 数字或字母的字符集合。
	/// </summary>
	private static readonly ReadOnlyCharSet ASCIILetterOrDigit = ReadOnlyCharSet.FromRange("09AZaz");
	/// <summary>
	/// 结束标点符号的集合。
	/// </summary>
	private static readonly ReadOnlyCharSet EndPunctuations = GetEndPunctuations();
	/// <summary>
	/// 闭合标点符号与其开始标点符号的映射关系。
	/// </summary>
	private static readonly Dictionary<char, char> PunctuationPairs = new()
	{
		{ '’', '‘' },
		{ '›', '‹' },
		{ '”', '“' },
		{ '〞', '〝' },
		// \u301F '〟' ClosePunctuation 未找到合适的 OpenPunctuation
		{ '»', '«' },
		{ ')', '(' },
		{ '）', '（' },
		{ '﹚', '﹙' },
		{ '⁾', '⁽' },
		{ '₎', '₍' },
		{ '︶', '︵' },
		{ ']', '[' },
		{ '］', '［' },
		{ '﹈', '﹇' },
		{ '}', '{' },
		{ '｝', '｛' },
		{ '﹜', '﹛' },
		{ '︸', '︷' },
		{ '༻', '༺' },
		{ '༽', '༼' },
		{ '᚜', '᚛' },
		{ '⁆', '⁅' },
		{ '⌉', '⌈' },
		{ '⌋', '⌊' },
		{ '⧽', '⧼' },
		{ '⦄', '⦃' },
		{ '⦆', '⦅' },
		{ '｠', '｟' },
		{ '⦈', '⦇' },
		{ '⦊', '⦉' },
		{ '⦌', '⦋' },
		{ '⦎', '⦍' },
		{ '⦐', '⦏' },
		{ '⦒', '⦑' },
		{ '⦔', '⦓' },
		{ '⦖', '⦕' },
		{ '⦘', '⦗' },
		{ '⟆', '⟅' },
		{ '⟧', '⟦' },
		{ '⟩', '⟨' },
		{ '⟫', '⟪' },
		{ '⟭', '⟬' },
		{ '⟯', '⟮' },
		{ '❩', '❨' },
		{ '❫', '❪' },
		{ '❭', '❬' },
		{ '❯', '❮' },
		{ '❱', '❰' },
		{ '❳', '❲' },
		{ '❵', '❴' },
		{ '⸃', '⸂' },
		{ '⸅', '⸄' },
		{ '⸊', '⸉' },
		{ '⸍', '⸌' },
		{ '⸝', '⸜' },
		{ '⸡', '⸠' },
		{ '⸣', '⸢' },
		{ '⸥', '⸤' },
		{ '⸧', '⸦' },
		{ '⸩', '⸨' },
		{ '〉', '〈' },
		{ '〉', '〈' },
		{ '﹀', '︿' },
		{ '》', '《' },
		{ '︾', '︽' },
		{ '」', '「' },
		{ '｣', '｢' },
		{ '﹂', '﹁' },
		{ '』', '『' },
		{ '﹄', '﹃' },
		{ '】', '【' },
		{ '︼', '︻' },
		{ '〕', '〔' },
		{ '﹞', '﹝' },
		{ '︺', '︹' },
		{ '〗', '〖' },
		{ '︘', '︗' },
		{ '〙', '〘' },
		{ '〛', '〚' },
		{ '﴾', '﴿' },
		{ '⧙', '⧘' },
		{ '⧛', '⧚' },
	};

	/// <summary>
	/// 返回指定 URL 是否包含有效的域名。
	/// </summary>
	/// <param name="url">要检查的 URL。</param>
	/// <returns>如果 URL 包含有效的域名，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public static bool IsValidDomain(string url)
	{
		return ValidDomainRegex.IsMatch(url);
	}

	/// <summary>
	/// 移除 URL 的结束标点符号。
	/// </summary>
	/// <param name="url">要检查的 URL。</param>
	/// <returns>被移除的字符个数。</returns>
	public static int TrimEndPunctuations(ref string url)
	{
		ReadOnlySpan<char> span = url;
		while (!span.IsEmpty)
		{
			char endChar = span[^1];
			if (endChar == ';')
			{
				// 检查是否可能是 HTML 实体。
				bool breakOuter = true;
				for (int i = 2; i < span.Length; i++)
				{
					char ch = span[^i];
					if (!ASCIILetterOrDigit.Contains(ch))
					{
						if (i > 2 && ch == '&')
						{
							// 要求至少有一个字母或数字
							span = span[0..^i];
							breakOuter = false;
							break;
						}
						else
						{
							break;
						}
					}
				}
				if (breakOuter)
				{
					break;
				}
			}
			else if (EndPunctuations.Contains(endChar))
			{
				// 末尾的标点符号不计入链接之内。
				span = span[0..^1];
			}
			else if (PunctuationPairs.TryGetValue(endChar, out char openChar))
			{
				// 检查标点符号是否配对。
				int openCount = 0;
				int closeCount = 0;
				bool breakOuter = true;
				for (int i = 0; i < span.Length; i++)
				{
					char ch = span[i];
					if (ch == openChar)
					{
						openCount++;
					}
					else if (ch == endChar)
					{
						closeCount++;
						if (closeCount > openCount)
						{
							span = span[0..i];
							breakOuter = false;
							break;
						}
					}
				}
				if (breakOuter)
				{
					break;
				}
			}
			else
			{
				break;
			}
		}
		int cnt = url.Length - span.Length;
		if (cnt > 0)
		{
			url = span.ToString();
		}
		return cnt;
	}

	/// <summary>
	/// 返回结束标点符号的集合。
	/// </summary>
	private static ReadOnlyCharSet GetEndPunctuations()
	{
		// [\p{P}-[\p{Pe}\p{Pf}@#%/&_*;\-]]
		CharSet set = new();
		// Pc
		set.UnionWith(UnicodeCategory.ConnectorPunctuation.GetChars());
		// Pd
		set.UnionWith(UnicodeCategory.DashPunctuation.GetChars());
		// Ps
		set.UnionWith(UnicodeCategory.OpenPunctuation.GetChars());
		// Pi
		set.UnionWith(UnicodeCategory.InitialQuotePunctuation.GetChars());
		// Po
		set.UnionWith(UnicodeCategory.OtherPunctuation.GetChars());
		set.ExceptWith(@"@#%/&_\;-");
		return set.AsReadOnly();
	}
}
