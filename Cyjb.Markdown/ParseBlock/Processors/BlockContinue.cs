namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 块节点的延伸结果。
/// </summary>
internal enum BlockContinue
{
	/// <summary>
	/// 不能延伸。
	/// </summary>
	None,
	/// <summary>
	/// 可以延伸。
	/// </summary>
	Continue,
	/// <summary>
	/// 延伸并结束当前节点。
	/// </summary>
	Closed,
}
