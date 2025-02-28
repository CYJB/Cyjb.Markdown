namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 Markdown 的属性类型。
/// </summary>
internal enum AttributeKind
{
	/// <summary>
	/// 标识符。
	/// </summary>
	Identifier,
	/// <summary>
	/// 类名。
	/// </summary>
	ClassName,
	/// <summary>
	/// 普通属性。
	/// </summary>
	Common,
	/// <summary>
	/// 属性的分隔符。
	/// </summary>
	Seperator,
	/// <summary>
	/// 属性的结束符。
	/// </summary>
	End,
	/// <summary>
	/// 无效的属性 Token。
	/// </summary>
	Invalid,
}
