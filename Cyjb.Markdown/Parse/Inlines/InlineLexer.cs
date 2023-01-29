using System.Text.RegularExpressions;
using Cyjb.Compilers.Lexers;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Inlines;

/// <summary>
/// 表示 Markdown 的行级元素语法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#inlines"/>
[LexerRejectable]
[LexerInclusiveContext("Link")]
[LexerRegex("WS", @"[ \t]*\r?\n?[ \t]*")]
[LexerRegex("WS_1", @"[ \t]+|[ \t]*\r?\n[ \t]*")]
[LexerRegex("TagName", @"[a-z][a-z0-9-]*", RegexOptions.IgnoreCase)]
[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
[LexerRegex("AttrValue", @"[^ \t\r\n""'=<>`']+|'[^']*'|\""[^""]*\""", RegexOptions.IgnoreCase)]
[LexerRegex("LinkLabel", @"\[([^\\\[\]]|\\.)*\]")]
[LexerRegex("LinkDestination", @"<([^<>\r\n\\]|\\.)*>|[^<\x00-\x1F\x7F ][^\x00-\x1F\x7F ]*")]
[LexerRegex("LinkTitle", @"\""([^""\\]|\\.)*\""|\'([^'\\]|\\.)*'|\(([^()\\]|\\.)*\)")]
internal partial class InlineLexer : LexerController<InlineKind>
{
	/// <summary>
	/// 链接上下文的名称。
	/// </summary>
	public const string LinkContext = "Link";
	/// <summary>
	/// 解析选项。
	/// </summary>
	private ParseOptions options;

	/// <summary>
	/// 硬换行的动作。
	/// </summary>
	[LexerSymbol(@"( {2,}|\\)\r?\n[ \t]*", Kind = InlineKind.Node)]
	private void HardBreakAction()
	{
		Accept(new Break(true));
	}

	/// <summary>
	/// 软换行的动作。
	/// </summary>
	[LexerSymbol(@" ?\r?\n[ \t]*", Kind = InlineKind.Node)]
	private void SoftBreakAction()
	{
		Accept(new Break(false));
	}

	/// <summary>
	/// 转义字符的动作。
	/// </summary>
	[LexerSymbol(@"\\[!""#$%&'()*+,-./:;<=>?@\[\\\]^_`{|}~]", Kind = InlineKind.Escaped)]
	private void EscapedAction()
	{
		Accept(Text);
	}

	/// <summary>
	/// 代码段的动作。
	/// </summary>
	[LexerSymbol("`+", Kind = InlineKind.Node)]
	private void CodeSpanAction()
	{
		// 检查是否是代码段的起始，需要能够找到对应的结束标志。
		int backtickCount = Text.Length;
		int idx = GetCodeSpansEnd(backtickCount);
		if (idx >= 0)
		{
			Source.Drop();
			Source.Index = idx;
			string content = Source.ReadedText().ReplacePattern("[\r\n]+", " ");
			// 如果代码两边都有一个空格（注意不是空白），那么可以移除前后各一的空格。
			// 不要移除多个空格，也不要修改只有空格组成的代码。
			if (content.Length >= 2 && content[0] == ' ' && content[^1] == ' ' &&
				content.Any(ch => ch != ' '))
			{
				content = content[1..^1];
			}
			// 跳过结束标志。
			Source.Index += backtickCount;
			Accept(new CodeSpan(content));
		}
		else
		{
			// 否则按照文本返回
			Accept(InlineKind.Literal, Text);
		}
	}

	/// <summary>
	/// 自动链接的动作。
	/// </summary>
	[LexerSymbol(@"[<][a-z][a-z0-9+.-]+:[^\0-\x1F\x7F <>]*>", RegexOptions.IgnoreCase, Kind = InlineKind.Autolink)]
	private void AutolinkAction()
	{
		string url = Text[1..^1];
		Link link = new(false, url);
		link.Children.Add(new Literal(url));
		Accept(link);
	}

	/// <summary>
	/// 邮件自动链接的动作。
	/// </summary>
	[LexerRegex("MailPart", "[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?", RegexOptions.IgnoreCase)]
	[LexerSymbol(@"[<][a-z0-9.!#$%&'*+/=?^_`{|}~-]+@{MailPart}(\.{MailPart})*>", RegexOptions.IgnoreCase, Kind = InlineKind.Autolink)]
	private void EmailAutolinkAction()
	{
		string url = Text[1..^1];
		Link link = new(false, "mailto:" + url);
		link.Children.Add(new Literal(url));
		Accept(link);
	}

	/// <summary>
	/// HTML 起始标签的动作。
	/// </summary>
	[LexerSymbol(@"[<]{TagName}(([ \t]+|[ \t]*\r?\n[ \t]*){AttrName}({WS}={WS}{AttrValue})?)*{WS}\/?>", Kind = InlineKind.Node)]
	private void HtmlStartTagAction()
	{
		Accept(new Html(MarkdownKind.HtmlStartTag, Text));
	}

	/// <summary>
	/// HTML 结束标签的动作。
	/// </summary>
	[LexerSymbol(@"[<]\/{TagName}{WS}>", Kind = InlineKind.Node)]
	private void HtmlEndTagAction()
	{
		Accept(new Html(MarkdownKind.HtmlEndTag, Text));
	}

	/// <summary>
	/// HTML 注释的动作。
	/// </summary>
	[LexerSymbol(@"[<]!---->", Kind = InlineKind.Node)]
	[LexerSymbol(@"[<]!--[^>-]-->", RegexOptions.Singleline, Kind = InlineKind.Node)]
	[LexerSymbol(@"[<]!--[^>].*[^-]-->", RegexOptions.Singleline, Kind = InlineKind.Node)]
	private void HtmlCommentAction()
	{
		var content = Text.AsSpan()[4..^3];
		if (content.StartsWith("->") || content.Contains("--", StringComparison.Ordinal))
		{
			Reject();
		}
		else
		{
			Accept(new Html(MarkdownKind.HtmlComment, Text));
		}
	}

	/// <summary>
	/// HTML 处理结构的动作。
	/// </summary>
	[LexerSymbol(@"[<]\?.*\?>", RegexOptions.Singleline, Kind = InlineKind.Node)]
	private void HtmlProcessingAction()
	{
		var content = Text.AsSpan()[2..^2];
		if (content.Contains("?>", StringComparison.Ordinal))
		{
			// 内容不能包含 ?>。
			Reject();
		}
		else
		{
			Accept(new Html(MarkdownKind.HtmlProcessing, Text));
		}
	}

	/// <summary>
	/// HTML 声明的动作。
	/// </summary>
	[LexerSymbol(@"[<]![a-z][^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline, Kind = InlineKind.Node)]
	private void HtmlDeclarationAction()
	{
		Accept(new Html(MarkdownKind.HtmlDeclaration, Text));
	}

	/// <summary>
	/// HTML CDATA 的动作。
	/// </summary>
	[LexerSymbol(@"[<]!\[CDATA\[.*\]\]>", RegexOptions.IgnoreCase | RegexOptions.Singleline, Kind = InlineKind.Node)]
	private void HtmlCDataAction()
	{
		var content = Text.AsSpan()[10..^3];
		if (content.Contains("]]>", StringComparison.Ordinal))
		{
			Reject();
		}
		else
		{
			Accept(new Html(MarkdownKind.HtmlCData, Text));
		}
	}

	/// <summary>
	/// 链接标签的动作。
	/// </summary>
	[LexerSymbol(@"<Link>{LinkLabel}", Kind = InlineKind.LinkLabel)]
	private void LinkLabelAction()
	{
		string? label;
		InlineParser parser = (InlineParser)SharedContext!;
		if (Text.Length == 2)
		{
			// 标签为空，将前面的文本作为标签使用。
			label = parser.GetCurrentLinkText();
		}
		else
		{
			// 标签不为空，使用标签本身。
			ReadOnlySpan<char> text = Text.AsSpan();
			if (!ParseUtil.TryParseLinkLabel(ref text, out label))
			{
				Reject();
				return;
			}
		}
		if (label != null && parser.TryGetLinkDefine(LinkUtil.NormalizeLabel(label), out LinkDefinition? define))
		{
			Accept(define);
		}
		else
		{
			Reject();
		}
	}
	/// <summary>
	/// 链接体的动作。
	/// </summary>
	[LexerSymbol(@"<Link>\({WS}{LinkDestination}?({WS_1}{LinkTitle})?{WS}\)", Kind = InlineKind.LinkBody)]
	private void LinkBodyAction()
	{
		ReadOnlySpan<char> text = Text.AsSpan()[1..^1].Trim();
		if (text.Length == 0)
		{
			// 没有链接目标和标题
			Accept(new LinkBody(string.Empty, null));
			return;
		}
		// 解析链接目标。
		if (!ParseUtil.TryParseLinkDestination(ref text, out string? destination))
		{
			Reject();
			return;
		}
		text = text.TrimStart();
		// 解析链接标题。
		string? title = null;
		if (text.Length > 0)
		{
			title = text[1..^1].Unescape();
		}
		Accept(new LinkBody(destination, title));
	}

	/// <summary>
	/// 表情符号的动作。
	/// </summary>
	/// <remarks>目前最长的 emoji 名称长度为 36。</remarks>
	[LexerSymbol(@":.{1,36}:", UseShortest = true, Kind = InlineKind.Emoji)]
	private void EmojiAction()
	{
		if (options.UseEmoji)
		{
			// 比较时忽略前后的 :。
			Emoji? emoji = Emoji.GetEmoji(Text[1..^1]);
			if (emoji != null)
			{
				Accept(emoji);
				return;
			}
		}
		Reject();
	}

	/// <summary>
	/// 普通字符的动作。
	/// </summary>
	/// <remarks>需要放在最后，确保优先级最低。</remarks>
	[LexerSymbol(@".", RegexOptions.Singleline, Kind = InlineKind.Literal)]
	private void CharAction()
	{
		Accept(Text);
	}

	/// <summary>
	/// 检查是否包含指定长度的代码段结束标志，并返回结束标志的位置。
	/// </summary>
	/// <param name="len">代码段开始标志的长度。</param>
	/// <returns>代码段结束标志的位置，使用 <c>-1</c> 表示不存在。</returns>
	private int GetCodeSpansEnd(int len)
	{
		char ch;
		int closeStart = -1;
		int closeEnd = -1;
		for (int i = 0; ; i++)
		{
			ch = Source.Peek(i);
			if (ch == '`')
			{
				if (closeStart == -1)
				{
					closeStart = i;
					closeEnd = i + 1;
				}
				else
				{
					closeEnd++;
				}
			}
			else if (closeEnd - closeStart == len && Source.Peek(closeStart - 1) != '/')
			{
				// 要求结束标识符长度与起始长度相同，且前面不是被转义的 `
				return Source.Index + closeStart;
			}
			else if (ch == SourceReader.InvalidCharacter)
			{
				return -1;
			}
			else
			{
				closeStart = -1;
			}
		}
	}

	/// <summary>
	/// 获取或设置共享的上下文对象。
	/// </summary>
	public override object? SharedContext
	{
		get => base.SharedContext;
		set
		{
			base.SharedContext = value;
			InlineParser parser = (InlineParser)value!;
			options = parser.Options;
			parser.Controller = this;
		}
	}
}
