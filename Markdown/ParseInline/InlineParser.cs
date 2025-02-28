using System.Diagnostics.CodeAnalysis;
using Cyjb.Compilers.Lexers;
using Cyjb.Markdown.ParseBlock;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 表示 Markdown 的行级语法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.31.2/"/>
internal sealed class InlineParser
{
	/// <summary>
	/// 链接定义。
	/// </summary>
	private readonly IReadOnlyDictionary<string, LinkDefinition> linkDefines;
	/// <summary>
	/// 脚注定义。
	/// </summary>
	private readonly IReadOnlyDictionary<string, Footnote> footnotes;
	/// <summary>
	/// 解析选项。
	/// </summary>
	private readonly ParseOptions options;
	/// <summary>
	/// 行级词法分析运行器。
	/// </summary>
	private readonly LexerRunner<int> runner = InlineLexer.Factory.CreateRunner();
	/// <summary>
	/// 源文件读取器。
	/// </summary>
	private SourceReader source;
	/// <summary>
	/// 要添加到的行级节点列表。
	/// </summary>
	internal NodeList<InlineNode> children;
	/// <summary>
	/// 分隔符链表。
	/// </summary>
	private DelimiterInfo? delimiterInfo;
	/// <summary>
	/// 括号堆栈。
	/// </summary>
	private readonly Stack<BracketInfo> brackets = new();
	/// <summary>
	/// 位置映射关系。
	/// </summary>
	private readonly LocationMap locationMap = new();
	/// <summary>
	/// 不再需要查找的分隔符位置。
	/// </summary>
	private readonly Dictionary<char, DelimiterInfo> openersBottom = new();

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	/// <summary>
	/// 使用指定的解析选项初始化 <see cref="InlineParser"/> 类的新实例。
	/// </summary>
	/// <param name="linkDefines">链接定义。</param>
	/// <param name="footnotes">脚注定义。</param>
	/// <param name="options">解析的选项。</param>
	internal InlineParser(IReadOnlyDictionary<string, LinkDefinition> linkDefines,
		IReadOnlyDictionary<string, Footnote> footnotes,
		ParseOptions options)
	{
		this.linkDefines = linkDefines;
		this.footnotes = footnotes;
		this.options = options;
		runner.SharedContext = this;
	}

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	/// <summary>
	/// 获取或设置行级词法分析器的控制器。
	/// </summary>
	internal InlineLexer? Controller { get; set; }
	/// <summary>
	/// 获取解析选项。
	/// </summary>
	public ParseOptions Options => options;

	/// <summary>
	/// 从指定文本解析行级节点。
	/// </summary>
	/// <param name="text">要解析的块文本。</param>
	/// <param name="children">子节点列表。</param>
	public void Parse(BlockText text, NodeList<InlineNode> children)
	{
		// 将文本拼接成源码流。
		source = SourceReader.Create(text.ToStringView());
		// 将词法单元重新映射成源码位置。
		locationMap.Clear();
		text.GetLocationMap(locationMap);
		// 重置状态。
		this.children = children;
		delimiterInfo = null;
		brackets.Clear();

		runner.Parse(source);

		ProcessDelimiter(null);
		MergeText(children);
	}

	/// <summary>
	/// 映射指定的位置。
	/// </summary>
	/// <param name="location">要映射的位置。</param>
	/// <returns>映射后的位置。</returns>
	internal int MapLocation(int location)
	{
		return locationMap.MapLocation(location);
	}

	/// <summary>
	/// 添加括号信息。
	/// </summary>
	/// <param name="node">括号对应的节点。</param>
	/// <param name="isImage">是否表示图片。</param>
	internal void AddBracket(TempLiteral node, bool isImage)
	{
		if (brackets.Count > 0)
		{
			brackets.Peek().BracketAfter = true;
		}
		brackets.Push(new BracketInfo(node, isImage, source.Index)
		{
			Delimiter = delimiterInfo,
		});
	}

	/// <summary>
	/// 添加分隔符信息。
	/// </summary>
	/// <param name="info">要添加的分隔符信息。</param>
	internal void AddDelimiter(DelimiterInfo info)
	{
		info.Prev = delimiterInfo;
		if (delimiterInfo != null)
		{
			delimiterInfo.Next = info;
		}
		delimiterInfo = info;
	}

	/// <summary>
	/// 尝试匹配关闭括号。
	/// </summary>
	/// <param name="span">关闭括号的文本范围。</param>
	/// <returns>如果匹配成功，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	internal bool ParseCloseBracket(TextSpan span)
	{
		BracketInfo? opener = FindOpenedBracket();
		if (opener == null)
		{
			return false;
		}
		// 进入链接状态，尝试匹配链接体或标签。
		Link link;
		TextSpan fullSpan = TextSpan.Combine(opener.Node.Span, span);
		if (TryParseFootnote(opener, out Footnote? footnote))
		{
			FootnoteRef footnoteRef = new(footnote, fullSpan);
			// 移除起始括号以及之后的所有内容。
			Controller!.AddLiteral(span.Start);
			children.RemoveRange(opener.Node, null);
			children.Add(footnoteRef);
			PopBracket();
			return true;
		}
		else if (!opener.BracketAfter && source.Peek() != '[' &&
			linkDefines.TryGetValue(LinkUtil.NormalizeLabel(GetCurrentLinkText(source.Index - 1)), out LinkDefinition? linkDefine2))
		{
			// 是 ]，后面没有 URL 或标签。
			// 如果没有更多待匹配的括号，缺失第二个 label 时会将当前文本当作标签解析。
			// label 不能包含未转义的标签，因此检查 BracketAfter 来快速排除这一场景。
			link = new Link(opener.IsImage, linkDefine2, fullSpan);
		}
		else
		{
			PopBracket();
			return false;
		}
		Controller!.AddLiteral(span.Start);
		AddLinkChildren(opener, link);
		return true;
	}

	/// <summary>
	/// 尝试解析为链接定义。
	/// </summary>
	/// <param name="label">要查找的链接标签。</param>
	/// <param name="span">文本范围。</param>
	/// <returns>如果找到了链接定义，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	internal bool ParseLinkDefinition(StringView label, TextSpan span)
	{
		BracketInfo? opener = FindOpenedBracket();
		if (opener == null)
		{
			// 未找到起始括号，不解析为链接定义。
			return false;
		}
		// 尝试寻找链接定义。
		if (!linkDefines.TryGetValue(LinkUtil.NormalizeLabel(label), out var linkDefine))
		{
			return false;
		}
		// 进入链接状态，尝试匹配链接体或标签。
		TextSpan fullSpan = TextSpan.Combine(opener.Node.Span, span);
		// 包含链接标签。
		Link link = new(opener.IsImage, linkDefine, fullSpan);
		Controller!.AddLiteral(span.Start);
		AddLinkChildren(opener, link);
		return true;
	}

	/// <summary>
	/// 尝试匹配链接体。
	/// </summary>
	/// <param name="linkBody">要匹配的体。</param>
	/// <param name="span">文本范围。</param>
	/// <returns>如果匹配成功，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	internal bool ParseLinkBody(LinkBody linkBody, TextSpan span)
	{
		BracketInfo? opener = FindOpenedBracket();
		if (opener == null)
		{
			return false;
		}

		// 进入链接状态，尝试匹配链接体或标签。
		TextSpan fullSpan = TextSpan.Combine(opener.Node.Span, span);
		// 包含链接 URL。
		Link link = new(opener.IsImage, linkBody.URL, linkBody.Title, fullSpan);
		if (linkBody.HasAttribute)
		{
			link.Attributes.AddRange(linkBody.Attributes);
		}
		if (options.AttributesPrefix != null)
		{
			link.Attributes.AddPrefix(options.AttributesPrefix);
		}
		Controller!.AddLiteral(span.Start);
		AddLinkChildren(opener, link);
		return true;
	}

	/// <summary>
	/// 寻找起始括号。
	/// </summary>
	/// <returns>找到的起始括号，如果未找到则返回 <c>null</c>。</returns>
	private BracketInfo? FindOpenedBracket()
	{
		// 没有匹配的括号，忽略。
		if (brackets.Count == 0)
		{
			return null;
		}
		BracketInfo opener = brackets.Peek();
		// 没有匹配的活动括号，忽略。
		if (!opener.Active)
		{
			PopBracket();
			return null;
		}
		return opener;
	}

	/// <summary>
	/// 添加链接的子节点。
	/// </summary>
	/// <param name="opener">起始括号。</param>
	/// <param name="link">链接节点。</param>
	private void AddLinkChildren(BracketInfo opener, Link link)
	{
		// 将起始和结束括号间的节点添加为 link 的子节点。
		// 先处理分隔符。
		ProcessDelimiter(opener.Delimiter);
		// 再转移子节点。
		if (opener.Node.Next != null)
		{
			link.Children.AddRange(opener.Node.Next, null);
		}
		MergeText(link.Children);
		children.Remove(opener.Node);
		children.Add(link);

		// 不允许在链接内部添加链接，将之前的所有非图片起始括号失效。
		if (link.Kind == MarkdownKind.Link)
		{
			foreach (BracketInfo info in brackets)
			{
				if (!info.IsImage)
				{
					info.Active = false;
				}
			}
		}
		PopBracket();
	}

	/// <summary>
	/// 尝试解析脚注。
	/// </summary>
	/// <param name="opener">起始中括号。</param>
	/// <param name="footnote">解析得到的脚注。</param>
	/// <returns>如果成功解析脚注，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	private bool TryParseFootnote(BracketInfo opener, [MaybeNullWhen(false)] out Footnote footnote)
	{
		footnote = null;
		if (opener.BracketAfter || footnotes.Count == 0)
		{
			return false;
		}
		ReadOnlySpan<char> label = GetCurrentLinkText(source.Index - 1);
		if (!MarkdownUtil.IsFootnotesLabel(label))
		{
			return false;
		}
		return footnotes.TryGetValue(LinkUtil.NormalizeLabel(label.Slice(1)), out footnote);
	}

	/// <summary>
	/// 弹出最后一个括号信息。
	/// </summary>
	private void PopBracket()
	{
		brackets.Pop();
		// 根据是否包含有效的起始中括号，决定后续是否要识别链接 URL 或标签。
		if (brackets.Count > 0 && brackets.Peek().Active)
		{
			Controller!.EnterContext(InlineLexer.LinkCloseContext);
		}
		else
		{
			Controller!.ExitContext(InlineLexer.LinkCloseContext);
		}
	}

	/// <summary>
	/// 处理分隔符。
	/// </summary>
	/// <param name="stackBottom">需要处理到的最后一个分隔符信息。</param>
	private void ProcessDelimiter(DelimiterInfo? stackBottom)
	{
		openersBottom.Clear();
		// 找到 stackBottom 之后的首个关闭分隔符。
		DelimiterInfo? closer = delimiterInfo;
		for (; closer != null && closer.Prev != stackBottom; closer = closer.Prev) ;
		// 向栈顶方向查找关闭分隔符，并依次处理。
		while (closer != null)
		{
			char delimiter = closer.Delimiter;
			if (!closer.CanClose)
			{
				closer = closer.Next;
				continue;
			}
			// 向栈底方向找到首个可以匹配的开始分隔符。
			DelimiterInfo? opener = closer.Prev;
			bool potentialOpenerFound = false;
			INodeContainer<InlineNode>? node = null;
			while (opener != null && opener != stackBottom &&
				opener != openersBottom.GetValueOrDefault(delimiter))
			{
				if (opener.CanOpen && opener.Delimiter == delimiter)
				{
					potentialOpenerFound = true;
					node = closer.Processor.Process(opener, closer);
					if (node != null)
					{
						break;
					}
				}
				opener = opener.Prev;
			}
			if (node == null)
			{
				// 未找到相应的起始分隔符节点。
				if (!potentialOpenerFound)
				{
					// 如果没有找到任何可能匹配的起始分隔符（包括数量不匹配的场景）
					// 那么后续同一分隔符可以不再重复查找此范围。
					openersBottom[delimiter] = closer.Prev!;
					if (!closer.CanOpen)
					{
						// 若当前结束分隔符不能当作开始分割符使用
						// 那么可以直接删掉。
						RemoveDelimiter(closer);
					}
				}
				closer = closer.Next;
			}
			else
			{
				// 找到了相应节点，转移子节点，并将新节点插入到 closer 之前。
				if (opener!.Node.Next != null)
				{
					node.Children.AddRange(opener.Node.Next, closer.Node);
				}
				MergeText(node.Children);
				RemoveDelimiters(opener, closer);
				int idx = children.IndexOf(closer.Node);
				children.Insert(idx, (InlineNode)node);
				// 检查 opener 和 closer 是否已被消耗完毕
				CheckDelimiter(opener, true);
				DelimiterInfo? nextCloser = closer.Next;
				CheckDelimiter(closer, false);
				if (closer.Length == 0)
				{
					closer = nextCloser;
				}
			}
		}
		// 移除所有分隔符。
		while (delimiterInfo != null && delimiterInfo != stackBottom)
		{
			RemoveDelimiter(delimiterInfo);
		}
	}

	/// <summary>
	/// 检查指定分隔符是否已被消耗。
	/// </summary>
	/// <param name="isOpen">是否是起始符号。</param>
	/// <param name="info">要检查的分隔符信息。</param>
	private void CheckDelimiter(DelimiterInfo info, bool isOpen)
	{
		int length = info.Length;
		if (length == 0)
		{
			children.Remove(info.Node);
			RemoveDelimiter(info);
		}
		else
		{
			// 调整符号个数。
			if (isOpen)
			{
				int start = info.Node.Span.Start;
				info.Node.Content = info.Node.Content[..length];
				info.Node.Span = new TextSpan(start, start + length);
			}
			else
			{
				int end = info.Node.Span.End;
				info.Node.Content = info.Node.Content.Substring(info.Node.Content.Length - length);
				info.Node.Span = new TextSpan(end - length, end);
			}
		}
	}

	/// <summary>
	/// 移除指定的分隔符信息。
	/// </summary>
	/// <param name="info">要移除的分隔符信息。</param>
	private void RemoveDelimiter(DelimiterInfo info)
	{
		if (info.Prev != null)
		{
			info.Prev.Next = info.Next;
		}
		if (info.Next == null)
		{
			// 更新栈顶。
			delimiterInfo = info.Prev;
		}
		else
		{
			info.Next.Prev = info.Prev;
		}
	}

	/// <summary>
	/// 移除指定分隔符之间的其它分隔符。
	/// </summary>
	/// <param name="start">起始分隔符（不含）。</param>
	/// <param name="end">结束分隔符（不含）。</param>
	private static void RemoveDelimiters(DelimiterInfo start, DelimiterInfo end)
	{
		if (start.Next != end)
		{
			start.Next = end;
			end.Prev = start;
		}
	}

	/// <summary>
	/// 返回当前的链接文本。
	/// </summary>
	/// <param name="endIndex">链接文本的结束索引（不含）。</param>
	/// <returns>当前的链接文本。</returns>
	internal StringView GetCurrentLinkText(int endIndex)
	{
		if (brackets.Count == 0)
		{
			return StringView.Empty;
		}
		BracketInfo info = brackets.Peek();
		return source.GetText(info.StartIndex, endIndex - info.StartIndex);
	}

	/// <summary>
	/// 合并指定列表内的文本节点。
	/// </summary>
	/// <param name="children">要合并的节点列表。</param>
	private static void MergeText(NodeList<InlineNode> children)
	{
		int count = children.Count;
		int idx = 0;
		bool updateNext = true;
		for (int i = 0; i < count; idx++, i++)
		{
			if (children[i] is TempLiteral literal)
			{
				for (; i + 1 < count && children[i + 1] is TempLiteral nextLiteral; i++)
				{
					literal.Concat(nextLiteral);
				}
				children.MoveItemTo(literal.GetLiteral(), idx, updateNext);
				// 后续节点都不需要再更新 Next，因为会在后续节点前移的时候设置。
				if (i != idx)
				{
					updateNext = false;
				}
			}
			else if (i != idx)
			{
				// 迁移的场景不需要更新 Next，会在后续节点前移的时候设置。
				children.MoveItemTo(children[i], idx, updateNext);
			}
		}
		children.RemoveRangeUnchecked(idx, count - idx);
	}
}
