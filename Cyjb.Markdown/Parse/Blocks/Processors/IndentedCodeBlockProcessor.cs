using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 缩进代码块的解析器。
/// </summary>
internal class IndentedCodeBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 尝试开始新的缩进代码块处理器。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>新的块处理器数组，若未能成功解析，则返回空数组。</returns>
	public static IEnumerable<BlockProcessor> TryStart(LineInfo line)
	{
		// 缩进代码块不会中断段落。
		if (line.IsCodeIndent && !line.IsBlank &&
			line.ActivatedProcessor.Kind != MarkdownKind.Paragraph)
		{
			// 代码块的起始位置包含缩进位置。
			int start = line.Start;
			// 跳过空白部分。
			line.SkipIndent(LineInfo.CodeIndent);
			yield return new IndentedCodeBlockProcessor(start, line.End);
		}
	}

	/// <summary>
	/// 代码行。
	/// </summary>
	private readonly List<MappedText> lines = new();
	/// <summary>
	/// 代码块的起始位置。
	/// </summary>
	private readonly int start;
	/// <summary>
	/// 代码块内容的结束位置。
	/// </summary>
	private int end;

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
	public override BlockContinue TryContinue(LineInfo line)
	{
		if (line.IsCodeIndent)
		{
			// 跳过空白部分。
			line.SkipIndent(LineInfo.CodeIndent);
			end = line.End;
			return BlockContinue.Continue;
		}
		else if (line.IsBlank)
		{
			// 跳过空白部分，但暂时不计入结尾。
			line.SkipIndent(LineInfo.CodeIndent);
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
	/// <param name="text">行的文本。</param>
	public override void AddLine(MappedText text)
	{
		lines.Add(text);
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		// 忽略末尾的空行。
		while (lines.Count > 0 && lines[^1].IsBlank)
		{
			lines.RemoveAt(lines.Count - 1);
		}
		return new CodeBlock(string.Concat(lines), new TextSpan(start, this.end));
	}
}
