using System.Text.RegularExpressions;
using Cyjb.Compilers.Lexers;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 Markdown 的属性词法分析器。
/// </summary>
/// <see href="https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/attributes.md"/>
[LexerRegex("Name", @"[^ \t\r\n""'=<>`{}]+")]
[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
internal partial class AttributeLexer : LexerController<AttributeKind>
{
	/// <summary>
	/// 标识符或类名的动作。
	/// </summary>
	[LexerSymbol(@"#{Name}", Kind = AttributeKind.Identifier)]
	[LexerSymbol(@"\.{Name}", Kind = AttributeKind.ClassName)]
	private void IdentifierOrClassNameAction()
	{
		// 文本排除前面的符号。
		Text = Text.Substring(1);
		Accept();
	}

	/// <summary>
	/// 无值属性的动作。
	/// </summary>
	[LexerSymbol(@"{AttrName}", Kind = AttributeKind.Common)]
	private void NoValueAttributeAction()
	{
		Accept(string.Empty);
	}

	/// <summary>
	/// 带有引号值属性的动作。
	/// </summary>
	[LexerRegex("AttrValue", @"'[^'\r\n]*'|\""[^""\r\n]*\""", RegexOptions.IgnoreCase)]
	[LexerSymbol(@"{AttrName}={AttrValue}", Kind = AttributeKind.Common)]
	private void QuotedValueAttributeAction()
	{
		int idx = Text.IndexOf('=');
		// 移除值两边的引号。
		string value = Text[(idx + 2)..^1].ToString();
		Text = Text.Substring(0, idx);
		Accept(value);
	}

	/// <summary>
	/// 带有未被引号括起来的属性值属性的动作。
	/// </summary>
	[LexerSymbol(@"{AttrName}=[^ \t\r\n\""'=<>`{}]+", Kind = AttributeKind.Common)]
	private void UnquotedValueAttributeAction()
	{
		int idx = Text.IndexOf('=');
		string value = Text[(idx + 1)..].ToString();
		Text = Text.Substring(0, idx);
		Accept(value);
	}

	/// <summary>
	/// 分隔符的动作。
	/// </summary>
	[LexerSymbol(@"[ \t\r\n]+", Kind = AttributeKind.Seperator)]
	private void SeperatorAction()
	{
		Accept();
	}

	/// <summary>
	/// 结束符的动作。
	/// </summary>
	[LexerSymbol(@"\}")]
	private void EndAction()
	{
		// 要求之后到换行之前只能包含空白。
		foreach (char ch in Source.ReadLine(false))
		{
			if (ch != ' ' && ch != '\t')
			{
				// 包含非空白字符，不识别为结束符。
				Accept(AttributeKind.Invalid);
				return;
			}
		}
		Accept(AttributeKind.End);
	}

	/// <summary>
	/// 无效字符的兜底动作。
	/// </summary>
	[LexerSymbol(".", Kind = AttributeKind.Invalid)]
	private void InvalidAction()
	{
		Accept();
	}
}
