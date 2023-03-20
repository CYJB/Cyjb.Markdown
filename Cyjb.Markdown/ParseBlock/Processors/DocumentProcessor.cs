using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 文档的解析器。
/// </summary>
internal sealed class DocumentProcessor : BlockProcessor
{
	/// <summary>
	/// Markdown 文档。
	/// </summary>
	private readonly Document document = new();

	/// <summary>
	/// 初始化 <see cref="DocumentProcessor"/> 类的新实例。
	/// </summary>
	public DocumentProcessor() : base(MarkdownKind.Document) { }

	/// <summary>
	/// 获取是否是容器节点。
	/// </summary>
	public override bool IsContainer => true;

	/// <summary>
	/// 获取是否可以延迟延伸。
	/// </summary>
	public override bool CanLazyContinuation => false;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		return BlockContinue.Continue;
	}

	/// <summary>
	/// 返回当前节点是否可以包含指定类型的子节点。
	/// </summary>
	/// <param name="kind">要检查的节点类型。</param>
	/// <returns>如果当前节点可以包含指定类型的子节点，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool CanContains(MarkdownKind kind)
	{
		return true;
	}

	/// <summary>
	/// 添加一个新节点。
	/// </summary>
	/// <param name="node">要添加的节点。</param>
	public override void AddNode(Node node)
	{
		document.Children.Add((BlockNode)node);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end)
	{
		return null;
	}

	/// <summary>
	/// 完成并返回 Markdown 文档。
	/// </summary>
	/// <param name="end">结束索引。</param>
	/// <returns>Markdown 文档。</returns>
	public Document GetDocument(int end)
	{
		document.Span = new TextSpan(0, end);
		return document;
	}
}
