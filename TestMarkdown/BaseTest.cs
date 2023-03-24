using System;
using System.Collections;
using System.Collections.Generic;
using Cyjb.Markdown;
using Cyjb.Markdown.Syntax;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown;

/// <summary>
/// 用于单元测试的基类。
/// </summary>
public abstract partial class BaseTest
{
	/// <summary>
	/// 要验证的子节点的堆栈。
	/// </summary>
	private readonly Stack<ValidationItem> validationStack = new();

	/// <summary>
	/// 测试解析 Markdown 字符串。
	/// </summary>
	/// <param name="text">要解析的 Markdown 字符串。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void AssertMarkdown(string text, Action validator)
	{
		Document doc = Document.Parse(text);
		Assert.AreEqual(new TextSpan(0, text.Length), doc.Span);
		validationStack.Clear();
		AssertChildren(doc, doc.Children, validator);
	}

	/// <summary>
	/// 测试解析 Markdown 字符串。
	/// </summary>
	/// <param name="text">要解析的 Markdown 字符串。</param>
	/// <param name="options">解析选项。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void AssertMarkdown(string text, ParseOptions options, Action validator)
	{
		Document doc = Document.Parse(text, options);
		Assert.AreEqual(new TextSpan(0, text.Length), doc.Span);
		validationStack.Clear();
		AssertChildren(doc, doc.Children, validator);
	}

	/// <summary>
	/// 测试解析 Markdown 字符串。
	/// </summary>
	/// <param name="text">要解析的 Markdown 字符串。</param>
	/// <param name="validator">子节点验证器。</param>
	protected void AssertCommonMark(string text, Action validator)
	{
		Document doc = Document.Parse(text, ParseOptions.CommonMark);
		Assert.AreEqual(new TextSpan(0, text.Length), doc.Span);
		validationStack.Clear();
		AssertChildren(doc, doc.Children, validator);
	}

	/// <summary>
	/// 验证指定的子节点列表。
	/// </summary>
	/// <typeparam name="T">子节点的类型。</typeparam>
	/// <param name="parent">父节点。</param>
	/// <param name="children">子节点列表。</param>
	/// <param name="validator">子节点验证器。</param>
	private void AssertChildren(Node? parent, IList children, Action validator)
	{
		ValidationItem item = new(parent, children);
		validationStack.Push(item);
		validator();
		string parentInfo = parent?.ToString() ?? "root";
		Assert.IsTrue(validationStack.Pop().IsReachEnd, $"{parentInfo} 子节点过多，预期达到子节点末尾");
	}

	/// <summary>
	/// 返回下一个待验证节点。
	/// </summary>
	/// <returns>下一个待验证节点。</returns>
	private Node Next()
	{
		return validationStack.Peek().Next();
	}

	/// <summary>
	/// 表示当前的验证项。
	/// </summary>
	private class ValidationItem
	{
		/// <summary>
		/// 父节点。
		/// </summary>
		private readonly Node? parent;
		/// <summary>
		/// 子节点列表。
		/// </summary>
		private readonly IList children;
		/// <summary>
		/// 当前节点。
		/// </summary>
		private int index = -1;

		/// <summary>
		/// 使用指定的父节点和子节点列表初始化 <see cref="ValidationItem"/> 类的新实例。
		/// </summary>
		/// <param name="parent">父节点。</param>
		/// <param name="children">子节点列表。</param>
		public ValidationItem(Node? parent, IList children)
		{
			this.parent = parent;
			this.children = children;
		}

		/// <summary>
		/// 获取当前子节点。
		/// </summary>
		public Node Current => (Node)children[index]!;

		/// <summary>
		/// 获取是否到达的列表的结尾。
		/// </summary>
		public bool IsReachEnd => index + 1 == children.Count;

		/// <summary>
		/// 返回下一个子节点。
		/// </summary>
		/// <returns>下一个子节点。</returns>
		public Node Next()
		{
			index++;
			string parentInfo = parent?.ToString() ?? "root";
			Assert.IsTrue(index < children.Count,
				$"{parentInfo} 子节点数不足，预期至少为 {index + 1}，实际为 {children.Count}");
			Node child = Current;
			Assert.AreEqual(parent, child.Parent);
			if (index == 0)
			{
				Assert.AreEqual(null, child.Prev);
			}
			else
			{
				Assert.AreEqual(children[index - 1], child.Prev);
			}
			if (index + 1 < children.Count)
			{
				Assert.AreEqual(children[index + 1], child.Next);
			}
			else
			{
				Assert.AreEqual(null, child.Next);
			}
			return child;
		}
	}
}
