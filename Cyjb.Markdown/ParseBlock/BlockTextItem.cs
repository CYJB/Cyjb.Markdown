namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 块的文本项。
/// </summary>
internal struct BlockTextItem
{
	/// <summary>
	/// 文本。
	/// </summary>
	public StringView Text;
	/// <summary>
	/// 起始索引。
	/// </summary>
	public int StartIndex;

	/// <summary>
	/// 初始化 <see cref="BlockTextItem"/> 结构的新实例。
	/// </summary>
	/// <param name="text">文本。</param>
	/// <param name="startIndex">起始索引。</param>
	public BlockTextItem(StringView text, int startIndex)
	{
		Text = text;
		StartIndex = startIndex;
	}

	/// <summary>
	/// 结束索引。
	/// </summary>
	public readonly int EndIndex => StartIndex + Text.Length;
}
