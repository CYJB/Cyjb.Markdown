using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Renderer;

/// <summary>
/// HTML 属性的修改器。
/// </summary>
public interface IAttributeModifier
{
	/// <summary>
	/// 更新 HTML 标签的属性。
	/// </summary>
	/// <param name="node">当前节点。</param>
	/// <param name="tagName">当前标签名。</param>
	/// <param name="attributes">当前属性列表。</param>
	void UpdateAttributes(Node node, string tagName, HtmlAttributeList attributes);
}
