using System.Text;
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
	private readonly StringBuilder builder = new();
	/// <summary>
	/// 代码块的起始位置。
	/// </summary>
	private readonly int start;
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
	/// 代码块的信息。
	/// </summary>
	private readonly string? info;
	/// <summary>
	/// 代码块的属性。
	/// </summary>
	private readonly HtmlAttributeList? attrs;

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
		this.start = start;
		this.fence = fence;
		this.fenceLength = fenceLength;
		this.indent = indent;
		this.info = info;
		this.attrs = attrs;
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
				GetFenceLength(token.Text) >= fenceLength)
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
		builder.Append(text.ToString());
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		CodeBlock node = new(builder.ToString(), new TextSpan(start, end))
		{
			Info = info,
		};
		if (attrs != null)
		{
			node.Attributes.AddRange(attrs);
		}
		return node;
	}

	/// <summary>
	/// 返回分隔符的长度。
	/// </summary>
	/// <param name="text">要检查的字符串。</param>
	/// <returns>分隔符的长度。</returns>
	private static int GetFenceLength(string text)
	{
		char fence = text[0];
		// 在词法分析中已确保分隔符长度至少为 3。
		int i = 3;
		for (; i < text.Length; i++)
		{
			if (text[i] != fence)
			{
				return i;
			}
		}
		return i;
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
			// 代码块的起始位置包含分隔符位置。
			int start = line.Start;
			// 跳过空白部分。
			int indent = line.Indent;
			line.SkipIndent();
			// 解析代码块的信息。
			Token<BlockKind> token = line.Peek();
			int fenceLength = GetFenceLength(token.Text);
			string? info = null;
			HtmlAttributeList? attrs = null;
			if (token.Kind == BlockKind.CodeFenceStart)
			{
				ReadOnlySpan<char> text = token.Text.AsSpan()[fenceLength..];
				MarkdownUtil.Trim(ref text);
				info = text.Unescape();
				attrs = token.Value as HtmlAttributeList;
			}
			// 标记当前行已处理完毕。
			line.Skip();
			yield return new FencedCodeBlockProcessor(start,
				token.Text[0], fenceLength, indent, info, attrs);
		}
	}
}
