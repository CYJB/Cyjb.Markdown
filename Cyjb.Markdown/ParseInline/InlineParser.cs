using System.Diagnostics.CodeAnalysis;
using Cyjb.IO;
using Cyjb.Markdown.ParseBlock;
using Cyjb.Markdown.Syntax;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseInline;

/// <summary>
/// 表示 Markdown 的行级语法分析器。
/// </summary>
/// <see href="https://spec.commonmark.org/0.30/"/>
internal sealed class InlineParser
{
	/// <summary>
	/// 链接定义。
	/// </summary>
	private readonly IReadOnlyDictionary<string, LinkDefinition> linkDefines;
	/// <summary>
	/// 解析选项。
	/// </summary>
	private readonly ParseOptions options;
	/// <summary>
	/// 源文件读取器。
	/// </summary>
	private SourceReader reader;
	/// <summary>
	/// 行级词法分析器。
	/// </summary>
	private MappedTokenizer<InlineKind> tokenizer;
	/// <summary>
	/// 要添加到的行级节点列表。
	/// </summary>
	private IList<InlineNode> children = Array.Empty<InlineNode>();
	/// <summary>
	/// 分隔符链表。
	/// </summary>
	private DelimiterInfo? delimiterInfo;
	/// <summary>
	/// 括号堆栈。
	/// </summary>
	private readonly Stack<BracketInfo> brackets = new();
	/// <summary>
	/// 当前字面量文本的起始位置。
	/// </summary>
	private int literalStart;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	/// <summary>
	/// 使用指定的解析选项初始化 <see cref="InlineParser"/> 类的新实例。
	/// </summary>
	/// <param name="linkDefines">链接定义。</param>
	/// <param name="options">解析的选项。</param>
	internal InlineParser(IReadOnlyDictionary<string, LinkDefinition> linkDefines,
		ParseOptions options)
	{
		this.linkDefines = linkDefines;
		this.options = options;
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
	/// <param name="texts">要解析的文本列表。</param>
	/// <param name="children">子节点列表。</param>
	public void Parse(IEnumerable<MappedText> texts, IList<InlineNode> children)
	{
		// 将文本拼接成源码流。
		reader = new SourceReader(TextReaderUtil.Combine(
			texts.Select(text => (Variant<TextReader, string>)text.ToString())));
		// 将词法单元重新映射成源码位置。
		tokenizer = new MappedTokenizer<InlineKind>(InlineLexer.Factory.CreateTokenizer(reader),
			GetMap(texts))
		{
			SharedContext = this
		};
		// 重置状态。
		this.children = children;
		delimiterInfo = null;
		brackets.Clear();
		literalStart = texts.First().Map[0].Item2;
		// 解析行级节点。
		while (true)
		{
			Token<InlineKind> token = tokenizer.Read();
			AddLiteral(token.Span.Start);
			if (token.IsEndOfFile)
			{
				break;
			}
			switch (token.Kind)
			{
				case InlineKind.Node:
					{
						InlineNode node = (InlineNode)token.Value!;
						// 词法分析器内部拿到的 Span 是映射前的，
						// 这里的 token.Span 才是正确的映射后的范围。
						node.Span = token.Span;
						// 子节点也需要重新映射。
						if (node is INodeContainer<InlineNode> container)
						{
							foreach (InlineNode subNode in container.Children)
							{
								subNode.Span = new TextSpan(tokenizer.MapLocation(subNode.Span.Start),
								tokenizer.MapLocation(subNode.Span.End));
							}
						}
						children.Add(node);
					}
					break;
				case InlineKind.LinkStart:
					{
						bool isImage = (bool)token.Value!;
						Literal node = new(token.Text, token.Span);
						children.Add(node);
						if (brackets.Count > 0)
						{
							brackets.Peek().BracketAfter = true;
						}
						brackets.Push(new(node, isImage, reader.Mark())
						{
							Delimiter = delimiterInfo,
						});
						// 允许识别链接闭合。
						Controller!.EnterContext(InlineLexer.LinkCloseContext);
					}
					break;
				case InlineKind.LinkClose:
					if (!ParseCloseBracket(token))
					{
						// 链接匹配失败，作为字面量字符添加。
						children.Add(new Literal(token.Text, token.Span));
					}
					break;
				case InlineKind.Delimiter:
					{
						DelimiterInfo info = (DelimiterInfo)token.Value!;
						info.Node.Span = token.Span;
						info.Prev = delimiterInfo;
						if (delimiterInfo != null)
						{
							delimiterInfo.Next = info;
						}
						delimiterInfo = info;
						// 添加分隔符的文本节点
						children.Add(info.Node);
					}
					break;
			}
			literalStart = token.Span.End;
		}
		ProcessDelimiter(null);
		MergeText(children);
	}

	/// <summary>
	/// 尝试匹配关闭括号。
	/// </summary>
	/// <param name="token">关闭括号对应的词法单元。</param>
	/// <returns>如果匹配成功，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	private bool ParseCloseBracket(Token<InlineKind> token)
	{
		// 没有匹配的括号，忽略。
		if (brackets.Count == 0)
		{
			return false;
		}
		BracketInfo opener = brackets.Peek();
		// 没有匹配的活动括号，忽略。
		if (!opener.Active)
		{
			PopBracket();
			return false;
		}

		// 进入链接状态，尝试匹配链接体或标签。
		Link link;
		TextSpan span = TextSpan.Combine(opener.Node.Span, token.Span);
		if (token.Value is LinkBody linkBody)
		{
			// 包含链接 URL。
			link = new Link(opener.IsImage, linkBody.URL, linkBody.Title, span);
			link.Attributes.AddRange(linkBody.Attributes);
			link.Attributes.AddPrefix(options.AttributesPrefix);
		}
		else if (token.Value is LinkDefinition linkDefine)
		{
			// 包含链接标签。
			link = new Link(opener.IsImage, linkDefine, span);
		}
		else if (!opener.BracketAfter && reader.Peek() != '[' &&
			linkDefines.TryGetValue(LinkUtil.NormalizeLabel(GetCurrentLinkText(reader.Index - 1)!), out LinkDefinition? linkDefine2))
		{
			// 是 ]，后面没有 URL 或标签。
			// 如果没有更多待匹配的括号，缺失第二个 label 时会将当前文本当作标签解析。
			// label 不能包含未转义的标签，因此检查 BracketAfter 来快速排除这一场景。
			link = new Link(opener.IsImage, linkDefine2, TextSpan.Combine(opener.Node.Span, token.Span));
		}
		else
		{
			PopBracket();
			return false;
		}

		// 将起始和结束括号间的节点添加为 link 的子节点。
		// 先处理分隔符。
		ProcessDelimiter(opener.Delimiter);
		// 再转移子节点。
		link.Children.AddRange(opener.Node.Next, null);
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
		return true;
	}

	/// <summary>
	/// 弹出最后一个括号信息。
	/// </summary>
	private void PopBracket()
	{
		BracketInfo opener = brackets.Pop();
		reader.Release(opener.StartMark);
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
		Dictionary<char, DelimiterInfo> openersBottom = new();
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
				node.Children.AddRange(opener!.Node.Next, closer.Node);
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
		if (info.Length == 0)
		{
			children.Remove(info.Node);
			RemoveDelimiter(info);
		}
		else
		{
			// 调整符号个数。
			info.Node.Content = info.Node.Content[..info.Length];
			if (isOpen)
			{
				int start = info.Node.Span.Start;
				info.Node.Span = new TextSpan(start, start + info.Length);
			}
			else
			{
				int end = info.Node.Span.End;
				info.Node.Span = new TextSpan(end - info.Length, end);
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
	/// 尝试寻找指定链接定义。
	/// </summary>
	/// <param name="label">要查找的链接标签。</param>
	/// <param name="define">查找到的链接定义。</param>
	/// <returns>如果找到了链接定义，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	internal bool TryGetLinkDefine(string label, [MaybeNullWhen(false)] out LinkDefinition define)
	{
		return linkDefines.TryGetValue(label, out define);
	}

	/// <summary>
	/// 返回当前的链接文本。
	/// </summary>
	/// <param name="endIndex">链接文本的结束索引（不含）。</param>
	/// <returns>当前的链接文本。</returns>
	internal string? GetCurrentLinkText(int endIndex)
	{
		if (brackets.Count == 0)
		{
			return null;
		}
		BracketInfo info = brackets.Peek();
		return reader.ReadBlock(info.StartMark.Index, endIndex - info.StartMark.Index);
	}

	/// <summary>
	/// 从指定文本中提取映射信息。
	/// </summary>
	/// <param name="texts">文本序列。</param>
	/// <returns>提取的映射信息。</returns>
	private static IEnumerable<Tuple<int, int>> GetMap(IEnumerable<MappedText> texts)
	{
		int start = 0;
		foreach (MappedText text in texts)
		{
			foreach (Tuple<int, int> map in text.Map)
			{
				yield return new Tuple<int, int>(map.Item1 + start, map.Item2);
			}
			start += text.Length;
		}
	}

	/// <summary>
	/// 添加字面量节点。
	/// </summary>
	/// <param name="end">节点的结束位置。</param>
	private void AddLiteral(int end)
	{
		string text = Controller!.GetLiteralText();
		if (text.Length > 0)
		{
			children.Add(new Literal(text, new TextSpan(literalStart, end)));
		}
	}

	/// <summary>
	/// 合并指定列表内的文本节点。
	/// </summary>
	/// <param name="children">要合并的节点列表。</param>
	private static void MergeText(IList<InlineNode> children)
	{
		for (int i = 1; i < children.Count; i++)
		{
			if (children[i - 1] is Literal prev && children[i] is Literal next)
			{
				prev.Content += next.Content;
				prev.Span = TextSpan.Combine(prev.Span, next.Span);
				children.RemoveAt(i);
				i--;
			}
		}
	}
}
