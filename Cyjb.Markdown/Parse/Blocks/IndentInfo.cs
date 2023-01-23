using System.Text;
using Cyjb.Text;

namespace Cyjb.Markdown.Parse.Blocks;

/// <summary>
/// 缩进信息。
/// </summary>
internal class IndentInfo
{
	/// <summary>
	/// 起始位置。
	/// </summary>
	private int start;
	/// <summary>
	/// 结束位置。
	/// </summary>
	private readonly int end;
	/// <summary>
	/// 行定位器。
	/// </summary>
	private readonly LineLocator locator;
	/// <summary>
	/// 缩进文本。
	/// </summary>
	private readonly string text;
	/// <summary>
	/// 原始起始位置。
	/// </summary>
	private readonly int originalStart;
	/// <summary>
	/// 起始列号。
	/// </summary>
	private int startColumn;
	/// <summary>
	/// 结束列号。
	/// </summary>
	private readonly int endColumn;

	/// <summary>
	/// 使用指定的空缩进信息初始化 <see cref="IndentInfo"/> 类的新实例。
	/// </summary>
	/// <param name="start">缩进起始位置。</param>
	/// <param name="locator">行定位器。</param>
	public IndentInfo(int start, LineLocator locator)
	{
		this.start = end = start;
		originalStart = start;
		this.locator = locator;
		text = string.Empty;
		startColumn = endColumn = 0;
	}

	/// <summary>
	/// 使用指定的缩进信息初始化 <see cref="IndentInfo"/> 类的新实例。
	/// </summary>
	/// <param name="span">缩进文本范围。</param>
	/// <param name="locator">行定位器。</param>
	/// <param name="text">缩进文本。</param>
	public IndentInfo(TextSpan span, LineLocator locator, string text)
	{
		start = span.Start;
		end = span.End;
		originalStart = span.Start;
		this.locator = locator;
		this.text = text;

		LinePositionSpan lineSpan = locator.GetSpan(span);
		startColumn = lineSpan.Start.Column;
		endColumn = lineSpan.End.Column;
	}

	/// <summary>
	/// 获取当前缩进宽度。
	/// </summary>
	public int Width => endColumn - startColumn;
	/// <summary>
	/// 获取当前缩进起始位置。
	/// </summary>
	public int Start => start;
	/// <summary>
	/// 是否是段落可以跳过的缩进。
	/// </summary>
	public bool ParagraphSkippable { get; set; } = true;

	/// <summary>
	/// 跳过指定个数的空白。
	/// </summary>
	/// <param name="count">要跳过的空白个数。</param>
	public void Skip(int count)
	{
		startColumn += count;
		if (startColumn >= endColumn)
		{
			Skip();
			return;
		}
		int column;
		// 由于 Tab 可能对应多列，因此需要找到首个 index 使得 column(index)≤startColumn。
		for (; start < end; start++)
		{
			column = locator.GetPosition(start).Column;
			if (column == startColumn)
			{
				return;
			}
			else if (column > startColumn)
			{
				start--;
				return;
			}
		}
		// 避免 end 的列位置超出 startColumn
		column = locator.GetPosition(start).Column;
		if (column > startColumn)
		{
			start--;
		}
	}

	/// <summary>
	/// 跳过当前空白。
	/// </summary>
	public void Skip()
	{
		startColumn = endColumn;
		start = end;
	}

	/// <summary>
	/// 返回剩余的缩进文本。
	/// </summary>
	/// <returns>缩进文本。</returns>
	public string GetText()
	{
		if (startColumn == endColumn)
		{
			// 所有缩进均已消费。
			return string.Empty;
		}
		int column = locator.GetPosition(start).Column;
		if (column == startColumn)
		{
			return text[(start - originalStart)..];
		}
		else
		{
			// 当前是部分 Tab，需要使用空格补齐 column(start) 到 startColumn 的位置。
			column = locator.GetPosition(start + 1).Column;
			StringBuilder result = new();
			result.Append(' ', column - startColumn);
			int idx = start + 1 - originalStart;
			// 存在 Tab 时，可能会出现列数超出字符数的场景。
			if (idx < text.Length)
			{
				result.Append(text, idx, text.Length - idx);
			}
			return result.ToString();
		}
	}
}
