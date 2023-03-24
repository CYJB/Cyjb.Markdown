using System.Text;
using System.Text.RegularExpressions;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// HTML 块的解析器。
/// </summary>
internal class HtmlBlockProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// HTML 文本。
	/// </summary>
	private readonly StringBuilder builder = new();
	/// <summary>
	/// 块的起始位置。
	/// </summary>
	private readonly int start;
	/// <summary>
	/// 结束的正则表达式。
	/// </summary>
	private readonly Regex? closeRegex;
	/// <summary>
	/// 是否已完成 HTML 块的识别。
	/// </summary>
	private bool finished = false;

	/// <summary>
	/// 初始化 <see cref="HtmlBlockProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">块的起始索引。</param>
	/// <param name="closeRegex">结束的正则表达式。</param>
	private HtmlBlockProcessor(int start, Regex? closeRegex)
		: base(MarkdownKind.CodeBlock)
	{
		this.start = start;
		this.closeRegex = closeRegex;
	}

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		if (finished)
		{
			return BlockContinue.None;
		}
		if (line.IsBlank && closeRegex == null)
		{
			// 遇到空行结束。
			return BlockContinue.None;
		}
		else
		{
			return BlockContinue.Continue;
		}
	}

	/// <summary>
	/// 添加一个新行。
	/// </summary>
	/// <param name="text">行的文本。</param>
	public override void AddLine(MappedText text)
	{
		string lineText = text.ToString();
		builder.Append(lineText);
		if (closeRegex != null && closeRegex.IsMatch(lineText))
		{
			finished = true;
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
		return new HtmlBlock(builder.ToString(), new TextSpan(start, end));
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
			HtmlInfo info = (HtmlInfo)line.Peek().Value!;
			if (!info.CanInterruptParagraph && (line.ActivatedProcessor.Kind == MarkdownKind.Paragraph ||
				line.ActivatedProcessor.CanLazyContinuation))
			{
				// 不中断段落。
				yield break;
			}
			// HTML 块的起始位置包含前面的空白。
			yield return new HtmlBlockProcessor(line.Start, info.CloseRegex);
		}
	}
}
