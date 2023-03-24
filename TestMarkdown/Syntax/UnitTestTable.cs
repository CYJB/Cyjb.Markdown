using System;
using System.Collections.Generic;
using Cyjb.Markdown.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarkdown.Syntax;

/// <summary>
/// <see cref="Table"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestTable
{
	/// <summary>
	/// 测试 <see cref="Table"/> 至少需要包含一行。
	/// </summary>
	[TestMethod]
	public void TestMustHaveHeading()
	{
		TableRow row = new(new TableCell());
		TableRow row2 = new(new TableCell());
		TableRow row3 = new(new TableCell());

		Table table = new(row);
		table.Children.Add(row2);
		Assert.AreEqual(2, table.Children.Count);
		Assert.ThrowsException<InvalidOperationException>(() => table.Children.Clear());
		table.Children.RemoveAt(0);
		Assert.AreEqual(1, table.Children.Count);
		Assert.ThrowsException<InvalidOperationException>(() => table.Children.RemoveAt(0));
		Assert.ThrowsException<InvalidOperationException>(() => table.Children.Clear());
		table.Children.Add(row3);
		Assert.AreEqual(2, table.Children.Count);
		table.Children.RemoveRange(1, 1);
		Assert.ThrowsException<InvalidOperationException>(() => table.Children.RemoveRange(0, 1));
	}

	/// <summary>
	/// 测试 <see cref="Table"/> 的对齐方式。
	/// </summary>
	[TestMethod]
	public void TestAlign()
	{
		List<TableAlign> expectedAligns = new()
		{
			TableAlign.None,
			TableAlign.None,
		};
		TableRow row = new(new TableCell());
		row.Children.Add(new TableCell());
		Table table = new(row);
		CollectionAssert.AreEqual(expectedAligns, table.Aligns);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => table.Aligns[2]);

		table.Aligns[0] = TableAlign.Right;
		table.Aligns[1] = TableAlign.Center;
		expectedAligns[0] = TableAlign.Right;
		expectedAligns[1] = TableAlign.Center;
		CollectionAssert.AreEqual(expectedAligns, table.Aligns);

		row.Children.Add(new TableCell());
		expectedAligns.Add(TableAlign.None);
		Assert.AreEqual(3, table.Aligns.Count);
		CollectionAssert.AreEqual(expectedAligns, table.Aligns);

		table.Aligns[2] = TableAlign.Left;
		expectedAligns[2] = TableAlign.Left;
		CollectionAssert.AreEqual(expectedAligns, table.Aligns);

		// 总是移除末尾的对齐。
		row.Children.RemoveAt(0);
		expectedAligns.RemoveAt(2);
		CollectionAssert.AreEqual(expectedAligns, table.Aligns);

		// 会保持之前的对齐状态。
		row.Children.Add(new TableCell());
		expectedAligns.Add(TableAlign.Left);
		CollectionAssert.AreEqual(expectedAligns, table.Aligns);
	}

	/// <summary>
	/// 测试 <see cref="TableRow"/> 至少需要包含一个单元格。
	/// </summary>
	[TestMethod]
	public void TestRowMustHaveCell()
	{
		Assert.ThrowsException<ArgumentException>(() => new TableRow(Array.Empty<TableCell>()));

		TableCell cell = new();
		TableCell cell2 = new();
		TableCell cell3 = new();

		TableRow row = new(cell);
		row.Children.Add(cell2);
		Assert.AreEqual(2, row.Children.Count);
		Assert.ThrowsException<InvalidOperationException>(() => row.Children.Clear());
		row.Children.RemoveAt(0);
		Assert.AreEqual(1, row.Children.Count);
		Assert.ThrowsException<InvalidOperationException>(() => row.Children.RemoveAt(0));
		Assert.ThrowsException<InvalidOperationException>(() => row.Children.Clear());
		row.Children.Add(cell3);
		Assert.AreEqual(2, row.Children.Count);
		row.Children.RemoveRange(1, 1);
		Assert.ThrowsException<InvalidOperationException>(() => row.Children.RemoveRange(0, 1));
	}
}

