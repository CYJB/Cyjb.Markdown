using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的表格。
/// </summary>
/// <remarks>总是将首行作为标题行，使用标题行的单元格数作为列数。</remarks>
public sealed class Table : BlockNode, INodeContainer<TableRow>
{
	/// <summary>
	/// 行列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly NodeList<TableRow> children;

	/// <summary>
	/// 使用指定的标题行和文本范围初始化 <see cref="Table"/> 类的新实例。
	/// </summary>
	/// <param name="heading">标题行。</param>
	/// <param name="span">文本范围。</param>
	/// <exception cref="ArgumentNullException"><paramref name="heading"/> 为 <c>null</c>。</exception>
	public Table(TableRow heading, TextSpan span = default) : base(MarkdownKind.Table)
	{
		ArgumentNullException.ThrowIfNull(heading);
		children = new NodeList<TableRow>(this)
		{
			heading
		};
		Aligns = new TableAlignList(this);
		Span = span;
	}

	/// <summary>
	/// 初始化 <see cref="Table"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	/// <remarks>表格不包含行的状态是非法的，必须在创建后正确填充行。</remarks>
	private Table(TextSpan span = default) : base(MarkdownKind.Table)
	{
		children = new NodeList<TableRow>(this);
		Aligns = new TableAlignList(this);
		Span = span;
	}

	/// <summary>
	/// 获取行列表。
	/// </summary>
	public NodeList<TableRow> Children => children;
	/// <summary>
	/// 获取表格的对齐方式列表。
	/// </summary>
	public TableAlignList Aligns { get; }
	/// <summary>
	/// 获取表格的列数。
	/// </summary>
	public int ColumnCount => children[0].Children.Count;
	/// <summary>
	/// 获取表格的行数（包含标题行）。
	/// </summary>
	public int RowCount => children.Count;

	/// <summary>
	/// 获取标题行。
	/// </summary>
	public override TableRow FirstChild => children[0];
	/// <summary>
	/// 获取最后一行。
	/// </summary>
	public override TableRow LastChild => children.Last();

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitTable(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitTable(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		if (deep)
		{
			// 深度复制。
			Table node = new(Span)
			{
				Locator = Locator,
			};
			children.CloneTo(node.children, context);
			Aligns.CloneTo(node.Aligns);
			return node;
		}
		else
		{
			// 非深度复制，创建含有一个标题单元格的表格。
			return new Table(new TableRow(new TableCell()), Span)
			{
				Locator = Locator,
			};
		}
	}
}
