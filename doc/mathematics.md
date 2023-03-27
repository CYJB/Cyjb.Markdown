# 数学公式

版本：`1.0`

数学公式分包含*行级数学公式（Math Span*）和*块级数学公式（Math Block*）两类。

## 行级数学公式

行级数学公式以至少一个 `$` 开始，并以相同长度的 `$` 结束，起始 `$` 前不能是反斜杠（`/`）。`$` 内部的字符串就是数学公式的内容。

如果数学公式的内容以空格（或换行）开始和结束，且不完全由空格（或换行）组成，那么会移除内容前后的一个空格。这样允许包含以 `$` 开头或结尾的数学公式。

使用一个 `$` 时，起始 `$` 右边必须有一个非空白（非空格、Tab 或换行）字符，结束 `$` 的左边同样必须非空白字符，且后面不能紧跟数字。因此 `$20,000 and $30,000` 不会被解析为数学公式。

使用两个或更多 `$` 时，则没有此限制。

如果需要在文本中包含 `$` 字符，可以使用反斜杠转义。

为了便于与其它数学公式库（如 [MathJax](https://www.mathjax.org/)）集成，输出 HTML 时可以在内容前后添加 `\(` 和 `\)` 分隔符。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
$foo$
```
```html
<p><span class="math">\(foo\)</span></p>
```

由于内容包含 `$`，这里使用两个 `$` 作为分隔符。同时演示了会移除数学公式前后的一个空格。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
$$ foo $ bar $$
```
```html
<p><span class="math">\(foo $ bar\)</span></p>
```

数学公式以空格开始或结束时，必须使用两个 `$`，否则不会被识别未数学公式。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
$ $$ $
```
```html
<p>$ $$ $</p>
```

使用一个 `$` 作为分隔符时，结束 `$` 后也不能是数字。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
$20,000 and $30,000
```
```html
<p>$20,000 and $30,000</p>
```

在数学公式以 `$` 开始或结束时，很适合移除前后的一个空格。

<a id="example-5" href="#example-5">示例 5</a>

```markdown
$$$ $$ $$$
```
```html
<p><span class="math">\($$\)</span></p>
```

注意只会移除*一个*空格。

<a id="example-6" href="#example-6">示例 6</a>

```markdown
$$$  $$  $$$
```
```html
<p><span class="math">\( $$ \)</span></p>
```

只有内容两端都有空格时，才会移除空格。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
$$$ a$$$
```
```html
<p><span class="math">\( a\)</span></p>
```

只会移除**空格**，而非**Unicode 空白**。这里使用的是全角空格。

<a id="example-8" href="#example-8">示例 8</a>

```markdown
$　a　$
```
```html
<p><span class="math">\(　a　\)</span></p>
```

在内容只有空格组成时，不会移除两端的空格。

<a id="example-9" href="#example-9">示例 9</a>

```markdown
$$ $$
$$  $$
```
```html
<p><span class="math">\( \)</span>
<span class="math">\(  \)</span></p>
```

起始和结尾的换行也会被移除，但内部的空格或换行都会被原样保留。

<a id="example-10" href="#example-10">示例 10</a>

```markdown
a $$
foo  bar
baz  
bim
$$ $
```
```html
<p>a <span class="math">\(foo  bar
baz  
bim\)</span> $</p>
```

在数学公式内，反斜杠转义不会生效。

<a id="example-11" href="#example-11">示例 11</a>

```markdown
$foo\$bar$
```
```html
<p><span class="math">\(foo\\)</span>bar$</p>
```

也可以通过增加分隔 `$` 的个数来避免转义内部 `$`。

<a id="example-12" href="#example-12">示例 12</a>

```markdown
$$foo$bar$$
```
```html
<p><span class="math">\(foo$bar\)</span></p>
```

<a id="example-13" href="#example-13">示例 13</a>

```markdown
$$$ foo $$ bar $$$
```
```html
<p><span class="math">\(foo $$ bar\)</span></p>
```

数学公式与代码段具有相同的优先级，高于 HTML 标签和自动链接之外的其它行级元素。

<a id="example-14" href="#example-14">示例 14</a>

```markdown
*foo$*$
```
```html
<p>*foo<span class="math">\(*\)</span></p>
```

这里也不会解析为链接。

<a id="example-15" href="#example-15">示例 15</a>

```markdown
[not a $link](/foo$)
```
```html
<p>[not a <span class="math">\(link](/foo\)</span>)</p>
```

与代码段具有相同优先级。

<a id="example-16" href="#example-16">示例 16</a>

```markdown
`code$foo$`
$math`code`$
```
```html
<p><code>code$foo$</code>
<span class="math">\(math`code`\)</span></p>
```

与 HTML 标签具有相同优先级。

<a id="example-17" href="#example-17">示例 17</a>

```markdown
<a href="$">$
$<a href="$">$
```
```html
<p><a href="$">$
<span class="math">\(&lt;a href=&quot;\)</span>&quot;&gt;$</p>
```

与自动链接具有相同优先级。

<a id="example-18" href="#example-18">示例 18</a>

```markdown
<http://foo.bar.$baz>$
$<http://foo.bar.$baz>$
```
```html
<p><a href="http://foo.bar.$baz">http://foo.bar.$baz</a>$
<span class="math">\(&lt;http://foo.bar.\)</span>baz&gt;$</p>
```

如果起始 `$` 并未被相同长度的 `$` 字符串闭合，直接作为普通字符串使用。

<a id="example-19" href="#example-19">示例 19</a>

```markdown
$$$foo$$
```
```html
<p>$$$foo$$</p>
```

<a id="example-20" href="#example-20">示例 20</a>

```markdown
$foo
```
```html
<p>$foo</p>
```

起始和结束 `$` 必须有相同长度。

<a id="example-21" href="#example-21">示例 21</a>

```markdown
$foo$$bar$$
```
```html
<p>$foo<span class="math">\(bar\)</span></p>
```

## 块级数学公式

块级数学公式以至少两个 `$` 开始，前面至多有三个空格的缩进。起始行可以包含可选文本，会移除前后空白，作为信息字符串使用。

数学公式的内容包含后续所有行，直到至少与起始分隔符数量相同的 `$` 作为结束分隔符。结束分隔符之前至多可有三个空格的缩进，且之后不能包含空格和 Tab 以外的其它的字符。

如果起始分隔符前有 `N` 格缩进，则从每行内容（如果存在）中删除至多 `N` 格缩进。

如果以到达容器块（或文档）的末尾但未找到结束分隔符，那么数学公式将包含起始分隔符之后的所有行，直到容器块（或文档）的末尾。

块级数学公式可以中断段落，且前后都不需要空行分割。

为了便于与其它数学公式库（如 [MathJax](https://www.mathjax.org/)）集成，输出 HTML 时可以在内容前后添加 `\[` 和 `\]` 分隔符。

<a id="example-22" href="#example-22">示例 22</a>

```markdown
$$
<
 >
$$
```
```html
<div class="math">\[&lt;
 &gt;
\]</div>
```

`$` 的个数必须大于等于两个。

<a id="example-23" href="#example-23">示例 23</a>

```markdown
$
foo
$
```
```html
<p>$
foo
$</p>
```

结束分隔符至少要与起始分隔符的长度相同。

<a id="example-24" href="#example-24">示例 24</a>

```markdown
$$$$
foo
$$$
$$$$$$
```
```html
<div class="math">\[foo
$$$
\]</div>
```

未闭合的数学公式块会在容器块（或文档）结束的位置闭合。

<a id="example-25" href="#example-25">示例 25</a>

```markdown
$$
```
```html
<div class="math">\[\]</div>
```

<a id="example-26" href="#example-26">示例 26</a>

```markdown
$$$$$

$$
aaa
```
```html
<div class="math">\[
$$
aaa
\]</div>
```

<a id="example-27" href="#example-27">示例 27</a>

```markdown
> $$
> aaa

bbb
```
```html
<blockquote>
<div class="math">\[aaa
\]</div>
</blockquote>
<p>bbb</p>
```

数学公式可以只包含空行作为内容。

<a id="example-28" href="#example-28">示例 28</a>

```markdown
$$

  
$$
```
```html
<div class="math">\[
  
\]</div>
```

数学公式可以是空的。

<a id="example-29" href="#example-29">示例 29</a>

```markdown
$$
$$
```
```html
<div class="math">\[\]</div>
```

数学公式可以被缩进。如果起始分隔符被缩进了，那么每个内容行都会被移除等量的缩进。

<a id="example-30" href="#example-30">示例 30</a>

```markdown
 $$
 aaa
aaa
$$
```
```html
<div class="math">\[aaa
aaa
\]</div>
```

<a id="example-31" href="#example-31">示例 31</a>

```markdown
  $$
aaa
  aaa
aaa
  $$
```
```html
<div class="math">\[aaa
aaa
aaa
\]</div>
```

<a id="example-32" href="#example-32">示例 32</a>

```markdown
   $$
   aaa
    aaa
  aaa
   $$
```
```html
<div class="math">\[aaa
 aaa
aaa
\]</div>
```

四个空格的缩进就太多了。

<a id="example-33" href="#example-33">示例 33</a>

```markdown
    $$
    aaa
    $$
```
```html
<pre><code>$$
aaa
$$
</code></pre>
```

结束分隔符最多可以包含三个空格的缩进，缩进并不需要与起始分隔符一致。

<a id="example-34" href="#example-34">示例 34</a>

```markdown
$$
aaa
  $$
```
```html
<div class="math">\[aaa
\]</div>
```

<a id="example-35" href="#example-35">示例 35</a>

```markdown
   $$
aaa
  $$
```
```html
<div class="math">\[aaa
\]</div>
```

包含四个缩进就不是结束分隔符了。

<a id="example-36" href="#example-36">示例 36</a>

```markdown
$$
aaa
    $$
```
```html
<div class="math">\[aaa
    $$
\]</div>
```

分隔符不能包含内部空格或 Tab。

<a id="example-37" href="#example-37">示例 37</a>

```markdown
$$ $$
aaa
```
```html
<p><span class="math">\( \)</span>
aaa</p>
```

<a id="example-38" href="#example-38">示例 38</a>

```markdown
$$$$$$
aaa
$$$ $$
```
```html
<div class="math">\[aaa
$$$ $$
\]</div>
```

数学公式块可以中断段落，并且可以直接后跟段落而不需要空行分割。

<a id="example-39" href="#example-39">示例 39</a>

```markdown
foo
$$
bar
$$
baz
```
```html
<p>foo</p>
<div class="math">\[bar
\]</div>
<p>baz</p>
```

其它块也可以直接出现在数学公式块后面。

<a id="example-40" href="#example-40">示例 40</a>

```markdown
foo
---
$$
bar
$$
# baz
```
```html
<h2>foo</h2>
<div class="math">\[bar
\]</div>
<h1>baz</h1>
```

在起始分隔符后可以提供信息字符串。规范并未规定信息字符串应当如何被使用。

<a id="example-41" href="#example-41">示例 41</a>

```markdown
$$ info startline=3 `%@#~
1 + 1
$$
```
```html
<div class="math">\[1 + 1
\]</div>
```

信息字符串不能包含 `$` 字符。

<a id="example-42" href="#example-42">示例 42</a>

```markdown
$$ info $$
foo
```
```html
<p><span class="math">\(info\)</span>
foo</p>
```

结束分隔符不能包含信息字符串。

<a id="example-43" href="#example-43">示例 43</a>

```markdown
$$
$$ aaa
$$
```
```html
<div class="math">\[$$ aaa
\]</div>
```

数学公式块同样支持自定义属性。

<a id="example-44" href="#example-44">示例 44</a>

```markdown
$$ {
  #my-id .class2
  key=value
}
foo
$$
```
```html
<div class="math class2" id="my-id" key="value">\[foo
\]</div>
```
