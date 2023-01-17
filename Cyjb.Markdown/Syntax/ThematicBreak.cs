using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的分隔线。
/// </summary>
public sealed class ThematicBreak : BlockNode, IEquatable<ThematicBreak>
{
	/// <summary>
	/// 使用指定的文本范围初始化 <see cref="ThematicBreak"/> 类的新实例。
	/// </summary>
	/// <param name="span">文本范围。</param>
	public ThematicBreak(TextSpan span = default) : base(MarkdownKind.ThematicBreak)
	{
		Span = span;
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitThematicBreak(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitThematicBreak(this)!;
	}

	#region IEquatable<ThematicBreak> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(ThematicBreak? other)
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
		if (obj is ThematicBreak other)
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
		return HashCode.Combine(MarkdownKind.ThematicBreak, Span);
	}

	/// <summary>
	/// 返回指定的 <see cref="ThematicBreak"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(ThematicBreak? left, ThematicBreak? right)
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
	/// 返回指定的 <see cref="ThematicBreak"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(ThematicBreak? left, ThematicBreak? right)
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

	#endregion // IEquatable<ThematicBreak> 成员

}
