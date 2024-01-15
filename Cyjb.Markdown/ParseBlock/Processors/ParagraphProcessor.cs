using System.Text;
using Cyjb.Markdown.ParseInline;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

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
	/// 是否需要移除行首的空白。
	/// </summary>
	private bool trimStart = false;
	/// <summary>
	/// 链接定义的解析器。
	/// </summary>
	private readonly LinkDefinitionParser linkDefinitionParser;

	/// <summary>
	/// 使用指定的解析选项初始化 <see cref="ParagraphProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="options">解析选项。</param>
	public ParagraphProcessor(ParseOptions options) : base(MarkdownKind.Paragraph)
	{
		linkDefinitionParser = new LinkDefinitionParser(options, () =>
		{
			lines.Clear();
			start = -1;
			trimStart = true;
		});
	}

	/// <summary>
	/// 获取是否可以延迟延伸。
	/// </summary>
	public override bool CanLazyContinuation => true;

	/// <summary>
	/// 获取是否允许尝试开始新的块。
	/// </summary>
	/// <remarks>段落允许被其它块节点中断。</remarks>
	public override bool TryBlockStarts => true;

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
		// 在之前的文本被识别为链接声明后，需要移除新的行首空白。
		if (trimStart)
		{
			text.TrimStart();
			trimStart = false;
		}
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
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		AddDefinitions(parser);
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
	/// 添加链接定义。
	/// </summary>
	/// <param name="parser">块解析器。</param>
	public void AddDefinitions(BlockParser parser)
	{
		string? attributesPrefix = parser.Options.AttributesPrefix;
		BlockProcessor parent = parser.ActivatedProcessor;
		foreach (LinkDefinition definition in linkDefinitionParser.GetDefinitions())
		{
			// 处理链接属性
			definition.Attributes.AddPrefix(attributesPrefix);
			parent.AddNode(definition);
			parser.LinkDefines.TryAdd(definition.Identifier, definition);
		}
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
