using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 列表项的处理器。
/// </summary>
internal sealed class ListItemProcessor : BlockProcessor
{
	/// <summary>
	/// 父列表处理器。
	/// </summary>
	private readonly ListProcessor parent;
	/// <summary>
	/// 列表项内容的缩进。
	/// </summary>
	private readonly int contentIndent;
	/// <summary>
	/// 列表项节点。
	/// </summary>
	private readonly ListItem item;
	/// <summary>
	/// 列表项的结束位置。
	/// </summary>
	private int end = -1;
	/// <summary>
	/// 列表项是否包含空行。
	/// </summary>
	private bool hadBlankLine = false;

	/// <summary>
	/// 使用列表项的起始位置和内容的缩进初始化 <see cref="ListItemProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="parent">父列表处理器。</param>
	/// <param name="start">列表项的起始位置。</param>
	/// <param name="contentIndent">列表项内容的缩进。</param>
	public ListItemProcessor(ListProcessor parent, int start, int contentIndent)
		: base(MarkdownKind.ListItem)
	{
		this.parent = parent;
		item = new ListItem(new TextSpan(start, start));
		this.contentIndent = contentIndent;
	}

	/// <summary>
	/// 获取是否是容器节点。
	/// </summary>
	public override bool IsContainer => true;

	/// <summary>
	/// 获取是否允许尝试开始新的块。
	/// </summary>
	/// <remarks>如果是空的任务列表项，必须后跟段落，不能尝试开始新的块。
	/// 其它情况都可以尝试开始新的块。</remarks>
	public override bool TryBlockStarts => !item.Checked.HasValue || item.Children.Count > 0;

	/// <summary>
	/// 获取或设置列表项的选中状态。
	/// </summary>
	/// <value>使用 <c>null</c> 表示不能选中（普通列表项）；<c>true</c> 表示已选中，
	/// <c>false</c> 表示未选中。</value>
	public bool? Checked
	{
		get => item.Checked;
		set => item.Checked = value;
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		if (line.IsBlank)
		{
			if (item.Children.Count == 0 && line.ActivatedProcessor.Kind == MarkdownKind.ListItem)
			{
				// 列表项最多可以包含一个起始空行，因此空列表项后的空行会闭合当前项。
				return BlockContinue.None;
			}
			else
			{
				// 记录已包含空行。
				MarkdownKind kind = line.ActivatedProcessor.Kind;
				hadBlankLine = kind == MarkdownKind.Paragraph || kind == MarkdownKind.ListItem;
				line.SkipIndent();
				return BlockContinue.Continue;
			}
		}
		if (line.Indent >= contentIndent)
		{
			line.SkipIndent(contentIndent);
			return BlockContinue.Continue;
		}
		else
		{
			// 对于延迟延伸的行，会之后被直接添加到内部的段落中。
			return BlockContinue.None;
		}
	}

	/// <summary>
	/// 返回当前节点是否可以包含指定类型的子节点。
	/// </summary>
	/// <param name="kind">要检查的节点类型。</param>
	/// <returns>如果当前节点可以包含指定类型的子节点，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool CanContains(MarkdownKind kind)
	{
		if (hadBlankLine)
		{
			// 列表项内部包含空行，将列表标记为松散的。
			parent.MarkLoose();
		}
		return true;
	}

	/// <summary>
	/// 添加一个新节点。
	/// </summary>
	/// <param name="node">要添加的节点。</param>
	public override void AddNode(BlockNode node)
	{
		item.Children.Add(node);
		end = node.Span.End;
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		// 优先使用列表项内容的结束位置。
		// 这样能够忽略未被添加到列表项内的空行位置。
		item.Span = item.Span with
		{
			End = this.end < 0 ? end : this.end,
		};
		return item;
	}
}
