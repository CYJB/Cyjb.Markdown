using System.Text;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Utils;

/// <summary>
/// Alt 文本的渲染器。
/// </summary>
internal sealed class AltTextRenderer : SyntaxWalker
{
	/// <summary>
	/// 文本构造器。
	/// </summary>
	private readonly StringBuilder text = new();

	/// <summary>
	/// 初始化 <see cref="AltTextRenderer"/> 类的新实例。
	/// </summary>
	public AltTextRenderer() { }

	/// <summary>
	/// 返回内部的文本构造器。
	/// </summary>
	public StringBuilder Text => text;

	/// <summary>
	/// 清除已生成的 HTML 文本。
	/// </summary>
	public void Clear()
	{
		text.Clear();
	}

	/// <summary>
	/// 访问指定的行内代码段节点。
	/// </summary>
	/// <param name="node">要访问的行内代码段节点。</param>
	public override void VisitCodeSpan(CodeSpan node)
	{
		text.Append(node.Content);
	}

	/// <summary>
	/// 访问指定的换行节点。
	/// </summary>
	/// <param name="node">要访问的换行节点。</param>
	public override void VisitBreak(Break node)
	{
		text.Append(' ');
	}

	/// <summary>
	/// 访问指定的文本节点。
	/// </summary>
	/// <param name="node">要访问的文本节点。</param>
	public override void VisitLiteral(Literal node)
	{
		text.Append(node.Content);
	}

	/// <summary>
	/// 访问指定的表情符号节点。
	/// </summary>
	/// <param name="node">要访问的表情符号节点。</param>
	public override void VisitEmoji(Emoji node)
	{
		text.Append(':');
		text.Append(node.Code);
		text.Append(':');
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return text.ToString();
	}
}
