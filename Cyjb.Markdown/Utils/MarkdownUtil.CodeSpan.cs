using System.Text;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 换行字符。
	/// </summary>
	private static readonly string NewLineChars = "\r\n";

	/// <summary>
	/// 处理代码片段。
	/// </summary>
	/// <param name="text">要检查的字符串。</param>
	/// <returns>分隔符的长度。</returns>
	public static string ProcessCodeSpan(ReadOnlySpan<char> text)
	{
		int idx = text.IndexOfAny(NewLineChars);
		int len;
		if (idx < 0)
		{
			// 不包含换行字符，不需要处理换行。
			// 如果代码两边都有一个空格（注意不是空白），那么可以移除前后各一的空格。
			// 不要移除多个空格，也不要修改只由空格组成的代码。
			len = text.Length - 1;
			if (text.Length >= 1 && text[0] == ' ' && text[^1] == ' ')
			{
				for (int i = 1; i < len; i++)
				{
					if (!IsWhitespace(text[i]))
					{
						text = text[1..^1];
						break;
					}
				}
			}
			return text.ToString();
		}
		// 包含换行符，将连续的换行符替换为一个空格。
		// 等价于 ReplacePattern("[\r\n]+", " ")
		StringBuilder builder = StringBuilderPool.Rent(text.Length);
		while (idx >= 0)
		{
			if (builder.Length > 0)
			{
				builder.Append(' ');
			}
			builder.Append(text.Slice(0, idx));
			// 跳过连续的换行。
			for (; idx < text.Length && text[idx] is '\r' or '\n'; idx++) ;
			text = text.Slice(idx);
			idx = text.IndexOfAny(NewLineChars);
		}
		if (text.Length > 0)
		{
			if (builder.Length > 0)
			{
				builder.Append(' ');
			}
			builder.Append(text);
		}
		// 如果代码两边都有一个空格（注意不是空白），那么可以移除前后各一的空格。
		// 不要移除多个空格，也不要修改只由空格组成的代码。
		len = builder.Length - 1;
		if (len >= 1 && builder[0] == ' ' && builder[^1] == ' ')
		{
			for (int i = 1; i < len; i++)
			{
				if (!IsWhitespace(builder[i]))
				{
					builder.Remove(len, 1);
					builder.Remove(0, 1);
					break;
				}
			}
		}
		return StringBuilderPool.GetStringAndReturn(builder);
	}
}
