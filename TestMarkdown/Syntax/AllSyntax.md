# 包含所有语法节点的测试用 Markdown 文件。

## 块节点

### Blockqueue

> text
> > text2
> > > text3

### CodeBlock

```code {#id .class attr=value }
code span
```

### CustomContainer

::: detail {
  #my-id .class2
  key=value
}
foo
:::

### Footnote & FootnoteRef

Here is a simple footnote[^1].

A footnote can also have multiple lines[^2].

多次引用[^1][^2].

[^1]: My reference.
[^2]: To add line breaks within a footnote, prefix new lines with 2 spaces.
  This is a second line.

### Heading

Heading 2
---------

### HtmlBlock

<table><tr><td a="b">
<pre class="foo, bar">
**Hello**,

_world_.
</pre>
</td></tr></table>

### List

- foo
- bar
+ baz

1. foo
	- bar
2. bar
3) baz
    a. foo
    b. bar
	A) baz
	B) boo

i. foo
ii. bar
I) baz
II) boo

α. foo
β. bar

### MathBlock

$$ info startline=3 `%@#~
1 + 1
$$

### Paragraph

foo
bar

baz
112233

### Table

| First Header  | Second Header |
| ------------- | ------------- |
| Content Cell  | Content Cell  |
| Content Cell  | Content Cell  |

### ThematicBreak

foo

---

bar


## 行节点

## Break

soft: foo bar
baz

hard: foo bar  
baz

### CodeSpan

foo `bar` baz

### Emoji

@octocat :+1: This PR looks great - it's ready to merge! :shipit:

### Emphasis

foo *bar* baz

### Html

<foo><bar a="b" c="d"></foo>

### Link

[link](/uri "title"){a=b}

![alt text](url.jpg "title"){#id .class key=value}

[foo_link][]
[Foo_link][]

[foo_link]: /url "title"

![foo *bar* link]

[foo *bar* link]: train.jpg "train & tracks"

### Literal

foo bar baz

### MathSpan

foo $bar$ baz

### Strikethrough

foo ~~bar~~ baz

### Strong

foo __bar__ **b*a*z** bar
