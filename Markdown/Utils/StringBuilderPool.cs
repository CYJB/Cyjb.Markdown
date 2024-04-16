using System.Text;

namespace Cyjb.Markdown.Utils;

/// <summary>
/// <see cref="StringBuilder"/> 池。
/// </summary>
internal class StringBuilderPool
{
	/// <summary>
	/// <see cref="StringBuilder"/> 池。
	/// </summary>
	[ThreadStatic]
	private static StringBuilderPool? pool;
	/// <summary>
	/// <see cref="StringBuilder"/> 实例列表。
	/// </summary>
	private readonly Stack<StringBuilder> stack = new();

	/// <summary>
	/// 返回一个 <see cref="StringBuilder"/> 实例。
	/// </summary>
	/// <param name="capacity"><see cref="StringBuilder"/> 实例的容量。</param>
	/// <returns><see cref="StringBuilder"/> 实例。</returns>
	public static StringBuilder Rent(int capacity)
	{
		pool ??= new StringBuilderPool();
		if (pool.stack.Count > 0)
		{
			StringBuilder builder = pool.stack.Pop();
			builder.Clear();
			builder.EnsureCapacity(capacity);
			return builder;
		}
		else
		{
			return new StringBuilder(capacity);
		}
	}

	/// <summary>
	/// 释放指定的 <see cref="StringBuilder"/> 实例。
	/// </summary>
	/// <param name="builder">可被复用的 <see cref="StringBuilder"/> 实例。</param>
	public static void Return(StringBuilder builder)
	{
		pool ??= new StringBuilderPool();
		pool.stack.Push(builder);
	}

	/// <summary>
	/// 释放指定的 <see cref="StringBuilder"/> 实例，并返回其内容。
	/// </summary>
	/// <param name="builder">可被复用的 <see cref="StringBuilder"/> 实例。</param>
	/// <returns><paramref name="builder"/> 的内容。</returns>
	public static string GetStringAndReturn(StringBuilder builder)
	{
		string result = builder.ToString();
		Return(builder);
		return result;
	}

	/// <summary>
	/// 释放当前线程的 <see cref="StringBuilder"/> 池。
	/// </summary>
	public static void ReleasePool()
	{
		pool = null;
	}
}
