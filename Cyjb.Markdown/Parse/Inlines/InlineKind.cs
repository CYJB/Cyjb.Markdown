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
	/// 链接标签。
	/// </summary>
	LinkLabel,
	/// <summary>
	/// 链接体。
	/// </summary>
	LinkBody,
	/// <summary>
	/// 行级节点。
	/// </summary>
	Node,
	/// <summary>
	/// 自动链接。
	/// </summary>
	Autolink,
}
