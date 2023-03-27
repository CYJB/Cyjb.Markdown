using Cyjb.Markdown.Syntax;

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
		UseMath = false,
		UseMathAttributes = false,
		UseExtAutolink = false,
		UseHeaderAttributes = false,
		UseCodeAttributes = false,
		UseLinkAttributes = false,
		UseAutoIdentifier = false,
		UseHeaderReferences = false,
		UseFootnotes = false,
		UseCustomContainers = false,
		UseCustomContainerAttributes = false,
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
	/// <see href="https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#styling-text"/>
	public bool UseStrikethrough { get; init; } = true;
	/// <summary>
	/// 是否解析额外的列表样式类型（英文字母、罗马数字、希腊字母），默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md"/>
	public bool UseExtraListStyleType { get; init; } = true;
	/// <summary>
	/// 是否解析任务列表项，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#task-lists"/>
	public bool UseTaskListItem { get; init; } = true;
	/// <summary>
	/// 是否解析表格，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://docs.github.com/zh/get-started/writing-on-github/working-with-advanced-formatting/organizing-information-with-tables"/>
	public bool UseTable { get; init; } = true;
	/// <summary>
	/// 是否解析表情符号，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#%E4%BD%BF%E7%94%A8%E8%A1%A8%E6%83%85%E7%AC%A6%E5%8F%B7"/>
	public bool UseEmoji { get; init; } = true;
	/// <summary>
	/// 是否解析数学公式，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md"/>
	public bool UseMath { get; init; } = true;
	/// <summary>
	/// 是否支持数学公式的属性，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/mathematics.md"/>
	public bool UseMathAttributes { get; init; } = true;
	/// <summary>
	/// 是否支持扩展自动链接，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://docs.github.com/zh/get-started/writing-on-github/working-with-advanced-formatting/autolinked-references-and-urls#url"/>
	public bool UseExtAutolink { get; init; } = true;
	/// <summary>
	/// 是否支持标题的属性，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#标题属性"/>
	public bool UseHeaderAttributes { get; init; } = true;
	/// <summary>
	/// 是否支持代码的属性，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#代码块属性"/>
	public bool UseCodeAttributes { get; init; } = true;
	/// <summary>
	/// 是否支持链接的属性，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md#链接属性"/>
	public bool UseLinkAttributes { get; init; } = true;
	/// <summary>
	/// 属性的前缀，用于指定标题、代码或链接属性的前缀。默认为 <c>null</c>。
	/// </summary>
	public string? AttributesPrefix { get; init; } = null;
	/// <summary>
	/// 是否支持为标题自动生成标识符，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/auto-identifiers.md"/>
	public bool UseAutoIdentifier { get; init; } = true;
	/// <summary>
	/// 是否支持将标题作为链接引用使用，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/header-references.md"/>
	public bool UseHeaderReferences { get; init; } = true;
	/// <summary>
	/// 是否支持脚注，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/footnotes.md"/>
	public bool UseFootnotes { get; init; } = true;
	/// <summary>
	/// 是否支持自定义容器，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md"/>
	public bool UseCustomContainers { get; init; } = true;
	/// <summary>
	/// 是否支持自定义容器的属性，默认为 <c>true</c>。
	/// </summary>
	/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/custom-containers.md"/>
	public bool UseCustomContainerAttributes { get; init; } = true;
}
