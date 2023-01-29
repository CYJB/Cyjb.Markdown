using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="Emoji"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestEmoji
{
	/// <summary>
	/// 测试 <see cref="Emoji"/> 节点。
	/// </summary>
	[TestMethod]
	public void TestEmoji()
	{
		Emoji? emoji = Emoji.GetEmoji("+1");
		Assert.IsNotNull(emoji);
		Assert.AreEqual("+1", emoji.Code);
		Assert.AreEqual("\U0001f44d", emoji.Text);
		Assert.AreEqual("https://github.githubassets.com/images/icons/emoji/unicode/1f44d.png?v8", emoji.GitHubFallbackUrl);

		emoji = Emoji.GetEmoji("no_such_emoji");
		Assert.IsNull(emoji);

		emoji = Emoji.GetEmoji("basecamp");
		Assert.IsNotNull(emoji);
		Assert.AreEqual("basecamp", emoji.Code);
		Assert.IsNull(emoji.Text);
		Assert.AreEqual("https://github.githubassets.com/images/icons/emoji/basecamp.png?v8", emoji.GitHubFallbackUrl);
	}
}

