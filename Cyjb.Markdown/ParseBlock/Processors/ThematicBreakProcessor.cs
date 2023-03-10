using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 分割线的处理器。
/// </summary>
internal sealed class ThematicBreakProcessor : BlockProcessor
{
	/// <summary>
	/// 工厂实例。
	/// </summary>
	public static readonly IBlockFactory Factory = new BlockFactory();

	/// <summary>
	/// 分割线的起始位置。
	/// </summary>
	private readonly int start;

	/// <summary>
	/// 使用分割线的起始位置初始化 <see cref="ThematicBreakProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="start">分割线的起始位置。</param>
	private ThematicBreakProcessor(int start) : base(MarkdownKind.ThematicBreak)
	{
		this.start = start;
	}

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
		return new ThematicBreak(new TextSpan(start, end));
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
		/// <returns>如果能够开始当前块的解析，则返回处理器序列。否则返回空序列。</returns>
		public IEnumerable<BlockProcessor> TryStart(LineInfo line, BlockProcessor matchedProcessor)
		{
			if (line.IsCodeIndent)
			{
				yield break;
			}
			Token<BlockKind> token = line.Peek();
			if (token.Kind == BlockKind.DashLine)
			{
				// 需要确保 DashLine 的长度至少为 3。
				int checkLen = Math.Min(3, token.Text.Length);
				int len = 1;
				for (; len < checkLen; len++)
				{
					if (token.Text[len] != '-')
					{
						break;
					}
				}
				if (len < 3)
				{
					yield break;
				}
			}
			else if (token.Kind == BlockKind.UnorderedListMarker)
			{
				// 分割线优先级高于列表项。
				if (!IsListMarkerThematicBreak(line.Tokens))
				{
					yield break;
				}
			}
			line.SkipIndent();
			yield return new ThematicBreakProcessor(line.Start);
		}

		/// <summary>
		/// 检查指定的列表项标志是否可能是分割线。
		/// </summary>
		/// <param name="tokens">要检查的词法单元列表。</param>
		/// <returns>如果是分割线，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private static bool IsListMarkerThematicBreak(IReadOnlyList<Token<BlockKind>> tokens)
		{
			char ch;
			int dashes = 0;
			int asterisks = 0;
			// 计算分隔符的个数（大于等于 3），且不会出现混用。
			foreach (Token<BlockKind> token in tokens)
			{
				switch (token.Kind)
				{
					case BlockKind.UnorderedListMarker:
						ch = token.Text[0];
						if (ch == '-')
						{
							dashes++;
						}
						else if (ch == '*')
						{
							asterisks++;
						}
						else
						{
							// 不是有效的分隔符。
							return false;
						}
						break;
					case BlockKind.DashLine:
						dashes += token.Text.Count((ch) => ch == '-');
						break;
					case BlockKind.ThematicBreak:
						ch = token.Text[0];
						if (ch == '-')
						{
							dashes += 3;
						}
						else if (ch == '*')
						{
							asterisks += 3;
						}
						else
						{
							// 存在分隔符混用的情况。
							return false;
						}
						break;
				}
			}
			return (dashes >= 3 && asterisks == 0) || (asterisks >= 3 && dashes == 0);
		}
	}
}
