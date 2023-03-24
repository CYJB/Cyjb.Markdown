namespace Cyjb.Markdown.Utils;

/// <summary>
/// 唯一标识符的生成器。
/// </summary>
internal sealed class UniqueIdentifier
{
	/// <summary>
	/// 标识符集合。
	/// </summary>
	private readonly Dictionary<string, int> identifiers = new();

	/// <summary>
	/// 返回唯一标识符。
	/// </summary>
	/// <param name="id">基础标识符。</param>
	/// <returns>唯一标识符。</returns>
	public string Unique(string id)
	{
		if (identifiers.TryGetValue(id, out int nextIdx))
		{
			string nextId;
			for (; identifiers.ContainsKey((nextId = $"{id}-{nextIdx}")); nextIdx++) ;
			identifiers[id] = nextIdx + 1;
			id = nextId;
		}
		identifiers[id] = 1;
		return id;
	}

	/// <summary>
	/// 清空标识符信息。
	/// </summary>
	public void Clear()
	{
		identifiers.Clear();
	}
}
