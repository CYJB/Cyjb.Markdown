using Cyjb.Markdown.Generator;

string markdownRoot = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Cyjb.Markdown");
string htmlEntityPath = Path.Combine(markdownRoot, "Utils/HtmlEntity.cs");
File.WriteAllText(htmlEntityPath, HtmlEntityGenerator.Generate());
Console.WriteLine("已生成 HtmlEntity.cs");
