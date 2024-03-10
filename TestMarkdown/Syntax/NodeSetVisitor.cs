using System;
using System.Collections.Generic;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;

namespace TestMarkdown.Syntax;

/// <summary>
/// 提取所有节点集合的访问器。
/// </summary>
/// <remarks>这里需要统一使用引用比较，避免 IEqutable 影响测试结果。</remarks>
internal class NodeSetVisitor : SyntaxWalker
{
	private static readonly ReferenceComparer Comparer = new();

	/// <summary>
	/// 所有节点的集合。
	/// </summary>
	private readonly HashSet<Node> nodes = new(Comparer);

	/// <summary>
	/// 返回所有节点的集合。
	/// </summary>
	public HashSet<Node> GetNodes()
	{
		HashSet<Node> result = new(nodes, Comparer);
		nodes.Clear();
		return result;
	}

	/// <summary>
	/// 提供默认的访问行为。
	/// </summary>
	/// <param name="node">要访问的节点。</param>
	public override void DefaultVisit(Node node)
	{
		nodes.Add(node);
		base.DefaultVisit(node);
	}

	private class ReferenceComparer : IEqualityComparer<Node>
	{
		public bool Equals(Node? x, Node? y)
		{
			return ReferenceEquals(x, y);
		}

		public int GetHashCode(Node obj)
		{
			return HashCode.Combine(obj);
		}
	}
}
