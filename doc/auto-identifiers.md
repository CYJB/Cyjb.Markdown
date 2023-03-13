# 自动生成标识符

版本：`1.0`

可以根据标题内容自动生成标识符，如果已经利用[标题属性](./attributes.md)显式指定了标识符，那么不会再自动生成。

自动生成标识符会根据标题内容，使用以下方式生成：

1. 移除所有 Markdown 结构，只保留文本、代码片段的内容（不包括作为分隔符的 `` ` ``）和表情符号（替换为其代码）。
2. 移除所有数字、字母（Unicode 代码 `L`、`N` 和 `Mn`）和 `_`、`-`、`.`、空格、Tab、换行之外的其它字符。
3. 将空格、Tab 和换行替换为 `-`。
4. 将所有字母转换为小写。
5. 移除标识符首个字母或数字之前，以及最后一个字母或数字之后的标点（`_`、`-` 和 `.`）。
6. 连续的相同标点（`_`、`-` 和 `.`）会被合并成一个。
7. 如果结果是空字符串，那么使用 `section` 作为标识符。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
# Heading identifiers in HTML
# Maître d'hôtel
# 示例 :grinning:
# *Dogs*?-in *my* house?
# [link](/url) and `code`
# --3. Applications--
# ??
```
```html
<h1 id="heading-identifiers-in-html">Heading identifiers in HTML</h1>
<h1 id="maître-dhôtel">Maître d'hôtel</h1>
<h1 id="示例-grinning">示例 😀</h1>
<h1 id="dogs-in-my-house"><em>Dogs</em>?-in <em>my</em> house?</h1>
<h1 id="link-and-code"><a href="/url">link</a> and <code>code</code></h1>
<h1 id="3.-applications">--3. Applications--</h1>
<h1 id="section">??</h1>
```

如果多个标题生成了重复的标识符，那么会为第二个标题添加 `-1` 后缀，第三个标题添加 `-2` 标题，并以此类推。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
# Heading
# Heading-1
# Heading
## Heading-2
## Heading-1
```
```html
<h1 id="heading">Heading</h1>
<h1 id="heading-1">Heading-1</h1>
<h1 id="heading-2">Heading</h1>
<h2 id="heading-2-1">Heading-2</h2>
<h2 id="heading-1-1">Heading-1</h2>
```

自动链接不会覆盖通过属性显示指定的标识符。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
# Section 1
# Section 2 {#sec2}
```
```html
<h1 id="section-1">Section 1</h1>
<h1 id="sec2">Section 2</h1>
```
