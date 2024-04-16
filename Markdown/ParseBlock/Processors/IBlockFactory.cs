namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 块处理器工厂。
/// </summary>
internal interface IBlockFactory
{
	/// <summary>
	/// 尝试开始当前块的解析。
	/// </summary>
	/// <param name="parser">块级语法分析器。</param>
	/// <param name="line">要检查的行。</param>
	/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
	/// <param name="processors">要添加到的处理器列表。</param>
	void TryStart(BlockParser parser, BlockLine line, BlockProcessor matchedProcessor, List<BlockProcessor> processors);
}
