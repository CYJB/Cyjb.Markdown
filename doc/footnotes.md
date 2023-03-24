# 脚注

版本：`1.0`

参考：[GitHub Footnotes](https://docs.github.com/zh/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#footnotes)

脚注由引用和声明组成。引用从 `[^` 开始，后跟引用标签，到 `]` 结束，引用标签不能空格、Tab、换行，或者未转义的 `[` 或 `]`。脚注声明由引用后跟 `:`、可选的任意个数空白以及脚注内容组成，声明前可以包含最多 3 个空格的缩进，脚注内容可以包含多行。

脚注总是按照出现顺序依次编号，脚注的引用名称仅用于生成锚点。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
Here is a simple footnote[^1].

A footnote can also have multiple lines[^2].

You can also use words, to fit your writing style more closely[^note].

[^1]:        My reference.
[^2]: New line can prefixed with any spaces.
  This allows you to have a footnote with multiple lines.
[^note]:
    Named footnotes will still render with numbers instead of the text but allow easier identification and linking.
    This footnote also has been made with a different syntax using 4 spaces for new lines.
```
```html
<p>Here is a simple footnote<sup><a href="#fn-1" id="fnref-1">1</a></sup>.</p>
<p>A footnote can also have multiple lines<sup><a href="#fn-2" id="fnref-2">2</a></sup>.</p>
<p>You can also use words, to fit your writing style more closely<sup><a href="#fn-note" id="fnref-note">3</a></sup>.</p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p>My reference. <a href="#fnref-1">↩</a></p>
</li>
<li id="fn-2">
<p>New line can prefixed with any spaces.
This allows you to have a footnote with multiple lines. <a href="#fnref-2">↩</a></p>
</li>
<li id="fn-note">
<p>Named footnotes will still render with numbers instead of the text but allow easier identification and linking.
This footnote also has been made with a different syntax using 4 spaces for new lines. <a href="#fnref-note">↩</a></p>
</li>
</ol>
</section>
```

脚注内容可以以任何顺序出现在任何位置，但总是会按照引用顺序输出到文档的结尾。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
[^first]: note 1
> [^second]: note 2
>
> foo[^second][^first].
```
```html
<blockquote>
<p>foo<sup><a href="#fn-second" id="fnref-second">1</a></sup><sup><a href="#fn-first" id="fnref-first">2</a></sup>.</p>
</blockquote>
<section id="footnotes">
<ol>
<li id="fn-second">
<p>note 2 <a href="#fnref-second">↩</a></p>
</li>
<li id="fn-first">
<p>note 1 <a href="#fnref-first">↩</a></p>
</li>
</ol>
</section>
```

会在规范化之后比较脚注的引用标签，其规范化行为与链接相同：移除首尾的 `[^` 和 `]`，执行 Unicode Case Fold 后再进行比较。如果有多个匹配的脚注声明，则使用出现更为靠前的。脚注的链接总是使用声明的标签。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
normalize: [^ẞ]

[^SS]: first
[^ss]: second
```
```html
<p>normalize: <sup><a href="#fn-SS" id="fnref-SS">1</a></sup></p>
<section id="footnotes">
<ol>
<li id="fn-SS">
<p>first <a href="#fnref-SS">↩</a></p>
</li>
</ol>
</section>
```

可以多次引用同一个脚注，会为每个引用生成唯一的 id。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
foo[^1] and bar[^1].

[^1]: note
```
```html
<p>foo<sup><a href="#fn-1" id="fnref-1">1</a></sup> and bar<sup><a href="#fn-1" id="fnref-1-1">1</a></sup>.</p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p>note <a href="#fnref-1">↩</a> <a href="#fnref-1-1">↩</a></p>
</li>
</ol>
</section>
```

脚注内容中可以包含空行，只要之后的行是被缩进的。脚注中也可以包含缩进代码段。

<a id="example-5" href="#example-5">示例 5</a>

```markdown
note[^1][^2].

[^1]: line1
line1-2

    line2
  line2-2

    line3

        code
    line4

  line5
  [^2]:
        code
```
```html
<p>note<sup><a href="#fn-1" id="fnref-1">1</a></sup><sup><a href="#fn-2" id="fnref-2">2</a></sup>.</p>
<p>line5</p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p>line1
line1-2</p>
<p>line2
line2-2</p>
<p>line3</p>
<pre><code>code
</code></pre>
<p>line4 <a href="#fnref-1">↩</a></p>
</li>
<li id="fn-2">
<pre><code>code
</code></pre>
<p><a href="#fnref-2">↩</a></p>
</li>
</ol>
</section>
```

同时满足脚注和链接声明格式时，会识别为脚注。

<a id="example-6" href="#example-6">示例 6</a>

```markdown
foo [^1][link] [link][^1] [[^1]][link].

[^1]: note

[link]:/url
```
```html
<p>foo <a href="/url">^1</a> [link]<sup><a href="#fn-1" id="fnref-1">1</a></sup> <a href="/url"><sup><a href="#fn-1" id="fnref-1-1">1</a></sup></a>.</p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p>note <a href="#fnref-1">↩</a> <a href="#fnref-1-1">↩</a></p>
</li>
</ol>
</section>
```

脚注中还可以嵌套包含其它脚注或脚注引用。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
foo [link] [^1][^2].

[^1]: [link]: /url
note1[^1][^2]
    [^2]: note2

    note1-end
```
```html
<p>foo <a href="/url">link</a> <sup><a href="#fn-1" id="fnref-1">1</a></sup><sup><a href="#fn-2" id="fnref-2">2</a></sup>.</p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p>note1<sup><a href="#fn-1" id="fnref-1-1">1</a></sup><sup><a href="#fn-2" id="fnref-2-1">2</a></sup></p>
<p>note1-end <a href="#fnref-1">↩</a> <a href="#fnref-1-1">↩</a></p>
</li>
<li id="fn-2">
<p>note2 <a href="#fnref-2">↩</a> <a href="#fnref-2-1">↩</a></p>
</li>
</ol>
</section>
```

脚注的内容可以是空的。

<a id="example-8" href="#example-8">示例 8</a>

```markdown
foo [^1]

[^1]:
```
```html
<p>foo <sup><a href="#fn-1" id="fnref-1">1</a></sup></p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p><a href="#fnref-1">↩</a></p>
</li>
</ol>
</section>
```

只有脚注最后一个节点是段落时，才会直接将反向引用添加到该段落内；否则总是会增加一个新段落。

<a id="example-9" href="#example-9">示例 9</a>

```markdown
foo [^1][^2]

[^1]: paragraph
[^2]: > block
```
```html
<p>foo <sup><a href="#fn-1" id="fnref-1">1</a></sup><sup><a href="#fn-2" id="fnref-2">2</a></sup></p>
<section id="footnotes">
<ol>
<li id="fn-1">
<p>paragraph <a href="#fnref-1">↩</a></p>
</li>
<li id="fn-2">
<blockquote>
<p>block</p>
</blockquote>
<p><a href="#fnref-2">↩</a></p>
</li>
</ol>
</section>
```

脚注的标签可以出现中文、符号等。

<a id="example-10" href="#example-10">示例 10</a>

```markdown
foo[^中文&<tag>]

[^中文&<tag>]: note
```
```html
<p>foo<sup><a href="#fn-%E4%B8%AD%E6%96%87&%3Ctag%3E" id="fnref-%E4%B8%AD%E6%96%87&%3Ctag%3E">1</a></sup></p>
<section id="footnotes">
<ol>
<li id="fn-%E4%B8%AD%E6%96%87&%3Ctag%3E">
<p>note <a href="#fnref-%E4%B8%AD%E6%96%87&%3Ctag%3E">↩</a></p>
</li>
</ol>
</section>
```

脚注自引用或互相引用时，也会生成脚注。

<a id="example-11" href="#example-11">示例 11</a>

```markdown
[^1]:note1 [^1]
[^2]:note2 [^3]
[^3]:note3 [^2]
```
```html
<section id="footnotes">
<ol>
<li id="fn-1">
<p>note1 <sup><a href="#fn-1" id="fnref-1">1</a></sup> <a href="#fnref-1">↩</a></p>
</li>
<li id="fn-3">
<p>note3 <sup><a href="#fn-2" id="fnref-2">3</a></sup> <a href="#fnref-3">↩</a></p>
</li>
<li id="fn-2">
<p>note2 <sup><a href="#fn-3" id="fnref-3">2</a></sup> <a href="#fnref-2">↩</a></p>
</li>
</ol>
</section>
```
