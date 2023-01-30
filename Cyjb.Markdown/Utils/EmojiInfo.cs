namespace Cyjb.Markdown.Utils;

/// <summary>
/// 表示 emoji 的信息。
/// </summary>
internal class EmojiInfo
{
	/// <summary>
	/// 使用指定的 emoji 信息初始化 <see cref="EmojiInfo"/> 类的新实例。
	/// </summary>
	/// <param name="isUnicode">是否是 Unicode emoji。</param>
	/// <param name="text">emoji 的文本。</param>
	/// <param name="fallbackUrl">后备链接。</param>
	public EmojiInfo(bool isUnicode, string? text, string? fallbackUrl)
	{
		IsUnicode = isUnicode;
		Text = text;
		FallbackUrl = fallbackUrl;
	}

	/// <summary>
	/// 获取是否是 Unicode emoji。
	/// </summary>
	public bool IsUnicode { get; }

	/// <summary>
	/// 获取 Unicode emoji 的文本；非 Unicode emoji 会返回 <c>null</c>。
	/// </summary>
	public string? Text { get; }

	/// <summary>
	/// 获取后备链接。
	/// </summary>
	public virtual string? FallbackUrl { get; }
}
