using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 行内分隔符的处理器。
/// </summary>
internal abstract class DelimiterProcessor
{
	/// <summary>
	/// 允许的最小长度。
	/// </summary>
	private readonly int minLength;
	/// <summary>
	/// 允许的最大长度。
	/// </summary>
	private readonly int maxLength;

	/// <summary>
	/// 使用指定的分隔符信息初始化 <see cref="DelimiterProcessor"/> 类的新实例。
	/// </summary>
	/// <param name="minLength">允许的最小长度。</param>
	/// <param name="maxLength">允许的最大长度。</param>
	protected DelimiterProcessor(int minLength = 1, int maxLength = int.MaxValue)
	{
		this.minLength = minLength;
		this.maxLength = maxLength;
	}

	/// <summary>
	/// 是否允许在单词内使用分隔符。
	/// </summary>
	public virtual bool AllowIntraword => true;

	/// <summary>
	/// 返回指定分隔符长度是否有效。
	/// </summary>
	/// <param name="length">分隔符的长度。</param>
	/// <returns>如果分隔符有效，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool IsValidLength(int length)
	{
		return length >= minLength && length <= maxLength;
	}

	/// <summary>
	/// 处理指定的分隔符对。
	/// </summary>
	/// <param name="opening">起始分隔符。</param>
	/// <param name="closing">结束分隔符。</param>
	/// <returns>容器节点，如果分隔符不匹配则返回 <c>null</c>。</returns>
	public abstract INodeContainer<InlineNode>? Process(DelimiterInfo opening, DelimiterInfo closing);
}
