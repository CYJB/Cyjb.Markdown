# 扩展自动链接

版本：`1.0`

为了更好的处理 Unicode 文本，这里基于 [GitHub Flavored Markdown Spec](https://github.github.com/gfm/#autolinks-extension-) 做了一些微调，采用以下的规范。

扩展自动链接也可以处理未被 `<..>` 括起来的链接，包括 URL 自动链接和邮件自动链接两类。

## URL 自动链接

URL 自动链接以 `http://`、`https://` 或 `www.` 开头的文本（均不区分大小写），直到 `<` 或 Unicode 空白。URL 前不能是 `[a-zA-Z0-9+.-/:]`，避免链接是其它 scheme 的一部分。

<a id="example-1" href="#example-1">示例 1</a>

```markdown
http://www.commonmark.org

https://www.commonmark.org foo

www.commonmark.org<bar

Visit www.commonmark.org/help for more information.

my-http://www.commonmark.org
```
```html
<p><a href="http://www.commonmark.org">http://www.commonmark.org</a></p>
<p><a href="https://www.commonmark.org">https://www.commonmark.org</a> foo</p>
<p><a href="http://www.commonmark.org">www.commonmark.org</a>&lt;bar</p>
<p>Visit <a href="http://www.commonmark.org/help">www.commonmark.org/help</a> for more information.</p>
<p>my-http://www.commonmark.org</p>
```

URL 必须包含有效的域名：包含一个或多个由字母、数字、下划线(`_`) 和中划线(`-`) 组成的部分，并使用 `.` 分割。
域名至少要包含两个部分，并且域名的后两个部分不能包含下划线(`_`)。

<a id="example-2" href="#example-2">示例 2</a>

```markdown
http://commonmark.org

http://www.commonmark._org

http://www
```
```html
<p><a href="http://commonmark.org">http://commonmark.org</a></p>
<p>http://www.commonmark._org</p>
<p>http://www</p>
```

URL 没有包含协议时，默认使用 `http://`。

<a id="example-3" href="#example-3">示例 3</a>

```markdown
Visit www.commonmark.org/help for more information.
```
```html
<p>Visit <a href="http://www.commonmark.org/help">www.commonmark.org/help</a> for more information.</p>
```

URL 末尾的标点符号不计入自动链接之内，但可以出现在 URL 内部。

标点符号包括 Unicode 代码 `P`，但排除 `Pe` 和 `Pf`（结束标点符号会单独检查是否平衡）和 `[@#%/&_\\-]`（它们可能是正常 URL 的一部分）。

<a id="example-4" href="#example-4">示例 4</a>

```markdown
Visit www.commonmark.org.

Visit www.commonmark.org/a.b.

访问http://www.commonmark.org/?p=你好,世界。

http://commonmark.org/`bar`
```
```html
<p>Visit <a href="http://www.commonmark.org">www.commonmark.org</a>.</p>
<p>Visit <a href="http://www.commonmark.org/a.b">www.commonmark.org/a.b</a>.</p>
<p>访问<a href="http://www.commonmark.org/?p=%E4%BD%A0%E5%A5%BD,%E4%B8%96%E7%95%8C">http://www.commonmark.org/?p=你好,世界</a>。</p>
<p><a href="http://commonmark.org/%60bar%60">http://commonmark.org/`bar`</a></p>
```

如果 URL 以结束标点符号（Unicode 代码 `Pe` 或 `Pf`）结尾，那么会检查 URL 中相应起始标点符号的个数，如果少于结束标点符号的个数，那么会在首个不匹配的位置结束。

<a id="example-5" href="#example-5">示例 5</a>

```markdown
www.google.com/search?q=Markup+(business)

www.google.com/search?q=([Markup])+(business))]

(www.google.com/search?q=Markup+(business))

(www.google.com/search?q=Markup+(business)

www.google.com/search?q=Markup+[business]]

（www.google.com/search?q=Markup+【business】）
```
```html
<p><a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a></p>
<p><a href="http://www.google.com/search?q=(%5BMarkup%5D)+(business)">www.google.com/search?q=([Markup])+(business)</a>)]</p>
<p>(<a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a>)</p>
<p>(<a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a></p>
<p><a href="http://www.google.com/search?q=Markup+%5Bbusiness%5D">www.google.com/search?q=Markup+[business]</a>]</p>
<p>（<a href="http://www.google.com/search?q=Markup+%E3%80%90business%E3%80%91">www.google.com/search?q=Markup+【business】</a>）</p>
```

如果 URL 以分号（`;`）结尾，会检查它是否可能是 HTML 实体的一部分。如果之前包含 `&` 跟随一个或多个字母或数字，那么会排除 `&` 以及之后的字符。

<a id="example-6" href="#example-6">示例 6</a>

```markdown
www.google.com/search?q=commonmark&hl=en

www.google.com/search?q=commonmark&hl;

www.google.com/search?q=commonmark&;
```
```html
<p><a href="http://www.google.com/search?q=commonmark&hl=en">www.google.com/search?q=commonmark&amp;hl=en</a></p>
<p><a href="http://www.google.com/search?q=commonmark">www.google.com/search?q=commonmark</a>&amp;hl;</p>
<p><a href="http://www.google.com/search?q=commonmark&;">www.google.com/search?q=commonmark&amp;;</a></p>
```

不检查未出现在结尾的标点符号。

<a id="example-7" href="#example-7">示例 7</a>

```markdown
www.google.com/search?q=(business))+ok

www.google.com/search?q=Markup]+(business))
```
```html
<p><a href="http://www.google.com/search?q=(business))+ok">www.google.com/search?q=(business))+ok</a></p>
<p><a href="http://www.google.com/search?q=Markup%5D+(business)">www.google.com/search?q=Markup]+(business)</a>)</p>
```

会递归的检查标点符号。

<a id="example-8" href="#example-8">示例 8</a>

```markdown
Visit www.commonmark.org/a.b.,,

www.google.com/search?q=Markup+(business)])

www.google.com/search?q=Markup+[business]].)&amp;
```
```html
<p>Visit <a href="http://www.commonmark.org/a.b">www.commonmark.org/a.b</a>.,,</p>
<p><a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a>])</p>
<p><a href="http://www.google.com/search?q=Markup+%5Bbusiness%5D">www.google.com/search?q=Markup+[business]</a>].)&amp;</p>
```

`<` 会立即结束 URL。

<a id="example-9" href="#example-9">示例 9</a>

```markdown
www.commonmark.org/he<lp
```
```html
<p><a href="http://www.commonmark.org/he">www.commonmark.org/he</a>&lt;lp</p>
```

自动链接可以包含 `*` 等分隔符，除非它们出现在 URL 的末尾。

<a id="example-10" href="#example-10">示例 10</a>

```markdown
*http://commonmark.org/*/*
```
```html
<p><em><a href="http://commonmark.org/*/">http://commonmark.org/*/</a></em></p>
```

自动链接不会出现在其它链接内部。即使是**后续可能识别为链接**的左中括号后，也会使自动链接不被识别。

<a id="example-11" href="#example-11">示例 11</a>

```markdown
<http://commonmark.org/>

<a href="http://commonmark.org/">

[http://commonmark.org/](http://commonmark.org/)

[foo http://commonmark.org/`bar`

[foo http://commonmark.org/`bar`](http://commonmark.org/

[foo [bar](/uri) http://commonmark.org/](/uri)
```
```html
<p><a href="http://commonmark.org/">http://commonmark.org/</a></p>
<a href="http://commonmark.org/">
<p><a href="http://commonmark.org/">http://commonmark.org/</a></p>
<p>[foo http://commonmark.org/<code>bar</code></p>
<p>[foo http://commonmark.org/<code>bar</code>](<a href="http://commonmark.org/">http://commonmark.org/</a></p>
<p>[foo <a href="/uri">bar</a> <a href="http://commonmark.org/%5D(/uri)">http://commonmark.org/](/uri)</a></p>
```

`http://` 和 `https://` 链接会保持它们的协议。

<a id="example-12" href="#example-12">示例 12</a>

```markdown
http://commonmark.org

(Visit https://encrypted.google.com/search?q=Markup+(business))
```
```html
<p><a href="http://commonmark.org">http://commonmark.org</a></p>
<p>(Visit <a href="https://encrypted.google.com/search?q=Markup+(business)">https://encrypted.google.com/search?q=Markup+(business)</a>)</p>
```

## 邮件自动链接

邮件自动链接是可以由正则表达式 ``[a-zA-Z0-9.+_-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+`` 的字符串，邮件文本前可以包含可选的 `mailto:` 协议。邮件链接前不能是 `[a-zA-Z0-9+.-/:]`，避免链接是其它 scheme 的一部分。

邮件链接没有包含协议时，会补充 `mailto:`。

<a id="example-13" href="#example-13">示例 13</a>

```markdown
foo@bar.baz

mailto:foo.bar@b-a-z.bim

foo@bar
```
```html
<p><a href="mailto:foo@bar.baz">foo@bar.baz</a></p>
<p><a href="mailto:foo.bar@b-a-z.bim">mailto:foo.bar@b-a-z.bim</a></p>
<p>foo@bar</p>
```

`+` 只能出现在 `@` 之前。

<a id="example-14" href="#example-14">示例 14</a>

```markdown
hello@mail+xyz.example isn't valid, but hello+xyz@mail.example is.
```
```html
<p>hello@mail+xyz.example isn't valid, but <a href="mailto:hello+xyz@mail.example">hello+xyz@mail.example</a> is.</p>
```

邮件链接可以包含 `.`、`-` 和 `_` 等符号，会忽略链接末尾的其它字符。但若链接后是 `-`，那之前不是有效的邮件链接。

<a id="example-15" href="#example-15">示例 15</a>

```markdown
a.b-c_d@a.b

a.b-c_d@a.b.

a.b-c_d@a.b-

a.b-c_d@a.b_

a.b-c_d@a.b其它
```
```html
<p><a href="mailto:a.b-c_d@a.b">a.b-c_d@a.b</a></p>
<p><a href="mailto:a.b-c_d@a.b">a.b-c_d@a.b</a>.</p>
<p>a.b-c_d@a.b-</p>
<p><a href="mailto:a.b-c_d@a.b">a.b-c_d@a.b</a>_</p>
<p><a href="mailto:a.b-c_d@a.b">a.b-c_d@a.b</a>其它</p>
```

允许包含 `mailto:` 协议，但不支持 `xmpp:` 协议。

<a id="example-16" href="#example-16">示例 16</a>

```markdown
mailto:foo@bar.baz

mailto:a.b-c_d@a.b

mailto:a.b-c_d@a.b.

mailto:a.b-c_d@a.b/

mailto:a.b-c_d@a.b-

mailto:a.b-c_d@a.b_

xmpp:foo@bar.baz
```
```html
<p><a href="mailto:foo@bar.baz">mailto:foo@bar.baz</a></p>
<p><a href="mailto:a.b-c_d@a.b">mailto:a.b-c_d@a.b</a></p>
<p><a href="mailto:a.b-c_d@a.b">mailto:a.b-c_d@a.b</a>.</p>
<p><a href="mailto:a.b-c_d@a.b">mailto:a.b-c_d@a.b</a>/</p>
<p>mailto:a.b-c_d@a.b-</p>
<p><a href="mailto:a.b-c_d@a.b">mailto:a.b-c_d@a.b</a>_</p>
<p>xmpp:foo@bar.baz</p>
```

邮件自动链接不会出现在其它链接内部。即使是**后续可能识别为链接**的左中括号后，也会使邮件自动链接不被识别。

<a id="example-17" href="#example-17">示例 17</a>

```markdown
<foo@bar.baz>

<a href="mailto:foo@bar.baz">

[foo@bar.baz](mailto:biz@boo.bim)

[foo foo@bar.baz`bar`

[foo foo@bar.baz`bar`](mailto:biz@boo.bim

[foo [bar](/uri) foo@bar.baz](/uri)
```
```html
<p><a href="mailto:foo@bar.baz">foo@bar.baz</a></p>
<a href="mailto:foo@bar.baz">
<p><a href="mailto:biz@boo.bim">foo@bar.baz</a></p>
<p>[foo foo@bar.baz<code>bar</code></p>
<p>[foo foo@bar.baz<code>bar</code>](<a href="mailto:biz@boo.bim">mailto:biz@boo.bim</a></p>
<p>[foo <a href="/uri">bar</a> <a href="mailto:foo@bar.baz">foo@bar.baz</a>](/uri)</p>
```
