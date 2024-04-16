using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的临时文本。
/// </summary>
public sealed class TempLiteral : InlineNode
{
	/// <summary>
	/// 文本的内容。
	/// </summary>
	public StringView Content;

	/// <summary>
	/// 使用指定的文本内容和文本范围初始化 <see cref="TempLiteral"/> 类的新实例。
	/// </summary>
	/// <param name="content">文本的内容。</param>
	/// <param name="span">文本的范围。</param>
	public TempLiteral(StringView content, TextSpan span = default) : base(MarkdownKind.Literal)
	{
		Content = content;
		Span = span;
	}


	/// <summary>
	/// 连接指定的临时文本。
	/// </summary>
	/// <param name="literal">要连接的临时文本。</param>
	public void Concat(TempLiteral literal)
	{
		if (!Content.TryConcat(literal.Content, out Content))
		{
			// 应该时一定要保证当前节点与 literal 是可以拼接起来的。
			throw CommonExceptions.Unreachable();
		}
		Span = Span with
		{
			End = literal.Span.End,
		};
	}

	/// <summary>
	/// 返回文本节点。
	/// </summary>
	/// <returns>文本节点。</returns>
	public Literal GetLiteral()
	{
		return new Literal(Content.Unescape(), Span)
		{
			Parent = Parent
		};
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
		return new TempLiteral(Content, Span)
		{
			Locator = Locator,
		};
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{TempLiteral \"{Content}\" {Span}}}";
	}
}
