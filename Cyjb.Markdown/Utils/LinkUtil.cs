using System.Text;
using System.Text.RegularExpressions;

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
	public static string NormalizeLabel(ReadOnlySpan<char> label)
	{
		MarkdownUtil.Trim(ref label);
		if (label.IsEmpty)
		{
			return string.Empty;
		}
		StringBuilder text = new(label.Length);
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
				text.Append(UnicodeCaseFolding.GetCaseFolding(ch));
			}
		}
		return text.ToString();
	}

	/// <summary>
	/// 编码指定的 URL。
	/// </summary>
	/// <param name="url">要编码的 URL。</param>
	/// <returns>编码后的 URL。</returns>
	public static string EncodeURL(string url)
	{
		return EncodeURLRegex.Replace(url, (Match match) =>
		{
			string value = match.Value;
			if (value.StartsWith('%'))
			{
				if (value.Length == 3)
				{
					// 已经是转移后的字符，直接返回。
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
				StringBuilder text = new();
				foreach (byte v in Encoding.Default.GetBytes(value))
				{
					text.AppendFormat("%{0:X}", v);
				}
				return text.ToString();
			}
		});
	}
}
