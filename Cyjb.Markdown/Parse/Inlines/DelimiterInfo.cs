using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Parse.Inlines;

/// <summary>
/// 表示 Markdown 的分隔符信息。
/// </summary>
internal sealed class DelimiterInfo
{
	/// <summary>
	/// 分隔符。
	/// </summary>
	public readonly char Delimiter;
	/// <summary>
	/// 是可以用作起始分隔符。
	/// </summary>
	public readonly bool CanOpen;
	/// <summary>
	/// 是可以用作结束分隔符。
	/// </summary>
	public readonly bool CanClose;
	/// <summary>
	/// 分隔符的原始长度。
	/// </summary>
	public readonly int OriginalLength;
	/// <summary>
	/// 分隔符对应的节点。
	/// </summary>
	public readonly Literal Node;
	/// <summary>
	/// 分隔符的长度。
	/// </summary>
	public int Length;
	/// <summary>
	/// 前驱分隔符信息。
	/// </summary>
	public DelimiterInfo? Prev;
	/// <summary>
	/// 后继分隔符信息。
	/// </summary>
	public DelimiterInfo? Next;

	/// <summary>
	/// 使用指定的分隔信息初始化 <see cref="DelimiterInfo"/> 类的新实例。
	/// </summary>
	/// <param name="delimiter">分隔符。</param>
	/// <param name="length">分隔符的长度。</param>
	/// <param name="canOpen">是否可以用作起始分隔符。</param>
	/// <param name="canClose">是否可以用作结束分隔符。</param>
	/// <param name="node">分隔符对应的节点。</param>
	public DelimiterInfo(char delimiter, int length, bool canOpen, bool canClose, Literal node)
	{
		Delimiter = delimiter;
		Node = node;
		OriginalLength = length;
		Length = length;
		CanOpen = canOpen;
		CanClose = canClose;
	}
}
