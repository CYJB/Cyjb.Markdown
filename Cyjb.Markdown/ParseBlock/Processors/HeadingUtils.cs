using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown.ParseBlock;

/// <summary>
/// 标题的工具类。
/// </summary>
internal static class HeadingUtils
{
	/// <summary>
	/// 处理标题。
	/// </summary>
	/// <param name="parser">块解析器。</param>
	/// <param name="heading">要处理的标题节点。</param>
	/// <param name="label">标题的链接标签。</param>
	public static void ProcessHeading(BlockParser parser, Heading heading, string label)
	{
		// 处理标题属性
		heading.Attributes.AddPrefix(parser.Options.AttributesPrefix);
		// 处理标题引用。
		if ((parser.Options.UseAutoIdentifier || heading.Attributes.Id != null) &&
			!parser.HeadingReferences.ContainsKey(label))
		{
			parser.HeadingReferences[label] = new Tuple<Heading, LinkDefinition>(heading,
				new LinkDefinition(label, string.Empty, null));
		}
	}
}
