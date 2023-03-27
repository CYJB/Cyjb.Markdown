using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的数学公式块。
/// </summary>
public sealed class MathBlock : BlockNode, IEquatable<MathBlock>
{
	/// <summary>
	/// 数学公式的内容。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string content;
	/// <summary>
	/// 数学公式的信息。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? info;
	/// <summary>
	/// 数学公式的属性。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly HtmlAttributeList attributes = new();

	/// <summary>
	/// 使用指定的数学公式内容和文本范围初始化 <see cref="MathBlock"/> 类的新实例。
	/// </summary>
	/// <param name="content">数学公式的内容。</param>
	/// <param name="span">文本的范围。</param>
	public MathBlock(string content, TextSpan span = default) : base(MarkdownKind.MathBlock)
	{
		this.content = content ?? string.Empty;
		Span = span;
	}

	/// <summary>
	/// 获取或设置数学公式的信息。
	/// </summary>
	public string? Info
	{
		get => info;
		set
		{
			if (value.IsNullOrWhiteSpace())
			{
				info = null;
			}
			else
			{
				info = value.Trim();
			}
		}
	}

	/// <summary>
	/// 获取数学公式的属性列表。
	/// </summary>
	public HtmlAttributeList Attributes => attributes;

	/// <summary>
	/// 获取或设置数学公式的内容。
	/// </summary>
	public string Content
	{
		get => content;
		set { content = value ?? string.Empty; }
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitMathBlock(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitMathBlock(this)!;
	}

	#region IEquatable<MathBlock> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(MathBlock? other)
	{
		if (other is null)
		{
			return false;
		}
		return Content == other.Content && Span == other.Span;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is MathBlock other)
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
		return HashCode.Combine(Content, Span);
	}

	/// <summary>
	/// 返回指定的 <see cref="MathBlock"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(MathBlock? left, MathBlock? right)
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
	/// 返回指定的 <see cref="MathBlock"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(MathBlock? left, MathBlock? right)
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

	#endregion // IEquatable<MathBlock> 成员

}
