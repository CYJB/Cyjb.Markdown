using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的引用块。
/// </summary>
public sealed class Quote : BlockNode, INodeContainer<BlockNode>
{
	/// <summary>
	/// 子节点列表。
	/// </summary>
	private readonly NodeList<BlockNode> children;

	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="Document"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public Quote(TextSpan span = default) : base(MarkdownKind.Quote)
	{
		children = new NodeList<BlockNode>(this);
		Span = span;
	}

	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<BlockNode> Children => children;

	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? LastChild => children.LastOrDefault();

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitQuote(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitQuote(this)!;
	}
}
