using System.Text;
using System.Text.RegularExpressions;
using Cyjb.Collections;
using Cyjb.Markdown.ParseBlock;

namespace Cyjb.Markdown.Utils;

/// <summary>
/// 提供链接相关实用方法。
/// </summary>
internal static class LinkUtil
{
	/// <summary>
	/// 用于编码 URL 的正则表达式。
	/// </summary>
	private static readonly Regex EncodeURLRegex = new("%[a-f0-9]{0,2}|[^:/?#@!$&'()*+,;=a-z0-9\\-._~]",
		RegexOptions.IgnoreCase);

	/// <summary>
	/// 检查链接标签是否是合法的。
	/// </summary>
	/// <param name="label">要检查的链接标签。</param>
	public static void CheckLabel(string label)
	{
		ArgumentNullException.ThrowIfNull(label);
		if (label.IsNullOrWhiteSpace())
		{
			throw new ArgumentException(Resources.EmptyLinkLabel, nameof(label));
		}
	}

	/// <summary>
	/// 标准化链接标签。
	/// </summary>
	/// <param name="label">要标准化的链接标签。</param>
	/// <returns>标准化后的标签。</returns>
	public static string NormalizeLabel(BlockText label)
	{
		StringBuilder text = StringBuilderPool.Rent(label.Length);
		var items = label.Items;
		int count = items.Count;
		bool isWhitespace = true;
		for (int i = 0; i < count; i++)
		{
			ReadOnlySpan<char> span = items[i].Text;
			for (int j = 0; j < span.Length; j++)
			{
				char ch = span[j];
				// 将中间的连续空白合并成一个。
				if (MarkdownUtil.IsWhitespace(ch))
				{
					if (!isWhitespace)
					{
						isWhitespace = true;
						text.Append(' ');
					}
				}
				else
				{
					isWhitespace = false;
					UnicodeCaseFolding.GetCaseFolding(ch, text);
				}
			}
		}
		return StringBuilderPool.GetStringAndReturn(text);
	}

	/// <summary>
	/// 标准化链接标签。
	/// </summary>
	/// <param name="label">要标准化的链接标签。</param>
	/// <returns>标准化后的标签。</returns>
	public static string NormalizeLabel(ReadOnlySpan<char> label)
	{
		label = label.Trim(MarkdownUtil.Whitespace);
		if (label.IsEmpty)
		{
			return string.Empty;
		}
		StringBuilder text = StringBuilderPool.Rent(label.Length);
		bool isWhitespace = false;
		foreach (char ch in label)
		{
			// 将中间的连续空白合并成一个。
			if (MarkdownUtil.IsWhitespace(ch))
			{
				if (!isWhitespace)
				{
					isWhitespace = true;
					text.Append(' ');
				}
			}
			else
			{
				isWhitespace = false;
				UnicodeCaseFolding.GetCaseFolding(ch, text);
			}
		}
		return StringBuilderPool.GetStringAndReturn(text);
	}

	/// <summary>
	/// 编码指定的 URL。
	/// </summary>
	/// <param name="url">要编码的 URL。</param>
	/// <returns>编码后的 URL。</returns>
	public static string EncodeURL(string url)
	{
		// TODO: 改为非正则表达式实现。
		return EncodeURLRegex.Replace(url, (Match match) =>
		{
			string value = match.Value;
			if (value.StartsWith('%'))
			{
				if (value.Length == 3)
				{
					// 已经是转义后的字符，直接返回。
					return value;
				}
				else
				{
					// 将 % 转义成 %25。
					return $"%25{value[1..]}";
				}
			}
			else
			{
				using ValueList<char> text = new(stackalloc char[10]);
				foreach (byte v in Encoding.Default.GetBytes(value))
				{
					text.Add('%');
					text.Add(v.ToString("X"));
				}
				return text.ToString();
			}
		});
	}
}
