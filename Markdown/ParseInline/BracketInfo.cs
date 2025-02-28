using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 表示 Markdown 的括号信息。
/// </summary>
internal sealed class BracketInfo
{
	/// <summary>
	/// 括号对应的节点。
	/// </summary>
	public readonly TempLiteral Node;
	/// <summary>
	/// 是否表示图片。
	/// </summary>
	public readonly bool IsImage;
	/// <summary>
	/// 源文件的起始索引。
	/// </summary>
	public readonly int StartIndex;
	/// <summary>
	/// 当前括号是否是活动状态。
	/// </summary>
	public bool Active = true;
	/// <summary>
	/// 当前括号之后是否包含了其它括号。
	/// </summary>
	public bool BracketAfter = false;
	/// <summary>
	/// 分隔符信息。
	/// </summary>
	public DelimiterInfo? Delimiter;

	/// <summary>
	/// 使用指定的括号信息初始化 <see cref="BracketInfo"/> 类的新实例。
	/// </summary>
	/// <param name="node">括号对应的节点。</param>
	/// <param name="isImage">是否表示图片。</param>
	/// <param name="startIndex">源文件的起始索引。</param>
	public BracketInfo(TempLiteral node, bool isImage, int startIndex)
	{
		Node = node;
		IsImage = isImage;
		StartIndex = startIndex;
	}
}
