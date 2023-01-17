using Cyjb.Markdown.Parse.Inlines;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 块节点的处理器。
/// </summary>
internal abstract class BlockProcessor
{
	/// <summary>
	/// 节点的类型。
	/// </summary>
	private readonly MarkdownKind kind;

	/// <summary>
	/// 使用指定的节点类型初始化 <see cref="BlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="kind">节点的类型。</param>
	protected BlockProcessor(MarkdownKind kind)
	{
		this.kind = kind;
	}

	/// <summary>
	/// 获取节点的类型。
	/// </summary>
	public MarkdownKind Kind => kind;

	/// <summary>
	/// 获取是否是容器节点。
	/// </summary>
	public virtual bool IsContainer => false;

	/// <summary>
	/// 获取是否可以延迟延伸。
	/// </summary>
	public virtual bool CanLazyContinuation => false;

	/// <summary>
	/// 获取当前块所有可能的起始字符。
	/// </summary>
	public virtual string StartChars => string.Empty;

	/// <summary>
	/// 获取当前块是否需要解析行内节点。
	/// </summary>
	public virtual bool NeedParseInlines => false;

	/// <summary>
	/// 获取或设置当前处理器是否需要被替换。
	/// </summary>
	internal bool IsNeedReplaced { get; set; }

	/// <summary>
	/// 将当前处理器标记为需要被替换。
	/// </summary>
	/// <remarks>一般用于使用新的块节点替换之前旧的匹配。</remarks>
	public void NeedReplace()
	{
		IsNeedReplaced = true;
	}

	/// <summary>
	/// 获取当前激活的段落的行。
	/// </summary>
	/// <value>当前激活的段落的行，如果激活的节点不是段落，则返回 <c>null</c>。</value>
	public virtual IList<MappedText>? ParagraphLines => null;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public abstract BlockContinue TryContinue(LineInfo line);

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="text">行的文本。</param>
	public virtual void AddLine(MappedText text) { }

	/// <summary>
	/// 返回当前节点是否可以包含指定类型的子节点。
	/// </summary>
	/// <param name="kind">要检查的节点类型。</param>
	/// <returns>如果当前节点可以包含指定类型的子节点，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public virtual bool CanContains(MarkdownKind kind)
	{
		return false;
	}

	/// <summary>
	/// 添加一个新节点。
	/// </summary>
	/// <param name="node">要添加的节点。</param>
	public virtual void AddNode(BlockNode node) { }

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public abstract BlockNode? CloseNode(int end);

	/// <summary>
	/// 解析行内节点。
	/// </summary>
	/// <param name="parser">行内节点的解析器。</param>
	public virtual void ParseInline(InlineParser parser) { }
}
