// 创建时间：2023-09-07-14:33
// 修改时间：2023-10-11-11:00

#region

using System.Windows.Media.Imaging;

#endregion

namespace LiveBoost.ToolKit.Tools;

public static class StringExtension
{
    /// <summary>
    ///     base64编码的字符串转为图片
    /// </summary>
    /// <param
    ///     name = "imageBase64Str" >
    ///     图片的Base64编码
    /// </param>
    /// <returns> 图片 </returns>
    public static ImageSource? Base64StringToImage(this string imageBase64Str)
    {
        try
        {
            var streamBase = Convert.FromBase64String(imageBase64Str);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(streamBase);
            bi.EndInit();
            return bi;
        }
        catch ( Exception )
        {
            return null;
        }
    }

    /// <summary>
    ///     组合基础路径和相对路径
    /// </summary>
    /// <param name = "basePath" > 基础路径 </param>
    /// <param name = "relativePath" > 相对路径 </param>
    /// <returns> 组合后的路径，使用正斜杠（/） </returns>
    public static string Combine(this string? basePath, string? relativePath)
    {
        // 如果基础路径为空，返回相对路径
        if ( string.IsNullOrEmpty(basePath) || string.IsNullOrEmpty(relativePath) )
        {
            return string.Empty;
        }

        // 组合路径，并将所有反斜杠替换为正斜杠
        return Path.Combine(basePath, relativePath!.Trim('\\', '/')).Replace('\\', '/');
    }
    /// <summary>
    ///     组合基础路径和多个相对路径参数
    /// </summary>
    /// <param name = "basePath" > 基础路径 </param>
    /// <param name = "relativePaths" > 一个或多个相对路径 </param>
    /// <returns> 组合后的路径，使用正斜杠（/） </returns>
    public static string Combine(this string? basePath, params string?[] relativePaths)
    {
        // 如果基础路径为空，返回空字符串
        if ( string.IsNullOrEmpty(basePath) || relativePaths.All(string.IsNullOrEmpty) )
        {
            return string.Empty;
        }

        // 先保证basePath的尾部没有 '/' 或者 '\\'
        basePath = basePath!.TrimEnd('\\', '/');

        // 遍历所有的相对路径，去除前后可能存在的 '/' 和 '\\'
        for ( var i = 0; i < relativePaths.Length; i++ )
        {
            if ( !string.IsNullOrEmpty(relativePaths[i]) )
            {
                relativePaths[i] = relativePaths[i]!.Trim('\\', '/');
            }
        }

        // 组合路径，并将所有反斜杠替换为正斜杠
        return Path.Combine(basePath, Path.Combine(relativePaths)).Replace('\\', '/');
    }
}
