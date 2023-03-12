namespace Cyjb.Markdown.Utils;

/// <summary>
/// 表示 GitHub 表情符号的信息。
/// </summary>
internal class GitHubEmojiInfo : EmojiInfo
{
	/// <summary>
	/// 使用指定的 Unicode 表情符号初始化。
	/// </summary>
	/// <param name="text">表情符号的文本。</param>
	/// <param name="fallbackName">后备链接的名称。</param>
	public GitHubEmojiInfo(string text, string fallbackName)
		: base(true, text, fallbackName)
	{ }

	/// <summary>
	/// 使用指定的 GitHub 自定义表情符号初始化。
	/// </summary>
	/// <param name="name">表情符号的名称。</param>
	public GitHubEmojiInfo(string name)
		: base(false, null, name)
	{ }

	/// <summary>
	/// 获取后备链接。
	/// </summary>
	public override string? FallbackUrl
	{
		get
		{
			string unicodePrefix = string.Empty;
			if (IsUnicode)
			{
				unicodePrefix = "unicode/";
			}
			return $"https://github.githubassets.com/images/icons/emoji/{unicodePrefix}{base.FallbackUrl}.png?v8";
		}
	}
}
