using Cyjb.Text;
using System.Diagnostics;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 表格的行。
/// </summary>
public sealed class TableRow : Node, INodeContainer<TableCell>
{
	/// <summary>
	/// 前驱兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TableRow? prev;
	/// <summary>
	/// 后继兄弟节点。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TableRow? next;
	/// <summary>
	/// 单元格列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<TableCell> children;

	/// <summary>
	/// 使用指定的单元格集合和文本范围初始化 <see cref="TableRow"/> 类的新实例。
	/// </summary>
	/// <param name="cells">单元格集合。</param>
	/// <param name="span">文本范围。</param>
	/// <exception cref="ArgumentNullException"><paramref name="cells"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="cells"/> 是空的。</exception>
	public TableRow(IEnumerable<TableCell> cells, TextSpan span = default)
		: base(MarkdownKind.TableRow)
	{
		ArgumentNullException.ThrowIfNull(cells);
		children = new NodeList<TableCell>(this);
		foreach (TableCell cell in cells)
		{
			children.Add(cell);
		}
		if (children.Count == 0)
		{
			throw new ArgumentException(Resources.RowMustHaveCell, nameof(cells));
		}
		Span = span;
	}

	/// <summary>
	/// 获取所属表格。
	/// </summary>
	public new Table? Parent => base.Parent as Table;
	/// <summary>
	/// 获取单元格列表。
	/// </summary>
	public NodeList<TableCell> Children => children;
	/// <summary>
	/// 获取前驱兄弟节点。
	/// </summary>
	public override TableRow? Prev => prev;
	/// <summary>
	/// 获取后继兄弟节点。
	/// </summary>
	public override TableRow? Next => next;

	/// <summary>
	/// 获取第一个单元格，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override TableCell? FirstChild => children.FirstOrDefault();
	/// <summary>
	/// 获取最后一个单元格，如果不存在则返回 <c>null</c>。
	/// </summary>
	public override TableCell? LastChild => children.LastOrDefault();

	/// <summary>
	/// 设置前驱兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetPrev(Node? node)
	{
		prev = node as TableRow;
	}
	/// <summary>
	/// 设置后继兄弟节点。
	/// </summary>
	/// <param name="node">要设置的节点。</param>
	internal override void SetNext(Node? node)
	{
		next = node as TableRow;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitTableRow(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitTableRow(this)!;
	}
}
