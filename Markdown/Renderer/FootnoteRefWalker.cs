using Cyjb.Markdown.Renderer;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown;

/// <summary>
/// 脚注引用的遍历器。
/// </summary>
/// <remarks>需要提前遍历脚注内部的脚注引用，确保脚注的反向引用是正确的。</remarks>
internal sealed class FootnoteRefWalker : SyntaxWalker
{
	/// <summary>
	/// 基础渲染器。
	/// </summary>
	private readonly BaseRenderer renderer;

	/// <summary>
	/// 使用指定的基础渲染器初始化 <see cref="FootnoteRefWalker"/> 类的新实例。
	/// </summary>
	/// <param name="renderer">基础渲染器。</param>
	public FootnoteRefWalker(BaseRenderer renderer)
	{
		this.renderer = renderer;
	}

	/// <summary>
	/// 访问指定的脚注引用节点。
	/// </summary>
	/// <param name="node">要访问的脚注引用节点。</param>
	public override void VisitFootnoteRef(FootnoteRef node)
	{
		renderer.GetBackref(node);
	}
}
