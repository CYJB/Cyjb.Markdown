using Cyjb.Markdown.Parse.Inlines;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 段落块的解析器。
/// </summary>
internal sealed class ParagraphProcessor : BlockProcessor
{
	/// <summary>
	/// 段落节点。
	/// </summary>
	private readonly Paragraph paragraph = new();
	/// <summary>
	/// 段落包含的行。
	/// </summary>
	private readonly List<MappedText> lines = new();
	/// <summary>
	/// 代码块的起始位置。
	/// </summary>
	private int start = -1;
	/// <summary>
	/// 链接定义的解析器。
	/// </summary>
	private readonly LinkDefinitionParser linkDefinitionParser = new();

	/// <summary>
	/// 初始化 <see cref="ParagraphProcessor"/> 类的新实例。
	/// </summary>
	public ParagraphProcessor() : base(MarkdownKind.Paragraph)
	{
		linkDefinitionParser.ClearLines += () =>
		{
			lines.Clear();
			start = -1;
		};
	}

	/// <summary>
	/// 获取是否可以延迟延伸。
	/// </summary>
	public override bool CanLazyContinuation => true;

	/// <summary>
	/// 获取当前块是否需要解析行内节点。
	/// </summary>
	public override bool NeedParseInlines => true;

	/// <summary>
	/// 获取当前激活的段落的行。
	/// </summary>
	/// <value>当前激活的段落的行，如果激活的节点不是段落，则返回 <c>null</c>。</value>
	public override IList<MappedText>? ParagraphLines => lines;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		return line.IsBlank ? BlockContinue.None : BlockContinue.Continue;
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="text">行的文本。</param>
	public override void AddLine(MappedText text)
	{
		if (start < 0)
		{
			start = text.Span.Start;
		}
		lines.Add(text);
		if (linkDefinitionParser.CanContinue)
		{
			linkDefinitionParser.Parse(text);
		}
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		if (lines.Count == 0)
		{
			// 没有有效的行。
			return null;
		}
		// 移除尾行后的空白。
		lines[^1].TrimEnd();
		paragraph.Span = new TextSpan(start, end);
		return paragraph;
	}

	/// <summary>
	/// 返回所有解析的链接定义。
	/// </summary>
	/// <returns>链接定义列表。</returns>
	public List<LinkDefinition> GetDefinitions()
	{
		return linkDefinitionParser.GetDefinitions();
	}

	/// <summary>
	/// 解析行内节点。
	/// </summary>
	/// <param name="parser">行内节点的解析器。</param>
	public override void ParseInline(InlineParser parser)
	{
		parser.Parse(lines, paragraph.Children);
	}
}
