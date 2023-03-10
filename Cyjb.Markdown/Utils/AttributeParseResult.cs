namespace Cyjb.Markdown.Utils;

/// <summary>
/// 属性的解析结果。
/// </summary>
internal sealed class AttributeParseResult
{
	/// <summary>
	/// 返回表示成功的解析结果。
	/// </summary>
	public static AttributeParseResult Success = new(true);
	/// <summary>
	/// 返回表示失败的解析结果。
	/// </summary>
	public static AttributeParseResult Failed = new(false);
	/// <summary>
	/// 返回表示缺少引号导致的失败结果。
	/// </summary>
	/// <param name="quoteChar">缺少的引号字符。</param>
	/// <param name="key">已解析的键值属性的键。</param>
	/// <param name="value">已解析的键值属性的值。</param>
	/// <returns>缺少引号导致的失败结果。</returns>
	public static AttributeParseResult GetMissingQuote(char quoteChar, string key, string value)
	{
		return new AttributeParseResult(quoteChar, key, value);
	}

	/// <summary>
	/// 属性解析是否成功。
	/// </summary>
	public readonly bool IsSuccess;
	/// <summary>
	/// 是否是由于缺少引号导致的失败。
	/// </summary>
	public readonly bool IsMissingQuote;
	/// <summary>
	/// 缺少的引号字符。
	/// </summary>
	public readonly char QuoteChar;
	/// <summary>
	/// 已解析的键值属性的键。
	/// </summary>
	public readonly string Key;
	/// <summary>
	/// 已解析的键值属性的值。
	/// </summary>
	public string Value;

	/// <summary>
	/// 使用是否成功初始化 <see cref="AttributeParseResult"/> 类的新实例。
	/// </summary>
	/// <param name="isSuccess">属性解析是否成功。</param>
	private AttributeParseResult(bool isSuccess)
	{
		IsSuccess = isSuccess;
		IsMissingQuote = false;
		QuoteChar = '\0';
		Key = string.Empty;
		Value = string.Empty;
	}

	/// <summary>
	/// 使用指定的键值属性信息初始化 <see cref="AttributeParseResult"/> 类的新实例。
	/// </summary>
	/// <param name="quoteChar">缺少的引号字符。</param>
	/// <param name="key">已解析的键值属性的键。</param>
	/// <param name="value">已解析的键值属性的值。</param>
	private AttributeParseResult(char quoteChar, string key, string value)
	{
		IsSuccess = false;
		IsMissingQuote = true;
		QuoteChar = quoteChar;
		Key = key;
		Value = value;
	}
}
