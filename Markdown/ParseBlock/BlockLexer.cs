using System.Text.RegularExpressions;
using Cyjb.Compilers.Lexers;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 Markdown 的块级元素词法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/"/>
[LexerRegex("WS", "[ \t]")]
[LexerRegex("WS_O", "{WS}*")]
[LexerRegex("WS_P", "{WS}+")]
internal partial class BlockLexer : LexerController<BlockKind>
{
	/// <summary>
	/// 获取解析选项。
	/// </summary>
	private ParseOptions Options => (SharedContext as BlockParser)!.Options;
	/// <summary>
	/// 当前行。
	/// </summary>
	private BlockLine line;
	/// <summary>
	/// 属性的词法分析器。
	/// </summary>
	private LexerTokenizer<AttributeKind>? attributeTokenizer;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
	public BlockLexer()
	{
	}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	/// <summary>
	/// 已加载源读取器。
	/// </summary>
	protected override void SourceLoaded()
	{
		base.SourceLoaded();
		line = new BlockLine(Source.Locator!);
	}

	/// <summary>
	/// 换行的动作。
	/// </summary>
	[LexerSymbol(@"\r|\r?\n", Kind = BlockKind.NewLine)]
	private void NewLineAction()
	{
		AcceptToken();
		(SharedContext as BlockParser)?.ParseLine(line);
		line.Clear();
	}

	/// <summary>
	/// 文件结束的动作。
	/// </summary>
	[LexerSymbol("<<EOF>>")]
	private void EndOfFindAction()
	{
		if (!line.IsEmpty)
		{
			(SharedContext as BlockParser)?.ParseLine(line);
			line.Clear();
		}
	}

	/// <summary>
	/// 通用操作。
	/// </summary>
	[LexerSymbol(@">", Kind = BlockKind.QuoteStart, UseShortest = true)]
	[LexerSymbol(@"\[[ \txX]\]", Kind = BlockKind.TaskListItemMarker, UseShortest = true)]
	private void CommonAction()
	{
		AcceptToken();
	}

	/// <summary>
	/// ATX 标题。
	/// </summary>
	[LexerSymbol(@"#{1,6}", Kind = BlockKind.ATXHeading)]
	private void ATXHeadingAction()
	{
		char nextChar = Source.Peek();
		// ATX 标题需要后跟空白字符或者换行。
		if (nextChar != SourceReader.InvalidCharacter && !MarkdownUtil.IsWhitespace(nextChar))
		{
			// 不是 ATX 标题，取消。
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
			return;
		}
		Heading heading = new(Text.Length, Span);
		Source.ReadLine(false);
		if (Options.UseHeaderAttributes)
		{
			// 解析额外属性。
			int idx = MarkdownUtil.FindAttributeStart(Text);
			if (idx > 0)
			{
				int oldIndex = Source.Index;
				StringView headingText = Text[0..idx];
				// 将索引调整到起始位置之后。
				Source.Index = Start + idx + 1;
				Source.StartIndex = Source.Index;
				if (ReadAttributes(heading.Attributes))
				{
					AcceptToken(headingText, heading);
					ReadNewLine();
					return;
				}
				// 读取失败，恢复索引。
				Source.StartIndex = Start;
				Source.Index = oldIndex;
			}
		}
		// 作为普通 ATX 标题解析。
		AcceptToken(heading);
		ReadNewLine();
	}

	/// <summary>
	/// 文本标题。
	/// </summary>
	[LexerSymbol(@"=+")]
	private void SetextHeadingAction()
	{
		StringView restChars = Source.ReadLine(false);
		if (MarkdownUtil.IsWhitespace(restChars))
		{
			line.Add(BlockKind.SetextUnderline, Text, Span);
		}
		else
		{
			// 包含其它字符，认为是普通文本行。
			line.Add(BlockKind.TextLine, Text, Span);
		}
		ReadNewLine();
	}

	/// <summary>
	/// 代码分隔符起始。
	/// </summary>
	[LexerSymbol(@"~{3,}", Kind = BlockKind.CodeFenceStart)]
	[LexerSymbol(@"`{3,}", Kind = BlockKind.CodeFenceStart)]
	private void CodeFenceStartAction()
	{
		BlockFenceInfo<CodeBlock> info = new(Text[0], Text.Length, new CodeBlock(string.Empty, Span));
		StringView infoText = Source.ReadLine(false);
		if (Text[0] == '`' && infoText.Contains('`'))
		{
			// '`' 分隔符的代码块，要求后面不能包含 '`' 符号，避免与行内代码混淆。
			// 作为普通文本行。
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
			return;
		}
		if (Options.UseCodeAttributes)
		{
			// 解析额外属性。
			int idx = MarkdownUtil.FindAttributeStart(Text);
			if (idx > 0)
			{
				int oldIndex = Source.Index;
				StringView codeText = Text[0..idx];
				// 将索引调整到起始位置之后。
				Source.Index = Start + idx + 1;
				Source.StartIndex = Source.Index;
				if (ReadAttributes(info.Node.Attributes))
				{
					AcceptToken(codeText, info);
					ReadNewLine();
					return;
				}
				// 读取失败，恢复索引。
				Source.StartIndex = Start;
				Source.Index = oldIndex;
			}
		}
		BlockKind kind = Kind!.Value;
		if (MarkdownUtil.IsWhitespace(infoText))
		{
			kind = BlockKind.CodeFence;
		}
		line.Add(kind, Text, Span, info);
		ReadNewLine();
	}
	/// <summary>
	/// 缩进的动作。
	/// </summary>
	[LexerSymbol(@"{WS_P}$", Kind = BlockKind.Indent, UseShortest = true)]
	[LexerSymbol(@"{WS_P}/[^ \t]", Kind = BlockKind.Indent, UseShortest = true)]
	private void IndentAction()
	{
		AcceptToken();
	}
	/// <summary>
	/// 无序列表标志的动作。
	/// </summary>
	[LexerSymbol(@"(-|\+|\*)/{WS}", Kind = BlockKind.UnorderedListMarker, UseShortest = true)]
	[LexerSymbol(@"(-|\+|\*)$", Kind = BlockKind.UnorderedListMarker)]
	private void UnorderedListMarkerAction()
	{
		AcceptToken(ListStyleType.Unordered);
	}

	/// <summary>
	/// 有序数字列表标志的动作。
	/// </summary>
	[LexerSymbol(@"[0-9]{1,9}(\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"[0-9]{1,9}(\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedListMarkerAction()
	{
		AcceptToken(ListStyleType.OrderedNumber);
	}

	/// <summary>
	/// 有序小写字母列表标志的动作。
	/// </summary>
	[LexerSymbol(@"[a-z](\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"[a-z](\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedLowerAlphaListMarkerAction()
	{
		if (Options.UseExtraListStyleType)
		{
			AcceptToken(ListStyleType.OrderedLowerAlpha);
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 有序大写字母列表标志的动作。
	/// </summary>
	[LexerSymbol(@"[A-Z](\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"[A-Z](\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedUpperAlphaListMarkerAction()
	{
		if (Options.UseExtraListStyleType)
		{
			AcceptToken(ListStyleType.OrderedUpperAlpha);
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 有序小写罗马数字列表标志的动作。
	/// </summary>
	[LexerRegex("roman", "m{0,3}(cm|cd|d?c{0,3})(xc|xl|l?x{0,3})(ix|iv|v?i{0,3})")]
	[LexerSymbol(@"{roman}(\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"{roman}(\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedLowerRomanListMarkerAction()
	{
		if (Options.UseExtraListStyleType)
		{
			AcceptToken(ListStyleType.OrderedLowerRoman);
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 有序大写罗马数字列表标志的动作。
	/// </summary>
	[LexerRegex("Roman", "M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})")]
	[LexerSymbol(@"{Roman}(\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"{Roman}(\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedUpperRomanListMarkerAction()
	{
		if (Options.UseExtraListStyleType)
		{
			AcceptToken(ListStyleType.OrderedUpperRoman);
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 有序小写希腊字母列表标志的动作。
	/// </summary>
	[LexerSymbol(@"[α-ω](\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"[α-ω](\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedLowerGreekListMarkerAction()
	{
		if (Options.UseExtraListStyleType)
		{
			AcceptToken(ListStyleType.OrderedLowerGreek);
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 分割线的动作。
	/// </summary>
	/// <remarks>优先级低于 <see cref="BlockKind.UnorderedListMarker"/>。</remarks>
	[LexerSymbol(@"-+{WS_O}$", Kind = BlockKind.DashLine)]
	[LexerSymbol(@"(\*{WS_O}){3,}|(-{WS_O}){3,}|(_{WS_O}){3,}$", Kind = BlockKind.ThematicBreak)]
	private void ThematicBreakAction()
	{
		AcceptToken();
		ReadNewLine();
	}

	/// <summary>
	/// HTML 成对标签的动作。
	/// </summary>
	[LexerSymbol("[<](script|pre|style|textarea)/[ \\t>]", RegexOptions.IgnoreCase, UseShortest = true, Kind = BlockKind.HtmlStart)]
	[LexerSymbol("[<](script|pre|style|textarea)$", RegexOptions.IgnoreCase, UseShortest = true, Kind = BlockKind.HtmlStart)]
	private void HtmlPairAction()
	{
		Source.ReadLine(false);
		AcceptToken(HtmlInfo.HtmlPair);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 注释的动作。
	/// </summary>
	[LexerSymbol("[<]!--", Kind = BlockKind.HtmlStart)]
	private void HtmlCommendAction()
	{
		Source.ReadLine(false);
		AcceptToken(HtmlInfo.HtmlComment);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 处理结构的动作。
	/// </summary>
	[LexerSymbol(@"[<]\?", Kind = BlockKind.HtmlStart)]
	private void HtmlProcessingAction()
	{
		Source.ReadLine(false);
		AcceptToken(HtmlInfo.HtmlProcessing);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 声明的动作。
	/// </summary>
	[LexerSymbol("[<]![a-z]", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlDeclarationAction()
	{
		Source.ReadLine(false);
		AcceptToken(HtmlInfo.HtmlDeclaration);
		ReadNewLine();
	}

	/// <summary>
	/// HTML CDATA 段的动作。
	/// </summary>
	[LexerSymbol(@"[<]!\[CDATA\[", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlCDataAction()
	{
		Source.ReadLine(false);
		AcceptToken(HtmlInfo.HtmlCData);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 单独标签的动作。
	/// </summary>
	[LexerRegex("HtmlSingleTags", "address|article|aside|" +
		"base|basefont|blockquote|body|" +
		"caption|center|col|colgroup|" +
		"dd|details|dialog|dir|div|dl|dt|" +
		"fieldset|figcaption|figure|footer|form|frame|frameset|" +
		"h1|h2|h3|h4|h5|h6|head|header|hr|html|" +
		"iframe|" +
		"legend|li|link|" +
		"main|menu|menuitem|" +
		"nav|noframes|" +
		"ol|optgroup|option|" +
		"p|param|" +
		"search|section|summary|" +
		"table|tbody|td|tfoot|th|thead|title|tr|track|" +
		"ul", RegexOptions.IgnoreCase)]
	[LexerSymbol("[<]\\/?{HtmlSingleTags}/( |\t|\\/?>)", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	[LexerSymbol("[<]\\/?{HtmlSingleTags}>$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlSingleAction()
	{
		Source.ReadLine(false);
		AcceptToken(HtmlInfo.HtmlSingle);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 其它标签的动作。
	/// </summary>
	[LexerRegex("TagName", @"[a-z][a-z0-9-]*", RegexOptions.IgnoreCase)]
	[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
	[LexerRegex("AttrValue", @"[^ \t\r\n""'=<>`']+|'[^'\r\n]*'|\""[^""\r\n]*\""", RegexOptions.IgnoreCase)]
	[LexerSymbol(@"[<]{TagName}({WS_P}{AttrName}({WS_O}={WS_O}{AttrValue})?)*{WS_O}\/?>{WS_O}$", Kind = BlockKind.HtmlStart)]
	[LexerSymbol(@"[<]\/{TagName}{WS_O}>{WS_O}$", Kind = BlockKind.HtmlStart)]
	private void HtmlOtherAction()
	{
		AcceptToken(HtmlInfo.HtmlOther);
		ReadNewLine();
	}

	/// <summary>
	/// 表格分割行的动作。
	/// </summary>
	[LexerRegex("TableDelim", "{WS_O}:?-+:?{WS_O}")]
	[LexerSymbol(@"\|?{TableDelim}(\|{TableDelim})*\|?{WS_O}$", Kind = BlockKind.TableDelimiterRow)]
	private void TableDelimiterRowAction()
	{
		// 要求必须包含至少一个竖划线。
		if (Options.UseTable && Text.Contains('|'))
		{
			AcceptToken();
			ReadNewLine();
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 数学公式分隔符的动作。
	/// </summary>
	[LexerSymbol(@"\${2,}", Kind = BlockKind.MathFenceStart)]
	private void MathFenceAction()
	{
		if (Options.UseMath)
		{
			BlockFenceInfo<MathBlock> info = new(Text[0], Text.Length, new MathBlock(string.Empty, Span));
			StringView infoText = Source.ReadLine(false);
			if (infoText.Contains('$'))
			{
				// 数学公式块中，要求后面不能包含 '$' 符号，避免与行内公式混淆。
				// 作为普通文本行。
				line.Add(BlockKind.TextLine, Text, Span);
				ReadNewLine();
				return;
			}
			if (Options.UseMathAttributes)
			{
				// 解析额外属性。
				int idx = MarkdownUtil.FindAttributeStart(Text);
				if (idx > 0)
				{
					int oldIndex = Source.Index;
					StringView codeText = Text[0..idx];
					// 将索引调整到起始位置之后。
					Source.Index = Start + idx + 1;
					Source.StartIndex = Source.Index;
					if (ReadAttributes(info.Node.Attributes))
					{
						AcceptToken(codeText, info);
						ReadNewLine();
						return;
					}
					// 读取失败，恢复索引。
					Source.StartIndex = Start;
					Source.Index = oldIndex;
				}
			}
			BlockKind kind = Kind!.Value;
			if (MarkdownUtil.IsWhitespace(infoText))
			{
				kind = BlockKind.MathFence;
			}
			line.Add(kind, Text, Span, info);
			ReadNewLine();
		}
		else
		{
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 脚注起始的动作。
	/// </summary>
	[LexerSymbol(@"\[^([^ \t\r\n\[\]]|\\.)+\]:", Kind = BlockKind.FootnoteStart, UseShortest = true)]
	private void FootnoteStartAction()
	{
		if (Options.UseFootnotes)
		{
			AcceptToken(Text[2..^2] as object);
		}
		else
		{
			Source.ReadLine(false);
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 自定义容器分隔符的动作。
	/// </summary>
	[LexerSymbol(@"\:{3,}", Kind = BlockKind.CustomContainerFenceStart)]
	private void CustomContainerFenceAction()
	{
		if (Options.UseCustomContainers)
		{
			BlockFenceInfo<CustomContainer> info = new(Text[0], Text.Length, new CustomContainer(Span));
			StringView infoText = Source.ReadLine(false);
			if (Options.UseCustomContainerAttributes)
			{
				// 解析额外属性。
				int idx = MarkdownUtil.FindAttributeStart(Text);
				if (idx > 0)
				{
					int oldIndex = Source.Index;
					StringView codeText = Text[0..idx];
					// 将索引调整到起始位置之后。
					Source.Index = Start + idx + 1;
					Source.StartIndex = Source.Index;
					if (ReadAttributes(info.Node.Attributes))
					{
						AcceptToken(codeText, info);
						ReadNewLine();
						return;
					}
					// 读取失败，恢复索引。
					Source.StartIndex = Start;
					Source.Index = oldIndex;
				}
			}
			BlockKind kind = Kind!.Value;
			if (MarkdownUtil.IsWhitespace(infoText))
			{
				kind = BlockKind.CustomContainerFence;
			}
			line.Add(kind, Text, Span, info);
			ReadNewLine();
		}
		else
		{
			line.Add(BlockKind.TextLine, Text, Span);
			ReadNewLine();
		}
	}

	/// <summary>
	/// 文本行。
	/// </summary>
	[LexerSymbol(@".", Kind = BlockKind.TextLine)]
	private void TextAction()
	{
		Source.ReadLine(false);
		AcceptToken();
		ReadNewLine();
	}

	/// <summary>
	/// 从源码读取器中读取属性。
	/// </summary>
	/// <param name="attrs">解析后的特性列表。</param>
	/// <returns>如果成功解析属性，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	private bool ReadAttributes(HtmlAttributeList attrs)
	{
		if (attributeTokenizer == null)
		{
			attributeTokenizer = AttributeLexer.Factory.CreateTokenizer();
			attributeTokenizer.Load(Source);
		}
		bool hasSeperator = true;
		while (true)
		{
			var token = attributeTokenizer.Read();
			switch (token.Kind)
			{
				case AttributeKind.Seperator:
					// 检查分隔符是否合法。
					if (!MarkdownUtil.IsValidAttributeSeperator(token.Text))
					{
						goto ParseFailed;
					}
					hasSeperator = true;
					break;
				case AttributeKind.Identifier:
					if (!hasSeperator)
					{
						// 缺少分隔符。
						goto ParseFailed;
					}
					attrs.Id = token.Text.ToString();
					break;
				case AttributeKind.ClassName:
					if (!hasSeperator)
					{
						// 缺少分隔符。
						goto ParseFailed;
					}
					attrs.AddClass(token.Text.ToString());
					break;
				case AttributeKind.Common:
					if (!hasSeperator)
					{
						// 缺少分隔符。
						goto ParseFailed;
					}
					attrs.Add(token.Text.ToString(), (token.Value as string)!);
					break;
				case AttributeKind.End:
					// 解析成功。
					return true;
				default:
					// 解析失败。
					goto ParseFailed;
			}
		}
	ParseFailed:
		// 清除之前可能部分成功的属性。
		attrs.Clear();
		return false;
	}

	/// <summary>
	/// 将当前内容添加到行。
	/// </summary>
	/// <param name="value">当前词法单元的值。</param>
	private void AcceptToken(object? value = null)
	{
		line.Add(Kind!.Value, Text, Span, value);
	}

	/// <summary>
	/// 将当前内容添加到行。
	/// </summary>
	/// <param name="text">当前词法单元的文本。</param>
	/// <param name="value">当前词法单元的值。</param>
	private void AcceptToken(StringView text, object? value = null)
	{
		line.Add(Kind!.Value, text, Span, value);
	}

	/// <summary>
	/// 读取换行。
	/// </summary>
	private void ReadNewLine()
	{
		Source.Drop();
		int spanStart = Source.Index;
		int spanEnd = spanStart + 1;
		char ch = Source.Read();
		if (ch == '\r')
		{
			if (Source.Peek() == '\n')
			{
				Source.Read();
				spanEnd++;
			}
		}
		else if (ch != '\n')
		{
			Source.Unget();
			return;
		}
		line.Add(BlockKind.NewLine, Source.GetReadedText(), new TextSpan(spanStart, spanEnd));
		(SharedContext as BlockParser)?.ParseLine(line);
		line.Clear();
	}
}
