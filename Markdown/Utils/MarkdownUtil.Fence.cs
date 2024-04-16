using Cyjb.Markdown.ParseBlock;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;

namespace Cyjb.Markdown.Utils;

internal static partial class MarkdownUtil
{
	/// <summary>
	/// 返回分隔符的长度。
	/// </summary>
	/// <param name="text">要检查的字符串视图。</param>
	/// <returns>分隔符的长度。</returns>
	public static int GetFenceLength(StringView text)
	{
		ReadOnlySpan<char> span = text;
		char fence = span[0];
		// 在词法分析中已确保分隔符长度至少为 2。
		int i = 2;
		for (; i < span.Length && span[i] == fence; i++) ;
		return i;
	}

	/// <summary>
	/// 解析分隔符的起始。
	/// </summary>
	/// <param name="parser">块级语法分析器。</param>
	/// <param name="line">要解析的行信息。</param>
	/// <param name="start">分隔符的起始索引。</param>
	/// <param name="indent">分隔符的缩进。</param>
	/// <param name="fenceChar">分隔的字符。</param>
	/// <param name="fenceLength">分隔符的长度。</param>
	/// <param name="info">分隔符的信息。</param>
	/// <param name="attrs">分隔符的属性。</param>
	public static void ParseFenceStart(BlockParser parser, BlockLine line, out int start, out int indent,
		out char fenceChar, out int fenceLength,
		out string? info, out HtmlAttributeList? attrs)
	{
		// 起始位置包含分隔符位置。
		start = line.Start;
		// 跳过空白部分。
		indent = line.Indent;
		line.SkipIndent();
		// 解析自定义容器的信息。
		Token<BlockKind> token = line.PeekFront();
		fenceChar = token.Text[0];
		fenceLength = GetFenceLength(token.Text);
		if (token.Kind is BlockKind.CodeFenceStart or BlockKind.MathFenceStart or BlockKind.CustomContainerFenceStart)
		{
			info = token.Text.AsSpan(fenceLength).Trim(Whitespace).Unescape();
			if (info.Length == 0)
			{
				info = null;
			}
			attrs = token.Value as HtmlAttributeList;
			attrs?.AddPrefix(parser.Options.AttributesPrefix);
		}
		else
		{
			attrs = null;
			info = null;
		}
		// 标记当前行已处理完毕。
		line.Skip();
	}
}
