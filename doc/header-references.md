# 标题引用

版本：`1.0`

可以直接将标题内容作为链接标签使用，而不需要显式指定标题的链接。注意需要启用[自动生成标识符](./auto-identifiers.md)的能力，或者利用[标题属性](./attributes.md)显式指定标题的标识符。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
# Heading 1
# Heading 2 {#sec2}

[Heading 1]
[Heading 2][]
[Second Section][heading  2]
```
```html
<h1 id="heading-1">Heading 1</h1>
<h1 id="sec2">Heading 2</h1>
<p><a href="#heading-1">Heading 1</a>
<a href="#sec2">Heading 2</a>
<a href="#sec2">Second Section</a></p>
```

如果有多个相同内容的标题，那么总是链接到首次出现的标题。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
# Heading {#sec1}
# Heading {#sec2}

[Heading]
```
```html
<h1 id="sec1">Heading</h1>
<h1 id="sec2">Heading</h1>
<p><a href="#sec1">Heading</a></p>
```

标题的优先级总是低于链接声明，无论其顺序如何。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
# Foo

[foo]: bar

See [foo]
```
```html
<h1 id="foo">Foo</h1>
<p>See <a href="bar">foo</a></p>
```

标题的属性不会随着链接引用传递。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
# Foo {.class}

[foo]
```
```html
<h1 class="class" id="foo">Foo</h1>
<p><a href="#foo">foo</a></p>
```
