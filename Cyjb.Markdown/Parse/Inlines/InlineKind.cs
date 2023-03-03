namespace Cyjb.Markdown.Parse.Inlines;

/// <summary>
/// 表示 Markdown 的行级单元类型。
/// </summary>
internal enum InlineKind
{
	/// <summary>
	/// 文本。
	/// </summary>
	Literal,
	/// <summary>
	/// 被转义的文本。
	/// </summary>
	Escaped,
	/// <summary>
	/// 链接的起始。
	/// </summary>
	LinkStart,
	/// <summary>
	/// 链接的闭合。
	/// </summary>
	LinkClose,
	/// <summary>
	/// 行级节点。
	/// </summary>
	Node,
	/// <summary>
	/// 自动链接。
	/// </summary>
	Autolink,
	/// <summary>
	/// 扩展自动链接。
	/// </summary>
	ExtAutolink,
}
