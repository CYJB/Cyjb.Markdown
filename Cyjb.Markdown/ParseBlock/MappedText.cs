using Cyjb.Collections;
using Cyjb.Markdown.Utils;
using Cyjb.Text;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 表示可以映射到源码的文本。
/// </summary>
internal sealed class MappedText
{
	/// <summary>
	/// 文本。
	/// </summary>
	private readonly List<StringView> texts;
	/// <summary>
	/// 文本的长度。
	/// </summary>
	private int length;
	/// <summary>
	/// 文本的范围。
	/// </summary>
	private TextSpan span;
	/// <summary>
	/// 源码映射表。
	/// </summary>
	private List<Tuple<int, int>> maps;
	/// <summary>
	/// 位置映射器。
	/// </summary>
	private LocationMap? locMap;

	/// <summary>
	/// 使用指定的文本和映射信息初始化 <see cref="MappedText"/> 类的新实例。
	/// </summary>
	/// <param name="texts">文本。</param>
	/// <param name="length">文本的长度。</param>
	/// <param name="span">文本的范围。</param>
	/// <param name="maps">映射关系。</param>
	internal MappedText(List<StringView> texts, int length, TextSpan span, List<Tuple<int, int>> maps)
	{
		this.texts = texts;
		this.length = length;
		this.span = span;
		this.maps = maps;
	}

	/// <summary>
	/// 获取源码映射表。
	/// </summary>
	internal List<Tuple<int, int>> Maps => maps;

	/// <summary>
	/// 获取文本的文本范围。
	/// </summary>
	public TextSpan Span => span;

	/// <summary>
	/// 获取文本的长度。
	/// </summary>
	public int Length => length;

	/// <summary>
	/// 获取行是否是空的（不包含任何字符）。
	/// </summary>
	public bool IsEmpty => length == 0;

	/// <summary>
	/// 获取行是否是空白的（只包含空格或 Tab）。
	/// </summary>
	public bool IsBlank
	{
		get
		{
			foreach (StringView text in texts)
			{
				if (!text.AsSpan().IsBlank())
				{
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// 获取指定索引的文本。
	/// </summary>
	/// <param name="index">要检查的索引。</param>
	/// <returns>指定索引的文本。</returns>
	public char this[int index]
	{
		get
		{
			foreach (StringView text in texts)
			{
				if (index >= text.Length)
				{
					index -= text.Length;
				}
				else
				{
					return text[index];
				}
			}
			return SourceReader.InvalidCharacter;
		}
	}

	/// <summary>
	/// 返回指定字符索引在映射后的索引。
	/// </summary>
	/// <param name="index">要检查的字符索引。</param>
	/// <returns><paramref name="index"/> 在映射后的索引。</returns>
	public int GetMappedIndex(int index)
	{
		locMap ??= new LocationMap(maps, LocationMapType.Offset);
		return locMap.MapLocation(index);
	}

	/// <summary>
	/// 移除起始位置的多个字符。
	/// </summary>
	/// <param name="count">要移除的字符个数。</param>
	public void RemoteStart(int count)
	{
		if (count < 0 || count > length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		RemoteTextStart(count);
		RemoteMapStart(count);
		length -= count;
		int spanStart = span.Start + count;
		span = new TextSpan(spanStart, spanStart + length);
	}

	/// <summary>
	/// 移除结束位置的多个字符。
	/// </summary>
	/// <param name="count">要移除的字符个数。</param>
	public void RemoteEnd(int count)
	{
		if (count < 0 || count > length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		RemoteTextEnd(count);
		length -= count;
		span = new TextSpan(span.Start, span.Start + length);
	}

	/// <summary>
	/// 移除起始空白。
	/// </summary>
	/// <returns>如果移除了任何起始空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimStart()
	{
		int diff = 0;
		int i;
		for (i = 0; i < texts.Count; i++)
		{
			StringView text = texts[i];
			int len = text.Length;
			text = text.TrimStart(MarkdownUtil.WhitespaceChars);
			if (text.Length == len)
			{
				break;
			}
			len -= text.Length;
			diff += len;
			if (!text.IsEmpty)
			{
				texts[i] = text;
				break;
			}
		}
		if (diff == 0)
		{
			return false;
		}
		texts.RemoveRange(0, i);
		RemoteMapStart(diff);
		length -= diff;
		span = new TextSpan(span.Start + diff, span.End);
		locMap = null;
		return true;
	}

	/// <summary>
	/// 移除结尾空白。
	/// </summary>
	/// <returns>如果移除了任何结尾空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimEnd()
	{
		int diff = 0;
		for (int i = texts.Count - 1; i >= 0; i--)
		{
			StringView span = texts[i];
			int len = span.Length;
			span = span.TrimEnd(MarkdownUtil.WhitespaceChars);
			if (span.Length == len)
			{
				break;
			}
			diff += len - span.Length;
			if (span.IsEmpty)
			{
				texts.RemoveAt(i);
			}
			else
			{
				texts[i] = texts[i][..span.Length];
				break;
			}
		}
		if (diff == 0)
		{
			return false;
		}
		length -= diff;
		span = new TextSpan(span.Start, span.Start + length);
		return true;
	}

	/// <summary>
	/// 返回当前文本中是否包含指定字符。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>如果当前文本中包含指定字符，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool Contains(char ch)
	{
		foreach (StringView text in texts)
		{
			if (text.Contains(ch))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 返回指定的索引处开始，指定长度的文本。
	/// </summary>
	/// <param name="start">开始切片的索引。</param>
	/// <param name="length">切片所需的长度。</param>
	/// <returns>从 <paramref name="start"/> 开始长为 <paramref name="length"/> 的文本。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于零。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> +
	/// <paramref name="length"/> 小于零或大于文本长度。</exception>
	public MappedText Slice(int start, int length)
	{
		if (start < 0)
		{
			throw CommonExceptions.ArgumentNegative(start);
		}
		if (length < 0 || start + length > this.length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		int originStart = start;
		int originLength = length;
		List<StringView> newTexts = new();
		foreach (StringView text in texts)
		{
			if (start >= text.Length)
			{
				start -= text.Length;
			}
			else
			{
				int end = start + length;
				if (end > text.Length)
				{
					newTexts.Add(text[start..]);
					length -= text.Length - start;
					start = 0;
				}
				else
				{
					newTexts.Add(text[start..end]);
					break;
				}
			}
		}
		int spanStart = span.Start + originStart;
		TextSpan newSpan = new(spanStart, spanStart + originLength);
		return new MappedText(newTexts, originLength, newSpan, GetMaps(start, length));
	}

	/// <summary>
	/// 将当前文本添加到指定字符串后。
	/// </summary>
	/// <param name="list">要添加到的字符串。</param>
	/// <param name="startIndex">添加的起始索引。</param>
	public void AppendTo(PooledList<char> list, int startIndex = 0)
	{
		foreach (StringView text in texts)
		{
			if (startIndex >= text.Length)
			{
				startIndex -= text.Length;
			}
			else
			{
				list.Add(text[startIndex..].AsSpan());
				startIndex = 0;
			}
		}
	}

	/// <summary>
	/// 将当前文本添加到指定字符串后。
	/// </summary>
	/// <param name="list">要添加到的字符串。</param>
	/// <param name="startIndex">添加的起始索引。</param>
	public void AppendTo(ref ValueList<char> list, int startIndex = 0)
	{
		foreach (StringView text in texts)
		{
			if (startIndex >= text.Length)
			{
				startIndex -= text.Length;
			}
			else
			{
				list.Add(text[startIndex..].AsSpan());
				startIndex = 0;
			}
		}
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		ValueList<char> text = new(stackalloc char[ValueList.StackallocCharSizeLimit]);
		AppendTo(ref text);
		string result = text.ToString();
		text.Dispose();
		return result;
	}

	/// <summary>
	/// 移除文本起始位置指定个数的字符。
	/// </summary>
	/// <param name="count">要移除的字符个数。</param>
	private void RemoteTextStart(int count)
	{
		int i;
		for (i = 0; i < texts.Count; i++)
		{
			StringView text = texts[i];
			if (count >= text.Length)
			{
				count -= text.Length;
			}
			else
			{
				break;
			}
		}
		if (i > 0)
		{
			texts.RemoveRange(0, i);
			if (texts.Count == 0)
			{
				return;
			}
		}
		texts[0] = texts[0].Substring(count);
	}

	/// <summary>
	/// 移除文本结束位置指定个数的字符。
	/// </summary>
	/// <param name="count">要移除的字符个数。</param>
	private void RemoteTextEnd(int count)
	{
		for (int i = texts.Count - 1; i >= 0; i--)
		{
			StringView text = texts[i];
			int len = text.Length;
			if (count >= len)
			{
				count -= len;
				texts.RemoveAt(i);
			}
			else
			{
				texts[i] = text.Substring(0, len - count);
				break;
			}
		}
	}

	/// <summary>
	/// 移除映射关系起始位置指定个数的字符。
	/// </summary>
	/// <param name="count">要移除的字符个数。</param>
	private void RemoteMapStart(int count)
	{
		int i;
		int mapCount = 0;
		for (i = 0; i < maps.Count; i++)
		{
			Tuple<int, int> item = maps[i];
			if (count >= item.Item1)
			{
				count -= item.Item1;
				mapCount += item.Item2;
			}
			else
			{
				break;
			}
		}
		if (i > 0)
		{
			maps.RemoveRange(0, i);
		}
		if (maps.Count > 0)
		{
			maps[0] = new Tuple<int, int>(maps[0].Item1 - count, maps[1].Item2 + mapCount);
		}
		else
		{
			maps.Add(new Tuple<int, int>(0, mapCount + count));
		}
	}

	/// <summary>
	/// 返回从指定位置开始指定长度的映射信息。
	/// </summary>
	/// <param name="start">文本的起始位置。</param>
	/// <param name="count">映射信息的长度。</param>
	/// <returns>相应的映射信息。</returns>
	private List<Tuple<int, int>> GetMaps(int start, int count)
	{
		if (start == 0)
		{
			return maps;
		}
		List<Tuple<int, int>> newMaps = new();
		int mapCount = 0;
		int index = start;
		int mappedIndex = start;
		int i;
		for (i = 0; i < maps.Count; i++)
		{
			Tuple<int, int> tuple = maps[i];
			if (start >= tuple.Item1)
			{
				start -= tuple.Item1;
				mapCount += tuple.Item2;
			}
			else
			{
				(index, mappedIndex) = maps[i];
				break;
			}
		}
		newMaps.Add(new Tuple<int, int>(index - start, mappedIndex + mapCount));
		// 后续直到 length 之前的都可以直接复制过去。
		count -= start;
		for (; i < maps.Count && count > 0; i++)
		{
			count -= maps[i].Item1;
			newMaps.Add(maps[i]);
		}
		return newMaps;
	}
}
