namespace Cyjb.Markdown;

/// <summary>
/// Markdown 的解析选项。
/// </summary>
public readonly struct ParseOptions
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
		UseTaskListItem = false,
		UseTable = false,
		UseEmoji = false,
	};

	/// <summary>
	/// 初始化 <see cref="ParseOptions"/> 结构的新实例。
	/// </summary>
	public ParseOptions() { }

	/// <summary>
	/// 是否填充节点的行定位器，默认为 <c>false</c>。
	/// </summary>
	/// <remarks>填充行定位器后，可以支持通过 <see cref="Node.LinePositionSpan"/>
	/// 获取节点的行列位置。</remarks>
	public bool UseLineLocator { get; init; } = false;
	/// <summary>
	/// 是否解析删除线，默认为 <c>true</c>。
	/// </summary>
	public bool UseStrikethrough { get; init; } = true;
	/// <summary>
	/// 是否解析额外的列表样式类型（英文字母、罗马数字、希腊字母），默认为 <c>true</c>。
	/// </summary>
	public bool UseExtraListStyleType { get; init; } = true;
	/// <summary>
	/// 是否解析任务列表项，默认为 <c>true</c>。
	/// </summary>
	public bool UseTaskListItem { get; init; } = true;
	/// <summary>
	/// 是否解析表格，默认为 <c>true</c>。
	/// </summary>
	public bool UseTable { get; init; } = true;
	/// <summary>
	/// 是否解析表情符号，默认为 <c>true</c>。
	/// </summary>
	public bool UseEmoji { get; init; } = true;
}
