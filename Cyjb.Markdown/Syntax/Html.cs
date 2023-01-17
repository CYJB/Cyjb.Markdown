using System.Text.RegularExpressions;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// 表示 Markdown 的行内 HTML 节点。
/// </summary>
public sealed class Html : InlineNode, IEquatable<Html>
{
	/// <summary>
	/// 提取 HTML 属性的正则表达式。
	/// </summary>
	private static readonly Regex AttributeRegex = new(
		@"([a-z_:][a-z0-9_.:-]*)\s*(?:=\s*([^ \t\r\n\""'=<>`']+|'[^']*'|\""[^""]*\""))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	/// <summary>
	/// 当前节点的内容。
	/// </summary>
	private string content;
	/// <summary>
	/// 当前节点的值。
	/// </summary>
	private string? value;
	/// <summary>
	/// 当前节点的 HTML 属性。
	/// </summary>
	private HtmlAttribute[]? attributes;

	/// <summary>
	/// 使用指定的 HTML 节点类型、内容和文本范围初始化 <see cref="Html"/> 类的新实例。
	/// </summary>
	/// <param name="kind">HTML 节点类型。</param>
	/// <param name="content">HTML 节点完整文本。</param>
	/// <param name="span">文本的范围。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> 不是
	/// <see cref="MarkdownKind.HtmlStartTag"/>、<see cref="MarkdownKind.HtmlEndTag"/>、
	/// <see cref="MarkdownKind.HtmlComment"/>、<see cref="MarkdownKind.HtmlCData"/>、
	/// <see cref="MarkdownKind.HtmlDeclaration"/> 或 <see cref="MarkdownKind.HtmlProcessing"/>
	/// 之一。</exception>
	/// <exception cref="ArgumentNullException"><paramref name="content"/> 为 <c>null</c>。</exception>
	public Html(MarkdownKind kind, string content, TextSpan span = default) : base(kind)
	{
		ArgumentNullException.ThrowIfNull(content);
		if (!kind.IsInlineHtml())
		{
			throw new ArgumentOutOfRangeException(nameof(kind), Resources.InvalidKindHtml);
		}
		this.content = content;
		Span = span;
	}

	/// <summary>
	/// 获取或设置 HTML 节点的内容。
	/// </summary>
	public string Content
	{
		get => content;
		set => content = value ?? string.Empty;
	}

	/// <summary>
	/// 获取 HTML 节点的值。
	/// </summary>
	/// <remarks>对于 <see cref="MarkdownKind.HtmlStartTag"/> 和 <see cref="MarkdownKind.HtmlEndTag"/>，
	/// 返回其标签名；对于其它 Html 类型，返回其内容（不包含起始和结束分隔符）</remarks>
	public string Value
	{
		get
		{
			value ??= GetContent(Kind, content);
			return value;
		}
	}

	/// <summary>
	/// 获取 HTML 节点的属性。
	/// </summary>
	/// <remarks>对于 <see cref="MarkdownKind.HtmlStartTag"/> 会返回其属性；
	/// 其它 Html 类型会返回空数组。</remarks>
	public HtmlAttribute[] Attributes
	{
		get
		{
			attributes ??= GetAttributes(Kind, content);
			return attributes;
		}
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitHtml(this);
	}

	/// <summary>
	/// 应用指定的访问器。
	/// </summary>
	/// <param name="visitor">节点访问器。</param>
	/// <returns>返回的结果。</returns>
	/// <typeparam name="TResult">返回结果的类型。</typeparam>
	public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor)
	{
		return visitor.VisitHtml(this)!;
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"{{{Kind} \"{Value}\" {Span}}}";
	}

	/// <summary>
	/// 返回指定 HTML 节点的内容。
	/// </summary>
	/// <param name="kind">节点的类型。</param>
	/// <param name="text">节点的文本。</param>
	/// <returns>HTML 节点的内容。</returns>
	private static string GetContent(MarkdownKind kind, string text)
	{
		switch (kind)
		{
			case MarkdownKind.HtmlStartTag:
				ReadOnlySpan<char> span = text.AsSpan()[1..];
				int idx = span.IndexOfAny(" \t\r\n>");
				if (idx > 0)
				{
					span = span[..idx];
				}
				return span.Trim().ToString();
			case MarkdownKind.HtmlEndTag:
			case MarkdownKind.HtmlDeclaration:
				return text.AsSpan()[2..^1].Trim().ToString();
			case MarkdownKind.HtmlComment:
				return text.AsSpan()[4..^3].Trim().ToString();
			case MarkdownKind.HtmlCData:
				return text.AsSpan()[9..^3].Trim().ToString();
			case MarkdownKind.HtmlProcessing:
				return text.AsSpan()[2..^2].Trim().ToString();
			default:
				throw CommonExceptions.Unreachable();
		}
	}

	/// <summary>
	/// 返回指定 HTML 节点的属性。
	/// </summary>
	/// <param name="kind">节点的类型。</param>
	/// <param name="text">节点的文本。</param>
	/// <returns>HTML 节点的属性。</returns>
	private static HtmlAttribute[] GetAttributes(MarkdownKind kind, string text)
	{
		if (kind == MarkdownKind.HtmlStartTag)
		{
			ReadOnlySpan<char> span = text.AsSpan()[1..^1];
			int idx = span.IndexOfAny(" \t\r\n");
			if (idx > 0)
			{
				span = span[idx..].Trim();
				if (span.Length > 0)
				{
					return AttributeRegex.Matches(span.ToString()).Select(match =>
					{
						string name = match.Groups[1].Value;
						string value = match.Groups[2].Value;
						string quote = string.Empty;
						char quotChar;
						if (value.Length >= 2 && (quotChar = value[0]) == value[^1] &&
							(quotChar == '"' || quotChar == '\''))
						{
							value = value[1..^1];
							quote = quotChar.ToString();
						}
						return new HtmlAttribute(name, value, quote);
					}).ToArray();
				}
			}
		}
		return Array.Empty<HtmlAttribute>();
	}

	#region IEquatable<Html> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(Html? other)
	{
		if (other is null)
		{
			return false;
		}
		return Kind == other.Kind && Content == other.Content;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is Html other)
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
		return HashCode.Combine(Kind, Content);
	}

	/// <summary>
	/// 返回指定的 <see cref="Html"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(Html? left, Html? right)
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
	/// 返回指定的 <see cref="Html"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(Html? left, Html? right)
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

	#endregion // IEquatable<Html> 成员

}
