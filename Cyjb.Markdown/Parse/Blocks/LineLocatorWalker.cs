using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 用于填充行定位器的节点访问器。
/// </summary>
internal sealed class LineLocatorWalker : SyntaxWalker
{
	/// <summary>
	/// 行定位器。
	/// </summary>
	private readonly LineLocator locator;

	/// <summary>
	/// 使用指定的行定位器初始化 <see cref="LineLocatorWalker"/> 类的新实例。
	/// </summary>
	/// <param name="locator">行定位器。</param>
	public LineLocatorWalker(LineLocator locator)
	{
		this.locator = locator;
	}

	/// <summary>
	/// 提供默认的访问行为。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	public override void DefaultVisit(Node node)
	{
		node.Locator = locator;
		base.DefaultVisit(node);
	}
}
