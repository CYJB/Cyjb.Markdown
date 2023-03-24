using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;
using Cyjb.Markdown.Syntax;

// 将 Markdown 字符串解析成语法树。
Document doc = Document.Parse("This is a **markdown**");

// 将语法树渲染成 HTML 字符串。
HtmlRenderer renderer = new();
doc.Accept(renderer);
Console.WriteLine(renderer);
// <p>This is a <strong>markdown</strong></p>

// 输出语法树。
doc.Accept(new TreeRenderer());
// {Document [0..22)}
//   {Paragraph [0..22)}
//     {Literal "This is a " [0..10)}
//     {Strong [10..22)}
//       {Literal "markdown" [12..20)}

class TreeRenderer : SyntaxWalker
{
	/// <summary>
	/// 当前深度。
	/// </summary>
	private int depth = 0;

	public override void DefaultVisit(Node node)
	{
		Console.Write(new string(' ', depth * 2));
		Console.WriteLine(node);
		depth++;
		base.DefaultVisit(node);
		depth--;
	}
}

