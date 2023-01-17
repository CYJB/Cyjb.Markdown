using Cyjb.Collections.ObjectModel;
using Cyjb.Markdown.Syntax;

namespace Cyjb.Markdown;

/// <summary>
/// 表示 Markdown 的子节点列表。
/// </summary>
/// <typeparam name="T">子节点的类型。</typeparam>
public sealed class NodeList<T> : ListBase<T>
	where T : Node
{
	/// <summary>
	/// 当前列表所属的节点。
	/// </summary>
	private readonly Node? owner;
	/// <summary>
	/// 子节点列表。
	/// </summary>
	private readonly List<T> nodes = new();

	/// <summary>
	/// 使用指定的所属节点初始化 <see cref="NodeList{T}"/> 类的新实例。
	/// </summary>
	/// <param name="owner">所属节点。</param>
	public NodeList(Node? owner = null)
	{
		this.owner = owner;
	}

	/// <summary>
	/// 获取当前列表所属的节点。
	/// </summary>
	public Node? Owner => owner;

	#region ListBase<T> 成员

	/// <summary>
	/// 获取当前列表包含的元素数。
	/// </summary>
	/// <value>当前列表中包含的元素数。</value>
	public override int Count => nodes.Count;

	/// <summary>
	/// 将元素插入当前列表的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
	/// <param name="item">要插入的对象。</param>
	protected override void InsertItem(int index, T item)
	{
		item.Unlink();
		item.Parent = owner;
		if (index > 0)
		{
			T prev = nodes[index - 1];
			item.SetPrev(prev);
			prev.SetNext(item);
		}
		if (index < nodes.Count)
		{
			item.SetNext(nodes[index]);
			nodes[index].SetPrev(item);
		}
		nodes.Insert(index, item);
	}

	/// <summary>
	/// 移除当前列表指定索引处的元素。
	/// </summary>
	/// <param name="index">要移除的元素的从零开始的索引。</param>
	protected override void RemoveItem(int index)
	{
		nodes[index].Unlink();
		nodes.RemoveAt(index);
	}

	/// <summary>
	/// 返回指定索引处的元素。
	/// </summary>
	/// <param name="index">要返回元素的从零开始的索引。</param>
	/// <returns>位于指定索引处的元素。</returns>
	protected override T GetItemAt(int index)
	{
		return nodes[index];
	}

	/// <summary>
	/// 替换指定索引处的元素。
	/// </summary>
	/// <param name="index">待替换元素的从零开始的索引。</param>
	/// <param name="item">位于指定索引处的元素的新值。</param>
	protected override void SetItemAt(int index, T item)
	{
		item.Unlink();
		item.Parent = owner;
		if (index > 0)
		{
			item.SetPrev(nodes[index]);
			nodes[index].SetNext(item);
		}
		if (index + 1 < nodes.Count)
		{
			T next = nodes[index + 1];
			item.SetNext(next);
			next.SetPrev(item);
		}
		nodes[index] = item;
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	public override int IndexOf(T item)
	{
		return nodes.IndexOf(item);
	}

	/// <summary>
	/// 从当前列表中移除所有元素。
	/// </summary>
	public override void Clear()
	{
		foreach (T node in nodes)
		{
			// 所有兄弟节点都在同一个列表内，不需要额外修复链接。
			node.Unlink(false);
		}
		nodes.Clear();
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		return nodes.GetEnumerator();
	}

	#endregion // ListBase<T> 成员

	/// <summary>
	/// 将指定范围的节点添加到当前列表中。
	/// </summary>
	/// <param name="start">要添加的起始节点（包含）。</param>
	/// <param name="end">要添加的结束节点（不含）。</param>
	/// <remarks>如果 <paramref name="start"/> 为 <c>null</c>，那么什么都不做。如果
	/// <paramref name="end"/> 为 <c>null</c>，那么将从 <paramref name="start"/>
	/// 开始的所有节点都添加到当前列表。</remarks>
	public void AddRange(T? start, T? end)
	{
		if (start == null)
		{
			return;
		}
		int index = nodes.Count;
		NodeList<T>? oldContainer = (start.Parent as INodeContainer<T>)?.Children;
		if (oldContainer == null)
		{
			Node? nextNode;
			while (start != null && start != end)
			{
				nextNode = start.Next;
				nodes.Add(start);
				start = (T?)nextNode;
			}
		}
		else
		{
			int startIdx = oldContainer.nodes.IndexOf(start);
			if (startIdx < 0)
			{
				// 正常来说不应当出现。
				throw CommonExceptions.Unreachable();
			}
			int endIdx;
			if (end == null)
			{
				endIdx = oldContainer.Count;
			}
			else
			{
				endIdx = oldContainer.nodes.IndexOf(end, startIdx);
				if (endIdx < 0)
				{
					throw CommonExceptions.Unreachable();
				}
			}
			nodes.AddRange(oldContainer.Skip(startIdx).Take(endIdx - startIdx));
			oldContainer.RemoveRange(startIdx, endIdx - startIdx);
		}
		// 修复关联关系
		FixLink(index);
		FixLink(nodes.Count);
		// 设置父节点。
		for (; index < nodes.Count; index++)
		{
			nodes[index].Parent = owner;
		}
	}

	/// <summary>
	/// 移除指定范围的子节点。
	/// </summary>
	/// <param name="index">要移除的节点起始索引。</param>
	/// <param name="count">要移除的节点个数。</param>
	private void RemoveRange(int index, int count)
	{
		nodes.RemoveRange(index, count);
		FixLink(index);
	}

	/// <summary>
	/// 修复指定索引处元素与其前驱元素的关联关系。
	/// </summary>
	/// <param name="index">要修复的元素索引。</param>
	private void FixLink(int index)
	{
		Node? prev = index > 0 ? nodes[index - 1] : null;
		Node? next = nodes.Count > index ? nodes[index] : null;
		prev?.SetNext(next);
		next?.SetPrev(prev);
	}
}
