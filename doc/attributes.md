# 属性

版本：`1.1`

参考：[pandoc attributes](https://pandoc.org/MANUAL.html#extension-attributes)

可以为标题、代码块、链接和图片添加属性。属性使用类似 `{#identifier .class key=value}` 的形式指定，具体格式为：

属性使用 `{` 和 `}` 包裹，内部可以包含任意个数的标识符、类名或属性，它们之间可以使用空格、Tab 或至多一个换行分割。

标识符以 `#` 开始，后跟至少一个字符（除空格、Tab、换行、`"`、`'`、`=`、`<`、`>`、`` ` ``、`{` 和 `}`）。如果存在多个标识符，会使用最后一个。

类名以 `.` 开始，后跟至少一个字符（除空格、Tab、换行、`"`、`'`、`=`、`<`、`>`、`` ` ``、`{` 和 `}`）。如果指定多个相同的类名，会重复添加到 `class` 部分。

属性与 [Raw HTML](https://spec.commonmark.org/0.30/#raw-html) 类似，但要求 `=` 两边不能包含空白，并且未用引号括起来的属性值不能包含 `{` 或 `}`。如果存在多个具有相同键的属性，会使用最后一个值。

## 标题属性

可以在标题的末尾指定属性，属性会附加到标题的元素上。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
# foo {#id .class attr=value attr2="value={2}"}

bar { .class2  #id2 .class2 .class3}
---

### baz {#other}{#id #id3}
```
```html
<h1 id="id" class="class" attr="value" attr2="value={2}">foo</h1>
<h2 class="class2 class2 class3" id="id2">bar</h2>
<h3 id="id3">baz {#other}</h3>
```

允许在标题中使用多行属性。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
# foo {
#id-foo
   key='value'   
}

bar
{
  .class
}
---
```
```html
<h1 id="id-foo" key="value">foo</h1>
<h2 class="class">bar
</h2>
```

会忽略属性后的空格或 Tab。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
# foo {key=value} 	

bar { .class }  	
---
```
```html
<h1 key="value">foo</h1>
<h2 class="class">bar</h2>
```

如果属性不合法，或者不在标题的末尾，那么会当成普通文本。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
# foo {
#id-foo
} bar

bar { # }
---
# baz \{#id}  
# bim \{#id} {#id2} {#id3}
```
```html
<h1>foo {</h1>
<p>#id-foo
} bar</p>
<h2>bar { # }</h2>
<h1>baz {#id}</h1>
<h1 id="id3">bim {#id} {#id2}</h1>
```

属性前不能是未转义的 `\` 符号。

<a id="example-5" href="#example-5">示例 5</a>

```markdown
# foo \{#id}
# foo \\{#id my-key}
```
```html
<h1>foo {#id}</h1>
<h1 id="id" my-key="">foo \</h1>
```

`#` 或 `.` 后面必须包含有效字符，`=` 前后也必须紧跟属性键或值。

<a id="example-6" href="#example-6">示例 6</a>

```markdown
# foo {#}
# bar { . }
# baz { = }
# bim { =value }
# boo { k = v}
```
```html
<h1>foo {#}</h1>
<h1>bar { . }</h1>
<h1>baz { = }</h1>
<h1>bim { =value }</h1>
<h1>boo { k = v}</h1>
```

属性中至多使用一个换行分割，不能包含更多换行。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
# foo {

  #id }
# bar { key=
value }
# baz {
.class

}
```
```html
<h1>foo {</h1>
<p>#id }</p>
<h1>bar { key=</h1>
<p>value }</p>
<h1>baz {</h1>
<p>.class</p>
<p>}</p>
```

属性值中不允许包含换行。

<a id="example-8" href="#example-8">示例 8</a>

```markdown
# foo { key="val
ue" }
```
```html
<h1>foo { key=&quot;val</h1>
<p>ue&quot; }</p>
```

## 代码块属性

可以在代码块信息的末尾指定属性，属性会附加到代码块的元素上。

<a id="example-9" href="#example-9">示例 9</a>

`````markdown
``` foo {#id .class attr=value attr2="value={2}"}
  bar
```
``` baz { .class2  #id2 .class2 .class3}
bim
```
`````
`````html
<pre><code class="language-foo class" id="id" attr="value" attr2="value={2}">  bar
</code></pre>
<pre><code class="language-baz class2 class2 class3" id="id2">bim
</code></pre>
`````

允许在代码块中使用多行属性，起始 `{` 必须与代码块的信息在一行内。

<a id="example-10" href="#example-10">示例 10</a>

```markdown
~~~ foo {
     key='value'   
    }
bar
~~~
~~~~~ baz  
{   
  .class
    }
  bim  
~~~~~
```
```html
<pre><code class="language-foo" key="value">bar
</code></pre>
<pre><code class="language-baz">{   
  .class
    }
  bim  
</code></pre>
```

会忽略属性后的空格或 Tab。

<a id="example-11" href="#example-11">示例 11</a>

```markdown
~~~ foo {key=value}   
bar
~~~
```
```html
<pre><code class="language-foo" key="value">bar
</code></pre>
```

如果属性不合法，或者不在代码块信息的末尾，那么会当成普通文本。

<a id="example-12" href="#example-12">示例 12</a>

```markdown
~~~ foo {
  #id-foo
} bar
~~~
~~~ bim { # }
boo
~~~
~~~ b1 \{#id}  
b2
~~~
~~~ b3 \{#id} {#id2} {#id3}
b4
~~~
```
```html
<pre><code class="language-foo">  #id-foo
} bar
</code></pre>
<pre><code class="language-bim">boo
</code></pre>
<pre><code class="language-b1">b2
</code></pre>
<pre><code class="language-b3" id="id3">b4
</code></pre>
```

属性前不能是未转义的 `\` 符号。

<a id="example-13" href="#example-13">示例 13</a>

```markdown
~~~ foo \{#id}
bar
~~~
~~~ foo\\{#id my-key}
bar
~~~
```
```html
<pre><code class="language-foo">bar
</code></pre>
<pre><code class="language-foo\" id="id" my-key="">bar
</code></pre>
```

`#` 或 `.` 后面必须包含有效字符，`=` 前后也必须紧跟属性键或值。

<a id="example-14" href="#example-14">示例 14</a>

```markdown
~~~foo{#}
~~~
~~~ bar{ . }
~~~
~~~ baz{ = }
~~~
~~~ bim { =value }
~~~
~~~ boo{k = v}
~~~
```
```html
<pre><code class="language-foo{#}"></code></pre>
<pre><code class="language-bar{"></code></pre>
<pre><code class="language-baz{"></code></pre>
<pre><code class="language-bim"></code></pre>
<pre><code class="language-boo{k"></code></pre>
```

属性中至多使用一个换行分割，不能包含更多换行。

<a id="example-15" href="#example-15">示例 15</a>

```markdown
~~~ foo {

  #id }
~~~
~~~ bar { key=
value }
~~~
~~~ baz {
.class

}
~~~
```
```html
<pre><code class="language-foo">
  #id }
</code></pre>
<pre><code class="language-bar">value }
</code></pre>
<pre><code class="language-baz">.class

}
</code></pre>
```

属性值中不允许包含换行。

<a id="example-16" href="#example-16">示例 16</a>

```markdown
~~~ foo { key="
value"}
~~~
```
```html
<pre><code class="language-foo">value&quot;}
</code></pre>
```

## 链接属性

可以在链接的末尾指定属性，属性会附加到链接的元素上。如果是附加到链接声明的属性，会附加到所有相关链接。

<a id="example-17" href="#example-17">示例 17</a>

```markdown
<http://foo>{#id .class attr=value attr2="value={2}"}

[bar](/uri){ .class2  #id2 .class2 .class3}

[baz text][baz]
[baz]

[baz]:/uri2 {#id #id3}
```
```html
<p><a id="id" class="class" attr="value" attr2="value={2}" href="http://foo">http://foo</a></p>
<p><a class="class2 class2 class3" id="id2" href="/uri">bar</a></p>
<p><a id="id3" href="/uri2">baz text</a>
<a id="id3" href="/uri2">baz</a></p>
```

链接与属性之前不能有空白，但是在链接声明中，属性之前必须要有空白。

<a id="example-18" href="#example-18">示例 18</a>

```markdown
<http://foo> {#id}
[bar](/uri)
{.class}
[baz]

[baz]:/uri2{#id #id3}
```
```html
<p><a href="http://foo">http://foo</a> {#id}
<a href="/uri">bar</a>
{.class}
[baz]</p>
<p>[baz]:/uri2{#id #id3}</p>
```

使用链接声明时，只会使用链接声明中的属性，而不会再识别附加的属性。

<a id="example-19" href="#example-19">示例 19</a>

```markdown
[baz]{#id}
[baz text][baz]{#id}

[baz]:/uri "title" {.class}
```
```html
<p><a class="class" href="/uri" title="title">baz</a>{#id}
<a class="class" href="/uri" title="title">baz text</a>{#id}</p>
```

允许在链接中使用多行属性。

<a id="example-20" href="#example-20">示例 20</a>

```markdown
<http://foo>{
   key='value'   
}
[bar](/uri){.class
#id}
[baz]

[baz]:/uri2  
{
  #id2 key="v
  alue"
  .class  .class2
  key2=value2
}
```
```html
<p><a key="value" href="http://foo">http://foo</a>
<a class="class" id="id" href="/uri">bar</a>
<a id="id2" key="v
  alue" class="class class2" key2="value2" href="/uri2">baz</a></p>
```

链接属性后的内容会当作普通字符处理，链接声明的属性后不能包含其它内容。

<a id="example-21" href="#example-21">示例 21</a>

```markdown
<http://foo>{key=value} 	
[bar](/uri){ .class }  text
[baz]
[baz3]

[baz]:/uri2  
  "title" { key="value{}"}  
[baz3]:/uri3  
  "title3" {#id3} other
```
```html
<p><a key="value" href="http://foo">http://foo</a> 	
<a class="class" href="/uri">bar</a>  text
<a key="value{}" href="/uri2" title="title">baz</a>
<a href="/uri3">baz3</a></p>
<p>&quot;title3&quot; {#id3} other</p>
```

属性前不能是未转义的 `\` 符号。

<a id="example-22" href="#example-22">示例 22</a>

```markdown
[foo]

[foo]:/uri \{#id}

[foo]:/uri2 \\{#id}
```
```html
<p>[foo]</p>
<p>[foo]:/uri {#id}</p>
<p>[foo]:/uri2 \{#id}</p>
```

`#` 或 `.` 后面必须包含有效字符，`=` 前后也必须紧跟属性键或值。

<a id="example-23" href="#example-23">示例 23</a>

```markdown
<http://foo>{#}
[bar](/uri){ . }
[baz]

[baz]:/uri2 { = }

[baz]:/uri3 { =value }

[baz]:/uri3 { k = v }
```
```html
<p><a href="http://foo">http://foo</a>{#}
<a href="/uri">bar</a>{ . }
[baz]</p>
<p>[baz]:/uri2 { = }</p>
<p>[baz]:/uri3 { =value }</p>
<p>[baz]:/uri3 { k = v }</p>
```

属性中至多使用一个换行分割，不能包含更多换行。

<a id="example-24" href="#example-24">示例 24</a>

```markdown
<http://foo>{

  #id }
[bar](/uri){ key=
value }
[baz]

[baz]:/uri2 {
.class

}
```
```html
<p><a href="http://foo">http://foo</a>{</p>
<p>#id }
<a href="/uri">bar</a>{ key=
value }
[baz]</p>
<p>[baz]:/uri2 {
.class</p>
<p>}</p>
```

属性值中不允许包含换行。

<a id="example-25" href="#example-25">示例 25</a>

```markdown
<https://foo>{#id key="v'
 alue'" }
~~~
```
```html
<p><a href="https://foo">https://foo</a>{#id key=&quot;
alue'&quot; }</p>
```
