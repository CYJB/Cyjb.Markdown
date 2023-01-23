using System.Text;
using Cyjb.Collections;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 行的信息。
/// </summary>
internal sealed class LineInfo
{
	/// <summary>
	/// 代码缩进长度。
	/// </summary>
	public const int CodeIndent = 4;

	/// <summary>
	/// 返回指定词法单元是否包含内容。
	/// </summary>
	/// <param name="token">要检查的词法单元。</param>
	/// <returns>如果指定词法单元包含内容，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	internal static bool HasContent(Token<BlockKind> token)
	{
		return token.Kind != BlockKind.Indent && token.Kind != BlockKind.NewLine;
	}

	/// <summary>
	/// 词法单元队列。
	/// </summary>
	private readonly ListQueue<Token<BlockKind>> tokens = new();
	/// <summary>
	/// 行定位器。
	/// </summary>
	private readonly LineLocator locator;
	/// <summary>
	/// 块解析器。
	/// </summary>
	private readonly BlockParser parser;
	/// <summary>
	/// 行的起始位置。
	/// </summary>
	private int start = -1;
	/// <summary>
	/// 行的结束位置。
	/// </summary>
	private int end;
	/// <summary>
	/// 缩进信息。
	/// </summary>
	private IndentInfo? indent;
	/// <summary>
	/// 映射的文本。
	/// </summary>
	private MappedText? text;

	/// <summary>
	/// 使用指定的行定位器初始化 <see cref="LineInfo"/> 类的新实例。
	/// </summary>
	/// <param name="parser">块解析器。</param>
	/// <param name="locator">行定位器。</param>
	internal LineInfo(BlockParser parser, LineLocator locator)
	{
		this.parser = parser;
		this.locator = locator;
	}

	/// <summary>
	/// 获取当前缩进宽度。
	/// </summary>
	public int Indent => GetIndent().Width;
	/// <summary>
	/// 是否是段落可以跳过的缩进。
	/// </summary>
	public bool ParagraphSkippable
	{
		get => GetIndent().ParagraphSkippable;
		set => GetIndent().ParagraphSkippable = value;
	}
	/// <summary>
	/// 获取是否是代码缩进。
	/// </summary>
	public bool IsCodeIndent => GetIndent().Width >= CodeIndent;
	/// <summary>
	/// 获取是否是空行。
	/// </summary>
	public bool IsBlank => !tokens.Any(HasContent);
	/// <summary>
	/// 获取行的起始位置。
	/// </summary>
	public int Start => indent == null ? start : indent.Start;
	/// <summary>
	/// 获取行的结束位置。
	/// </summary>
	public int End => end;
	/// <summary>
	/// 获取解析的选项。
	/// </summary>
	public ParseOptions Options => parser.Options;

	/// <summary>
	/// 获取当前行的文本。
	/// </summary>
	public MappedText Text
	{
		get
		{
			if (text == null)
			{
				StringBuilder builder = new();
				SortedTextSpanBuilder spanBuilder = new();
				List<Tuple<int, int>> map = new();
				// 添加缩进。
				if (indent != null && indent.Width > 0)
				{
					map.Add(new Tuple<int, int>(0, indent.Start));
					spanBuilder.Add(indent.Start);
					builder.Append(indent.GetText());
				}
				// 添加剩余词法单元。
				bool isFirst = true;
				foreach (Token<BlockKind> token in tokens)
				{
					// 首个缩进可能会存在 Tab 部分替换为空格的情况，因此之后的词法单元也需要添加索引。
					if (isFirst)
					{
						map.Add(new Tuple<int, int>(builder.Length, token.Span.Start));
						isFirst = false;
					}
					builder.Append(token.Text);
					spanBuilder.Add(token.Span);
				}
				text = new MappedText(builder.ToString(), spanBuilder.GetSpan(), map.ToArray());
			}
			return text;
		}
	}

	/// <summary>
	/// 获取当前激活的节点处理器。
	/// </summary>
	public BlockProcessor ActivatedProcessor => parser.ActivatedProcessor;

	/// <summary>
	/// 获取行是否是空的。
	/// </summary>
	internal bool IsEmpty => tokens.Count == 0;

	/// <summary>
	/// 获取词法单元列表。
	/// </summary>
	internal IReadOnlyList<Token<BlockKind>> Tokens => tokens;

	/// <summary>
	/// 跳过指定个数的空白。
	/// </summary>
	/// <param name="count">要跳过的空白个数。</param>
	public void SkipIndent(int count)
	{
		GetIndent().Skip(count);
		text = null;
	}

	/// <summary>
	/// 跳过全部空白。
	/// </summary>
	public void SkipIndent()
	{
		GetIndent().Skip();
		text = null;
	}

	/// <summary>
	/// 跳过当前行。
	/// </summary>
	public void Skip()
	{
		tokens.Clear();
		indent = new IndentInfo(end, locator);
		text = null;
	}

	/// <summary>
	/// 添加新的词法单元。
	/// </summary>
	/// <param name="token">要添加的词法单元。</param>
	internal void AddToken(Token<BlockKind> token)
	{
		if (start < 0)
		{
			start = token.Span.Start;
		}
		end = token.Span.End;
		tokens.Enqueue(token);
	}

	/// <summary>
	/// 获取缩进信息。
	/// </summary>
	private IndentInfo GetIndent()
	{
		if (indent == null)
		{
			if (tokens.Count == 0)
			{
				indent = new IndentInfo(end, locator);
				return indent;
			}
			Token<BlockKind> token = tokens.Peek();
			if (token.Kind == BlockKind.Indent)
			{
				indent = (IndentInfo)tokens.Dequeue().Value!;
			}
			else
			{
				indent = new IndentInfo(token.Span.Start, locator);
			}
		}
		return indent;
	}

	/// <summary>
	/// 返回下一词法单元，但不将其消费。
	/// </summary>
	/// <returns>下一词法单元。</returns>
	internal Token<BlockKind> Peek()
	{
		return tokens.Peek();
	}

	/// <summary>
	/// 读取并返回下一词法单元。
	/// </summary>
	/// <returns>下一词法单元。</returns>
	internal Token<BlockKind> Read()
	{
		Token<BlockKind> token = tokens.Dequeue();
		// 读取词法单元后，需要重新检查缩进、文本和位置信息。
		indent = null;
		text = null;
		start = token.Span.End;
		return token;
	}
}
