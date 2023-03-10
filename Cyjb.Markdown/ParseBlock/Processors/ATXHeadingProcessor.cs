using Cyjb.Markdown.ParseInline;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// ATX 标题的解析器。
/// </summary>
internal sealed class ATXHeadingProcessor : BlockProcessor, IHeadingProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// ATX 标题节点。
	/// </summary>
	private readonly Heading heading;

	/// <summary>
	/// ATX 标题的文本。
	/// </summary>
	private readonly MappedText text;

	/// <summary>
	/// 使用ATX 标题的起始位置和文本初始化 <see cref="ATXHeadingProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">ATX 标题的起始位置。</param>
	/// <param name="depth">ATX 标题的深度。</param>
	/// <param name="text">ATX 标题的文本。</param>
	/// <param name="attrs">ATX 标题的属性。</param>
	private ATXHeadingProcessor(int start, int depth, MappedText text, HtmlAttributeList? attrs)
		: base(MarkdownKind.Heading)
	{
		heading = new Heading(depth, new TextSpan(start, start));
		if (attrs != null)
		{
			heading.Attributes.AddRange(attrs);
		}
		this.text = text;
	}

	/// <summary>
	/// 获取当前块是否需要解析行内节点。
	/// </summary>
	public override bool NeedParseInlines => !text.IsEmpty;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(LineInfo line)
	{
		return BlockContinue.None;
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override BlockNode? CloseNode(int end)
	{
		heading.Span = new TextSpan(heading.Span.Start, end);
		return heading;
	}

	/// <summary>
	/// 解析行内节点。
	/// </summary>
	/// <param name="parser">行内节点的解析器。</param>
	public override void ParseInline(InlineParser parser)
	{
		parser.Parse(Enumerable.Repeat(text, 1), heading.Children);
	}

	/// <summary>
	/// 返回标题的链接标签。
	/// </summary>
	/// <returns>当前标题的链接标签。</returns>
	public string GetIdentifier()
	{
		return LinkUtil.NormalizeLabel(text.ToString());
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
			line.SkipIndent();
			HtmlAttributeList? attrs = line.Peek().Value as HtmlAttributeList;
			MappedText text = line.Text;
			// 计算标题的深度。
			int depth = 0;
			for (; depth < text.Length && text[depth] == '#'; depth++) ;
			// 忽略内容前后的空白
			text = text[depth..];
			text.TrimStart();
			text.TrimEnd();
			// 检查闭合 #
			int end = text.Length;
			for (; end > 0 && text[end - 1] == '#'; end--) ;
			if (end < text.Length)
			{
				// 要求闭合 # 前包含空格或 Tab。
				if (end == 0 || (end > 0 && MarkdownUtil.IsWhitespace(text[end - 1])))
				{
					text = text[0..end];
					// 忽略结尾 # 前的空白。
					text.TrimEnd();
				}
			}
			yield return new ATXHeadingProcessor(line.Start, depth, text, attrs);
		}
	}
}
