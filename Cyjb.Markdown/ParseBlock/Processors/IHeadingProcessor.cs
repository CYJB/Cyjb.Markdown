namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 标题的处理器。
/// </summary>
internal interface IHeadingProcessor
{
	/// <summary>
	/// 返回标题的链接标签。
	/// </summary>
	/// <returns>当前标题的链接标签。</returns>
	string GetIdentifier();
}
