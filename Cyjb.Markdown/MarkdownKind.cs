namespace Cyjb.Markdown;

/// <summary>
/// 表示 Markdown 的节点类型。
/// </summary>
public enum MarkdownKind
{
	/// <summary>
	/// Markdown 文档。
	/// </summary>
	Document,

	/// <summary>
	/// 分隔线。
	/// </summary>
	ThematicBreak,
	/// <summary>
	/// 标题。
	/// </summary>
	Heading,
	/// <summary>
	/// 代码块
	/// </summary>
	CodeBlock,
	/// <summary>
	/// 数学公式块
	/// </summary>
	MathBlock,
	/// <summary>
	/// HTML 块。
	/// </summary>
	HtmlBlock,
	/// <summary>
	/// 链接定义。
	/// </summary>
	LinkDefinition,
	/// <summary>
	/// 段落。
	/// </summary>
	Paragraph,
	/// <summary>
	/// 块引用。
	/// </summary>
	Blockquote,
	/// <summary>
	/// 列表。
	/// </summary>
	List,
	/// <summary>
	/// 列表项。
	/// </summary>
	ListItem,
	/// <summary>
	/// 表格。
	/// </summary>
	Table,
	/// <summary>
	/// 表格的行。
	/// </summary>
	TableRow,
	/// <summary>
	/// 表格的单元格。
	/// </summary>
	TableCell,

	/// <summary>
	/// 行内代码段。
	/// </summary>
	CodeSpan,
	/// <summary>
	/// 强调。
	/// </summary>
	Emphasis,
	/// <summary>
	/// 粗体。
	/// </summary>
	Strong,
	/// <summary>
	/// 删除线。
	/// </summary>
	Strikethrough,
	/// <summary>
	/// 链接。
	/// </summary>
	Link,
	/// <summary>
	/// 图片。
	/// </summary>
	Image,
	/// <summary>
	/// 行内 HTML 起始标签。
	/// </summary>
	HtmlStartTag,
	/// <summary>
	/// 行内 HTML 结束标签。
	/// </summary>
	HtmlEndTag,
	/// <summary>
	/// 行内 HTML 注释。
	/// </summary>
	HtmlComment,
	/// <summary>
	/// 行内 HTML 处理结构。
	/// </summary>
	HtmlProcessing,
	/// <summary>
	/// 行内 HTML 声明。
	/// </summary>
	HtmlDeclaration,
	/// <summary>
	/// 行内 HTML CDATA 段。
	/// </summary>
	HtmlCData,
	/// <summary>
	/// 硬换行。
	/// </summary>
	HardBreak,
	/// <summary>
	/// 软换行。
	/// </summary>
	SoftBreak,
	/// <summary>
	/// 表情符号。
	/// </summary>
	Emoji,
	/// <summary>
	/// 行内数学公式。
	/// </summary>
	MathSpan,
	/// <summary>
	/// 文本。
	/// </summary>
	Literal,
}
