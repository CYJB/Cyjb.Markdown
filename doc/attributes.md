# 属性

版本：`1.0`

可以为标题、代码块、链接和图片添加属性。属性使用类似 `{#identifier .class key=value}` 的形式指定，具体格式为：

属性使用 `{` 和 `}` 包裹，内部可以包含任意个数的标识符、类名或属性，它们之间可以使用空格、Tab 或至多一个换行分割。

标识符以 `#` 开始，后跟至少一个字符（除空格、Tab、换行、`"`、`'`、`=`、`<`、`>`、`` ` ``、`{` 和 `}`）。如果存在多个标识符，会使用最后一个。

类名以 `.` 开始，后跟至少一个字符（除空格、Tab、换行、`"`、`'`、`=`、`<`、`>`、`` ` ``、`{` 和 `}`）。如果指定多个相同的类名，会重复添加到 `class` 部分。

属性包含属性名和一个可选的属性值，属性名由 ASCII 字母、`_` 或 `:` 后跟零个或更多 ASCII 字母、数字、`_`、`.`、`:` 或 `-` 组成。属性值由 `=` 和至少一个字符（除空格、Tab、换行、`"`、`'`、`=`、`<`、`>`、`` ` ``、`{` 和 `}`）组成。与 [Raw HTML](https://spec.commonmark.org/0.30/#raw-html) 相比，`=` 两边不能包含空白，且不支持使用 `"` 或 `'` 括起来的属性值。如果存在多个具有相同键的属性，会使用最后一个值。

## 标题属性

可以在标题的末尾指定属性，属性会附加到标题的元素上。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
# foo {#id .class attr=value attr2=value2}

bar { .class2  #id2 .class2 .class3}
---

### baz {#other}{#id #id3}
```
```html
<h1 id="id" class="class" attr="value" attr2="value2">foo</h1>
<h2 class="class2 class2 class3" id="id2">bar</h2>
<h3 id="id3">baz {#other}</h2>
```

允许在标题中使用多行属性。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
# foo {
#id-foo
   key=value   
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
} bar
</p>
<h2 class="class">bar { # }</h2>
<h1>baz {#id}</h1>
<h1 id="id3">bim {#id} {#id2}</h1>
```

属性前不能是未转义的 `\` 符号。

<a id="example-5" href="#example-5">示例 5</a>

```markdown
# foo \{#id}
# foo \\{#id}
```
```html
<h1>foo {#id}</h1>
<h1 id="id">foo \</h1>
```
