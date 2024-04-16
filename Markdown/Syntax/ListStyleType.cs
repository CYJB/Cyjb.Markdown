namespace Cyjb.Markdown.Syntax;

/// <summary>
/// Markdown 列表的样式类型。
/// </summary>
public enum ListStyleType
{
	/// <summary>
	/// 无序列表。
	/// </summary>
	Unordered,
	/// <summary>
	/// 有序数字列表。
	/// </summary>
	OrderedNumber,
	/// <summary>
	/// 有序小写字母列表。
	/// </summary>
	OrderedLowerAlpha,
	/// <summary>
	/// 有序大写字母列表。
	/// </summary>
	OrderedUpperAlpha,
	/// <summary>
	/// 有序小写罗马数字列表。
	/// </summary>
	OrderedLowerRoman,
	/// <summary>
	/// 有序大写罗马数字列表。
	/// </summary>
	OrderedUpperRoman,
	/// <summary>
	/// 有序小写希腊字母列表。
	/// </summary>
	OrderedLowerGreek,
}
