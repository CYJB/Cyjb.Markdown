using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的脚注引用。
/// </summary>
public sealed class FootnoteRef : InlineNode
{
	/// <summary>
	/// 引用的脚注。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Footnote footnote;

	/// <summary>
	/// 使用要引用的脚注和文本范围初始化 <see cref="FootnoteRef"/> 类的新实例。
	/// </summary>
	/// <param name="footnote">引用的脚注。</param>
	/// <param name="span">文本范围。</param>
	public FootnoteRef(Footnote footnote, TextSpan span = default) : base(MarkdownKind.FootnoteRef)
	{
		this.footnote = footnote;
		Span = span;
	}

	/// <summary>
	/// 获取或设置引用的脚注。
	/// </summary>
	/// <exception cref="ArgumentNullException">引用的脚注为 <c>null</c>。</exception>
	public Footnote Footnote
	{
		get => footnote;
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			footnote = value;
		}
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitFootnoteRef(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitFootnoteRef(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		return new FootnoteRef((Footnote)footnote.CloneNode(deep, context), Span)
		{
			Locator = Locator,
		};
	}
}
