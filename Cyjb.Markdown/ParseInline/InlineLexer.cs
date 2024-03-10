using System.Text.RegularExpressions;
using Cyjb.Collections;
using Cyjb.Compilers.Lexers;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 表示 Markdown 的行级元素语法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/#inlines"/>
[LexerRejectable]
[LexerInclusiveContext("LinkClose")]
[LexerRegex("WS", @"[ \t]*\r?\n?[ \t]*")]
[LexerRegex("WS_1", @"[ \t]+|[ \t]*\r?\n[ \t]*")]
[LexerRegex("TagName", @"[a-z][a-z0-9-]*", RegexOptions.IgnoreCase)]
[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
[LexerRegex("AttrValue", @"[^ \t\r\n""'=<>`']+|'[^']*'|\""[^""]*\""", RegexOptions.IgnoreCase)]
[LexerRegex("ExtAttr", @"\{([^""'<>`'{}]*|'[^'\r\n]*'|\""[^""\r\n]*\"")*\}")]
internal partial class InlineLexer : LexerController<InlineKind>
{
	/// <summary>
	/// 识别链接闭合上下文的名称。
	/// </summary>
	public const string LinkCloseContext = "LinkClose";
	/// <summary>
	/// <c>*</c> 的强调分隔符处理器。
	/// </summary>
	private static readonly EmphasisProcessor StarEmphasisProcessor = new(true);
	/// <summary>
	/// <c>_</c> 的强调分隔符处理器。
	/// </summary>
	private static readonly EmphasisProcessor UnderlineEmphasisProcessor = new(false);
	/// <summary>
	/// <c>~</c> 的删除线分隔符处理器。
	/// </summary>
	private static readonly StrikethroughProcessor StrikethroughProcessor = new();

	/// <summary>
	/// 解析选项。
	/// </summary>
	private ParseOptions options;
	/// <summary>
	/// 前一 Token 的最后一个字符，这里使用换行，可以正确处理行首相关逻辑。
	/// </summary>
	private char lastChar = '\n';
	/// <summary>
	/// 字面量缓冲区。
	/// </summary>
	private readonly PooledList<char> literalBuffer = new (0x1000);

	/// <summary>
	/// 获取当前词法单元的文本范围。
	/// </summary>
	public override TextSpan Span => ((InlineParser)SharedContext!).MapSpan(base.Span);

	/// <summary>
	/// 返回并清空当前字面量文本。
	/// </summary>
	/// <returns>当前字面量文本。</returns>
	public string GetLiteralText()
	{
		if (literalBuffer.Length == 0)
		{
			return string.Empty;
		}
		Span<char> span = literalBuffer.AsSpan();
		string result = span.Unescape();
		literalBuffer.Clear();
		return result;
	}

	/// <summary>
	/// 硬换行的动作。
	/// </summary>
	[LexerSymbol(@"( {2,}|\\)\r?\n[ \t]*", Kind = InlineKind.Node)]
	private void HardBreakAction()
	{
		Accept(new Break(true, Span));
	}

	/// <summary>
	/// 软换行的动作。
	/// </summary>
	[LexerSymbol(@" ?\r?\n[ \t]*", Kind = InlineKind.Node)]
	private void SoftBreakAction()
	{
		Accept(new Break(false, Span));
	}

	/// <summary>
	/// 转义字符的动作，直接添加到字面量文本而不返回 Token。
	/// </summary>
	[LexerSymbol(@"\\[!""#$%&'()*+,-./:;<=>?@\[\\\]^_`{|}~]")]
	private void EscapedAction()
	{
		AppendLiteralText(Text);
	}

	/// <summary>
	/// 代码段的动作。
	/// </summary>
	[LexerSymbol("`+")]
	private void CodeSpanAction()
	{
		// 检查是否是代码段的起始，需要能够找到对应的结束标志。
		int backtickCount = Text.Length;
		int idx = GetSpansEnd('`', backtickCount);
		if (idx >= 0)
		{
			Source.Drop();
			Source.Index = idx;
			string content = MarkdownUtil.ProcessCodeSpan(Source.GetReadedText());
			// 跳过结束标志。
			Source.Index += backtickCount;
			Accept(InlineKind.Node, new CodeSpan(content, Span));
		}
		else
		{
			// 否则添加为文本。
			AppendLiteralText(Text);
		}
	}

	/// <summary>
	/// 自动链接的动作。
	/// </summary>
	[LexerSymbol(@"[<][a-z][a-z0-9+.-]+:[^\0-\x20\x7F<>]*>{ExtAttr}?", RegexOptions.IgnoreCase, Kind = InlineKind.Node)]
	private void AutolinkAction()
	{
		string url = ParseAutolink(out HtmlAttributeList? attrs);
		Link link = new(false, url, null, Span);
		if (attrs != null)
		{
			link.Attributes.AddRange(attrs);
		}
		int start = Start + 1;
		link.Children.Add(new Literal(url, new TextSpan(start, start + url.Length)));
		Accept(link);
	}

	/// <summary>
	/// 邮件自动链接的动作。
	/// </summary>
	[LexerRegex("MailName", "[a-z0-9.!#$%&'*+/=?^_`{|}~-]+", RegexOptions.IgnoreCase)]
	[LexerRegex("MailDomain", "[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?", RegexOptions.IgnoreCase)]
	[LexerSymbol(@"[<]{MailName}@{MailDomain}(\.{MailDomain})*>{ExtAttr}?", Kind = InlineKind.Node)]
	private void EmailAutolinkAction()
	{
		string url = ParseAutolink(out HtmlAttributeList? attrs);
		Link link = new(false, "mailto:" + url, null, Span);
		if (attrs != null)
		{
			link.Attributes.AddRange(attrs);
		}
		int start = Start + 1;
		link.Children.Add(new Literal(url, new TextSpan(start, start + url.Length)));
		Accept(link);
	}

	/// <summary>
	/// 解析自动链接的 URL。
	/// </summary>
	/// <param name="attrs">链接的属性。</param>
	/// <returns>链接的 URL。</returns>
	private string ParseAutolink(out HtmlAttributeList? attrs)
	{
		int idx = Text.IndexOf('>');
		string url = Text[1..idx].ToString();
		// 解析属性。
		StringView attrText = Text[(idx + 1)..];
		int attrLen = attrText.Length;
		if (options.UseLinkAttributes && attrText.Length > 0 &&
			attrText[0] == '{' && attrText[^1] == '}')
		{
			attrs = new HtmlAttributeList();
			if (!MarkdownUtil.TryParseAttributes(ref attrText, attrs) || attrText.Length > 0)
			{
				// 如果属性解析失败，或者并未正确解析所有属性，那么同样需要回滚属性。
				attrs = null;
				Source.Index -= attrLen;
			}
		}
		else
		{
			attrs = null;
			Source.Index -= attrLen;
		}
		return url;
	}

	/// <summary>
	/// 无效的扩展自动链接前置字符 <c>[a-zA-Z0-9+.-/:]</c>。
	/// </summary>
	private static readonly ReadOnlyCharSet InvalidExtAutolinkPreChar = ReadOnlyCharSet.FromRange("++-:AZaz");

	/// <summary>
	/// 扩展自动链接的动作。
	/// </summary>
	[LexerSymbol(@"(https?:\/\/|www\.).+/<|\s|{EOF}", RegexOptions.IgnoreCase,
		Kind = InlineKind.Node, UseShortest = true)]
	private void ExtAutolinkAction()
	{
		// 在链接体里，不识别扩展自动链接。
		if (options.UseExtAutolink && Context != LinkCloseContext &&
			!InvalidExtAutolinkPreChar.Contains(lastChar))
		{
			string text = Text.ToString();
			string url = char.ToLower(text[0]) == 'w' ? "http://" + text : text;
			int cnt = MarkdownUtil.TrimEndPunctuations(ref url);
			if (MarkdownUtil.IsValidDomain(url))
			{
				Source.Index -= cnt;
				Link link = new(false, url, null, Span);
				link.Children.Add(new Literal(Text.ToString(), Span));
				Accept(link);
				return;
			}
		}
		Reject(RejectOptions.State);
	}

	/// <summary>
	/// 扩展邮件自动链接的动作。
	/// </summary>
	[LexerSymbol(@"(mailto:)?[a-z0-9.+_-]+@{MailDomain}(\.{MailDomain})+", RegexOptions.IgnoreCase)]
	private void ExtEmailAutolinkAction()
	{
		// 在链接体里，不识别扩展自动链接。
		// 邮件链接后不能是 -
		if (options.UseExtAutolink && Context != LinkCloseContext &&
			!InvalidExtAutolinkPreChar.Contains(lastChar) && Source.Peek() != '-')
		{
			string text = Text.ToString();
			string url = text.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ? text : "mailto:" + text;
			Link link = new(false, url, null, Span);
			link.Children.Add(new Literal(text, Span));
			Accept(InlineKind.Node, link);
			return;
		}
		Reject(RejectOptions.State);
	}

	/// <summary>
	/// HTML 起始标签的动作。
	/// </summary>
	[LexerSymbol(@"[<]{TagName}({WS_1}{AttrName}({WS}={WS}{AttrValue})?)*{WS}\/?>", Kind = InlineKind.Node)]
	private void HtmlStartTagAction()
	{
		Accept(new Html(MarkdownKind.HtmlStartTag, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 结束标签的动作。
	/// </summary>
	[LexerSymbol(@"[<]\/{TagName}{WS}>", Kind = InlineKind.Node)]
	private void HtmlEndTagAction()
	{
		Accept(new Html(MarkdownKind.HtmlEndTag, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 注释的动作。
	/// </summary>
	[LexerSymbol(@"[<]!---->", Kind = InlineKind.Node)]
	[LexerSymbol(@"[<]!--[^>-]-->", RegexOptions.Singleline, Kind = InlineKind.Node)]
	[LexerSymbol(@"[<]!--[^>].*[^-]-->", RegexOptions.Singleline, Kind = InlineKind.Node)]
	private void HtmlCommentAction()
	{
		var content = Text[4..^3];
		if (content.StartsWith("->") || content.Contains("--", StringComparison.Ordinal))
		{
			Reject();
		}
		else
		{
			Accept(new Html(MarkdownKind.HtmlComment, Text.ToString(), Span));
		}
	}

	/// <summary>
	/// HTML 处理结构的动作。
	/// </summary>
	[LexerSymbol(@"[<]\?.*\?>", RegexOptions.Singleline, Kind = InlineKind.Node, UseShortest = true)]
	private void HtmlProcessingAction()
	{
		Accept(new Html(MarkdownKind.HtmlProcessing, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 声明的动作。
	/// </summary>
	[LexerSymbol(@"[<]![a-z][^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline, Kind = InlineKind.Node)]
	private void HtmlDeclarationAction()
	{
		Accept(new Html(MarkdownKind.HtmlDeclaration, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML CDATA 的动作。
	/// </summary>
	[LexerSymbol(@"[<]!\[CDATA\[.*\]\]>", RegexOptions.IgnoreCase | RegexOptions.Singleline, Kind = InlineKind.Node,
		UseShortest = true)]
	private void HtmlCDataAction()
	{
		Accept(new Html(MarkdownKind.HtmlCData, Text.ToString(), Span));
	}

	/// <summary>
	/// 链接标签的动作。
	/// </summary>
	[LexerRegex("LinkLabel", @"\[([^\\\[\]]|\\.)*\]")]
	[LexerSymbol(@"<LinkClose>]{LinkLabel}", Kind = InlineKind.LinkClose)]
	private void LinkLabelAction()
	{
		InlineParser parser = (InlineParser)SharedContext!;
		// 标签不为空，使用标签本身。
		ReadOnlySpan<char> text = Text[2..];
		if (!MarkdownUtil.TryParseLinkLabel(ref text, out string? label))
		{
			Reject();
			return;
		}
		if (label.Length == 0)
		{
			// 标签为空，将前面的文本作为标签使用。
			label = parser.GetCurrentLinkText(Start);
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
	[LexerRegex("LinkDestination", @"<([^<>\r\n\\]|\\.)*>|[^<\x00-\x1F\x7F ][^\x00-\x1F\x7F ]*")]
	[LexerRegex("LinkTitle", @"\""([^""\\]|\\.)*\""|\'([^'\\]|\\.)*'|\(([^()\\]|\\.)*\)")]
	[LexerSymbol(@"<LinkClose>]\({WS}{LinkDestination}?({WS_1}{LinkTitle})?{WS}\){ExtAttr}?", Kind = InlineKind.LinkClose)]
	private void LinkBodyAction()
	{
		StringView text = Text[2..];
		MarkdownUtil.TrimStart(ref text);
		// 解析链接目标和标题。
		if (!MarkdownUtil.TryParseLinkDestination(ref text, out string? destination))
		{
			Reject();
			return;
		}
		string? title = null;
		if (MarkdownUtil.TrimStart(ref text) && !MarkdownUtil.TryParseLinkTitle(ref text, out title))
		{
			Reject();
			return;
		}
		MarkdownUtil.TrimStart(ref text);
		// 链接目标和标题要求以 ) 闭合。
		if (text.Length == 0 || text[0] != ')')
		{
			Reject();
			return;
		}
		text = text[1..];
		LinkBody body = new(destination, title);
		// 解析属性。
		int attrLen = text.Length;
		if (options.UseLinkAttributes && text.Length > 0 &&
			text[0] == '{' && text[^1] == '}')
		{
			if (!MarkdownUtil.TryParseAttributes(ref text, body.Attributes) || text.Length > 0)
			{
				// 如果属性解析失败，或者并未正确解析所有属性，那么同样需要回滚属性。
				body.Attributes.Clear();
				Source.Index -= attrLen;
			}
		}
		else
		{
			Source.Index -= attrLen;
		}
		Accept(body);
	}

	/// <summary>
	/// 表情符号的动作。
	/// </summary>
	/// <remarks>目前最长的 emoji 名称长度为 36。</remarks>
	[LexerSymbol(@":.{1,36}:", UseShortest = true, Kind = InlineKind.Node)]
	private void EmojiAction()
	{
		if (options.UseEmoji)
		{
			// 比较时忽略前后的 :。
			Emoji? emoji = Emoji.GetEmoji(Text[1..^1], Span);
			if (emoji != null)
			{
				Accept(emoji);
				return;
			}
		}
		Reject();
	}

	/// <summary>
	/// 数学公式段的动作。
	/// </summary>
	[LexerSymbol("$+")]
	private void MathSpanAction()
	{
		int dollarCount = Text.Length;
		int idx = -1;
		if (options.UseMath)
		{
			// 检查是否是数学公式段的起始，需要能够找到对应的结束标志。
			if (dollarCount == 1)
			{
				// 使用一个 $ 时，起始 $ 右边必须有一个非空白（非空格、Tab 或换行）字符。
				char ch = Source.Peek();
				if (!MarkdownUtil.IsWhitespace(Source.Peek()))
				{
					idx = GetSpansEnd('$', 1);
					// 结束 `$` 的左边同样必须非空白字符，且后面不能紧跟数字。
					if (idx >= 0 &&
						(MarkdownUtil.IsWhitespace(Source.Peek(idx - Source.Index - 1)) ||
						char.IsDigit(Source.Peek(idx - Source.Index + 1)))
						)
					{
						idx = -1;
					}
				}
			}
			else
			{
				idx = GetSpansEnd('$', dollarCount);
			}
		}
		if (idx >= 0)
		{
			Source.Drop();
			Source.Index = idx;
			string content = Source.GetReadedText().ToString();
			// 如果内容两边都有一个空格（或换行），那么可以移除前后各一的空格（或换行）。
			// 不要移除多个空格，也不要修改只由空格组成的代码。
			if (content.Length >= 2 && content.Any(ch => !MarkdownUtil.IsWhitespace(ch)))
			{
				if (content[0] == ' ' && content[^1] == ' ')
				{
					content = content[1..^1];
				}
				else if ((content[0] == '\r' || content[0] == '\n') &&
					(content[^1] == '\r' || content[^1] == '\n'))
				{
					int start = 1;
					int end = 1;
					if (content[0] == '\r' && content[1] == '\n')
					{
						start++;
					}
					if (content[^1] == '\n' && content[^2] == '\r')
					{
						end++;
					}
					content = content[start..^end];
				}
			}
			// 跳过结束标志。
			Source.Index += dollarCount;
			Accept(InlineKind.Node, new MathSpan(content, Span));
		}
		else
		{
			// 否则添加为文本。
			AppendLiteralText(Text);
		}
	}

	/// <summary>
	/// 链接起始的动作。
	/// </summary>
	[LexerSymbol(@"\[", Kind = InlineKind.LinkStart)]
	[LexerSymbol(@"!\[", Kind = InlineKind.LinkStart)]
	private void LinkStartAction()
	{
		Accept(Text.Length > 1);
	}

	/// <summary>
	/// 链接结束的动作。
	/// </summary>
	/// <remarks>这里需要负责闭合非活动状态的左中括号，因此不能限定在 LinkClose 上下文。</remarks>
	[LexerSymbol(@"]", Kind = InlineKind.LinkClose)]
	private void LinkCloseAction()
	{
		// value 为 null，与之前的 LinkBody、LinkLabel 区分。
		Accept();
	}

	/// <summary>
	/// 多个普通字符的动作，避开了所有的起始特殊符号。
	/// </summary>
	[LexerSymbol(@"[^hmw \\\r\n<\[\]"":$!*_~`]+", RegexOptions.Singleline)]
	private void MultiCharAction()
	{
		AppendLiteralText(Text);
	}

	/// <summary>
	/// 强调分隔符的动作。
	/// </summary>
	/// <remarks>需要放在最后，确保优先级最低。</remarks>
	[LexerSymbol(@"\*|_", RegexOptions.Singleline)]
	private void EmphasisAction()
	{
		char ch = Text[0];
		DelimiterProcessor processor = ch == '*' ? StarEmphasisProcessor : UnderlineEmphasisProcessor;
		DelimiterInfo? info = ScanDelimiters(ch, processor);
		if (info != null)
		{
			Accept(InlineKind.Delimiter, info);
			return;
		}
		// 在 ScanDelimiters 可能会将后续的分隔符一起消费掉。
		AppendLiteralText(Text);
	}

	/// <summary>
	/// 删除分隔符的动作。
	/// </summary>
	/// <remarks>需要放在最后，确保优先级最低。</remarks>
	[LexerSymbol(@"~", RegexOptions.Singleline)]
	private void StrikethroughAction()
	{
		if (options.UseStrikethrough)
		{
			DelimiterInfo? info = ScanDelimiters(Text[0], StrikethroughProcessor);
			if (info != null)
			{
				Accept(InlineKind.Delimiter, info);
				return;
			}
		}
		// 在 ScanDelimiters 可能会将后续的分隔符一起消费掉。
		AppendLiteralText(Text);
	}

	/// <summary>
	/// 兜底的普通字符的动作。
	/// </summary>
	/// <remarks>需要放在最后，确保优先级最低。</remarks>
	[LexerSymbol(@".", RegexOptions.Singleline)]
	private void CharAction()
	{
		AppendLiteralText(Text);
	}

	/// <summary>
	/// 扫描分隔符。
	/// </summary>
	/// <param name="delimiter">要扫描的分隔符。</param>
	/// <param name="processor">分隔符的处理器。</param>
	/// <returns>扫描得到的分隔符信息。</returns>
	private DelimiterInfo? ScanDelimiters(char delimiter, DelimiterProcessor processor)
	{
		bool beforeIsWhitespace = char.IsWhiteSpace(lastChar);
		bool beforeIsPunctuation = MarkdownUtil.IsPunctuation(lastChar);

		int length = 1;
		char afterChar;
		while ((afterChar = Source.Peek()) == delimiter)
		{
			length++;
			Source.Read();
		}
		// 补充上起始的一个分隔符。
		if (!processor.IsValidLength(length))
		{
			return null;
		}

		bool afterIsWhitespace = char.IsWhiteSpace(afterChar) ||
			afterChar == SourceReader.InvalidCharacter;
		bool afterIsPunctuation = MarkdownUtil.IsPunctuation(afterChar);

		bool leftFlanking = !afterIsWhitespace &&
			(!afterIsPunctuation || beforeIsWhitespace || beforeIsPunctuation);
		bool rightFlanking =
			!beforeIsWhitespace &&
			(!beforeIsPunctuation || afterIsWhitespace || afterIsPunctuation);

		bool canOpen, canClose;
		if (processor.AllowIntraword)
		{
			canOpen = leftFlanking;
			canClose = rightFlanking;
		}
		else
		{
			canOpen = leftFlanking && (!rightFlanking || beforeIsPunctuation);
			canClose = rightFlanking && (!leftFlanking || afterIsPunctuation);
		}
		if (canOpen || canClose)
		{
			return new DelimiterInfo(delimiter, length, canOpen, canClose,
				new Literal(new string(delimiter, length)), processor);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 检查是否包含指定长度的段结束标志，并返回结束标志的位置。
	/// </summary>
	/// <param name="delim">要查找的段结束字符。</param>
	/// <param name="len">要查找的段结束标志的长度。</param>
	/// <returns>段结束标志的位置，使用 <c>-1</c> 表示不存在。</returns>
	private int GetSpansEnd(char delim, int len)
	{
		char ch;
		int closeStart = -1;
		int closeEnd = -1;
		for (int i = 0; ; i++)
		{
			ch = Source.Peek(i);
			if (ch == delim)
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
				closeEnd = -1;
			}
		}
	}

	/// <summary>
	/// 添加指定的字面量字符串。
	/// </summary>
	/// <param name="text">要添加的字面量字符串。</param>
	private void AppendLiteralText(StringView text)
	{
		if (text.Length > 0)
		{
			lastChar = text[^1];
			literalBuffer.Add(text);
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

	/// <summary>
	/// 根据当前词法分析接受结果创建 <see cref="Token{InlineKind}"/> 的新实例。
	/// </summary>
	/// <returns><see cref="Token{InlineKind}"/> 的新实例。</returns>
	protected override Token<InlineKind> CreateToken()
	{
		if (Text.Length > 0)
		{
			lastChar = Text[^1];
		}
		return base.CreateToken();
	}
}
