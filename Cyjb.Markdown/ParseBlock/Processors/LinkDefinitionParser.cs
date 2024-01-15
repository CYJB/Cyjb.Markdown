using System.Text;
using Cyjb.Text;
using Cyjb.Markdown.Utils;
using Cyjb.Markdown.Syntax;
using Cyjb.Collections;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 链接定义的解析器。
/// </summary>
internal sealed class LinkDefinitionParser
{
	/// <summary>
	/// 解析状态。
	/// </summary>
	private enum State
	{
		/// <summary>
		/// 寻找链接定义的起始（<c>[</c>）。
		/// </summary>
		StartDefinition,
		/// <summary>
		/// 解析链接定义的标签（<c>[foo]</c> 中的 <c>foo</c>）。
		/// </summary>
		Label,
		/// <summary>
		/// 解析链接目标（<c>[foo]: /url</c> 中的 <c>/url</c>）。
		/// </summary>
		Destination,
		/// <summary>
		/// 解析标题或属性的起始（<c>[foo]: /url "title"</c> 中的首个 <c>"</c>，
		/// 或者 <c>[foo]: /url {#id}</c> 中的首个 <c>{</c>）。
		/// </summary>
		StartTitleOrAttr,
		/// <summary>
		/// 解析标题内容（<c>[foo]: /url "title"</c> 中的 <c>title</c>）。
		/// </summary>
		Title,
		/// <summary>
		/// 解析链接属性（<c>[foo]: /url "title" {#id}</c> 中的首个 <c>{</c>）。
		/// </summary>
		StartAttributes,
		/// <summary>
		/// 解析链接属性（<c>[foo]: /url {#id}</c> 中的 <c>#id}</c>）。
		/// </summary>
		Attributes,
		/// <summary>
		/// 解析链接属性值（<c>[foo]: /url {key="value"}</c> 中的 <c>value"</c>）。
		/// </summary>
		AttributeValue,
		/// <summary>
		/// 解析失败，无法继续解析链接定义。
		/// </summary>
		Failed,
	}

	/// <summary>
	/// 找到的所有链接定义。
	/// </summary>
	private readonly List<LinkDefinition> definitions = new();
	/// <summary>
	/// 字符串构造器。
	/// </summary>
	private readonly PooledList<char> builder = new();
	/// <summary>
	/// 解析选项。
	/// </summary>
	private readonly ParseOptions options;
	/// <summary>
	/// 清除之前行的动作。
	/// </summary>
	private readonly Action clearLines;
	/// <summary>
	/// 当前解析状态。
	/// </summary>
	private State state = State.StartDefinition;
	/// <summary>
	/// 是否已找到了链接定义。
	/// </summary>
	private bool hasDefinition;
	/// <summary>
	/// 链接的标签。
	/// </summary>
	private string? label;
	/// <summary>
	/// 链接的目标。
	/// </summary>
	private string? destination;
	/// <summary>
	/// 链接的标题。
	/// </summary>
	private string? title;
	/// <summary>
	/// 链接的标题结束字符。
	/// </summary>
	private char titleEnd;
	/// <summary>
	/// 属性列表。
	/// </summary>
	private readonly HtmlAttributeList attributes = new();
	/// <summary>
	/// 属性解析结果。
	/// </summary>
	private AttributeParseResult? attributeResult;
	/// <summary>
	/// 链接定义块的起始范围。
	/// </summary>
	private int start;
	/// <summary>
	/// 链接定义块的结束范围。
	/// </summary>
	private int end;

	/// <summary>
	/// 使用指定的解析选项初始化 <see cref="LinkDefinitionParser"/> 类的新实例。
	/// </summary>
	/// <param name="options">解析选项。</param>
	/// <param name="clearLines">清除之前行的动作。</param>
	public LinkDefinitionParser(ParseOptions options, Action clearLines)
	{
		this.options = options;
		this.clearLines = clearLines;
	}

	/// <summary>
	/// 获取是否可以继续解析链接定义。
	/// </summary>
	public bool CanContinue => state != State.Failed;

	/// <summary>
	/// 解析指定的行。
	/// </summary>
	/// <param name="text">行的文本。</param>
	public void Parse(MappedText text)
	{
		ValueList<char> list = new(stackalloc char[ValueList.StackallocCharSizeLimit]);
		text.AppendTo(ref list);
		ReadOnlySpan<char> textSpan = list.AsSpan();
		while (!textSpan.IsEmpty)
		{
			bool success = false;
			switch (state)
			{
				case State.StartDefinition:
					success = ParseStartDefinition(ref textSpan, text.Span);
					break;
				case State.Label:
					success = ParseLabel(ref textSpan);
					break;
				case State.Destination:
					success = ParseDestination(ref textSpan, text.Span);
					break;
				case State.StartTitleOrAttr:
					success = ParseStartTitleOrAttr(ref textSpan);
					break;
				case State.Title:
					success = ParseTitle(ref textSpan, text.Span);
					break;
				case State.StartAttributes:
					success = ParseStartAttribute(ref textSpan);
					break;
				case State.Attributes:
					success = ParseAttributes(ref textSpan, text.Span);
					break;
				case State.AttributeValue:
					success = ParseAttributeValue(ref textSpan, text.Span);
					break;
			}
			if (!success)
			{
				state = State.Failed;
				list.Dispose();
				return;
			}
		}
		list.Dispose();
	}

	/// <summary>
	/// 返回所有解析的链接定义。
	/// </summary>
	/// <returns>链接定义列表。</returns>
	public List<LinkDefinition> GetDefinitions()
	{
		CreateDefinition();
		return definitions;
	}

	/// <summary>
	/// 解析链接定义的起始。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="span">行的范围。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseStartDefinition(ref ReadOnlySpan<char> text, TextSpan span)
	{
		MarkdownUtil.TrimStart(ref text);
		if (text.Length == 0 || text[0] != '[')
		{
			return false;
		}
		else
		{
			start = span.End - text.Length;
			text = text[1..];
			state = State.Label;
			builder.Clear();
			return true;
		}
	}

	/// <summary>
	/// 解析链接定义的标签。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseLabel(ref ReadOnlySpan<char> text)
	{
		// 链接标签中不允许出现未转义的 [。
		int idx = text.IndexOfUnescaped(']', '[');
		if (idx == -2)
		{
			return false;
		}
		else if (idx < 0)
		{
			idx = text.Length;
		}
		builder.Add(text[..idx]);
		text = text[idx..];
		if (text.Length == 0)
		{
			return true;
		}
		else if (text.Length < 2 || text[1] != ':')
		{
			// 之前已经保证了 text[0] 一定是 ]，需要确保后跟 :。
			return false;
		}
		// 链接标签最多只能包含 999 个字符。
		if (builder.Length > 999)
		{
			return false;
		}
		// 链接定义中不允许使用空白的标签。
		ReadOnlySpan<char> labelSpan = builder.AsSpan();
		MarkdownUtil.TrimStart(ref labelSpan);
		if (labelSpan.Length == 0)
		{
			return false;
		}
		label = builder.ToString();
		text = text[2..];
		state = State.Destination;
		// 移除后面的空白，避免空白字符串进入到解析链接定义的流程中。
		MarkdownUtil.TrimStart(ref text);
		return true;
	}

	/// <summary>
	/// 解析链接定义的目标。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="span">行的范围。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseDestination(ref ReadOnlySpan<char> text, TextSpan span)
	{
		MarkdownUtil.TrimStart(ref text);
		// 解析链接目标，在定义中不允许使用空目标。
		if (!MarkdownUtil.TryParseLinkDestination(ref text, out destination) || destination == null)
		{
			return false;
		}
		bool hasWhitespace = MarkdownUtil.TrimStart(ref text);
		if (text.Length == 0)
		{
			// 链接目标在行尾，当前已经是一个有效的链接定义。
			// 之后需要继续检查可能的链接标题。
			hasDefinition = true;
			clearLines();
		}
		else if (!hasWhitespace)
		{
			// 链接定义的目标和标题间需要使用空白分割。
			return false;
		}
		// 将空白也包含在链接定义范围内。
		end = span.End - text.Length;
		state = State.StartTitleOrAttr;
		title = null;
		// 移除后面的空白，避免空白字符串进入到解析链接标题起始的流程中。
		MarkdownUtil.TrimStart(ref text);
		return true;
	}

	/// <summary>
	/// 解析链接标题或属性的起始。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseStartTitleOrAttr(ref ReadOnlySpan<char> text)
	{
		MarkdownUtil.TrimStart(ref text);
		if (text.Length == 0)
		{
			state = State.StartDefinition;
			return true;
		}
		char ch = text[0];
		switch (ch)
		{
			case '"':
			case '\'':
				titleEnd = ch;
				break;
			case '(':
				titleEnd = ')';
				break;
			case '{':
				if (options.UseLinkAttributes)
				{
					EnterAttribute(ref text);
					return true;
				}
				else
				{
					goto default;
				}
			default:
				CreateDefinition();
				// 尝试继续解析新的链接定义。
				state = State.StartDefinition;
				return true;
		}
		state = State.Title;
		builder.Clear();
		text = text[1..];
		return true;
	}

	/// <summary>
	/// 进入解析属性的状态。
	/// </summary>
	private void EnterAttribute(ref ReadOnlySpan<char> text)
	{
		text = text[1..];
		// 移除掉行首的空白，避免被识别为空行。
		MarkdownUtil.TrimStart(ref text);
		state = State.Attributes;
		attributes.Clear();
	}

	/// <summary>
	/// 解析链接标题。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="span">行的范围。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseTitle(ref ReadOnlySpan<char> text, TextSpan span)
	{
		int idx;
		// 使用 ) 作为结束符时，不允许出现未转义的 (。
		if (titleEnd == ')')
		{
			idx = text.IndexOfUnescaped(')', '(');
			if (idx == -2)
			{
				return false;
			}
		}
		else
		{
			idx = text.IndexOfUnescaped(titleEnd);
		}
		if (idx < 0)
		{
			idx = text.Length;
		}
		builder.Add(text[..idx]);
		text = text[idx..];
		if (text.Length == 0)
		{
			return true;
		}
		// 跳过结束分隔符。
		text = text[1..];
		bool hasWhitespace = MarkdownUtil.TrimStart(ref text);
		if (text.Length == 0)
		{
			// 标题已经完整，后面即使有属性也在下一行。
			hasDefinition = true;
			// 将空白也包含在链接定义范围内。
			end = span.End;
			if (builder.Length > 0)
			{
				title = MarkdownUtil.Unescape(builder.AsSpan());
			}
			clearLines();
			if (options.UseLinkAttributes)
			{
				// 后面继续尝试解析属性。
				state = State.StartAttributes;
			}
			else
			{
				CreateDefinition();
				// 后面继续尝试解析链接定义。
				state = State.StartDefinition;
			}
			return true;
		}
		else if (options.UseLinkAttributes && text[0] == '{')
		{
			if (builder.Length > 0)
			{
				title = MarkdownUtil.Unescape(builder.AsSpan());
			}
			// 继续解析属性，如果属性解析失败，标题也不被识别。
			EnterAttribute(ref text);
			return true;
		}
		else
		{
			// 在链接标题后不允许出现其它非空白字符，且不是属性的起始。
			return false;
		}
	}

	/// <summary>
	/// 解析链接属性的起始。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseStartAttribute(ref ReadOnlySpan<char> text)
	{
		MarkdownUtil.TrimStart(ref text);
		if (text.Length > 0 && text[0] == '{')
		{
			EnterAttribute(ref text);
			MarkdownUtil.TrimStart(ref text);
		}
		else
		{
			CreateDefinition();
			// 尝试继续解析新的链接定义。
			state = State.StartDefinition;
		}
		return true;
	}

	/// <summary>
	/// 解析链接属性。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="span">行的范围。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseAttributes(ref ReadOnlySpan<char> text, TextSpan span)
	{
		MarkdownUtil.TrimStart(ref text);
		if (text.Length == 0)
		{
			// 属性解析过程中的空白行会导致链接解析失败。
			return false;
		}
		AttributeParseResult result = MarkdownUtil.TryParseAttribute(ref text, attributes);
		if (!result.IsSuccess && result.IsMissingQuote)
		{
			attributeResult = result;
			state = State.AttributeValue;
			text = ReadOnlySpan<char>.Empty;
			return true;
		}
		else
		{
			return ParseAttributeEnd(ref text, span);
		}
	}

	/// <summary>
	/// 解析链接属性值。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="span">行的范围。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseAttributeValue(ref ReadOnlySpan<char> text, TextSpan span)
	{
		int idx = text.IndexOf(attributeResult!.QuoteChar);
		if (idx < 0)
		{
			attributeResult!.Value += text.ToString();
			text = ReadOnlySpan<char>.Empty;
			return true;
		}
		attributeResult!.Value += text[0..idx].ToString();
		text = text[(idx + 1)..];
		attributes[attributeResult.Key] = attributeResult.Value;
		return ParseAttributeEnd(ref text, span);
	}
	/// <summary>
	/// 解析属性的结束。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <param name="span">行的范围。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseAttributeEnd(ref ReadOnlySpan<char> text, TextSpan span)
	{
		if (!MarkdownUtil.TrimStart(ref text) && text.Length > 0 && text[0] != '}')
		{
			// 属性间缺少分隔符，或错误的属性结束字符。
			// 如果有 title，也不能正常使用。
			title = null;
			attributes.Clear();
			return false;
		}
		if (text.Length > 0 && text[0] == '}')
		{
			// 属性解析结束，要求属性后不能有其它非空白字符。
			text = text[1..];
			MarkdownUtil.TrimStart(ref text);
			if (text.Length > 0)
			{
				// 后面有非空白字符，清理失效数据。
				title = null;
				attributes.Clear();
				return false;
			}
			end = span.End;
			hasDefinition = true;
			clearLines();
			CreateDefinition();
			// 后面继续尝试解析链接定义。
			state = State.StartDefinition;
		}
		else
		{
			// 后面按照普通属性解析。
			state = State.Attributes;
		}
		return true;
	}

	/// <summary>
	/// 将现有信息创建链接定义。
	/// </summary>
	private void CreateDefinition()
	{
		if (hasDefinition)
		{
			LinkDefinition definition = new(label!, destination!, title, new TextSpan(start, end));
			definition.Attributes.AddRange(attributes);
			definitions.Add(definition);
			hasDefinition = false;
			attributes.Clear();
		}
	}
}
