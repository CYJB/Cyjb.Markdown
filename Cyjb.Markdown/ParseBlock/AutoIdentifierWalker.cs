using System.Globalization;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 自动生成标识符的遍历器。
/// </summary>
internal sealed class AutoIdentifierWalker : SyntaxWalker, IDisposable
{
	/// <summary>
	/// 唯一标识符。
	/// </summary>
	private readonly UniqueIdentifier uniqueIdentifier = new();
	/// <summary>
	/// Alt 文本的渲染器。
	/// </summary>
	private readonly AltTextRenderer altTextRenderer = new();

	/// <summary>
	/// 初始化 <see cref="AutoIdentifierWalker"/> 类的新实例。
	/// </summary>
	public AutoIdentifierWalker() : base(SyntaxWalkerDepth.BlockNode) { }

	/// <summary>
	/// 访问指定的标题节点。
	/// </summary>
	/// <param name="node">要访问的标题节点。</param>
	public override void VisitHeading(Heading node)
	{
		// 已设置标识符的不需要重复生成。
		if (node.Attributes.Id != null)
		{
			return;
		}
		node.Accept(altTextRenderer);
		node.Attributes.Id = uniqueIdentifier.Unique(GetIdentifier(altTextRenderer.Text));
		altTextRenderer.Clear();
	}

	/// <summary>
	/// 返回指定文本对应的标识符。
	/// </summary>
	/// <param name="text">原始文本。</param>
	/// <returns>从文本生成的标识符。</returns>
	private static string GetIdentifier(Span<char> text)
	{
		int idx = 0;
		for (int i = 0; i < text.Length; i++)
		{
			char ch = text[i];
			// 2. 移除所有数字、字母（Unicode 代码 `L`、`N` 和 `Mn`）和 `_`、`-`、`.`、空格、Tab、换行之外的其它字符。
			if (!IsValidIdentifierChars(ch))
			{
				continue;
			}
			// 3. 将空格、Tab 和换行替换为 `-`。
			if (ch is ' ' or '\t' or '\r' or '\n')
			{
				ch = '-';
			}
			else
			{
				// 4. 将所有字母转换为小写。
				ch = char.ToLowerInvariant(ch);
			}
			if (IsPunctuation(ch))
			{
				// 5. 移除标识符首个字母或数字之前的标点（`_`、`-` 和 `.`）。
				if (idx == 0)
				{
					continue;
				}
				// 6. 连续的相同标点（`_`、`-` 和 `.`）会被合并成一个。
				if (text[idx - 1] == ch)
				{
					continue;
				}
			}
			text[idx++] = ch;
		}
		for (idx--; idx >= 0 && IsPunctuation(text[idx]); idx--) ;
		// 7. 如果结果是空字符串，那么使用 `section` 作为标识符。
		if (idx < 0)
		{
			return "section";
		}
		else
		{
			return text[0..(idx + 1)].ToString();
		}
	}

	/// <summary>
	/// 返回指定字符是否是标识符的有效字符。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>如果是有效字符，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	/// <remarks>Unicode 代码 <c>L</c>、<c>N</c> 和 <c>Mn</c>，和 `_`、`-`、`.`。</remarks>
	private static bool IsValidIdentifierChars(char ch)
	{
		if (ch is '_' or '-' or '.' or ' ' or '\t' or '\r' or '\n')
		{
			return true;
		}
		UnicodeCategory category = char.GetUnicodeCategory(ch);
		return category is UnicodeCategory.UppercaseLetter
			or UnicodeCategory.LowercaseLetter
			or UnicodeCategory.TitlecaseLetter
			or UnicodeCategory.ModifierLetter
			or UnicodeCategory.OtherLetter
			or UnicodeCategory.NonSpacingMark
			or UnicodeCategory.DecimalDigitNumber
			or UnicodeCategory.LetterNumber
			or UnicodeCategory.OtherNumber;
	}

	/// <summary>
	/// 返回指定字符是否是标点字符。
	/// </summary>
	/// <param name="ch"></param>
	/// <returns>如果是标点字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	private static bool IsPunctuation(char ch)
	{
		return ch is '_' or '-' or '.';
	}

	/// <summary>
	/// 释放非托管资源。
	/// </summary>
	public void Dispose()
	{
		altTextRenderer.Dispose();
		GC.SuppressFinalize(this);
	}
}
