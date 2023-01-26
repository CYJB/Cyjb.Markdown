using Cyjb.Collections.ObjectModel;

namespace Cyjb.Markdown.Syntax;

/// <summary>
/// Markdown 表格的对齐方式列表。
/// </summary>
public sealed class TableAlignList : ReadOnlyListBase<TableAlign>
{
	/// <summary>
	/// 所属表格。
	/// </summary>
	private readonly Table table;
	/// <summary>
	/// 对齐方式列表。
	/// </summary>
	private readonly List<TableAlign> aligns = new();

	/// <summary>
	/// 使用指定的表格初始化 <see cref="TableAlignList"/> 类的新实例。
	/// </summary>
	/// <param name="table">所属的表格。</param>
	internal TableAlignList(Table table)
	{
		this.table = table;
	}

	/// <summary>
	/// 获取或设置指定列的对齐方式。
	/// </summary>
	/// <param name="index">要设置的列。</param>
	/// <returns>指定列的对齐方式。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零或大于等于列数。</exception>
	public new TableAlign this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			return GetItemAt(index);
		}
		set
		{
			if (index < 0 || index >= Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			if (index >= aligns.Count)
			{
				aligns.AddRange(Enumerable.Repeat(TableAlign.None, index - aligns.Count + 1));
			}
			aligns[index] = value;
		}
	}

	#region ReadOnlyListBase<TableAlign> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => table.ColumnCount;

	/// <summary>
	/// 返回指定索引处的元素。
	/// </summary>
	/// <param name="index">要返回元素的从零开始的索引。</param>
	/// <returns>位于指定索引处的元素。</returns>
	protected override TableAlign GetItemAt(int index)
	{
		if (index < aligns.Count)
		{
			return aligns[index];
		}
		else
		{
			return TableAlign.None;
		}
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	public override int IndexOf(TableAlign item)
	{
		int idx = aligns.IndexOf(item);
		if (idx >= 0 && idx < table.ColumnCount)
		{
			return idx;
		}
		if (item == TableAlign.None && aligns.Count < table.ColumnCount)
		{
			return aligns.Count;
		}
		return -1;
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<TableAlign> GetEnumerator()
	{
		int count = table.ColumnCount;
		for (int i = 0; i < count; i++)
		{
			yield return GetItemAt(i);
		}
	}

	#endregion // ReadOnlyListBase<TableAlign> 成员

}
