namespace Cyjb.Markdown;

/// <summary>
/// Markdown 的解析选项。
/// </summary>
public sealed class ParseOptions
{
	/// <summary>
	/// 默认的解析选项。
	/// </summary>
	public static readonly ParseOptions Default = new();
	/// <summary>
	/// CommonMark 的解析选项。
	/// </summary>
	/// <remarks>只解析 CommonMark 语法。</remarks>
	/// <see href="https://commonmark.org/"/>
	public static readonly ParseOptions CommonMark = new()
	{
		UseStrikethrough = false,
		UseExtraListStyleType = false,
	};

	/// <summary>
	/// 是否解析删除线，默认为 <c>true</c>。
	/// </summary>
	public bool UseStrikethrough { get; init; } = true;
	/// <summary>
	/// 是否解析额外的列表样式类型（英文字母、罗马数字、希腊字母），默认为 <c>true</c>。
	/// </summary>
	public bool UseExtraListStyleType { get; init; } = true;
}
