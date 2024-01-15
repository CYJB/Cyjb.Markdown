using Cyjb.Collections;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Utils;

/// <summary>
/// Alt 文本的渲染器。
/// </summary>
internal sealed class AltTextRenderer : SyntaxWalker, IDisposable
{
	/// <summary>
	/// 文本列表。
	/// </summary>
	private readonly PooledList<char> text = new();

	/// <summary>
	/// 初始化 <see cref="AltTextRenderer"/> 类的新实例。
	/// </summary>
	public AltTextRenderer() { }

	/// <summary>
	/// 返回 Alt 文本内容。
	/// </summary>
	public ReadOnlySpan<char> Text => text.AsSpan();

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
		text.Add(node.Content);
	}

	/// <summary>
	/// 访问指定的换行节点。
	/// </summary>
	/// <param name="node">要访问的换行节点。</param>
	public override void VisitBreak(Break node)
	{
		text.Add(' ');
	}

	/// <summary>
	/// 访问指定的文本节点。
	/// </summary>
	/// <param name="node">要访问的文本节点。</param>
	public override void VisitLiteral(Literal node)
	{
		text.Add(node.Content);
	}

	/// <summary>
	/// 访问指定的表情符号节点。
	/// </summary>
	/// <param name="node">要访问的表情符号节点。</param>
	public override void VisitEmoji(Emoji node)
	{
		text.Add(':');
		text.Add(node.Code);
		text.Add(':');
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return text.ToString();
	}

	/// <summary>
	/// 释放非托管资源。
	/// </summary>
	public void Dispose()
	{
		text.Dispose();
		GC.SuppressFinalize(this);
	}
}
