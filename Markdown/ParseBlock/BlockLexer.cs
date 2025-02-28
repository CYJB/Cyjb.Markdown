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
[LexerRejectable]
[LexerRegex("WS", "[ \t]")]
[LexerRegex("WS_1", @"[ \t]+|[ \t]*\r?\n[ \t]*")]
[LexerRegex("WS_O", "{WS}*")]
[LexerRegex("WS_P", "{WS}+")]
[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
[LexerRegex("ExtAttr", @"\{([^""'<>`'{}]*|'[^'\r\n]*'|\""[^""\r\n]*\"")*\}{WS_O}")]
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
	[LexerSymbol(@"`{3,}{WS_O}$", Kind = BlockKind.CodeFence)]
	[LexerSymbol(@"~{3,}{WS_O}$", Kind = BlockKind.CodeFence)]
	[LexerSymbol(@"=+{WS_O}$", Kind = BlockKind.SetextUnderline)]
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
			Reject(RejectOptions.State);
			return;
		}
		Heading heading = new(Text.Length, Span);
		if (!Text.TryConcat(Source.ReadLine(false), out StringView text))
		{
			// 应该是一定会保证 Text 与行后续内容是可以拼接起来的。
			throw CommonExceptions.Unreachable();
		}
		if (Options.UseHeaderAttributes)
		{
			// 解析额外属性。
			int idx = MarkdownUtil.FindAttributeStart(text);
			if (idx > 0)
			{
				int oldIndex = Source.Index;
				StringView headingText = text[0..idx];
				// 将索引调整到起始位置之后。
				Source.Index = Start + idx + 1;
				Source.StartIndex = Source.Index;
				if (MarkdownUtil.ReadAttributes(Source, heading.Attributes))
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
		AcceptToken(text, heading);
		ReadNewLine();
	}
	/// <summary>
	/// 带有额外属性的代码分隔符起始。
	/// </summary>
	[LexerSymbol(@"`{3,}[^`\r\n]+{ExtAttr}$", Kind = BlockKind.CodeFenceStart)]
	[LexerSymbol(@"~{3,}.+{ExtAttr}$", Kind = BlockKind.CodeFenceStart)]
	private void CodeFenceStartWithAttributesAction()
	{
		if (Options.UseCodeAttributes)
		{
			StringView text = Text;
			HtmlAttributeList? attrs = MarkdownUtil.ParseAttributes(ref text);
			if (attrs != null)
			{
				AcceptToken(text, attrs);
				ReadNewLine();
				return;
			}
		}
		Reject(RejectOptions.State);
	}
	/// <summary>
	/// 普通代码分隔符起始。
	/// </summary>
	[LexerSymbol(@"`{3,}[^`\r\n]+$", Kind = BlockKind.CodeFenceStart)]
	[LexerSymbol(@"~{3,}.+$", Kind = BlockKind.CodeFenceStart)]
	private void CodeFenceStartAction()
	{
		AcceptToken();
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
			Reject();
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
			Reject();
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
			Reject();
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
			Reject();
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
			Reject();
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
	[LexerSymbol("[<](script|pre|style|textarea)([ \t>].*)?$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlPairAction()
	{
		AcceptToken(HtmlInfo.HtmlPair);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 注释的动作。
	/// </summary>
	[LexerSymbol("[<]!--.*$", Kind = BlockKind.HtmlStart)]
	private void HtmlCommendAction()
	{
		AcceptToken(HtmlInfo.HtmlComment);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 处理结构的动作。
	/// </summary>
	[LexerSymbol(@"[<]\?.*$", Kind = BlockKind.HtmlStart)]
	private void HtmlProcessingAction()
	{
		AcceptToken(HtmlInfo.HtmlProcessing);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 声明的动作。
	/// </summary>
	[LexerSymbol("[<]![a-z].*$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlDeclarationAction()
	{
		AcceptToken(HtmlInfo.HtmlDeclaration);
		ReadNewLine();
	}

	/// <summary>
	/// HTML CDATA 段的动作。
	/// </summary>
	[LexerSymbol(@"[<]!\[CDATA\[.*$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlCDataAction()
	{
		AcceptToken(HtmlInfo.HtmlCData);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 单独标签的动作。
	/// </summary>
	[LexerSymbol("[<]\\/?(" +
		"address|article|aside|" +
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
		"ul" +
		")(( |\t|\\/?>).*)?$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlSingleAction()
	{
		AcceptToken(HtmlInfo.HtmlSingle);
		ReadNewLine();
	}

	/// <summary>
	/// HTML 其它标签的动作。
	/// </summary>
	[LexerRegex("TagName", @"[a-z][a-z0-9-]*", RegexOptions.IgnoreCase)]
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
			Reject();
		}
	}

	/// <summary>
	/// 数学公式分隔符的动作。
	/// </summary>
	[LexerSymbol(@"\${2,}{WS_O}$", Kind = BlockKind.MathFence)]
	[LexerSymbol(@"\${2,}[^$\r\n]+$", Kind = BlockKind.MathFenceStart)]
	private void MathFenceAction()
	{
		if (Options.UseMath)
		{
			AcceptToken();
			ReadNewLine();
		}
		else
		{
			Reject(RejectOptions.State);
		}
	}

	/// <summary>
	/// 带有额外属性的数学公式分隔符起始。
	/// </summary>
	[LexerSymbol(@"\${2,}[^$\r\n]+{ExtAttr}$", Kind = BlockKind.MathFenceStart)]
	private void MathFenceStartWithAttributesAction()
	{
		if (Options.UseMath && Options.UseMathAttributes)
		{
			StringView text = Text;
			HtmlAttributeList? attrs = MarkdownUtil.ParseAttributes(ref text);
			if (attrs != null)
			{
				AcceptToken(text, attrs);
				ReadNewLine();
				return;
			}
		}
		Reject(RejectOptions.State);
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
			Reject(RejectOptions.State);
		}
	}

	/// <summary>
	/// 自定义容器分隔符的动作。
	/// </summary>
	[LexerSymbol(@"\:{3,}{WS_O}$", Kind = BlockKind.CustomContainerFence)]
	[LexerSymbol(@"\:{3,}.+$", Kind = BlockKind.CustomContainerFenceStart)]
	private void CustomContainerFenceAction()
	{
		if (Options.UseCustomContainers)
		{
			AcceptToken();
			ReadNewLine();
		}
		else
		{
			Reject(RejectOptions.State);
		}
	}

	/// <summary>
	/// 带有额外属性的自定义容器分隔符起始。
	/// </summary>
	[LexerSymbol(@":{3,}.+{ExtAttr}$", Kind = BlockKind.CustomContainerFenceStart)]
	private void CustomContainerFenceStartWithAttributesAction()
	{
		if (Options.UseCustomContainers && Options.UseCustomContainerAttributes)
		{
			StringView text = Text;
			HtmlAttributeList? attrs = MarkdownUtil.ParseAttributes(ref text);
			if (attrs != null)
			{
				AcceptToken(text, attrs);
				ReadNewLine();
				return;
			}
		}
		Reject(RejectOptions.State);
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
