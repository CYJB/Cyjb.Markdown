using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 块分隔符的信息。
/// </summary>
internal class BlockFenceInfo<TBlock>
{
	/// <summary>
	/// 块的分割字符。
	/// </summary>
	public char Fence;
	/// <summary>
	/// 块的分割符长度。
	/// </summary>
	public int FenceLength;
	/// <summary>
	/// 块节点。
	/// </summary>
	public TBlock Node;

	/// <summary>
	/// 初始化 <see cref="BlockFenceInfo{TBlock}"/> 类的新实例。
	/// </summary>
	/// <param name="fence">块的分割字符。</param>
	/// <param name="length">块的分割长度。</param>
	/// <param name="node">块的节点实例。</param>
	public BlockFenceInfo(char fence, int length, TBlock node)
	{
		Fence = fence;
		FenceLength = length;
		Node = node;
	}
}
