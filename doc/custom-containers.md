# 自定义容器

版本：`1.0`

自定义容器以至少三个 `:` 开始，前面最多可以有三个空白的缩进。包含 `:` 的行可以包含可选的文本，会在移除两端空白后作为自定义容器的信息字符串。最后以至少与起始 `:` 长度相同的分隔符结束，末尾 `:` 之后不能包含其它非空白字符。

自定义容器可以生成 `<div>...</div>` 结构，内部可以包含其它 Markdown 节点。信息字符串首个空格前的部分一般用于指定自定义容器的类型，一般会渲染为 `<div>` 标签的 `class` 属性。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
:::
foo *bar*

baz
:::
```
```html
<div>
<p>foo <em>bar</em></p>
<p>baz</p>
</div>
```

少于三个 `:` 是无效的。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
::
foo
::
```
```html
<p>::
foo
::</p>
```

自定义容器允许嵌套，但要求内部分隔符的长度短于外部分隔符。较长的分隔符可能会将内部自定义容器一起关闭。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
::::: type-1
foo
:::
bar
:::
baz
:::::

:::
foo
::::::: type-2
bar
:::::::
baz
```
```html
<div class="type-1">
<p>foo</p>
<div>
<p>bar</p>
</div>
<p>baz</p>
</div>
<div>
<p>foo</p>
<div class="type-2">
<p>bar</p>
</div>
</div>
<p>baz</p>
```

未闭合的自定义容器会在文档结束（或父容器结束）时闭合。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
:::
```
```html
<div>
</div>
```

<a id="example-5" href="#example-5">示例 5</a>

```markdown
:::::

:::
foo
```
```html
<div>
<div>
<p>foo</p>
</div>
</div>
```

<a id="example-6" href="#example-6">示例 6</a>

```markdown
> :::::
> foo
:::
bar
```
```html
<blockquote>
<div>
<p>foo</p>
</div>
</blockquote>
<div>
<p>bar</p>
</div>
```

自定义容器可以只包含空白行。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
:::

  
:::
```
```html
<div>
</div>
```

自定义容器也可以是空的。

<a id="example-8" href="#example-8">示例 8</a>

```markdown
:::
:::
```
```html
<div>
</div>
```

自定义容器的起始分隔符可以包含至多三个空格的缩进，但不会影响内部节点的解析。

<a id="example-9" href="#example-9">示例 9</a>

```markdown
   :::
    code
:::
```
```html
<div>
<pre><code>code
</code></pre>
</div>
```

四个空格的缩进就太多了。

<a id="example-10" href="#example-10">示例 10</a>

```markdown
    :::
foo
:::
```
```html
<pre><code>:::
</code></pre>
<p>foo</p>
<div>
</div>
```

结束分隔符也可以包含至多三个空白的缩进，缩进并不需要与起始分隔符一致。

<a id="example-11" href="#example-11">示例 11</a>

```markdown
 :::
foo
  :::
```
```html
<div>
<p>foo</p>
</div>
```

由于有四个空白，因此并不会被识别成结束分隔符。

<a id="example-12" href="#example-12">示例 12</a>

```markdown
:::
foo
    :::
```
```html
<div>
<p>foo
:::</p>
</div>
```

结束分隔符不能包含内部的空格或 Tab。

<a id="example-13" href="#example-13">示例 13</a>

```markdown
::::::
foo
::: :::
```
```html
<div>
<p>foo</p>
<div class=":::">
</div>
</div>
```

自定义容器可以直接中断段落，或者直接后跟段落，必须要使用空行分隔。

<a id="example-14" href="#example-14">示例 14</a>

```markdown
foo
:::
bar
:::
baz
```
```html
<p>foo</p>
<div>
<p>bar</p>
</div>
<p>baz</p>
```

其它块也可以直接紧邻自定义容器。

<a id="example-15" href="#example-15">示例 15</a>

```markdown
foo
---
:::
bar
:::
# baz
```
```html
<h2>foo</h2>
<div>
<p>bar</p>
</div>
<h1>baz</h1>
```

自定义容器的起始分隔符后可包含信息，首个空格前的部分一般会作为 `<div>` 的 `class` 属性使用。

<a id="example-16" href="#example-16">示例 16</a>

```markdown
:::detail
foo
:::
```
```html
<div class="detail">
<p>foo</p>
</div>
```

<a id="example-17" href="#example-17">示例 17</a>

```markdown
::: detail other info $%@:#$ 
foo
:::
```
```html
<div class="detail">
<p>foo</p>
</div>
```

闭合分隔符不能包含信息。

<a id="example-18" href="#example-18">示例 18</a>

```markdown
:::
foo
::: detail
```
```html
<div>
<p>foo</p>
<div class="detail">
</div>
</div>
```

与代码块类似，自定义容器同样支持自定义属性。

<a id="example-19" href="#example-19">示例 19</a>

```markdown
::: detail {
  #my-id .class2
  key=value
}
foo
:::
```
```html
<div class="detail class2" id="my-id" key="value">
<p>foo</p>
</div>
```

自定义容器内可以包含其它结构。

<a id="example-20" href="#example-20">示例 20</a>

```markdown
- list1
  ::: detail
  - foo
  > bar
  :::
- list2
```
```html
<ul>
<li>list1
<div class="detail">
<ul>
<li>foo</li>
</ul>
<blockquote>
<p>bar</p>
</blockquote>
</div>
</li>
<li>list2</li>
</ul>
```
