# 额外的列表类型

## 1. 英文字母列表

使用英文字母作为序号使用时，会生成英文字母列表

```markdown
a. foo
b. bar
A) baz
B) boo
```

会生成

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

注意只支持一个英文字母作为序号，不支持两个或更多字母。

```markdown
aa. foo
```

会生成

```html
<p>aa. foo</p>
```

如果起始序号是 `i`，那么会当作罗马数字看待，而非英文字母。

## 2. 罗马数字列表

使用罗马数字作为序号使用时，会生成罗马数字列表

```markdown
i. foo
ii. bar
I) baz
II) boo
```

会生成

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

罗马数字的长度没有限制，如果首个列表项不是有效的罗马数字或者大于 4000，那么不会被识别为罗马数字。

```markdown
VC. foo
X. bar
```

会生成

```html
<p>VC. foo</p>
<ol type="I" start="10">
  <li>bar</li>
</ol>
```

## 3. 希腊字母列表

使用希腊字母作为序号使用时，会生成希腊字母列表

```markdown
α. foo
β. bar
```

会生成

```html
<ol style="list-style: lower-greek;">
  <li>foo</li>
  <li>bar</li>
</ol>
```

注意只支持一个希腊字母作为序号，不支持两个或更多字母。

```markdown
αβ. foo
```

会生成

```html
<p>αβ. foo</p>
```
