namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 块处理器工厂。
/// </summary>
internal interface IBlockFactory
{
	/// <summary>
	/// 尝试开始当前块的解析。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
	/// <returns>如果能够开始当前块的解析，则返回解析器序列。否则返回空序列。</returns>
	IEnumerable<BlockProcessor> TryStart(BlockText line, BlockProcessor matchedProcessor);
}
