using Cyjb.Collections;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 分隔符代码块的解析器。
/// </summary>
internal class FencedCodeBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 代码的文本。
	/// </summary>
	private readonly PooledList<char> builder = new();
	/// <summary>
	/// 代码块。
	/// </summary>
	private readonly CodeBlock code;
	/// <summary>
	/// 代码块的分割字符。
	/// </summary>
	private readonly char fence;
	/// <summary>
	/// 代码块的分割符长度。
	/// </summary>
	private readonly int fenceLength;
	/// <summary>
	/// 代码块的缩进。
	/// </summary>
	private readonly int indent;

	/// <summary>
	/// 初始化 <see cref="FencedCodeBlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">代码块的起始索引。</param>
	/// <param name="fence">代码块的分割字符。</param>
	/// <param name="fenceLength">代码块的分隔符长度。</param>
	/// <param name="indent">代码块的缩进。</param>
	/// <param name="info">代码块的信息。</param>
	/// <param name="attrs">代码块的属性。</param>
	private FencedCodeBlockProcessor(int start,
		char fence, int fenceLength, int indent, string? info, HtmlAttributeList? attrs)
		: base(MarkdownKind.CodeBlock)
	{
		this.fence = fence;
		this.fenceLength = fenceLength;
		this.indent = indent;
		code = new CodeBlock(string.Empty, new TextSpan(start, start))
		{
			Info = info,
		};
		if (attrs != null)
		{
			code.Attributes.AddRange(attrs);
		}
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		if (!line.IsCodeIndent)
		{
			Token<BlockKind> token = line.Peek();
			if (token.Kind == BlockKind.CodeFence && token.Text[0] == fence &&
				MarkdownUtil.GetFenceLength(token.Text.AsSpan()) >= fenceLength)
			{
				return BlockContinue.Closed;
			}
		}
		// 允许跳过部分空白。
		line.SkipIndent(indent);
		return BlockContinue.Continue;
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="text">行的文本。</param>
	public override void AddLine(MappedText text)
	{
		text.AppendTo(builder);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		code.Span = code.Span with { End = end };
		code.Content = builder.ToString();
		builder.Dispose();
		return code;
	}

	/// <summary>
	/// 处理器工厂。
	/// </summary>
	private sealed class BlockFactory : IBlockFactory
	{
		/// <summary>
		/// 尝试开始当前块的解析。
		/// </summary>
		/// <param name="line">要检查的行。</param>
		/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
		/// <returns>如果能够开始当前块的解析，则返回解析器序列。否则返回空序列。</returns>
		public IEnumerable<BlockProcessor> TryStart(LineInfo line, BlockProcessor matchedProcessor)
		{
			if (line.IsCodeIndent)
			{
				yield break;
			}
			MarkdownUtil.ParseFenceStart(line, out int start, out int indent,
				out char fenceChar, out int fenceLength, out string? info, out HtmlAttributeList? attrs);
			yield return new FencedCodeBlockProcessor(start, fenceChar, fenceLength, indent, info, attrs);
		}
	}
}
