namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 表示 Markdown 的行级单元类型。
/// </summary>
internal enum InlineKind
{
	/// <summary>
	/// 分隔符。
	/// </summary>
	Delimiter,
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
}
