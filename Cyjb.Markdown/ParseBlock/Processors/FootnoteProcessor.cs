using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 脚注的解析器。
/// </summary>
internal class FootnoteProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 脚注节点。
	/// </summary>
	private readonly Footnote footnote;
	/// <summary>
	/// 结束索引。
	/// </summary>
	private int end;

	/// <summary>
	/// 初始化 <see cref="FootnoteProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">脚注的起始位置。</param>
	/// <param name="end">脚注的结束位置。</param>
	/// <param name="label">脚注的标签。</param>
	private FootnoteProcessor(int start, int end, string label)
		: base(MarkdownKind.Footnote)
	{
		footnote = new Footnote(label, new TextSpan(start, end));
		this.end = end;
	}

	/// <summary>
	/// 获取是否是容器节点。
	/// </summary>
	public override bool IsContainer => true;

	/// <summary>
	/// 尝试将当前节点延伸到下一行。
	/// </summary>
	/// <param name="line">要检查的行。</param>
	/// <returns>当前节点是否可以延伸到下一行。</returns>
	public override BlockContinue TryContinue(BlockLine line)
	{
		if (line.IsCodeIndent || line.IsBlank())
		{
			// 缩进会被认为是脚注的一部分，这时要吃掉 4 个缩进。
			// 空白行也是脚注的一部分。
			line.SkipIndent(BlockLine.CodeIndent);
			return BlockContinue.Continue;
		}
		else
		{
			return BlockContinue.None;
		}
	}

	/// <summary>
	/// 返回当前节点是否可以包含指定类型的子节点。
	/// </summary>
	/// <param name="kind">要检查的节点类型。</param>
	/// <returns>如果当前节点可以包含指定类型的子节点，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool CanContains(MarkdownKind kind)
	{
		return true;
	}

	/// <summary>
	/// 添加一个新节点。
	/// </summary>
	/// <param name="node">要添加的节点。</param>
	public override void AddNode(Node node)
	{
		footnote.Children.Add((BlockNode)node);
		end = node.Span.End;
	}

	/// <summary>
	/// 关闭当前处理器的节点。
	/// </summary>
	/// <param name="end">行的结束位置。</param>
	/// <param name="parser">块解析器。</param>
	/// <returns>如果存在有效的节点，则返回节点本身。否则返回 <c>null</c>。</returns>
	public override Node? CloseNode(int end, BlockParser parser)
	{
		parser.Footnotes.TryAdd(footnote.Identifier, footnote);
		// 这里忽略空行的位置。
		footnote.Span = footnote.Span with
		{
			End = this.end,
		};
		return footnote;
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
			// 提前保存结束位置，避免行内文本被清空后无法找到正确的结束位置。
			int end = line.End;
			Token<BlockKind> token = line.PopFront();
			// 跳过之后的空白。
			line.SkipIndent();
			// 如果达到了行尾，那么消费整行，确保在空脚注时能够拿到正确的结束位置。
			if (line.IsBlank())
			{
				line.Skip();
			}
			else
			{
				end = line.Start;
			}
			yield return new FootnoteProcessor(token.Span.Start, end, ((StringView)token.Value!).ToString());
		}
	}
}
