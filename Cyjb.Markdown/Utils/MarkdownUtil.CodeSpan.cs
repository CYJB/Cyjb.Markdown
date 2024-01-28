using System.Text;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 换行字符。
	/// </summary>
	private static readonly char[] NewLineChars = new char[] { '\n', '\r' };

	/// <summary>
	/// 处理代码片段。
	/// </summary>
	/// <param name="text">要检查的字符串。</param>
	/// <returns>分隔符的长度。</returns>
	public static string ProcessCodeSpan(StringView text)
	{
		int idx = text.IndexOfAny(NewLineChars);
		if (idx < 0)
		{
			// 不包含换行字符，不需要处理换行。
			// 如果代码两边都有一个空格（注意不是空白），那么可以移除前后各一的空格。
			// 不要移除多个空格，也不要修改只由空格组成的代码。
			if (text.Length >= 2 && text[0] == ' ' && text[^1] == ' ' &&
				text.Any(ch => !IsWhitespace(ch)))
			{
				text = text[1..^1];
			}
			return text.ToString();
		}
		// 包含换行符，将连续的换行符替换为一个空格。
		// 等价于 ReplacePattern("[\r\n]+", " ")
		StringBuilder builder = StringBuilderPool.Rent(text.Length);
		ReadOnlySpan<char> span = text;
		builder.Append(span.Slice(0, idx));
		builder.Append(' ');
		bool hasNonWhitespace = false;
		bool hasAppendSpace = true;
		for (idx++; idx < span.Length; idx++)
		{
			char ch = span[idx];
			if (ch is '\n' or '\r')
			{
				if (!hasAppendSpace)
				{
					builder.Append(' ');
					hasAppendSpace = true;
				}
			}
			else
			{
				if (hasAppendSpace)
				{
					hasAppendSpace = false;
				}
				if (!hasNonWhitespace && !IsWhitespace(ch))
				{
					hasNonWhitespace = true;
				}
				builder.Append(ch);
			}
		}
		// 如果代码两边都有一个空格（注意不是空白），那么可以移除前后各一的空格。
		// 不要移除多个空格，也不要修改只由空格组成的代码。
		if (builder.Length >= 2 && builder[0] == ' ' && builder[^1] == ' ' && hasNonWhitespace)
		{
			builder.Remove(0, 1);
			builder.Remove(builder.Length - 1, 1);
		}
		return StringBuilderPool.GetStringAndReturn(builder);
	}
}
