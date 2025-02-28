using Cyjb.Markdown.ParseBlock;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// <see cref="AttributeLexer"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestAttributeLexer
{
	/// <summary>
	/// 测试解析属性。
	/// </summary>
	[TestMethod]
	public void TestParse1()
	{
		var tokenizer = AttributeLexer.Factory.CreateTokenizer();
		tokenizer.Load("#id .class attr=value attr2=\"value={2}\"}\r\nxxx");
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Identifier, "id", new TextSpan(0, 3)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, " ", new TextSpan(3, 4)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.ClassName, "class", new TextSpan(4, 10)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, " ", new TextSpan(10, 11)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Common, "attr", new TextSpan(11, 21), "value"), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, " ", new TextSpan(21, 22)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Common, "attr2", new TextSpan(22, 39), "value={2}"), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.End, "}", new TextSpan(39, 40)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, "\r\n", new TextSpan(40, 42)), tokenizer.Read());
	}

	/// <summary>
	/// 测试解析属性。
	/// </summary>
	[TestMethod]
	public void TestParse2()
	{
		var tokenizer = AttributeLexer.Factory.CreateTokenizer();
		tokenizer.Load("\r\n#id-foo\r\n   key='value'   \r\n}\r\n");
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, "\r\n", new TextSpan(0, 2)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Identifier, "id-foo", new TextSpan(2, 9)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, "\r\n   ", new TextSpan(9, 14)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Common, "key", new TextSpan(14, 25), "value"), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, "   \r\n", new TextSpan(25, 30)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.End, "}", new TextSpan(30, 31)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Seperator, "\r\n", new TextSpan(31, 33)), tokenizer.Read());
	}

	/// <summary>
	/// 测试无效的标识符。
	/// </summary>
	[TestMethod]
	public void TestInvalidIdentiier()
	{
		var tokenizer = AttributeLexer.Factory.CreateTokenizer();
		tokenizer.Load("# }");
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Invalid, "#", new TextSpan(0, 1)), tokenizer.Read());
	}

	/// <summary>
	/// 测试无效的结束符。
	/// </summary>
	[TestMethod]
	public void TestInvalidEnd()
	{
		var tokenizer = AttributeLexer.Factory.CreateTokenizer();
		tokenizer.Load("#id} foo");
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Identifier, "id", new TextSpan(0, 3)), tokenizer.Read());
		Assert.AreEqual(new Token<AttributeKind>(AttributeKind.Invalid, "} foo", new TextSpan(3, 8)), tokenizer.Read());
	}
}

