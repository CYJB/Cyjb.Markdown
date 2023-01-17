using System.Text;
using Cyjb.Text;
using Cyjb.Markdown.Utils;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.Parse.Blocks;

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
		/// 解析标题的起始（<c>[foo]: /url "title"</c> 中的首个 <c>"</c>）。
		/// </summary>
		StartTitle,
		/// <summary>
		/// 解析标题内容（<c>[foo]: /url "title"</c> 中的 <c>title</c>）。
		/// </summary>
		Title,
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
	private readonly StringBuilder builder = new();
	/// <summary>
	/// 当前解析状态。
	/// </summary>
	private State state = State.StartDefinition;
	/// <summary>
	/// 是否已找到了链接定义。
	/// </summary>
	private bool hasDefinition = false;
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
	/// 链接定义块的起始范围。
	/// </summary>
	private int start;
	/// <summary>
	/// 链接定义块的结束范围。
	/// </summary>
	private int end;

	/// <summary>
	/// 可以请求之前行的事件。
	/// </summary>
	public event Action? ClearLines;
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
		ReadOnlySpan<char> textSpan = text.ToString();
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
				case State.StartTitle:
					success = ParseStartTitle(ref textSpan);
					break;
				case State.Title:
					success = ParseTitle(ref textSpan, text.Span);
					break;
			}
			if (!success)
			{
				state = State.Failed;
				return;
			}
		}
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
		builder.Append(text[0..idx]);
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
		label = builder.ToString();
		// 链接定义中不允许使用空白的标签。
		ReadOnlySpan<char> labelSpan = label;
		MarkdownUtil.TrimStart(ref labelSpan);
		if (labelSpan.Length == 0)
		{
			return false;
		}
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
		if (!ParseUtil.TryParseLinkDestination(ref text, out destination) || destination == null)
		{
			return false;
		}
		bool hasWhitespace = MarkdownUtil.TrimStart(ref text);
		if (text.Length == 0)
		{
			// 链接目标在行尾，当前已经是一个有效的链接定义。
			// 之后需要继续检查可能的链接标题。
			hasDefinition = true;
			ClearLines?.Invoke();
		}
		else if (!hasWhitespace)
		{
			// 链接定义的目标和标题间需要使用空白分割。
			return false;
		}
		// 将空白也包含在链接定义范围内。
		end = span.End - text.Length;
		state = State.StartTitle;
		title = null;
		// 移除后面的空白，避免空白字符串进入到解析链接标题起始的流程中。
		MarkdownUtil.TrimStart(ref text);
		return true;
	}

	/// <summary>
	/// 解析链接标题的起始。
	/// </summary>
	/// <param name="text">要解析的文本。</param>
	/// <returns>是否解析成功。</returns>
	private bool ParseStartTitle(ref ReadOnlySpan<char> text)
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
		builder.Append(text[0..idx]);
		text = text[idx..];
		if (text.Length == 0)
		{
			return true;
		}
		// 跳过结束分隔符。
		text = text[1..];
		// 在链接标题后不允许出现其它非空白字符。
		MarkdownUtil.TrimStart(ref text);
		if (text.Length > 0)
		{
			return false;
		}
		// 将空白也包含在链接定义范围内。
		end = span.End - text.Length;
		if (builder.Length > 0)
		{
			title = ParseUtil.Unescape(builder.ToString());
		}
		ClearLines?.Invoke();
		hasDefinition = true;
		CreateDefinition();

		// 后面继续尝试解析链接定义。
		state = State.StartDefinition;
		return true;
	}

	/// <summary>
	/// 将现有信息创建链接定义。
	/// </summary>
	private void CreateDefinition()
	{
		if (hasDefinition)
		{
			definitions.Add(new LinkDefinition(label!, destination!, title, new TextSpan(start, end)));
			hasDefinition = false;
		}
	}
}
