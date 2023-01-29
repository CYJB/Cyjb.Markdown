namespace Cyjb.Markdown.Utils;

/// <summary>
/// 表示 emoji 的信息。
/// </summary>
internal readonly struct EmojiInfo : IEquatable<EmojiInfo>
{
	/// <summary>
	/// GitHub 后备链接的前缀。
	/// </summary>
	private static readonly string GitHubFallbackPrefix = "https://github.githubassets.com/images/icons/emoji/";
	/// <summary>
	/// GitHub 后备链接的后缀。
	/// </summary>
	private static readonly string GitHubFallbackSuffix = ".png?v8";

	/// <summary>
	/// 是否是 Unicode emoji。
	/// </summary>
	private readonly bool isUnicode;
	/// <summary>
	/// emoji 的文本，使用 <c>null</c> 表示非 Unicode emoji。
	/// </summary>
	private readonly string? text;
	/// <summary>
	/// 后备链接的名称。
	/// </summary>
	private readonly string fallbackName;

	/// <summary>
	/// 使用指定的 Unicode emoji 初始化。
	/// </summary>
	/// <param name="text">emoji 的文本。</param>
	/// <param name="fallbackName">后备链接的名称。</param>
	public EmojiInfo(string text, string fallbackName)
	{
		isUnicode = true;
		this.text = text;
		this.fallbackName = fallbackName;
	}

	/// <summary>
	/// 使用指定的 GitHub 自定义 emoji 初始化。
	/// </summary>
	/// <param name="name">emoji 的名称。</param>
	public EmojiInfo(string name)
	{
		isUnicode = false;
		text = null;
		fallbackName = name;
	}

	/// <summary>
	/// 获取是否是 Unicode emoji。
	/// </summary>
	public readonly bool IsUnicode => isUnicode;

	/// <summary>
	/// 获取 Unicode emoji 的文本；非 Unicode emoji 会返回 <c>null</c>。
	/// </summary>
	public readonly string? Text => text;

	/// <summary>
	/// 获取 GitHub 的后备链接。
	/// </summary>
	public readonly string GitHubFallbackUrl => GitHubFallbackPrefix + (isUnicode ? "unicode/" : "") +
		fallbackName + GitHubFallbackSuffix;

	#region IEquatable<EmojiInfo> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(EmojiInfo other)
	{
		return fallbackName == other.fallbackName;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is EmojiInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	/// <summary>
	/// 返回当前对象的哈希值。
	/// </summary>
	/// <returns>当前对象的哈希值。</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(GitHubFallbackPrefix, fallbackName);
	}

	/// <summary>
	/// 返回指定的 <see cref="EmojiInfo"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(EmojiInfo left, EmojiInfo right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="EmojiInfo"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(EmojiInfo left, EmojiInfo right)
	{
		return !left.Equals(right);
	}

	#endregion // IEquatable<EmojiInfo> 成员

}
