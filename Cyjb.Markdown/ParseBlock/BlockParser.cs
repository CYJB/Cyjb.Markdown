using Cyjb.Collections;
using Cyjb.Markdown.ParseInline;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 Markdown 的块级语法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/"/>
internal sealed class BlockParser
{
	/// <summary>
	/// 快速解析的工厂。
	/// </summary>
	private static readonly Dictionary<BlockKind, IBlockFactory[]> FastFactories = new()
	{
		{ BlockKind.ThematicBreak, new IBlockFactory[] { ThematicBreakProcessor.Factory } },
		{ BlockKind.ATXHeading, new IBlockFactory[] { ATXHeadingProcessor.Factory } },
		{ BlockKind.QuoteStart, new IBlockFactory[] { BlockquoteProcessor.Factory } },
		{ BlockKind.UnorderedListMarker, new IBlockFactory[] {
			// 如果即可能是列表项有可能是分割线，优先选择分割线。
			ThematicBreakProcessor.Factory, ListProcessor.Factory } },
		{ BlockKind.OrderedListMarker, new IBlockFactory[] { ListProcessor.Factory } },
		{ BlockKind.CodeFence, new IBlockFactory[] { FencedCodeBlockProcessor.Factory } },
		{ BlockKind.MathFence, new IBlockFactory[] { MathBlockProcessor.Factory } },
		{ BlockKind.MathFenceStart, new IBlockFactory[] { MathBlockProcessor.Factory } },
		{ BlockKind.HtmlStart, new IBlockFactory[] { HtmlBlockProcessor.Factory } },
		{ BlockKind.CodeFenceStart, new IBlockFactory[] { FencedCodeBlockProcessor.Factory } },
		{ BlockKind.SetextUnderline, new IBlockFactory[] { SetextHeadingProcessor.Factory } },
		{ BlockKind.DashLine, new IBlockFactory[] {
			// 中划线行可能存在歧义，需要都做检测。
			SetextHeadingProcessor.Factory, ThematicBreakProcessor.Factory } },
		{ BlockKind.TableDelimiterRow, new IBlockFactory[] { TableProcessor.Factory } },
	};

	/// <summary>
	/// 块级词法分析器。
	/// </summary>
	private readonly ITokenizer<BlockKind> tokenizer;
	/// <summary>
	/// 行列定位器。
	/// </summary>
	private readonly LineLocator locator;
	/// <summary>
	/// 源文件读取器。
	/// </summary>
	private readonly SourceReader reader;
	/// <summary>
	/// 解析的选项。
	/// </summary>
	private readonly ParseOptions options;

	/// <summary>
	/// 文档处理器。
	/// </summary>
	private readonly DocumentProcessor document = new();
	/// <summary>
	/// 链接定义。
	/// </summary>
	private readonly Dictionary<string, LinkDefinition> linkDefines = new();
	/// <summary>
	/// 开启状态的处理器。
	/// </summary>
	private readonly ListStack<BlockProcessor> openedProcessors = new();
	/// <summary>
	/// 待解析行内节点的处理器。
	/// </summary>
	private readonly List<BlockProcessor> pendingInlineProcessors = new();

	/// <summary>
	/// 使用要解析的文本初始化 <see cref="BlockParser"/> 类的新实例。
	/// </summary>
	/// <param name="text">要解析的文本读取器。</param>
	/// <param name="options">解析的选项。</param>
	public BlockParser(TextReader text, ParseOptions? options)
	{
		// 为了正确处理 Tab 的位置，解析块时需要对列定位。
		reader = new SourceReader(text);
		reader.UseLineLocator();
		locator = reader.Locator!;
		tokenizer = BlockLexer.Factory.CreateTokenizer(reader);
		this.options = options ?? ParseOptions.Default;
		tokenizer.SharedContext = this.options;
		openedProcessors.Push(document);
	}

	/// <summary>
	/// 获取当前激活的节点处理器。
	/// </summary>
	public BlockProcessor ActivatedProcessor => openedProcessors.Peek();

	/// <summary>
	/// 获取解析的选项。
	/// </summary>
	public ParseOptions Options => options;

	/// <summary>
	/// 解析当前文档。
	/// </summary>
	public Document Parse()
	{
		Token<BlockKind> token;
		while (true)
		{
			LineInfo line = new(this, locator);
			// 添加所有换行符前的 Token。
			while (!(token = tokenizer.Read()).IsEndOfFile)
			{
				line.AddToken(token);
				if (token.Kind == BlockKind.NewLine)
				{
					ParseLine(line);
					break;
				}
			}
			if (token.IsEndOfFile)
			{
				if (!line.IsEmpty)
				{
					ParseLine(line);
				}
				break;
			}
		}
		// 栈底总是文档，总是不需要 Close。
		int end = token.Span.Start;
		CloseProcessor(document, end);
		// 解析行内节点。
		if (pendingInlineProcessors.Count > 0)
		{
			InlineParser parser = new(linkDefines, options);
			foreach (BlockProcessor processor in pendingInlineProcessors)
			{
				processor.ParseInline(parser);
			}
		}
		Document doc = document.GetDocument(end);
		// 填充行定位器。
		if (options.UseLineLocator)
		{
			LineLocatorWalker walker = new(locator);
			doc.Accept(walker);
		}
		return doc;
	}

	/// <summary>
	/// 解析指定行。
	/// </summary>
	/// <param name="line">要解析的行。</param>
	private void ParseLine(LineInfo line)
	{
		int lineStart = line.Start;
		// 栈底总是 document，总是可以接受任何行，因此总是跳过。
		int matches = 1;
		for (int i = openedProcessors.Count - 2; i >= 0; i--, matches++)
		{
			// 确保缩进被解析了。
			_ = line.Indent;
			BlockContinue result = openedProcessors[i].TryContinue(line);
			if (result == BlockContinue.None)
			{
				// 无法延伸，停止处理。
				break;
			}
			else if (result == BlockContinue.Closed)
			{
				// 关闭当前节点。
				CloseProcessor(openedProcessors[i + 1], line.End);
				return;
			}
		}

		BlockProcessor processor = openedProcessors[^matches];
		bool startedNewBlock = false;
		bool tryBlockStarts = processor.TryBlockStarts;
		Token<BlockKind> token;
		while (tryBlockStarts)
		{
			// 确保缩进被解析了。
			_ = line.Indent;
			if (line.IsEmpty)
			{
				// 没有能够解析的文本，忽略。
				break;
			}
			processor.IsNeedReplaced = false;
			token = line.Peek();
			List<BlockProcessor> processors = new();
			if (token.Kind == BlockKind.NewLine)
			{
				// 是空行，不需要继续解析块起始。
				break;
			}
			if (FastFactories.TryGetValue(token.Kind, out IBlockFactory[]? factories))
			{
				foreach (IBlockFactory factory in factories)
				{
					processors.AddRange(factory.TryStart(line, processor));
					if (processors.Count > 0)
					{
						break;
					}
				};
			}
			// 尝试检查缩进代码块
			if (processors.Count == 0)
			{
				processors.AddRange(IndentedCodeBlockProcessor.TryStart(line));
			}
			if (processors.Count == 0)
			{
				// 未找到合适的处理器，返回。
				break;
			}
			startedNewBlock = true;
			// 开始新块前，先闭合之前未延伸的块。
			CloseProcessor(processor, lineStart);
			// 替换之前的处理器。
			if (processor.IsNeedReplaced)
			{
				AddLinkDefinition(openedProcessors.Pop(), openedProcessors.Peek());
			}
			foreach (BlockProcessor newProcessor in processors)
			{
				AddProcessor(newProcessor, lineStart);
			}
			tryBlockStarts = openedProcessors.Peek().TryBlockStarts;
			processor = openedProcessors.Peek();
		}
		// 检查延迟延伸。
		if (!startedNewBlock && !line.IsBlank && openedProcessors.Peek().CanLazyContinuation)
		{
			ActivatedProcessor.AddLine(line.Text);
			return;
		}
		// 闭合之前未延伸的块。
		CloseProcessor(processor, lineStart);
		if (!processor.IsContainer)
		{
			// 只有行非空或者并不是由于开始新块而清空的，才添加到处理器。
			if (!line.IsEmpty || !startedNewBlock)
			{
				ActivatedProcessor.AddLine(line.Text);
			}
			return;
		}
		if (!line.IsBlank)
		{
			// 为行添加一个新的段落。
			processor = new ParagraphProcessor(options);
			AddProcessor(processor, lineStart);
			// 需要跳过段落的起始缩进。
			if (line.ParagraphSkippable)
			{
				line.SkipIndent();
			}
			ActivatedProcessor.AddLine(line.Text);
		}
	}

	/// <summary>
	/// 关闭指定处理器后的其它块处理器。
	/// </summary>
	/// <param name="target">要检查的处理器。</param>
	/// <param name="end">结束位置。</param>
	private void CloseProcessor(BlockProcessor target, int end)
	{
		while (openedProcessors.Peek() != target)
		{
			CloseLastProcessor(end);
		}
	}

	/// <summary>
	/// 关闭最后一个处理器。
	/// </summary>
	/// <param name="end">结束位置。</param>
	private void CloseLastProcessor(int end)
	{
		BlockProcessor processor = openedProcessors.Pop();
		BlockProcessor parent = openedProcessors.Peek();
		AddLinkDefinition(processor, parent);
		BlockNode? node = processor.CloseNode(end);
		if (node != null)
		{
			parent.AddNode(node);
			if (processor.NeedParseInlines)
			{
				pendingInlineProcessors.Add(processor);
			}
		}
	}

	/// <summary>
	/// 添加段落的链接定义。
	/// </summary>
	public void AddLinkDefinition(BlockProcessor processor, BlockProcessor parent)
	{
		if (processor is ParagraphProcessor paragraph)
		{
			foreach (LinkDefinition definition in paragraph.GetDefinitions())
			{
				parent.AddNode(definition);
				linkDefines.TryAdd(definition.Identifier, definition);
			}
		}
	}

	/// <summary>
	/// 添加指定的处理器作为子节点。
	/// </summary>
	/// <param name="processor">要添加的子节点处理器。</param>
	/// <param name="lastEnd">之前处理器的结束位置。</param>
	private void AddProcessor(BlockProcessor processor, int lastEnd)
	{
		// 确保父节点可以包含当前子节点处理器。
		while (!openedProcessors.Peek().CanContains(processor.Kind))
		{
			CloseLastProcessor(lastEnd);
		}
		openedProcessors.Push(processor);
	}
}