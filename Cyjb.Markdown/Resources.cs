//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由工具生成。
//
// 对此文件的更改可能会导致不正确的行为，并且如果
// 重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cyjb.Markdown;

using ComponentModel = global::System.ComponentModel;
using ResourceManager = global::System.Resources.ResourceManager;
using CultureInfo = global::System.Globalization.CultureInfo;

/// <summary>
/// 一个强类型的资源类，用于查找本地化的字符串等。
/// </summary>
/// <remarks>此类是由 T4 文本模板通过 Visual Studio 的工具自动生成的。
/// 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 T4 模板。
/// </remarks>
[global::System.Diagnostics.DebuggerNonUserCodeAttribute]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
internal partial class Resources
{
	#nullable enable

	private static ResourceManager? resourceManager;
	private static CultureInfo? resourceCulture;

	/// <summary>
	/// 获取此类使用的缓存的 <see cref="ResourceManager"/> 实例。
	/// </summary>
	[ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			resourceManager ??= new ResourceManager("Cyjb.Markdown.Resources", typeof(Resources).Assembly);
			return resourceManager;
		}
	}

	/// <summary>
	/// 获取或设置资源使用的区域信息。
	/// </summary>
	[ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)]
	internal static CultureInfo? Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}
	
	/// <summary>
	/// 返回类似 <c>Emoji name can not contains colon.</c> 的本地化字符串。
	/// </summary>
	internal static string EmojiNameCanNotContainsColon => ResourceManager.GetString("EmojiNameCanNotContainsColon", resourceCulture)!;
	
	/// <summary>
	/// 返回类似 <c>Link label can not be empty or consists only of white-space characters.</c> 的本地化字符串。
	/// </summary>
	internal static string EmptyLinkLabel => ResourceManager.GetString("EmptyLinkLabel", resourceCulture)!;
	
	/// <summary>
	/// 返回类似 <c>Invalid kind, expect one of 'HtmlStartTag', 'HtmlEndTag', 'HtmlComment', 'HtmlCData', 'HtmlDeclaration' or 'HtmlProcessing'.</c> 的本地化字符串。
	/// </summary>
	internal static string InvalidKindHtml => ResourceManager.GetString("InvalidKind_Html", resourceCulture)!;
	
	/// <summary>
	/// 返回类似 <c>TableRow must have at least one TableCell.</c> 的本地化字符串。
	/// </summary>
	internal static string RowMustHaveCell => ResourceManager.GetString("RowMustHaveCell", resourceCulture)!;
	
	/// <summary>
	/// 返回类似 <c>Table must have at least one TableRow as heading.</c> 的本地化字符串。
	/// </summary>
	internal static string TableMustHaveHeading => ResourceManager.GetString("TableMustHaveHeading", resourceCulture)!;
	
	/// <summary>
	/// 将指定对象格式化为字符串。
	/// </summary>
	/// <param name="value">要格式化的对象。</param>
	private static object Format(object? value)
	{
		if (value == null)
		{
			return "(null)";
		}
		return value;
	}

	#nullable restore

}


