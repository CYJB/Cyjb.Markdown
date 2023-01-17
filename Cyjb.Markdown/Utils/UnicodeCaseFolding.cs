namespace Cyjb.Markdown.Utils;

/// <summary>
/// 提供 Unicode Case Fold 功能。
/// </summary>
internal static class UnicodeCaseFolding
{
	/// <summary>
	/// Unicode Case Folding 列表，其中全部被转换为大写。
	/// </summary>
	/// <see href="https://www.unicode.org/Public/13.0.0/ucd/CaseFolding.txt"/>
	private static readonly Dictionary<char, string> FullCaseFolding = new()
	{
		{ '\u00DF', "\u0053\u0053" },
		{ '\u0130', "\u0049\u0307" },
		{ '\u0131', "\u0049\u0307" },
		{ '\u0149', "\u02BC\u004E" },
		{ '\u01F0', "\u004A\u030C" },
		{ '\u0390', "\u0399\u0308\u0301" },
		{ '\u03B0', "\u03A5\u0308\u0301" },
		{ '\u0587', "\u0535\u0552" },
		{ '\u1E96', "\u0048\u0331" },
		{ '\u1E97', "\u0054\u0308" },
		{ '\u1E98', "\u0057\u030A" },
		{ '\u1E99', "\u0059\u030A" },
		{ '\u1E9A', "\u0041\u02BE" },
		{ '\u1E9E', "\u0053\u0053" },
		{ '\u1F50', "\u03A5\u0313" },
		{ '\u1F52', "\u03A5\u0313\u0300" },
		{ '\u1F54', "\u03A5\u0313\u0301" },
		{ '\u1F56', "\u03A5\u0313\u0342" },
		{ '\u1F80', "\u1F08\u0399" },
		{ '\u1F81', "\u1F09\u0399" },
		{ '\u1F82', "\u1F0A\u0399" },
		{ '\u1F83', "\u1F0B\u0399" },
		{ '\u1F84', "\u1F0C\u0399" },
		{ '\u1F85', "\u1F0D\u0399" },
		{ '\u1F86', "\u1F0E\u0399" },
		{ '\u1F87', "\u1F0F\u0399" },
		{ '\u1F88', "\u1F08\u0399" },
		{ '\u1F89', "\u1F09\u0399" },
		{ '\u1F8A', "\u1F0A\u0399" },
		{ '\u1F8B', "\u1F0B\u0399" },
		{ '\u1F8C', "\u1F0C\u0399" },
		{ '\u1F8D', "\u1F0D\u0399" },
		{ '\u1F8E', "\u1F0E\u0399" },
		{ '\u1F8F', "\u1F0F\u0399" },
		{ '\u1F90', "\u1F28\u0399" },
		{ '\u1F91', "\u1F29\u0399" },
		{ '\u1F92', "\u1F2A\u0399" },
		{ '\u1F93', "\u1F2B\u0399" },
		{ '\u1F94', "\u1F2C\u0399" },
		{ '\u1F95', "\u1F2D\u0399" },
		{ '\u1F96', "\u1F2E\u0399" },
		{ '\u1F97', "\u1F2F\u0399" },
		{ '\u1F98', "\u1F28\u0399" },
		{ '\u1F99', "\u1F29\u0399" },
		{ '\u1F9A', "\u1F2A\u0399" },
		{ '\u1F9B', "\u1F2B\u0399" },
		{ '\u1F9C', "\u1F2C\u0399" },
		{ '\u1F9D', "\u1F2D\u0399" },
		{ '\u1F9E', "\u1F2E\u0399" },
		{ '\u1F9F', "\u1F2F\u0399" },
		{ '\u1FA0', "\u1F68\u0399" },
		{ '\u1FA1', "\u1F69\u0399" },
		{ '\u1FA2', "\u1F6A\u0399" },
		{ '\u1FA3', "\u1F6B\u0399" },
		{ '\u1FA4', "\u1F6C\u0399" },
		{ '\u1FA5', "\u1F6D\u0399" },
		{ '\u1FA6', "\u1F6E\u0399" },
		{ '\u1FA7', "\u1F6F\u0399" },
		{ '\u1FA8', "\u1F68\u0399" },
		{ '\u1FA9', "\u1F69\u0399" },
		{ '\u1FAA', "\u1F6A\u0399" },
		{ '\u1FAB', "\u1F6B\u0399" },
		{ '\u1FAC', "\u1F6C\u0399" },
		{ '\u1FAD', "\u1F6D\u0399" },
		{ '\u1FAE', "\u1F6E\u0399" },
		{ '\u1FAF', "\u1F6F\u0399" },
		{ '\u1FB2', "\u1FBA\u0399" },
		{ '\u1FB3', "\u0391\u0399" },
		{ '\u1FB4', "\u0386\u0399" },
		{ '\u1FB6', "\u0391\u0342" },
		{ '\u1FB7', "\u0391\u0342\u0399" },
		{ '\u1FBC', "\u0391\u0399" },
		{ '\u1FC2', "\u1FCA\u0399" },
		{ '\u1FC3', "\u0397\u0399" },
		{ '\u1FC4', "\u0389\u0399" },
		{ '\u1FC6', "\u0397\u0342" },
		{ '\u1FC7', "\u0397\u0342\u0399" },
		{ '\u1FCC', "\u0397\u0399" },
		{ '\u1FD2', "\u0399\u0308\u0300" },
		{ '\u1FD3', "\u0399\u0308\u0301" },
		{ '\u1FD6', "\u0399\u0342" },
		{ '\u1FD7', "\u0399\u0308\u0342" },
		{ '\u1FE2', "\u03A5\u0308\u0300" },
		{ '\u1FE3', "\u03A5\u0308\u0301" },
		{ '\u1FE4', "\u03A1\u0313" },
		{ '\u1FE6', "\u03A5\u0342" },
		{ '\u1FE7', "\u03A5\u0308\u0342" },
		{ '\u1FF2', "\u1FFA\u0399" },
		{ '\u1FF3', "\u03A9\u0399" },
		{ '\u1FF4', "\u038F\u0399" },
		{ '\u1FF6', "\u03A9\u0342" },
		{ '\u1FF7', "\u03A9\u0342\u0399" },
		{ '\u1FFC', "\u03A9\u0399" },
		{ '\uFB00', "\u0046\u0046" },
		{ '\uFB01', "\u0046\u0049" },
		{ '\uFB02', "\u0046\u004C" },
		{ '\uFB03', "\u0046\u0046\u0049" },
		{ '\uFB04', "\u0046\u0046\u004C" },
		{ '\uFB05', "\u0053\u0054" },
		{ '\uFB06', "\u0053\u0054" },
		{ '\uFB13', "\u0544\u0546" },
		{ '\uFB14', "\u0544\u0535" },
		{ '\uFB15', "\u0544\u053B" },
		{ '\uFB16', "\u054E\u0546" },
		{ '\uFB17', "\u0544\u053D" },
		// 大小写转换过程中的额外补充
		{ '\u03F4', "\u0398" },
		{ '\u2126', "\u03A9" },
		{ '\u212A', "\u004B" },
		{ '\u212B', "\u00C5" },
	};

	/// <summary>
	/// 返回指定字符的 Case Folding 结果。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>指定字符的 Case Folding 结果。</returns>
	public static string GetCaseFolding(char ch)
	{
		if (FullCaseFolding.TryGetValue(ch, out string? result))
		{
			return result;
		}
		return char.ToUpperInvariant(ch).ToString();
	}
}
