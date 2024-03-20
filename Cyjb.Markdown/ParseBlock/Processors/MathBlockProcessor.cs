using System.Text;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 数学公式块的解析器。
/// </summary>
internal class MathBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 数学公式的文本。
	/// </summary>
	private readonly StringBuilder builder = new();
	/// <summary>
	/// 数学公式节点。
	/// </summary>
	private readonly MathBlock math;
	/// <summary>
	/// 数学公式块的分割符长度。
	/// </summary>
	private readonly int fenceLength;
	/// <summary>
	/// 数学公式块的缩进。
	/// </summary>
	private readonly int indent;

	/// <summary>
	/// 初始化 <see cref="MathBlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">数学公式块的起始索引。</param>
	/// <param name="fenceLength">数学公式块的分隔符长度。</param>
	/// <param name="indent">数学公式块的缩进。</param>
	/// <param name="info">数学公式块的信息。</param>
	/// <param name="attrs">数学公式块的属性。</param>
	private MathBlockProcessor(int start, int fenceLength, int indent, string? info, HtmlAttributeList? attrs)
		: base(MarkdownKind.MathBlock)
	{
		this.fenceLength = fenceLength;
		this.indent = indent;
		math = new MathBlock(string.Empty, new TextSpan(start, start))
		{
			Info = info
		};
		if (attrs != null)
		{
			math.Attributes.AddRange(attrs);
		}
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		if (!line.IsCodeIndent)
		{
			Token<BlockKind> token = line.PeekFront();
			if (token.Kind == BlockKind.MathFence &&
				MarkdownUtil.GetFenceLength(token.Text) >= fenceLength)
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
	/// <param name="line">新添加的行。</param>
	public override void AddLine(BlockLine line)
	{
		line.AppendTo(builder);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		math.Span = math.Span with { End = end };
		math.Content = builder.ToString();
		StringBuilderPool.Return(builder);
		return math;
	}

	/// <summary>
	/// 处理器工厂。
	/// </summary>
	private sealed class BlockFactory : IBlockFactory
	{
		/// <summary>
		/// 尝试开始当前块的解析。
		/// </summary>
		/// <param name="parser">块级语法分析器。</param>
		/// <param name="line">要检查的行。</param>
		/// <param name="matchedProcessor">当前匹配到的块处理器。</param>
		/// <returns>如果能够开始当前块的解析，则返回解析器序列。否则返回空序列。</returns>
		public IEnumerable<BlockProcessor> TryStart(BlockParser parser, BlockLine line, BlockProcessor matchedProcessor)
		{
			if (line.IsCodeIndent)
			{
				yield break;
			}
			MarkdownUtil.ParseFenceStart(parser, line, out int start, out int indent,
				out char _, out int fenceLength, out string? info, out HtmlAttributeList? attrs);
			yield return new MathBlockProcessor(start, fenceLength, indent, info, attrs);
		}
	}
}
