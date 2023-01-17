namespace Cyjb.Markdown.Parse;

/// <summary>
/// 提供 Markdown 解析相关工具方法。
/// </summary>
internal static partial class ParseUtil
{
	/// <summary>
	/// 返回指定字符序列中，指定字符首次出现的索引。
	/// </summary>
	/// <param name="text">要检查的字符序列。</param>
	/// <param name="ch">要检查的字符。</param>
	/// <param name="startIndex">要检查的起始索引。</param>
	/// <returns><paramref name="ch"/> 首次出现的索引。</returns>
	public static int IndexOf(this ReadOnlySpan<char> text, char ch, int startIndex)
	{
		for (int i = startIndex; i < text.Length; i++)
		{
			char cur = text[i];
			if (cur == ch)
			{
				return i;
			}
		}
		return -1;
	}
}
