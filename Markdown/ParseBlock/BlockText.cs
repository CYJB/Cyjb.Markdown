using System.Text;
using Cyjb.Collections;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示块的文本。
/// </summary>
internal sealed class BlockText
{
	/// <summary>
	/// 文本项队列。
	/// </summary>
	private readonly Deque<BlockTextItem> items = new();
	/// <summary>
	/// 文本的长度。
	/// </summary>
	private int length;

	/// <summary>
	/// 获取文本的起始位置。
	/// </summary>
	public int Start
	{
		get
		{
			if (items.TryPeekFront(out var item))
			{
				return item.StartIndex;
			}
			else
			{
				return 0;
			}
		}
	}
	/// <summary>
	/// 获取文本的结束位置。}
	/// </summary>
	public int End
	{
		get
		{
			if (items.TryPeekBack(out var item))
			{
				return item.EndIndex;
			}
			else
			{
				return 0;
			}
		}
	}
	/// <summary>
	/// 获取文本的范围。
	/// </summary>
	public TextSpan Span
	{
		get
		{
			int count = items.Count;
			if (count == 0)
			{
				return new TextSpan();
			}
			else if (count == 1)
			{
				var item = items.PeekFront();
				return new TextSpan(item.StartIndex, item.EndIndex);
			}
			else
			{
				return new TextSpan(items.PeekFront().StartIndex, items.PeekBack().EndIndex);
			}
		}
	}

	/// <summary>
	/// 获取文本的长度。
	/// </summary>
	public int Length => length;

	/// <summary>
	/// 文本项队列。
	/// </summary>
	public Deque<BlockTextItem> Items => items;

	/// <summary>
	/// 添加新的文本。
	/// </summary>
	/// <param name="text">要添加的文本。</param>
	/// <param name="startIndex">起始索引。</param>
	public void Add(StringView text, int startIndex)
	{
		length += text.Length;
		if (items.Count > 0)
		{
			var lastItem = items.PeekBack();
			if (lastItem.Text.TryConcat(text, out var concated))
			{
				items[^1] = lastItem with
				{
					Text = concated,
				};
				return;
			}
		}
		items.PushBack(new BlockTextItem(text, startIndex));
	}

	/// <summary>
	/// 添加新的文本的一部分。
	/// </summary>
	/// <param name="item">要添加的文本。</param>
	/// <param name="start">要添加的起始索引。</param>
	/// <param name="length">要添加的长度。</param>
	public void Add(BlockTextItem item, int start, int length)
	{
		if (length > 0)
		{
			Add(item.Text.Slice(start, length), item.StartIndex + start);
		}
	}

	/// <summary>
	/// 检查是否是单行文本。
	/// </summary>
	/// <returns>如果是单行文本，会返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool IsSingleLine()
	{
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			ReadOnlySpan<char> span = items[i].Text;
			int idx = span.IndexOf('\n');
			if (idx >= 0 && idx < span.Length - 1)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 移除结束位置的多个字符。
	/// </summary>
	/// <param name="length">要移除的字符个数。</param>
	public void RemoteEnd(int length)
	{
		this.length -= length;
		while (items.TryPeekBack(out var item))
		{
			StringView text = item.Text;
			length -= text.Length;
			if (length >= 0)
			{
				items.PopBack();
			}
			else
			{
				items[^1] = item with
				{
					Text = text[..-length],
				};
				break;
			}
		}
	}

	/// <summary>
	/// 移除起始空白。
	/// </summary>
	/// <returns>如果移除了任何起始空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimStart()
	{
		int diff = 0;
		while (items.TryPeekFront(out var item))
		{
			StringView text = item.Text;
			int textLen = text.Length;
			text = text.TrimStart(MarkdownUtil.WhitespaceChars);
			textLen -= text.Length;
			if (textLen == 0)
			{
				break;
			}
			diff += textLen;
			if (text.IsEmpty)
			{
				items.PopFront();
			}
			else
			{
				items[0] = new BlockTextItem(text, item.StartIndex + textLen);
				break;
			}
		}
		if (diff == 0)
		{
			return false;
		}
		length -= diff;
		return true;
	}

	/// <summary>
	/// 移除结尾空白。
	/// </summary>
	/// <returns>如果移除了任何结尾空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimEnd(bool trimNewLine = true)
	{
		int diff = 0;
		char[] chars = trimNewLine ? MarkdownUtil.WhitespaceChars : MarkdownUtil.WhitespaceCharsWithoutNewLine;
		while (items.TryPeekBack(out var item))
		{
			StringView text = item.Text;
			int textLen = text.Length;
			text = text.TrimEnd(chars);
			textLen -= text.Length;
			if (textLen == 0)
			{
				break;
			}
			diff += textLen;
			if (text.IsEmpty)
			{
				items.PopBack();
			}
			else
			{
				items[^1] = item with
				{
					Text = text,
				};
				break;
			}
		}
		if (diff == 0)
		{
			return false;
		}
		length -= diff;
		return true;
	}

	/// <summary>
	/// 返回末尾处的下一词法文本项，但不将其消费。
	/// </summary>
	/// <returns>末尾处的文本项。</returns>
	public BlockTextItem PeekBack()
	{
		return items.PeekBack();
	}

	/// <summary>
	/// 清除所有文本。
	/// </summary>
	public void Clear()
	{
		items.Clear();
		length = 0;
	}

	/// <summary>
	/// 读取位置映射。
	/// </summary>
	/// <param name="locMap">要写入的位置映射表。</param>
	public void GetLocationMap(LocationMap locMap)
	{
		int count = items.Count;
		int index = 0;
		for (int i = 0; i < count; i++)
		{
			var item = items[i];
			locMap.Add(index, item.StartIndex);
			index += item.Text.Length;
		}
	}

	/// <summary>
	/// 返回当前对象的字符串视图表示形式。
	/// </summary>
	/// <returns>当前对象的字符串视图表示形式。</returns>
	public StringView ToStringView()
	{
		int count = items.Count;
		if (count == 0)
		{
			return StringView.Empty;
		}
		else if (count == 1)
		{
			return items.PeekFront().Text;
		}
		// 优先连接字符串视图。
		StringView view = items.PeekFront().Text;
		int i = 1;
		for (; i < count && view.TryConcat(items[i].Text, out var newView); i++)
		{
			view = newView;
		}
		if (i >= count)
		{
			return view;
		}
		// 存在无法连接的字符串，改为使用 StringBuilder 拼接。
		StringBuilder text = StringBuilderPool.Rent(length);
		text.Append(view.AsSpan());
		for (; i < count; i++)
		{
			text.Append(items[i].Text.AsSpan());
		}
		return StringBuilderPool.GetStringAndReturn(text);
	}

	/// <summary>
	/// 返回当前对象的字符串视图表示形式。
	/// </summary>
	/// <param name="start">转换为字符串视图的起始索引。</param>
	/// <returns>当前对象的字符串视图表示形式。</returns>
	public StringView ToStringView(int start)
	{
		int count = items.Count;
		if (count == 0)
		{
			return StringView.Empty;
		}
		else if (count == 1)
		{
			return items[0].Text.Substring(start);
		}
		// 优先连接字符串视图。
		StringView view = StringView.Empty;
		int i = 0;
		for (; i < count; i++)
		{
			StringView curView = items[i].Text;
			if (curView.Length <= start)
			{
				start -= curView.Length;
				continue;
			}
			start = 0;
			if (view.TryConcat(curView.Substring(start), out var newView))
			{
				view = newView;
			}
			else
			{
				break;
			}
		}
		if (i >= count)
		{
			return view;
		}
		// 存在无法连接的字符串，改为使用 StringBuilder 拼接。
		StringBuilder text = StringBuilderPool.Rent(length);
		text.Append(view.AsSpan());
		for (; i < count; i++)
		{
			text.Append(items[i].Text.AsSpan());
		}
		return StringBuilderPool.GetStringAndReturn(text);
	}
}
