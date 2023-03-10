using System.Text.RegularExpressions;
using Cyjb.Compilers.Lexers;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示 Markdown 的块级元素词法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/"/>
[LexerRejectable]
[LexerRegex("WS", "[ \t]")]
[LexerRegex("WS_1", @"[ \t]+|[ \t]*\r?\n[ \t]*")]
[LexerRegex("WS_O", "{WS}*")]
[LexerRegex("WS_P", "{WS}+")]
[LexerRegex("AttrName", @"[a-z_:][a-z0-9_.:-]*", RegexOptions.IgnoreCase)]
[LexerRegex("ExtAttr", @"\{([^""'<>`'{}]*|'[^'\r\n]*'|\""[^""\r\n]*\"")*\}{WS_O}")]
[LexerSymbol(@"\r|\r?\n", Kind = BlockKind.NewLine)]
[LexerSymbol(@">", Kind = BlockKind.QuoteStart, UseShortest = true)]
[LexerSymbol(@"`{3,}{WS_O}$", Kind = BlockKind.CodeFence)]
[LexerSymbol(@"~{3,}{WS_O}$", Kind = BlockKind.CodeFence)]
[LexerSymbol(@"=+{WS_O}$", Kind = BlockKind.SetextUnderline)]
[LexerSymbol(@"\[[ \txX]\]", Kind = BlockKind.TaskListItemMarker, UseShortest = true)]
internal partial class BlockLexer : LexerController<BlockKind>
{
	/// <summary>
	/// 获取解析选项。
	/// </summary>
	private ParseOptions Options => (ParseOptions)SharedContext!;

	/// <summary>
	/// 带有额外属性的 ATX 标题。
	/// </summary>
	[LexerSymbol(@"#{1,6}({WS_P}.*)?{ExtAttr}$", Kind = BlockKind.ATXHeading)]
	private void ATXHeadingWithAttributesAction()
	{
		if (Options.UseHeaderAttributes)
		{
			ReadOnlySpan<char> text = Text;
			HtmlAttributeList attrs = new();
			if (MarkdownUtil.TryParseAttributes(ref text, attrs))
			{
				Text = text.ToString();
				Accept(attrs);
				return;
			}
		}
		Reject(RejectOptions.State);
	}
	/// <summary>
	/// 普通 ATX 标题。
	/// </summary>
	[LexerSymbol(@"#{1,6}({WS_P}.*)?$", Kind = BlockKind.ATXHeading)]
	private void ATXHeadingAction()
	{
		Accept();
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
			ReadOnlySpan<char> text = Text;
			HtmlAttributeList attrs = new();
			if (MarkdownUtil.TryParseAttributes(ref text, attrs))
			{
				Text = text.ToString();
				Accept(attrs);
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
		Accept();
	}
	/// <summary>
	/// 缩进的动作。
	/// </summary>
	[LexerSymbol(@"{WS_P}$", Kind = BlockKind.Indent, UseShortest = true)]
	[LexerSymbol(@"{WS_P}/[^ \t]", Kind = BlockKind.Indent, UseShortest = true)]
	private void IndentAction()
	{
		Accept(new IndentInfo(Span, Source.Locator!, Text));
	}
	/// <summary>
	/// 无序列表标志的动作。
	/// </summary>
	[LexerSymbol(@"(-|\+|\*)/{WS}", Kind = BlockKind.UnorderedListMarker, UseShortest = true)]
	[LexerSymbol(@"(-|\+|\*)$", Kind = BlockKind.UnorderedListMarker)]
	private void UnorderedListMarkerAction()
	{
		Accept(ListStyleType.Unordered);
	}

	/// <summary>
	/// 有序数字列表标志的动作。
	/// </summary>
	[LexerSymbol(@"[0-9]{1,9}(\.|\))/{WS}", Kind = BlockKind.OrderedListMarker, UseShortest = true)]
	[LexerSymbol(@"[0-9]{1,9}(\.|\))$", Kind = BlockKind.OrderedListMarker)]
	private void OrderedListMarkerAction()
	{
		Accept(ListStyleType.OrderedNumber);
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
			Accept(ListStyleType.OrderedLowerAlpha);
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
			Accept(ListStyleType.OrderedUpperAlpha);
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
			Accept(ListStyleType.OrderedLowerRoman);
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
			Accept(ListStyleType.OrderedUpperRoman);
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
			Accept(ListStyleType.OrderedLowerGreek);
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
		Accept();
	}

	/// <summary>
	/// HTML 成对标签的动作。
	/// </summary>
	[LexerSymbol("[<](script|pre|style|textarea)([ \t>].*)?$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlPairAction()
	{
		Accept(HtmlInfo.HtmlPair);
	}

	/// <summary>
	/// HTML 注释的动作。
	/// </summary>
	[LexerSymbol("[<]!--.*$", Kind = BlockKind.HtmlStart)]
	private void HtmlCommendAction()
	{
		Accept(HtmlInfo.HtmlComment);
	}

	/// <summary>
	/// HTML 处理结构的动作。
	/// </summary>
	[LexerSymbol(@"[<]\?.*$", Kind = BlockKind.HtmlStart)]
	private void HtmlProcessingAction()
	{
		Accept(HtmlInfo.HtmlProcessing);
	}

	/// <summary>
	/// HTML 声明的动作。
	/// </summary>
	[LexerSymbol("[<]![a-z].*$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlDeclarationAction()
	{
		Accept(HtmlInfo.HtmlDeclaration);
	}

	/// <summary>
	/// HTML CDATA 段的动作。
	/// </summary>
	[LexerSymbol(@"[<]!\[CDATA\[.*$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlCDataAction()
	{
		Accept(HtmlInfo.HtmlCData);
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
		"section|source|summary|" +
		"table|tbody|td|tfoot|th|thead|title|tr|track|" +
		"ul" +
		")(( |\t|\\/?>).*)?$", RegexOptions.IgnoreCase, Kind = BlockKind.HtmlStart)]
	private void HtmlSingleAction()
	{
		Accept(HtmlInfo.HtmlSingle);
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
		Accept(HtmlInfo.HtmlOther);
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
			Accept();
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
			Accept();
		}
		else
		{
			Reject();
		}
	}

	/// <summary>
	/// 文本行。
	/// </summary>
	[LexerSymbol(@".+$", Kind = BlockKind.TextLine)]
	private void TextAction()
	{
		Accept(Text);
	}
}
