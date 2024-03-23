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
/// <see href="https://spec.commonmark.org/0.31.2/#inlines"/>
[LexerRejectable]
[LexerInclusiveContext("LinkClose")]
[LexerRegex("WS", @"[ \t]*\r?\n?[ \t]*")]
[LexerRegex("WS_1", @"[ \t]+|[ \t]*\r?\n[ \t]*")]
[LexerRegex("TagName", @"[a-z][a-z0-9-]*", RegexOptions.IgnoreCase)]
[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
[LexerRegex("AttrValue", @"[^ \t\r\n""'=<>`']+|'[^']*'|\""[^""]*\""", RegexOptions.IgnoreCase)]
[LexerRegex("ExtAttr", @"\{([^""'<>`'{}]*|'[^'\r\n]*'|\""[^""\r\n]*\"")*\}")]
internal partial class InlineLexer : LexerController<int>
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
	private readonly PooledList<char> literalBuffer = new(0x1000);
	/// <summary>
	/// 当前字面量文本的起始位置。
	/// </summary>
	private int literalStart;
	/// <summary>
	/// 要添加到的行级节点列表。
	/// </summary>
	private NodeList<InlineNode> children;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
	public InlineLexer()
	{
	}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	/// <summary>
	/// 获取当前词法单元的文本范围。
	/// </summary>
	public override TextSpan Span
	{
		get
		{
			TextSpan span = base.Span;
			return new TextSpan(MapLocation(span.Start), MapLocation(span.End));
		}
	}

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
	/// 文件结束的动作。
	/// </summary>
	[LexerSymbol("<*><<EOF>>")]
	private void EndOfFileAction()
	{
		AddLiteral(Span.Start);
	}

	/// <summary>
	/// 硬换行的动作。
	/// </summary>
	[LexerSymbol(@"( {2,}|\\)\r?\n[ \t]*")]
	private void HardBreakAction()
	{
		Add(new Break(true, Span));
	}

	/// <summary>
	/// 软换行的动作。
	/// </summary>
	[LexerSymbol(@" ?\r?\n[ \t]*")]
	private void SoftBreakAction()
	{
		Add(new Break(false, Span));
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
			Add(new CodeSpan(content, Span));
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
	[LexerSymbol(@"[<][a-z][a-z0-9+.-]+:[^\0-\x20\x7F<>]*>{ExtAttr}?", RegexOptions.IgnoreCase)]
	private void AutolinkAction()
	{
		string url = ParseAutolink(out HtmlAttributeList? attrs);
		TextSpan span = Span;
		Link link = new(false, url, null, span);
		if (attrs != null)
		{
			link.Attributes.AddRange(attrs);
		}
		int literalStart = span.Start + 1;
		link.Children.Add(new Literal(url, new TextSpan(literalStart, literalStart + url.Length)));
		Add(link);
	}

	/// <summary>
	/// 邮件自动链接的动作。
	/// </summary>
	[LexerRegex("MailName", "[a-z0-9.!#$%&'*+/=?^_`{|}~-]+", RegexOptions.IgnoreCase)]
	[LexerRegex("MailDomain", "[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?", RegexOptions.IgnoreCase)]
	[LexerSymbol(@"[<]{MailName}@{MailDomain}(\.{MailDomain})*>{ExtAttr}?")]
	private void EmailAutolinkAction()
	{
		string url = ParseAutolink(out HtmlAttributeList? attrs);
		TextSpan span = Span;
		Link link = new(false, "mailto:" + url, null, span);
		if (attrs != null)
		{
			link.Attributes.AddRange(attrs);
		}
		int literalStart = span.Start + 1;
		link.Children.Add(new Literal(url, new TextSpan(literalStart, literalStart + url.Length)));
		Add(link);
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
		ReadOnlySpan<char> attrText = Text.AsSpan(idx + 1);
		int attrLen = attrText.Length;
		if (options.UseLinkAttributes && attrLen > 0 && attrText[0] == '{' && attrText[^1] == '}')
		{
			attrs = MarkdownUtil.ParseAttributes(ref attrText);
			if (attrs == null || attrText.Length > 0)
			{
				// 如果属性解析失败，或者并未正确解析所有属性，那么同样需要回滚属性。
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
	[LexerSymbol(@"(https?:\/\/|www\.).+/<|\s|{EOF}", RegexOptions.IgnoreCase, UseShortest = true)]
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
				TextSpan span = Span;
				Link link = new(false, url, null, span);
				link.Children.Add(new Literal(Text.ToString(), span));
				Add(link);
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
			TextSpan span = Span;
			Link link = new(false, url, null, span);
			link.Children.Add(new Literal(text, span));
			Add(link);
			return;
		}
		Reject(RejectOptions.State);
	}

	/// <summary>
	/// HTML 起始标签的动作。
	/// </summary>
	[LexerSymbol(@"[<]{TagName}({WS_1}{AttrName}({WS}={WS}{AttrValue})?)*{WS}\/?>")]
	private void HtmlStartTagAction()
	{
		Add(new Html(MarkdownKind.HtmlStartTag, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 结束标签的动作。
	/// </summary>
	[LexerSymbol(@"[<]\/{TagName}{WS}>")]
	private void HtmlEndTagAction()
	{
		Add(new Html(MarkdownKind.HtmlEndTag, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 注释的动作。
	/// </summary>
	[LexerSymbol(@"[<]!-->", UseShortest = true)]
	[LexerSymbol(@"[<]!--->", UseShortest = true)]
	[LexerSymbol(@"[<]!---->", UseShortest = true)]
	[LexerSymbol(@"[<]!--.*-->", RegexOptions.Singleline, UseShortest = true)]
	private void HtmlCommentAction()
	{
		Add(new Html(MarkdownKind.HtmlComment, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 处理结构的动作。
	/// </summary>
	[LexerSymbol(@"[<]\?.*\?>", RegexOptions.Singleline, UseShortest = true)]
	private void HtmlProcessingAction()
	{
		Add(new Html(MarkdownKind.HtmlProcessing, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML 声明的动作。
	/// </summary>
	[LexerSymbol(@"[<]![a-z][^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
	private void HtmlDeclarationAction()
	{
		Add(new Html(MarkdownKind.HtmlDeclaration, Text.ToString(), Span));
	}

	/// <summary>
	/// HTML CDATA 的动作。
	/// </summary>
	[LexerSymbol(@"[<]!\[CDATA\[.*\]\]>", RegexOptions.IgnoreCase | RegexOptions.Singleline, UseShortest = true)]
	private void HtmlCDataAction()
	{
		Add(new Html(MarkdownKind.HtmlCData, Text.ToString(), Span));
	}

	/// <summary>
	/// 链接标签的动作。
	/// </summary>
	[LexerRegex("LinkLabel", @"\[([^\\\[\]]|\\.)*\]")]
	[LexerSymbol(@"<LinkClose>]{LinkLabel}")]
	private void LinkLabelAction()
	{
		InlineParser parser = (InlineParser)SharedContext!;
		// 标签不为空，使用标签本身。
		ReadOnlySpan<char> text = Text.AsSpan(2);
		ReadOnlySpan<char> label = ReadOnlySpan<char>.Empty;
		if (!MarkdownUtil.TryParseLinkLabel(ref text, ref label))
		{
			Reject();
			return;
		}
		if (label.Length == 0)
		{
			// 标签为空，将前面的文本作为标签使用。
			label = parser.GetCurrentLinkText(Start);
		}
		if (!label.IsEmpty && parser.TryGetLinkDefine(LinkUtil.NormalizeLabel(label), out LinkDefinition? define))
		{
			TextSpan span = Span;
			AddLiteral(span.Start);
			if (parser.ParseLinkDefinition(define, span))
			{
				// 调整字面量起始位置。
				literalStart = span.End;
			}
			else
			{
				// 链接匹配失败，作为字面量字符添加。
				AppendLiteralText(Text);
			}
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
	[LexerSymbol(@"<LinkClose>]\({WS}{LinkDestination}?({WS_1}{LinkTitle})?{WS}\){ExtAttr}?")]
	private void LinkBodyAction()
	{
		ReadOnlySpan<char> text = Text.AsSpan(2);
		text = text.TrimStart(MarkdownUtil.Whitespace);
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
		text = text.TrimStart(MarkdownUtil.Whitespace);
		// 链接目标和标题要求以 ) 闭合。
		if (text.IsEmpty || text[0] != ')')
		{
			Reject();
			return;
		}
		text = text.Slice(1);
		LinkBody body = new(destination, title);
		// 解析属性。
		int attrLen = text.Length;
		if (options.UseLinkAttributes && attrLen > 0 && text[0] == '{' && text[^1] == '}')
		{
			HtmlAttributeList? attrs = MarkdownUtil.ParseAttributes(ref text);
			if (attrs == null || text.Length > 0)
			{
				// 如果属性解析失败，或者并未正确解析所有属性，那么同样需要回滚属性。
				Source.Index -= attrLen;
			}
			else
			{
				// 复制 HTML 属性。
				if (attrs?.Count > 0)
				{
					body.Attributes.AddRange(attrs);
				}
			}
		}
		else
		{
			Source.Index -= attrLen;
		}
		InlineParser parser = (InlineParser)SharedContext!;
		TextSpan span = Span;
		AddLiteral(span.Start);
		if (parser.ParseLinkBody(body, span))
		{
			// 调整字面量起始位置。
			literalStart = span.End;
		}
		else
		{
			// 链接匹配失败，作为字面量字符添加。
			AppendLiteralText(Text);
		}
	}

	/// <summary>
	/// 表情符号的动作。
	/// </summary>
	/// <remarks>目前最长的 emoji 名称长度为 36。</remarks>
	[LexerSymbol(@":.{1,36}:", UseShortest = true)]
	private void EmojiAction()
	{
		if (options.UseEmoji)
		{
			// 比较时忽略前后的 :。
			Emoji? emoji = Emoji.GetEmoji(Text[1..^1], Span);
			if (emoji != null)
			{
				Add(emoji);
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
			Add(new MathSpan(content, Span));
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
	[LexerSymbol(@"\[")]
	[LexerSymbol(@"!\[")]
	private void LinkStartAction()
	{
		bool isImage = Text.Length > 1;
		Literal node = new(Text.ToString(), Span);
		Add(node);
		(SharedContext as InlineParser)!.AddBracket(node, isImage);
		// 允许识别链接闭合。
		EnterContext(LinkCloseContext);
	}

	/// <summary>
	/// 链接结束的动作。
	/// </summary>
	/// <remarks>这里需要负责闭合非活动状态的左中括号，因此不能限定在 LinkClose 上下文。</remarks>
	[LexerSymbol(@"]")]
	private void LinkCloseAction()
	{
		TextSpan span = Span;
		AddLiteral(span.Start);
		if ((SharedContext as InlineParser)!.ParseCloseBracket(span))
		{
			// 链接匹配成功，调整字面量起始位置。
			literalStart = span.End;
		}
		else
		{
			// 链接匹配失败，作为字面量字符添加。
			AppendLiteralText(Text);
		}
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
			Add(info.Node);
			(SharedContext as InlineParser)!.AddDelimiter(info);
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
				Add(info.Node);
				(SharedContext as InlineParser)!.AddDelimiter(info);
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
				new Literal(new string(delimiter, length), Span), processor);
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
	/// 添加字面量节点。
	/// </summary>
	/// <param name="end">字面量节点的结束位置。</param>
	private void AddLiteral(int end)
	{
		string text = GetLiteralText();
		if (text.Length > 0)
		{
			children.Add(new Literal(text, new TextSpan(literalStart, end)));
		}
		literalStart = end;
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
	/// 重新加载源读取器。
	/// </summary>
	protected override void SourceLoaded()
	{
		base.SourceLoaded();
		lastChar = '\n';
		literalStart = MapLocation(0);
		children = (SharedContext as InlineParser)!.children;
	}

	/// <summary>
	/// 映射指定的位置。
	/// </summary>
	/// <param name="location">要映射的位置。</param>
	/// <returns>映射后的位置。</returns>
	private int MapLocation(int location)
	{
		return ((InlineParser)SharedContext!).MapLocation(location);
	}

	/// <summary>
	/// 添加指定的内联节点。
	/// </summary>
	/// <param name="node">要添加的内联节点。</param>
	private void Add(InlineNode node)
	{
		AddLiteral(node.Span.Start);
		children.Add(node);
		literalStart = node.Span.End;
		if (Text.Length > 0)
		{
			lastChar = Text[^1];
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			literalBuffer.Dispose();
		}
		base.Dispose(disposing);
	}
}
