using System.Diagnostics;
using System.Text;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// HTML 属性的列表。
/// </summary>
public sealed class HtmlAttributeList : ReadOnlyListBase<KeyValuePair<string, string>>
{
	/// <summary>
	/// <c>id</c> 的键。
	/// </summary>
	private const string IdKey = "id";
	/// <summary>
	/// <c>class</c> 的键。
	/// </summary>
	private const string ClassKey = "class";

	/// <summary>
	/// HTML 属性列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly List<KeyValuePair<string, string>> list = new();

	/// <summary>
	/// 初始化 <see cref="HtmlAttributeList"/> 类的新实例。
	/// </summary>
	public HtmlAttributeList() { }

	/// <summary>
	/// 获取或设置 <c>id</c> 属性的值。
	/// </summary>
	public string? Id
	{
		get => GetInternal(IdKey);
		set => AddInternal(IdKey, value);
	}

	/// <summary>
	/// 获取或设置与指定键关联的值。
	/// </summary>
	/// <param name="key">要检查的键。</param>
	/// <value>与指定键关联的值，<c>null</c> 表示不存在指定键。</value>
	/// <returns>与指定键关联的值。</returns>
	public string? this[string key]
	{
		get
		{
			if (key == null)
			{
				return null;
			}
			return GetInternal(key.ToLowerInvariant());
		}
		set
		{
			if (key == null)
			{
				return;
			}
			AddInternal(key.ToLowerInvariant(), value);
		}
	}

	/// <summary>
	/// 添加指定的键值。
	/// </summary>
	/// <param name="key">属性的键。</param>
	/// <param name="value">属性的值。</param>
	public void Add(string key, string value)
	{
		if (value == null)
		{
			// 不添加 null 的值。
			return;
		}
		AddInternal(key.ToLowerInvariant(), value);
	}

	/// <summary>
	/// 批量添加指定的键值对。
	/// </summary>
	/// <param name="attrs">要添加的键值对。</param>
	public void AddRange(IEnumerable<KeyValuePair<string, string>> attrs)
	{
		foreach (KeyValuePair<string, string> attr in attrs)
		{
			string key = attr.Key.ToLowerInvariant();
			if (key == ClassKey)
			{
				AddClass(attr.Value);
			}
			else
			{
				AddInternal(key, attr.Value);
			}
		}
	}

	/// <summary>
	/// 添加新的类名。
	/// </summary>
	/// <param name="className">要添加的类名。</param>
	public void AddClass(string className)
	{
		if (className.IsNullOrEmpty())
		{
			return;
		}
		int idx = GetIndex(ClassKey);
		if (idx >= 0)
		{
			string value = list[idx].Value;
			if (value.Length == 0)
			{
				value = className;
			}
			else
			{
				value += " " + className;
			}
			list[idx] = new KeyValuePair<string, string>(ClassKey, value);
		}
		else
		{
			list.Add(new KeyValuePair<string, string>(ClassKey, className));
		}
	}

	/// <summary>
	/// 移除指定的类名。
	/// </summary>
	/// <param name="className">要移除的类名。</param>
	public void RemoveClass(string className)
	{
		if (className.IsNullOrEmpty())
		{
			return;
		}
		int idx = GetIndex(ClassKey);
		if (idx < 0)
		{
			return;
		}
		string value = string.Join(' ', list[idx].Value
			.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			.Where(name => name != className));
		list[idx] = new KeyValuePair<string, string>(ClassKey, value);
	}

	/// <summary>
	/// 从当前集合中移除所有元素。
	/// </summary>
	public void Clear()
	{
		list.Clear();
	}

	/// <summary>
	/// 将属性内容添加到指定文本。
	/// </summary>
	/// <param name="text">要添加到的文本。</param>
	public void AppendTo(StringBuilder text)
	{
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (i > 0)
			{
				text.Append(' ');
			}
			var item = list[i];
			text.Append(item.Key);
			text.Append("=\"");
			text.Append(item.Value.Replace("\"", "&quot;"));
			text.Append('\"');
		}
	}

	/// <summary>
	/// 添加指定的属性前缀。
	/// </summary>
	/// <param name="prefix">要添加的前缀。</param>
	internal void AddPrefix(string? prefix)
	{
		if (prefix.IsNullOrEmpty())
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			string key = list[i].Key;
			if (key != IdKey && key != ClassKey)
			{
				list[i] = new KeyValuePair<string, string>(prefix + key, list[i].Value);
			}
		}
	}

	/// <summary>
	/// 将当前属性列表复制到指定列表中。
	/// </summary>
	/// <param name="other">要复制到的列表，会假设该列表是空的。</param>
	internal void CloneTo(HtmlAttributeList other)
	{
		other.list.AddRange(list);
	}

	#region ReadOnlyListBase<KeyValuePair<string, string>> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => list.Count;

	/// <summary>
	/// 返回指定索引处的元素。
	/// </summary>
	/// <param name="index">要返回元素的从零开始的索引。</param>
	/// <returns>位于指定索引处的元素。</returns>
	protected override KeyValuePair<string, string> GetItemAt(int index)
	{
		return list[index];
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	public override int IndexOf(KeyValuePair<string, string> item)
	{
		return list.IndexOf(item);
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	#endregion // ReadOnlyListBase<KeyValuePair<string, string>> 成员

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		StringBuilder text = new();
		AppendTo(text);
		return text.ToString();
	}

	/// <summary>
	/// 返回指定键的索引。
	/// </summary>
	/// <param name="key">要检查的键，要求已经转换为小写。</param>
	/// <returns>指定键的索引，如果未找到键，则返回 <c>-1</c>。</returns>
	private int GetIndex(string key)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Key == key)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定的键值。
	/// </summary>
	/// <param name="key">属性的键，要求已经转换为小写。</param>
	private string? GetInternal(string key)
	{
		int idx = GetIndex(key);
		if (idx >= 0)
		{
			return list[idx].Value;
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 添加指定的键值。
	/// </summary>
	/// <param name="key">属性的键，要求已经转换为小写。</param>
	/// <param name="value">属性的值。</param>
	private void AddInternal(string key, string? value)
	{
		int idx = GetIndex(key);
		if (idx >= 0)
		{
			if (value == null)
			{
				list.RemoveAt(idx);
			}
			else
			{
				list[idx] = new KeyValuePair<string, string>(key, value);
			}
		}
		else if (value != null)
		{
			list.Add(new KeyValuePair<string, string>(key, value));
		}
	}
}
