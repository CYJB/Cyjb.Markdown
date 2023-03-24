namespace Cyjb.Markdown.Renderer;

/// <summary>
/// 脚注的信息。
/// </summary>
public sealed class FootnoteInfo
{
	/// <summary>
	/// 脚注的索引。
	/// </summary>
	public readonly string Index;
	/// <summary>
	/// 索引的 ID。
	/// </summary>
	public readonly string Id;
	/// <summary>
	/// 索引的引用 ID。
	/// </summary>
	public readonly string RefId;
	/// <summary>
	/// 脚注的反向引用列表。
	/// </summary>
	public readonly List<FootnoteBackref> Backrefs = new();

	/// <summary>
	/// 使用指定的脚注索引和标识符初始化 <see cref="FootnoteInfo"/> 类的新实例。
	/// </summary>
	/// <param name="index">脚注的索引。</param>
	/// <param name="id">脚注的标识符。</param>
	/// <param name="refId">脚注引用的标识符。</param>
	public FootnoteInfo(string index, string id, string refId)
	{
		Index = index;
		Id = id;
		RefId = refId;
	}
}
