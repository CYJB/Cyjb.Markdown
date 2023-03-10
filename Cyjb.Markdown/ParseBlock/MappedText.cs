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
	private string text;
	/// <summary>
	/// 文本的范围。
	/// </summary>
	private TextSpan span;
	/// <summary>
	/// 源码映射表。
	/// </summary>
	private Tuple<int, int>[] map;
	/// <summary>
	/// 位置映射器。
	/// </summary>
	private LocationMap? locMap;

	/// <summary>
	/// 使用指定的文本和映射信息初始化 <see cref="MappedText"/> 类的新实例。
	/// </summary>
	/// <param name="text">文本。</param>
	/// <param name="span">文本的范围。</param>
	/// <param name="map">映射关系。</param>
	internal MappedText(string text, TextSpan span, Tuple<int, int>[] map)
	{
		this.text = text;
		this.span = span;
		this.map = map;
	}

	/// <summary>
	/// 获取源码映射表。
	/// </summary>
	internal Tuple<int, int>[] Map => map;

	/// <summary>
	/// 获取文本的文本范围。
	/// </summary>
	public TextSpan Span => span;

	/// <summary>
	/// 获取文本的长度。
	/// </summary>
	public int Length => text.Length;

	/// <summary>
	/// 获取行是否是空的（不包含任何字符）。
	/// </summary>
	public bool IsEmpty => text.Length == 0;

	/// <summary>
	/// 获取行是否是空白的（只包含空格或 Tab）。
	/// </summary>
	public bool IsBlank => text.AsSpan().IsBlank();

	/// <summary>
	/// 获取指定索引的文本。
	/// </summary>
	/// <param name="index">要检查的索引。</param>
	/// <returns>指定索引的文本。</returns>
	public char this[int index] => text[index];

	/// <summary>
	/// 返回指定字符索引在映射后的索引。
	/// </summary>
	/// <param name="index">要检查的字符索引。</param>
	/// <returns><paramref name="index"/> 在映射后的索引。</returns>
	public int GetMappedIndex(int index)
	{
		locMap ??= new LocationMap(map);
		return locMap.MapLocation(index);
	}

	/// <summary>
	/// 移除起始空白。
	/// </summary>
	/// <returns>如果移除了任何起始空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimStart()
	{
		ReadOnlySpan<char> textSpan = text.AsSpan();
		if (!MarkdownUtil.TrimStart(ref textSpan))
		{
			return false;
		}
		int diff = text.Length - textSpan.Length;
		text = text[diff..];
		span = new TextSpan(span.Start + diff, span.End);
		// 映射关系也需要调整
		map = GetMap(diff);
		locMap = null;
		return true;
	}

	/// <summary>
	/// 移除结尾空白。
	/// </summary>
	/// <returns>如果移除了任何结尾空白，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool TrimEnd()
	{
		ReadOnlySpan<char> textSpan = text.AsSpan();
		if (!MarkdownUtil.TrimEnd(ref textSpan))
		{
			return false;
		}
		text = text[0..textSpan.Length];
		span = new TextSpan(span.Start, span.Start + text.Length);
		return true;
	}

	/// <summary>
	/// 返回当前文本中是否包含指定字符。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>如果当前文本中包含指定字符，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool Contains(char ch)
	{
		return text.Contains(ch);
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
		if (length < 0 || start + length > text.Length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		string substr = text.Substring(start, length);
		int spanStart = span.Start + start;
		return new MappedText(substr, new TextSpan(spanStart, spanStart + length), GetMap(start));
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return text;
	}

	/// <summary>
	/// 返回从指定偏移开始的映射信息。
	/// </summary>
	/// <param name="offset">文本的起始偏移。</param>
	/// <returns>相应的映射信息。</returns>
	private Tuple<int, int>[] GetMap(int offset)
	{
		if (offset == 0)
		{
			return map;
		}
		Stack<Tuple<int, int>> mapStack = new();
		for (int j = map.Length - 1; j >= 0; j--)
		{
			Tuple<int, int> tuple = map[j];
			int newIdx = tuple.Item1 - offset;
			if (newIdx >= 0)
			{
				mapStack.Push(new Tuple<int, int>(newIdx, tuple.Item2));
			}
			else
			{
				mapStack.Push(new Tuple<int, int>(0, tuple.Item2 - newIdx));
				break;
			}
		}
		return mapStack.ToArray();
	}
}
