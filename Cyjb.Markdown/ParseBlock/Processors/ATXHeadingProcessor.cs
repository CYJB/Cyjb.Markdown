using System.Runtime.CompilerServices;
using Cyjb.Markdown.ParseInline;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// ATX 标题的解析器。
/// </summary>
internal sealed class ATXHeadingProcessor : BlockProcessor
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
	private readonly BlockText text;

	/// <summary>
	/// 使用ATX 标题的起始位置和文本初始化 <see cref="ATXHeadingProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">ATX 标题的起始位置。</param>
	/// <param name="depth">ATX 标题的深度。</param>
	/// <param name="text">ATX 标题的文本。</param>
	/// <param name="attrs">ATX 标题的属性。</param>
	private ATXHeadingProcessor(int start, int depth, BlockText text, HtmlAttributeList? attrs)
		: base(MarkdownKind.Heading)
	{
		heading = new Heading(depth, new TextSpan(start, start));
		if (attrs != null && attrs.Count > 0)
		{
			heading.Attributes.AddRange(attrs);
		}
		this.text = text;
	}

	/// <summary>
	/// 获取当前块是否需要解析行内节点。
	/// </summary>
	public override bool NeedParseInlines => text.Length > 0;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		return BlockContinue.None;
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		HeadingUtils.ProcessHeading(parser, heading, LinkUtil.NormalizeLabel(text));
		heading.Span = new TextSpan(heading.Span.Start, end);
		return heading;
	}

	/// <summary>
	/// 解析行内节点。
	/// </summary>
	/// <param name="parser">行内节点的解析器。</param>
	public override void ParseInline(InlineParser parser)
	{
		parser.Parse(text, heading.Children);
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
			line.SkipIndent();
			BlockText text = line.BlockText;
			int start = text.Start;
			var token = text.PeekFront();
			HtmlAttributeList? attrs = token.Value as HtmlAttributeList;
			// 计算标题的深度，标题一定在同一个 Token 内。
			int depth = GetHeadingDepth(token.Text);
			text.RemoteStart(depth);
			// 忽略内容前后的空白
			text.TrimStart();
			text.TrimEnd();
			// 检查闭合 #
			if (text.Tokens.Count > 0)
			{
				TrimEndingSharp(text);
			}
			yield return new ATXHeadingProcessor(start, depth, text.Clone(), attrs);
		}

		/// <summary>
		/// 计算标题的深度。
		/// </summary>
		/// <param name="text">标题的文本。</param>
		/// <returns>标题的深度。</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetHeadingDepth(StringView text)
		{
			ReadOnlySpan<char> span = text;
			return span.Length - span.TrimStart('#').Length;
		}

		/// <summary>
		/// 移除标题的结束 # 符号。
		/// </summary>
		/// <param name="text">标题的文本。</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TrimEndingSharp(BlockText text)
		{
			ReadOnlySpan<char> span = text.PeekBack().Text;
			int sharpCount = span.Length - span.TrimEnd('#').Length;
			if (sharpCount == 0)
			{
				return;
			}
			if (sharpCount == span.Length)
			{
				// 最后一个 Token 被全部消费。
				text.PopBack();
				text.TrimEnd();
			}
			else if (MarkdownUtil.IsWhitespace(span[span.Length - sharpCount - 1]))
			{
				// 要求闭合 # 前包含空格或 Tab。
				text.RemoteEnd(sharpCount);
				text.TrimEnd();
			}
		}
	}
}
