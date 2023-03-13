namespace TestMarkdown;

internal static class SyntaxConstants
{
	/// <summary>
	/// 覆盖所有语法节点的 Markdown 文档。
	/// </summary>
	public static readonly string SyntaxMarkdown = @"---
# heading
```foo
bar
```
<script></script>

[foo]:/url

> - baz
> - bim

| h1 | h2 |
|:--:| -- |
| c1 | c2 |

$$
math
$$

[foo] `bar` _a **b** c_ ~~d~~ <test>
:atom: $3$";

	/// <summary>
	/// 所有 Syntax 节点的名称顺序。
	/// </summary>
	public static readonly string[] SyntaxNames = new string[]
	{
		"Document",
		"ThematicBreak",
		"Heading",
		"Literal",
		"CodeBlock",
		"HtmlBlock",
		"LinkDefinition",
		"Blockquote",
		"List",
		"ListItem",
		"Paragraph",
		"Literal",
		"ListItem",
		"Paragraph",
		"Literal",
		"Table",
		"TableRow",
		"TableCell",
		"Literal",
		"TableCell",
		"Literal",
		"TableRow",
		"TableCell",
		"Literal",
		"TableCell",
		"Literal",
		"MathBlock",
		"Paragraph",
		"Link",
		"Literal",
		"Literal",
		"CodeSpan",
		"Literal",
		"Emphasis",
		"Literal",
		"Strong",
		"Literal",
		"Literal",
		"Literal",
		"Strikethrough",
		"Literal",
		"Literal",
		"Html",
		"Break",
		"Emoji",
		"Literal",
		"MathSpan",
	};
}
