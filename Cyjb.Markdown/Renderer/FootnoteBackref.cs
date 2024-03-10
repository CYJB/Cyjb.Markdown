using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Renderer;

/// <summary>
/// 脚注的反向引用，仅用于渲染时临时使用。
/// </summary>
public sealed class FootnoteBackref : InlineNode
{
	/// <summary>
	/// 脚注信息。
	/// </summary>
	public readonly FootnoteInfo Info;
	/// <summary>
	/// 脚注引用的唯一标识符。
	/// </summary>
	public readonly string Identifier;

	/// <summary>
	/// 使用指定的脚注信息和唯一标识符初始化 <see cref="FootnoteBackref"/> 类的新实例。
	/// </summary>
	/// <param name="info">脚注信息。</param>
	/// <param name="identifier">脚注引用的唯一标识符。</param>
	public FootnoteBackref(FootnoteInfo info, string identifier) : base((MarkdownKind)(-1))
	{
		Info = info;
		Identifier = identifier;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return default!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		return new FootnoteBackref(Info, Identifier)
		{
			Locator = Locator,
		};
	}
}
