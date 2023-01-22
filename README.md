Cyjb.Markdown
====

[![](https://img.shields.io/nuget/v/Cyjb.Markdown.svg)](https://www.nuget.org/packages/Cyjb.Markdown)

提供 Markdown 的解析和 HTML 渲染能力，基于 .NET 6。

## 使用方法

可以解析任意 Markdown 字符串

```C#
using Cyjb.Markdown;
using Cyjb.Markdown.Renderer;

// 将 Markdown 字符串解析成语法树。
Document doc = Document.Parse("This is a **markdown**");

// 将语法树渲染成 HTML 字符串。
HtmlRenderer renderer = new();
doc.Accept(renderer);
Console.WriteLine(renderer);
// <p>This is a <strong>markdown</strong></p>
```

可以通过 `Document.Parse` 方法的 `options` 参数指定需要使用的 Markdown 特性，所有支持的 Markdown 特性参见下文。

可以利用 `SyntaxVisitor` 遍历 Markdown 语法树，提供定制的逻辑。

## 支持的 Markdown 特性

- 支持完整的 [CommonMark 0.30](https://spec.commonmark.org/0.30)，具体请参见[这里](https://commonmark.org/help/)。
- 支持部分 [GitHub Flavored Markdown (GFM)](https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)，具体包括：
	- [删除线](https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#styling-text)：`~~ ~~`。
	- [任务列表](https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#task-lists)：`- [ ] task`。
- 支持额外的列表样式，具体请参见[这里](https://github.com/CYJB/Cyjb.Markdown/blob/main/doc/extra-list-style-type.md)。

### 其它的建议用法

- 下标：`<sub> </sub>`。
- 上标：`<sup> </sup>`。
- 下划线：`<u> </u>`。

Markdown 解析用到了由 [Cyjb.Compilers.Design](https://github.com/CYJB/Cyjb.Compilers/blob/master/Design/README.md) 在设计时生成的词法分析器，因此预计不会提供运行时扩展 Markdown 特性的能力。

详细的类库文档，请参见 [Wiki](https://github.com/CYJB/Cyjb.Markdown/wiki)。

欢迎访问我的[博客](http://www.cnblogs.com/cyjb/)获取更多信息。

