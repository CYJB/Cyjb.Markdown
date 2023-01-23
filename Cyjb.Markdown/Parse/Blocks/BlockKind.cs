namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 表示 Markdown 的块级单元类型。
/// </summary>
internal enum BlockKind
{
	/// <summary>
	/// 缩进。
	/// </summary>
	Indent,
	/// <summary>
	/// 引用的起始标记。
	/// </summary>
	QuoteStart,
	/// <summary>
	/// 代码分隔符。
	/// </summary>
	CodeFence,
	/// <summary>
	/// HTML 块的起始。
	/// </summary>
	HtmlStart,
	/// <summary>
	/// 无序列表标志。
	/// </summary>
	UnorderedListMarker,
	/// <summary>
	/// 有序列表标志。
	/// </summary>
	OrderedListMarker,
	/// <summary>
	/// 任务列表项标志。
	/// </summary>
	TaskListItemMarker,
	/// <summary>
	/// 代码分隔符的起始。
	/// </summary>
	CodeFenceStart,
	/// <summary>
	/// 由一个或多个 -，后跟空白组成的行。
	/// </summary>
	DashLine,
	/// <summary>
	/// 分割线。
	/// </summary>
	ThematicBreak,
	/// <summary>
	/// ATX 标题。
	/// </summary>
	ATXHeading,
	/// <summary>
	/// Setext 标题的下划线。
	/// </summary>
	SetextUnderline,

	/// <summary>
	/// 块级节点。
	/// </summary>
	BlockNode,
	/// <summary>
	/// 换行。
	/// </summary>
	NewLine,
	/// <summary>
	/// 文本行。
	/// </summary>
	TextLine,
}
