using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的换行。
/// </summary>
public sealed class Break : InlineNode, IEquatable<Break>
{
	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="Break"/> 类的新实例。
	/// </summary>
	/// <param name="isHard">是否是硬换行。</param>
	/// <param name="span">文本的范围。</param>
	public Break(bool isHard, TextSpan span = default)
		: base(isHard ? MarkdownKind.HardBreak : MarkdownKind.SoftBreak)
	{
		Span = span;
	}

	/// <summary>
	/// 使用指定的节点类型和文本范围初始化 <see cref="Break"/> 类的新实例。
	/// </summary>
	/// <param name="kind">节点类型。</param>
	/// <param name="span">文本的范围。</param>
	public Break(MarkdownKind kind, TextSpan span = default)
		: base(kind)
	{
		Span = span;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitBreak(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitBreak(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		return new Break(Kind, Span)
		{
			Locator = Locator,
		};
	}

	#region IEquatable<Break> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(Break? other)
	{
		if (other is null)
		{
			return false;
		}
		return Span == other.Span;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is Break other)
		{
			return Equals(other);
		}
		return false;
	}

	/// <summary>
	/// 返回当前对象的哈希值。
	/// </summary>
	/// <returns>当前对象的哈希值。</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(MarkdownKind.HardBreak, Span);
	}

	/// <summary>
	/// 返回指定的 <see cref="Break"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(Break? left, Break? right)
	{
		if (ReferenceEquals(left, right))
		{
			return true;
		}
		if (left is null)
		{
			return false;
		}
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="Break"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(Break? left, Break? right)
	{
		if (ReferenceEquals(left, right))
		{
			return false;
		}
		if (left is null)
		{
			return true;
		}
		return !left.Equals(right);
	}

	#endregion // IEquatable<Break> 成员

}
