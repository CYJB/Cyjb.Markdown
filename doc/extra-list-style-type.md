# 额外的列表样式类型

版本：`1.0`

## 1. 英文字母列表

使用英文字母作为序号使用时，会生成英文字母列表。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
a. foo
b. bar
A) baz
B) boo
```
```html
<ol type="a">
  <li>foo</li>
  <li>bar</li>
</ol>
<ol type="A">
  <li>baz</li>
  <li>boo</li>
</ol>
```

只支持一个英文字母作为序号，不支持两个或更多字母。

<a id="example-2" href="#example-2">示例 2</a>

``` markdown
aa. foo
```
``` html
<p>aa. foo</p>
```

## 2. 罗马数字列表

使用罗马数字作为序号使用时，会生成罗马数字列表。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
i. foo
ii. bar
I) baz
II) boo
```
```html
<ol type="i">
  <li>foo</li>
  <li>bar</li>
</ol>
<ol type="I">
  <li>baz</li>
  <li>boo</li>
</ol>
```

对于列表中的首个列表项，只有 `i` 和长度大于等于 2 的序号才会被识别成罗马数字。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
v. foo
ix. bar
x. baz
```
```html
<ol type="a" start="22">
  <li>foo</li>
</ol>
<ol type="i" start="9">
  <li>bar</li>
  <li>baz</li>
</ol>
```

罗马数字的长度没有限制，如果首个列表项不是有效的罗马数字或者大于 4000，那么该列表不会被识别为罗马数字。

<a id="example-5" href="#example-4">示例 5</a>

```markdown
VC. foo

X. bar

iiii. baz

mmmm. boo
```
```html
<p>VC. foo</p>
<ol type="A" start="24">
<li>bar</li>
</ol>
<p>iiii. baz</p>
<p>mmmm. boo</p>
```

## 3. 希腊字母列表

使用希腊字母作为序号使用时，会生成希腊字母列表。

<a id="example-6" href="#example-6">示例 6</a>

```markdown
α. foo
β. bar
```
```html
<ol style="list-style-type: lower-greek;">
  <li>foo</li>
  <li>bar</li>
</ol>
```

注意只支持一个希腊字母作为序号，不支持两个或更多字母。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
αβ. foo
```
```html
<p>αβ. foo</p>
```
