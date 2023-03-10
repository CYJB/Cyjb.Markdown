using System.Diagnostics;
using Cyjb.Markdown.ParseBlock;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown;

/// <summary>
/// 表示一个 Markdown 文档。
/// </summary>
public sealed class Document : Node, INodeContainer<BlockNode>
{
	/// <summary>
	/// 解析指定 Markdown 字符串，并返回解析后的语法树。
	/// </summary>
	/// <param name="text">要解析的字符串。</param>
	/// <param name="options">解析的选项。</param>
	/// <returns>解析得到的 Markdown 语法树。</returns>
	public static Document Parse(string text, ParseOptions? options = null)
	{
		return new BlockParser(new StringReader(text), options).Parse();
	}

	/// <summary>
	/// 解析指定 Markdown 字符串，并返回解析后的语法树。
	/// </summary>
	/// <param name="textReader">要解析的文本读取器。</param>
	/// <param name="options">解析的选项。</param>
	/// <returns>解析得到的 Markdown 语法树。</returns>
	public static Document Parse(TextReader textReader, ParseOptions? options = null)
	{
		return new BlockParser(textReader, options).Parse();
	}

	/// <summary>
	/// 子节点列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<BlockNode> children;

	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="Document"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public Document(TextSpan span = default) : base(MarkdownKind.Document)
	{
		children = new NodeList<BlockNode>(this);
		Span = span;
	}

	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public NodeList<BlockNode> Children => children;

	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	/// <value>总是为 <c>null</c>。</value>
	public override Node? Prev => null;
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	/// <value>总是为 <c>null</c>。</value>
	public override Node? Next => null;
	/// <summary>
	/// 获取第一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个子节点，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override BlockNode? LastChild => children.LastOrDefault();

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetPrev(Node? node) { }
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetNext(Node? node) { }

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitDocument(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitDocument(this)!;
	}
}
