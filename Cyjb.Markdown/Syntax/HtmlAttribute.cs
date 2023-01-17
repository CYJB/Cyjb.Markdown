namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的 HTML 标签属性。
/// </summary>
public readonly struct HtmlAttribute : IEquatable<HtmlAttribute>
{
	/// <summary>
	/// 使用指定的属性名称和值初始化 <see cref="HtmlAttribute"/> 结构的新实例。
	/// </summary>
	/// <param name="name">属性的名称。</param>
	/// <param name="value">属性的值。</param>
	/// <param name="quote">值的引号。</param>
	/// <exception cref="ArgumentNullException">任意参数为 <c>null</c>。</exception>
	public HtmlAttribute(string name, string value = "", string quote = "")
	{
		ArgumentNullException.ThrowIfNull(name);
		ArgumentNullException.ThrowIfNull(value);
		ArgumentNullException.ThrowIfNull(quote);
		Name = name;
		Value = value;
		Quote = quote;
	}

	/// <summary>
	/// 获取属性的名称。
	/// </summary>
	public string Name { get; }
	/// <summary>
	/// 获取属性的值。
	/// </summary>
	public string Value { get; }
	/// <summary>
	/// 值的引号符号。
	/// </summary>
	/// <remarks>可能为 <c>"</c>、<c>'</c> 或者空字符串（表示无引号的值）。</remarks>
	public string Quote { get; }

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		if (Value.Length == 0)
		{
			return Name;
		}
		return $"{Name}={Quote}{Value}{Quote}";
	}

	#region IEquatable<HtmlAttribute> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(HtmlAttribute other)
	{
		return Name == other.Name && Value == other.Value;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is HtmlAttribute other)
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
		return HashCode.Combine(Name, Value);
	}

	/// <summary>
	/// 返回指定的 <see cref="HtmlAttribute"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(HtmlAttribute left, HtmlAttribute right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="HtmlAttribute"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(HtmlAttribute left, HtmlAttribute right)
	{
		return !left.Equals(right);
	}

	#endregion // IEquatable<HtmlAttribute> 成员

}
