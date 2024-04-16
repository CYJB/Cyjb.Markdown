using System.Text;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 缩进代码块的解析器。
/// </summary>
internal class IndentedCodeBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 尝试开始新的缩进代码块处理器。
	/// </summary>
	/// <param name="parser">块级语法分析器。</param>
	/// <param name="line">要检查的行。</param>
	/// <returns>新的块处理器，若未能成功解析，则返回 <c>null</c>。</returns>
	public static BlockProcessor? TryStart(BlockParser parser, BlockLine line)
	{
		// 缩进代码块不会中断段落。
		if (line.IsCodeIndent && !line.IsBlank() &&
			parser.ActivatedProcessor.Kind != MarkdownKind.Paragraph)
		{
			// 代码块的起始位置包含缩进位置。
			int start = line.Start;
			// 跳过空白部分。
			line.SkipIndent(BlockLine.CodeIndent);
			return new IndentedCodeBlockProcessor(start, line.End);
		}
		return null;
	}

	/// <summary>
	/// 代码行。
	/// </summary>
	private readonly StringBuilder builder = StringBuilderPool.Rent(16);
	/// <summary>
	/// 代码块的起始位置。
	/// </summary>
	private readonly int start;
	/// <summary>
	/// 代码块内容的结束位置。
	/// </summary>
	private int end;
	/// <summary>
	/// 非空白内容的长度。
	/// </summary>
	private int notBlankLength;

	/// <summary>
	/// 初始化 <see cref="IndentedCodeBlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">代码块的起始索引。</param>
	/// <param name="end">代码块的结束索引。</param>
	private IndentedCodeBlockProcessor(int start, int end) : base(MarkdownKind.CodeBlock)
	{
		this.start = start;
		this.end = end;
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		if (line.IsCodeIndent)
		{
			// 跳过空白部分。
			line.SkipIndent(BlockLine.CodeIndent);
			end = line.End;
			return BlockContinue.Continue;
		}
		else if (line.IsBlank())
		{
			// 跳过空白部分，但暂时不计入结尾。
			line.SkipIndent(BlockLine.CodeIndent);
			return BlockContinue.Continue;
		}
		else
		{
			return BlockContinue.None;
		}
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="line">新添加的行。</param>
	public override void AddLine(BlockLine line)
	{
		line.AppendTo(builder);
		if (!line.IsBlank())
		{
			notBlankLength = builder.Length;
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
		// 忽略末尾的空行。
		CodeBlock node = new(builder.ToString(0, notBlankLength), new TextSpan(start, this.end));
		StringBuilderPool.Return(builder);
		return node;
	}
}
