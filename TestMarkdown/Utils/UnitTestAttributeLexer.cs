using System;
using Cyjb;
using Cyjb.Compilers.Lexers;
using Cyjb.Markdown.ParseBlock;
using Cyjb.Test;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

enum TestAttributeKind
{
}

/// <summary>
/// <see cref="AttributeLexer"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestAttributeLexer
{
	/// <summary>
	/// <see cref="AttributeLexer"/> 的工厂。
	/// </summary>
	private static readonly PrivateObject AttributeLexerFactory = new(
		new PrivateType("Cyjb.Markdown", "Cyjb.Markdown.ParseBlock.AttributeLexer")
			.GetStaticField("Factory")!);

	private const int AttributeKindIdentifier = 0;
	private const int AttributeKindClassName = 1;
	private const int AttributeKindCommon = 2;
	private const int AttributeKindSeperator = 3;
	private const int AttributeKindEnd = 4;
	private const int AttributeKindInvalid = 5;

	/// <summary>
	/// 测试解析属性。
	/// </summary>
	[TestMethod]
	public void TestParse1()
	{
		PrivateObject tokenizer = new(AttributeLexerFactory.Invoke("CreateTokenizer", false)!);
		tokenizer.Invoke("Load", "#id .class attr=value attr2=\"value={2}\"}\r\nxxx");
		Assert.AreEqual(new Token<int>(AttributeKindIdentifier, "id", new TextSpan(0, 3)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, " ", new TextSpan(3, 4)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindClassName, "class", new TextSpan(4, 10)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, " ", new TextSpan(10, 11)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindCommon, "attr", new TextSpan(11, 21), "value"), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, " ", new TextSpan(21, 22)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindCommon, "attr2", new TextSpan(22, 39), "value={2}"), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindEnd, "}", new TextSpan(39, 40)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, "\r\n", new TextSpan(40, 42)), Read(tokenizer));
	}

	/// <summary>
	/// 测试解析属性。
	/// </summary>
	[TestMethod]
	public void TestParse2()
	{
		PrivateObject tokenizer = new(AttributeLexerFactory.Invoke("CreateTokenizer", false)!);
		tokenizer.Invoke("Load", "\r\n#id-foo\r\n   key='value'   \r\n}\r\n");
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, "\r\n", new TextSpan(0, 2)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindIdentifier, "id-foo", new TextSpan(2, 9)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, "\r\n   ", new TextSpan(9, 14)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindCommon, "key", new TextSpan(14, 25), "value"), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, "   \r\n", new TextSpan(25, 30)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindEnd, "}", new TextSpan(30, 31)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindSeperator, "\r\n", new TextSpan(31, 33)), Read(tokenizer));
	}

	/// <summary>
	/// 测试无效的标识符。
	/// </summary>
	[TestMethod]
	public void TestInvalidIdentiier()
	{
		PrivateObject tokenizer = new(AttributeLexerFactory.Invoke("CreateTokenizer", false)!);
		tokenizer.Invoke("Load", "# }");
		Assert.AreEqual(new Token<int>(AttributeKindInvalid, "#", new TextSpan(0, 1)), Read(tokenizer));
	}

	/// <summary>
	/// 测试无效的结束符。
	/// </summary>
	[TestMethod]
	public void TestInvalidEnd()
	{
		PrivateObject tokenizer = new(AttributeLexerFactory.Invoke("CreateTokenizer", false)!);
		tokenizer.Invoke("Load", "#id} foo");
		Assert.AreEqual(new Token<int>(AttributeKindIdentifier, "id", new TextSpan(0, 3)), Read(tokenizer));
		Assert.AreEqual(new Token<int>(AttributeKindInvalid, "} foo", new TextSpan(3, 8)), Read(tokenizer));
	}

	private static Token<int> Read(PrivateObject tokenizer)
	{
		PrivateObject token = new(tokenizer.Invoke("Read")!);
		var kind = (int)token.GetProperty("Kind")!;
		var text = (StringView)token.GetProperty("Text")!;
		var span = (TextSpan)token.GetProperty("Span")!;
		return new Token<int>(kind, text, span);
	}
}

