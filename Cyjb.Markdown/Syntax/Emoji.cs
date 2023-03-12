using System.Diagnostics;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的表情符号。
/// </summary>
public class Emoji : InlineNode, IEquatable<Emoji>
{
	/// <summary>
	/// 返回指定代码表示的表情符号。
	/// </summary>
	/// <param name="code">表情符号代码。</param>
	/// <param name="span">文本的范围。</param>
	/// <returns>指定代码表示的表情符号，如果并非表情符号代码，则返回 <c>null</c>。</returns>
	/// <remarks>有关可用表情符号和代码的完整列表，请查看
	/// [Emoji-Cheat-Sheet](https://github.com/ikatyang/emoji-cheat-sheet/)。</remarks>
	/// <see href="https://github.com/ikatyang/emoji-cheat-sheet/"/>
	public static Emoji? GetEmoji(string code, TextSpan span = default)
	{
		if (Emojis.Infos.TryGetValue(code, out EmojiInfo? info))
		{
			return new Emoji(code, info, span);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 注册指定的 Unicode 表情符号。
	/// </summary>
	/// <param name="code">表情符号的代码。</param>
	/// <param name="text">表情符号的 Unicode 文本。</param>
	/// <param name="fallbackUrl">后备链接。</param>
	/// <exception cref="ArgumentNullException"><paramref name="code"/> 或 <paramref name="text"/>
	/// 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="code"/> 包含 <c>:</c>。</exception>
	public static void RegisterUnicodeEmoji(string code, string text, string? fallbackUrl = null)
	{
		ArgumentNullException.ThrowIfNull(code);
		ArgumentNullException.ThrowIfNull(text);
		if (code.Contains(':'))
		{
			throw new ArgumentException(Resources.EmojiNameCanNotContainsColon, nameof(code));
		}
		Emojis.Infos[code] = new EmojiInfo(true, text, fallbackUrl);
	}

	/// <summary>
	/// 注册指定的自定义表情符号。
	/// </summary>
	/// <param name="code">表情符号的代码。</param>
	/// <param name="url">表情符号的链接。</param>
	/// <exception cref="ArgumentNullException"><paramref name="code"/> 或 <paramref name="url"/>
	/// 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="code"/> 包含 <c>:</c>。</exception>
	public static void RegisterCustomEmoji(string code, string url)
	{
		ArgumentNullException.ThrowIfNull(code);
		ArgumentNullException.ThrowIfNull(url);
		if (code.Contains(':'))
		{
			throw new ArgumentException(Resources.EmojiNameCanNotContainsColon, nameof(code));
		}
		Emojis.Infos[code] = new EmojiInfo(false, null, url);
	}

	/// <summary>
	/// 表情符号的代码。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly EmojiInfo info;

	/// <summary>
	/// 使用指定的表情符号代码、信息和文本范围初始化 <see cref="Emoji"/> 类的新实例。
	/// </summary>
	/// <param name="code">表情符号的代码。</param>
	/// <param name="info">表情符号信息。</param>
	/// <param name="span">文本的范围。</param>
	private Emoji(string code, EmojiInfo info, TextSpan span = default) : base(MarkdownKind.Emoji)
	{
		Code = code;
		this.info = info;
		Span = span;
	}

	/// <summary>
	/// 获取表情符号的代码。
	/// </summary>
	public string Code { get; }

	/// <summary>
	/// 获取是否是 Unicode emoji。
	/// </summary>
	public bool IsUnicode => info.IsUnicode;

	/// <summary>
	/// 获取 Unicode emoji 的文本；非 Unicode emoji 会返回 <c>null</c>。
	/// </summary>
	public string? Text => info.Text;

	/// <summary>
	/// 获取后备链接。
	/// </summary>
	/// <remarks>用于非 Unicode emoji，或本地环境不支持特定 Unicode emoji 的场景降级到图片。</remarks>
	public string? FallbackUrl => info.FallbackUrl;

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitEmoji(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitEmoji(this)!;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{Emoji \"{Code}\" {Span}}}";
	}

	#region IEquatable<Emoji> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(Emoji? other)
	{
		if (other is null)
		{
			return false;
		}
		return Code == other.Code && Span == other.Span;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is Emoji other)
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
		return HashCode.Combine(Code, Span);
	}

	/// <summary>
	/// 返回指定的 <see cref="Emoji"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(Emoji? left, Emoji? right)
	{
		if (ReferenceEquals(left, right))
		{
			return true;
		}
		if (left is null)
		{
			return false;
		}
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="Emoji"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(Emoji? left, Emoji? right)
	{
		if (ReferenceEquals(left, right))
		{
			return false;
		}
		if (left is null)
		{
			return true;
		}
		return !left.Equals(right);
	}

	#endregion // IEquatable<Emoji> 成员

}
