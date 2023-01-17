using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Inlines;

/// <summary>
/// 强调分隔符的处理器。
/// </summary>
internal sealed class EmphasisProcessor : DelimiterProcessor
{
	/// <summary>
	/// 是否允许在单词内使用分隔符。
	/// </summary>
	private readonly bool allowIntraword;

	/// <summary>
	/// 初始化 <see cref="EmphasisProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="allowIntraword">是否允许在单词内使用分隔符。</param>
	public EmphasisProcessor(bool allowIntraword) : base()
	{
		this.allowIntraword = allowIntraword;
	}

	/// <summary>
	/// 是否允许在单词内使用分隔符。
	/// </summary>
	public override bool AllowIntraword => allowIntraword;

	/// <summary>
	/// 处理指定的分隔符对。
	/// </summary>
	/// <param name="opening">起始分隔符。</param>
	/// <param name="closing">结束分隔符。</param>
	/// <returns>容器节点，如果分隔符不匹配则返回 <c>null</c>。</returns>
	public override INodeContainer<InlineNode>? Process(DelimiterInfo opening, DelimiterInfo closing)
	{
		// 如果起始和结束分隔符的总长度为 3 的倍数，且其各自长度并非 3 的倍数，那么认为不是成对的。
		if ((opening.CanClose || closing.CanOpen) &&
				closing.OriginalLength % 3 != 0 &&
				(opening.OriginalLength + closing.OriginalLength) % 3 == 0)
		{
			return null;
		}
		bool isStrong;
		int usedDelimiters;
		if (opening.Length >= 2 && closing.Length >= 2)
		{
			isStrong = true;
			usedDelimiters = 2;
		}
		else
		{
			isStrong = false;
			usedDelimiters = 1;
		}
		opening.Length -= usedDelimiters;
		closing.Length -= usedDelimiters;
		TextSpan span = new(opening.Node.Span.Start + opening.Length,
			closing.Node.Span.End - closing.Length);
		return isStrong ? new Strong(span) : new Emphasis(span);
	}
}
