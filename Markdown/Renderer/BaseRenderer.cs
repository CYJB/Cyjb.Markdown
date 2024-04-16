using Cyjb.Collections;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.Renderer;

/// <summary>
/// Markdown 的基础渲染器。
/// </summary>
public abstract class BaseRenderer : SyntaxWalker
{
	/// <summary>
	/// Alt 文本的渲染器。
	/// </summary>
	private AltTextRenderer? altTextRenderer;
	/// <summary>
	/// 脚注引用的遍历器。
	/// </summary>
	private FootnoteRefWalker? footnoteRefWalker;
	/// <summary>
	/// 脚注列表。
	/// </summary>
	private readonly List<Footnote> footnotes = new();
	/// <summary>
	/// 脚注的信息字典。
	/// </summary>
	private readonly Dictionary<Footnote, FootnoteInfo> footnoteInfo = new();
	/// <summary>
	/// 脚注的反向引用字典。
	/// </summary>
	private readonly Dictionary<FootnoteRef, FootnoteBackref> footnoteBackrefs = new();
	/// <summary>
	/// 唯一标识符生成器。
	/// </summary>
	private readonly UniqueIdentifier uniqueIdentifier = new();

	/// <summary>
	/// 当前是否正在输出表格标题。
	/// </summary>
	protected bool isTableHeading = false;
	/// <summary>
	/// 当前表格对齐。
	/// </summary>
	protected TableAlign tableAlign = TableAlign.None;
	/// <summary>
	/// 当前是否正输出脚注。
	/// </summary>
	protected bool isInFootnote = false;

	/// <summary>
	/// 初始化 <see cref="BaseRenderer"/> 类的新实例。
	/// </summary>
	protected BaseRenderer() { }

	/// <summary>
	/// 获取或设置软换行的字符，默认为 <c>\n</c>。
	/// </summary>
	public string SoftBreak { get; set; } = "\n";
	/// <summary>
	/// 是否输出空的表格标题，默认为 <c>false</c>。
	/// </summary>
	/// <remarks>如果表格标题的单元格全部是空的，那么若设置为 <c>false</c>，
	/// 不会输出 <c>&lt;thead&gt;</c>；若设置为 <c>true</c>，则会输出 <c>&lt;thead&gt;</c>。</remarks>
	public bool OutputEmptyTableHeading { get; set; } = false;

	/// <summary>
	/// 清除已生成的 HTML 文本。
	/// </summary>
	public virtual void Clear()
	{
		footnotes.Clear();
		footnoteInfo.Clear();
		footnoteBackrefs.Clear();
		uniqueIdentifier.Clear();
	}

	/// <summary>
	/// 访问指定的文档节点。
	/// </summary>
	/// <param name="node">要访问的文档节点。</param>
	public override void VisitDocument(Document node)
	{
		DefaultVisit(node);
		// 在文档输出完毕后，再输出脚注。
		if (footnotes.Count > 0)
		{
			isInFootnote = true;
			WriteFootnotes(node, footnotes);
			isInFootnote = false;
		}
	}

	/// <summary>
	/// 访问指定的链接定义节点。
	/// </summary>
	/// <param name="node">要访问的链接定义节点。</param>
	public override void VisitLinkDefinition(LinkDefinition node) { }

	/// <summary>
	/// 访问指定的表格行。
	/// </summary>
	/// <param name="node">要访问的表格行节点。</param>
	public override void VisitTableRow(TableRow node)
	{
		Table? table = node.Parent;
		if (table == null)
		{
			// 不存在父表格，直接依次输出。
			DefaultVisit(node);
		}
		else
		{
			// 存在父表格，设置每列的对齐，并确保每行的列数一致。
			int columnCount = table.ColumnCount;
			int len = Math.Min(node.Children.Count, columnCount);
			for (int i = 0; i < len; i++)
			{
				tableAlign = table.Aligns[i];
				node.Children[i].Accept(this);
			}
			tableAlign = TableAlign.None;
			for (int i = len; i < columnCount; i++)
			{
				WriteEmptyTableCell(node);
			}
		}
	}

	/// <summary>
	/// 写入空的表格单元格，用于补齐表格行缺少的单元格。
	/// </summary>
	/// <param name="row">缺少单元格的表格行。</param>
	protected virtual void WriteEmptyTableCell(TableRow row)
	{
	}

	/// <summary>
	/// 访问指定的表格。
	/// </summary>
	/// <param name="node">要访问的表格节点。</param>
	public override void VisitTable(Table node)
	{
		// 输出表格头。
		bool hasHeading = true;
		if (!OutputEmptyTableHeading)
		{
			hasHeading = node.Children[0].Children.Any(cell => cell.Children.Count > 0);
		}
		if (hasHeading)
		{
			WriteTableHead(node, node.Children[0]);
		}
		// 输出表格的内容
		if (node.Children.Count > 1)
		{
			WriteTableBody(node, node.Children.Skip(1));
		}
	}

	/// <summary>
	/// 写入表格的头。
	/// </summary>
	/// <param name="table">表格节点。</param>
	/// <param name="heading">表格的行。</param>
	protected virtual void WriteTableHead(Table table, TableRow heading)
	{
		isTableHeading = true;
		heading.Accept(this);
		isTableHeading = false;
	}

	/// <summary>
	/// 写入表格的内容。
	/// </summary>
	/// <param name="table">表格节点。</param>
	/// <param name="rows">表格的行。</param>
	protected virtual void WriteTableBody(Table table, IEnumerable<TableRow> rows)
	{
		foreach (TableRow row in rows)
		{
			row.Accept(this);
		}
	}

	/// <summary>
	/// 访问指定的脚注节点。
	/// </summary>
	/// <param name="node">要访问的脚注节点。</param>
	/// <remarks>这里只提前处理内部脚注引用，但不做输出，避免在渲染正常节点时输出脚注。</remarks>
	public override void VisitFootnote(Footnote node)
	{
		footnoteRefWalker ??= new FootnoteRefWalker(this);
		// 需要提前遍历脚注内部的脚注引用，确保脚注的反向引用是正确的。
		node.Accept(footnoteRefWalker);
	}

	/// <summary>
	/// 访问指定的脚注引用节点。
	/// </summary>
	/// <param name="node">要访问的脚注引用节点。</param>
	/// <remarks>请勿重写此方法，请改为重写 <see cref="VisitFootnoteRef(FootnoteRef, FootnoteBackref)"/> 方法。</remarks>
	public override void VisitFootnoteRef(FootnoteRef node)
	{
		VisitFootnoteRef(node, GetBackref(node));
	}

	/// <summary>
	/// 访问指定的脚注引用节点。
	/// </summary>
	/// <param name="node">要访问的脚注引用节点。</param>
	/// <param name="backref">反向引用信息。</param>
	protected virtual void VisitFootnoteRef(FootnoteRef node, FootnoteBackref backref)
	{
	}

	/// <summary>
	/// 访问脚注的反向引用。
	/// </summary>
	/// <param name="node">要访问的脚注反向引用节点。</param>
	protected virtual void VisitFootnoteBackRef(FootnoteBackref node)
	{
	}

	/// <summary>
	/// 提供默认的访问行为。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	public override void DefaultVisit(Node node)
	{
		if (node is INodeContainer<InlineNode> inlineContainer)
		{
			// 特殊处理 FootnoteBackref
			NodeList<InlineNode> children = inlineContainer.Children;
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				var childNode = children[i];
				if (childNode is FootnoteBackref backref)
				{
					VisitFootnoteBackRef(backref);
				}
				else
				{
					childNode.Accept(this);
				}
			}
		}
		else
		{
			base.DefaultVisit(node);
		}
	}

	/// <summary>
	/// 写入脚注段。
	/// </summary>
	/// <param name="doc">文档节点。</param>
	/// <param name="footnotes">要写入的脚注列表。</param>
	protected virtual void WriteFootnotes(Document doc, List<Footnote> footnotes)
	{
		int count = footnotes.Count;
		for (int i = 0; i < count; i++)
		{
			Footnote footnote = footnotes[i];
			FootnoteInfo? info = footnoteInfo.GetValueOrDefault(footnote);
			// 只输出被使用的脚注。
			if (info != null)
			{
				WriteFootnote(footnote, info);
			}
		}
	}

	/// <summary>
	/// 写入脚注节点。
	/// </summary>
	/// <param name="node">脚注节点。</param>
	/// <param name="info">脚注的信息。</param>
	protected virtual void WriteFootnote(Footnote node, FootnoteInfo info)
	{
		// 添加临时的反向引用节点。
		bool isTempParagraph = false;
		if (node.LastChild is not Paragraph paragraph)
		{
			// 最后不是段落时，需要补充一个临时段落。
			isTempParagraph = true;
			paragraph = new Paragraph();
			node.Children.Add(paragraph);
		}
		int idx = paragraph.Children.Count;
		paragraph.Children.AddRange(info.Backrefs);
		DefaultVisit(node);
		// 移除临时的反向引用节点。
		if (isTempParagraph)
		{
			node.Children.RemoveAt(node.Children.Count - 1);
		}
		else
		{
			paragraph.Children.RemoveRange(idx, paragraph.Children.Count - idx);
		}
	}

	/// <summary>
	/// 返回指定节点的 Alt 文本。
	/// </summary>
	/// <param name="node">要检查的节点。</param>
	/// <returns>节点的 Alt 文本。</returns>
	protected string GetAltText(Node node)
	{
		altTextRenderer ??= new AltTextRenderer();
		altTextRenderer.Clear();
		node.Accept(altTextRenderer);
		return altTextRenderer.ToString();
	}

	/// <summary>
	/// 返回指定脚注引用的反向引用。
	/// </summary>
	/// <param name="footnoteRef">要检查的脚注引用。</param>
	/// <returns>脚注引用的反向引用。</returns>
	internal FootnoteBackref GetBackref(FootnoteRef footnoteRef)
	{
		if (footnoteBackrefs.TryGetValue(footnoteRef, out FootnoteBackref? backref))
		{
			return backref;
		}
		Footnote footnote = footnoteRef.Footnote;
		if (!footnoteInfo.TryGetValue(footnote, out FootnoteInfo? info))
		{
			footnotes.Add(footnote);
			GenerateFootnoteId(footnote, out string id, out string refId);
			info = new FootnoteInfo(footnotes.Count.ToString(), id, refId);
			footnoteInfo[footnote] = info;
		}
		backref = new(info, GetUniqueIdentifier(info.RefId));
		info.Backrefs.Add(backref);
		footnoteBackrefs[footnoteRef] = backref;
		return backref;
	}

	/// <summary>
	/// 生成指定脚注的标识符。
	/// </summary>
	/// <param name="footnote">要检查的脚注。</param>
	/// <param name="id">脚注的标识符。</param>
	/// <param name="refId">脚注引用的标识符。</param>
	protected virtual void GenerateFootnoteId(Footnote footnote, out string id, out string refId)
	{
		string label = footnote.Label;
		id = $"fn-{label}";
		refId = $"fnref-{label}";
	}

	/// <summary>
	/// 生成唯一的标识符。
	/// </summary>
	/// <param name="id">基础标识符。</param>
	/// <returns>唯一标识符。</returns>
	protected virtual string GetUniqueIdentifier(string id)
	{
		return uniqueIdentifier.Unique(id);
	}
}
