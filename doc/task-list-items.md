# 任务列表项

[GitHub Flavored Markdown Spec](https://github.github.com/gfm/#task-list-items-extension-) 并未给出详细的任务列表项定义，根据一些测试和对比，这里采用以下的规范。

任务列表项是一个列表项，它的首个内容块是以*任务列表项标记*开头的段落，并且与之后的内容间至少存在一个空格或 Tab。

*任务列表项标记*可以包含最多 3 个空格的缩进，一个左方括号（`[`），一个空白缩进或字符 `X` 或 `x`，和右方括号（`]`）。

*任务列表项标记*会被渲染成复选框元素，在 HTML 中可能是 `<input type="checkbox">`。复选框可以禁止用户交互，也可以允许用户交互，具体视实现而定。

如果方括号间是空白，那么复选框是未被选中的。否则复选框是被选中的。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
- [ ] foo
- [x] bar
```
```html
<ul>
<li><input type="checkbox" disabled>foo</li>
<li><input type="checkbox" disabled checked>bar</li>
</ul>
```

任务列表项可以是嵌套的。

<a id="example-2" href="#example-2">示例 2</a>

```markown
- [x] foo
  - [ ] bar
  - [x] baz
- [ ] bim
```
```html
<ul>
<li><input type="checkbox" disabled checked>foo
<ul>
<li><input type="checkbox" disabled>bar</li>
<li><input type="checkbox" disabled checked>baz</li>
</ul>
</li>
<li><input type="checkbox" disabled>bim</li>
</ul>
```

任务列表项可以与有序列表一起使用，或者是不同的列表项标志。

<a id="example-3" href="#example-3">示例 3</a>

```markown
1. [ ] one
2. [x] two
3. [ ] three
   - [ ] foo
   * [X] bar
     + [X] baz
```
```html
<ol>
<li><input type="checkbox" disabled>one</li>
<li><input type="checkbox" disabled checked>two</li>
<li><input type="checkbox" disabled>three
<ul>
<li><input type="checkbox" disabled>foo</li>
</ul>
<ul>
<li><input type="checkbox" disabled checked>bar
<ul>
<li><input type="checkbox" disabled checked>baz</li>
</ul>
</li>
</ul>
</li>
</ol>
```

任务列表项只能用于列表。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
[ ] foo
[x] bar
 [ ] baz
 [X] bim
```
```html
<p>[ ] foo
[x] bar
[ ] baz
[X] bim</p>
```

任务列表项前可以包含最多三个空白（注意列表项标记后的一个空白不计入在内）。

<a id="example-5" href="#example-5">示例 5</a>

```markdown
- [ ]  foo
-  [ ] bar
-   [x] baz
-    [ ] bim
```
```html
<ul>
<li><input type="checkbox" disabled> foo</li>
<li><input type="checkbox" disabled>bar</li>
<li><input type="checkbox" disabled checked>baz</li>
<li><input type="checkbox" disabled>bim</li>
</ul>
```

四个空白就太多了，也不能包含其它字符。

<a id="example-6" href="#example-6">示例 6</a>

```markdown
-     [ ] foo
- bar [ ] baz
```
```html
<ul>
<li>
<pre><code>[ ] foo
</code></pre>
</li>
<li>bar [ ] baz</li>
</ul>
```

必须使用成对的方括号。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
- [x one
- [[x two
- x] three
- x]] four
- (x) five
- {x} six
- [[x]] seven
```
```html
<ul>
<li>[x one</li>
<li>[[x two</li>
<li>x] three</li>
<li>x]] four</li>
<li>(x) five</li>
<li>{x} six</li>
<li>[[x]] seven</li>
</ul>
```

未选中的情况下，方括号中必须有且之后一个缩进。注意这里 `baz` 项中是一个全角空格。

<a id="example-8" href="#example-8">示例 8</a>

```markdown
- [  ] foo
-  [→] bar
- [　] baz
- [] bim
```
```html
<ul>
<li>[  ] foo</li>
<li>[→] bar</li>
<li>[　] baz</li>
<li>[] bim</li>
</ul>
```

只要是一个缩进就行，这个缩进可以是由 Tab 形成的。这里由于 Tab 前恰好有 3 列，Tab 本身的宽度只有 1 列。

<a id="example-9" href="#example-9">示例 9</a>

```markdown
- [→] foo
```
```html
<ul>
<li><input type="checkbox" disabled>foo</li>
</ul>
```

选中的情况下，方括号中的 `x` 不区分大小写，但不能包含其它内容。

<a id="example-10" href="#example-10">示例 10</a>

```markdown
- [x] foo
- [X] bar
- [x ] baz
- [xx] bim
- [o] boo
```
```html
<ul>
<li><input type="checkbox" disabled checked>foo</li>
<li><input type="checkbox" disabled checked>bar</li>
<li>[x ] baz</li>
<li>[xx] bim</li>
<li>[o] boo</li>
</ul>
```

方括号后必须包含至少一个空白的缩进，且这个空白不会包含在内容中。

<a id="example-11" href="#example-11">示例 11</a>

```markdown
- [x]   foo
- [ ]→bar
- [ ]baz
```
```html
<ul>
<li><input type="checkbox" disabled checked>  foo</li>
<li><input type="checkbox" disabled>  bar</li>
<li>[ ]baz</li>
</ul>
```

方括号后必须包含内容，不能为空或者只包含空白。

<a id="example-12" href="#example-12">示例 12</a>

```markdown
- [ ]
- [x]
- [ ]  
- [X]→
```
```html
<ul>
<li>[ ]</li>
<li>[x]</li>
<li>[ ]</li>
<li>[X]</li>
</ul>
```

方括号后总是被识别为段落。

<a id="example-13" href="#example-13">示例 13</a>

```markdown
- [ ] > foo
- [x] - bar
- [ ]     baz
- [ ] [x] bim
- [x] - [x] boo
- [ ] one
  [ ] two
  > [ ] three
```
```html
<ul>
<li><input type="checkbox" disabled>&gt; foo</li>
<li><input type="checkbox" disabled checked>- bar</li>
<li><input type="checkbox" disabled>    baz</li>
<li><input type="checkbox" disabled>[x] bim</li>
<li><input type="checkbox" disabled checked>- [x] boo</li>
<li><input type="checkbox" disabled>one
[ ] two
<blockquote>
<p>[ ] three</p>
</blockquote>
</li>
</ul>
```
