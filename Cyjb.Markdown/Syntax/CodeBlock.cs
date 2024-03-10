using System.Diagnostics;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的代码块。
/// </summary>
public sealed class CodeBlock : BlockNode, IEquatable<CodeBlock>
{
	/// <summary>
	/// 代码块的内容。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string content;
	/// <summary>
	/// 代码块的信息。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? info;
	/// <summary>
	/// 代码块的属性。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly HtmlAttributeList attributes = new();

	/// <summary>
	/// 使用指定的代码内容和文本范围初始化 <see cref="CodeBlock"/> 类的新实例。
	/// </summary>
	/// <param name="content">代码块的内容。</param>
	/// <param name="span">文本的范围。</param>
	public CodeBlock(string content, TextSpan span = default) : base(MarkdownKind.CodeBlock)
	{
		this.content = content ?? string.Empty;
		Span = span;
	}

	/// <summary>
	/// 获取或设置代码的信息。
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
	/// 获取代码块的属性列表。
	/// </summary>
	public HtmlAttributeList Attributes => attributes;

	/// <summary>
	/// 获取或设置代码块的内容。
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
		visitor.VisitCodeBlock(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitCodeBlock(this)!;
	}

	/// <summary>
	/// 复制当前节点。
	/// </summary>
	/// <param name="deep">是仅复制当前节点还是需要复制所有子节点。</param>
	/// <param name="context">节点复制上下文。</param>
	/// <returns>复制的结果。</returns>
	internal override Node CloneNode(bool deep, NodeCloneContext context)
	{
		CodeBlock node = new(content, Span)
		{
			info = info,
			Locator = Locator,
		};
		attributes.CloneTo(node.attributes);
		return node;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		if (Info == null)
		{
			return base.ToString();
		}
		else
		{
			return $"{{CodeBlock {Info} {Span}}}";
		}
	}

	#region IEquatable<CodeBlock> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(CodeBlock? other)
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
		if (obj is CodeBlock other)
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
	/// 返回指定的 <see cref="CodeBlock"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(CodeBlock? left, CodeBlock? right)
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
	/// 返回指定的 <see cref="CodeBlock"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(CodeBlock? left, CodeBlock? right)
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

	#endregion // IEquatable<CodeBlock> 成员

}
