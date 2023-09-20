// 创建时间：2023-09-07-14:02
// 修改时间：2023-09-19-14:01

#region

using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;

#endregion

namespace LiveBoost.ToolKit.Tools;

public static class LoadImageHelper
{
    /// <summary>
    ///     获取缩略图
    /// </summary>
    /// <param
    ///     name = "filePath" >
    /// </param>
    /// <param
    ///     name = "defaultImage" >
    ///     默认图片
    /// </param>
    /// <returns> ImageSource </returns>
    public static ImageSource GetThumbnailByPath(this string filePath, ImageSource defaultImage)
    {
        if ( !File.Exists(filePath) )
        {
            return defaultImage;
        }
        using var shellFile = ShellFile.FromFilePath(filePath);
        try
        {
            if ( shellFile.Thumbnail is null )
            {
                return defaultImage;
            }
            shellFile.Thumbnail.RetrievalOption = ShellThumbnailRetrievalOption.Default;
            shellFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
            ImageSource source = shellFile.Thumbnail.MediumBitmapSource;
            source.Freeze();
            return source;
        }
        catch ( Exception )
        {
            try
            {
                if ( shellFile.Thumbnail is null )
                {
                    return defaultImage;
                }
                shellFile.Thumbnail.RetrievalOption = ShellThumbnailRetrievalOption.Default;
                shellFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                ImageSource source = shellFile.Thumbnail.MediumBitmapSource;
                source.Freeze();
                return source;
            }
            catch ( Exception )
            {
                return defaultImage;
            }
        }
    }

    /// <summary>
    ///     保存图片
    /// </summary>
    /// <param
    ///     name = "image" >
    ///     图片
    /// </param>
    /// <param
    ///     name = "fileName" >
    ///     保存地址
    /// </param>
    /// <returns> 保存结果 </returns>
    public static bool SaveImage(this ImageSource image, string fileName)
    {
        try
        {
            using var fileStream = new FileStream(fileName, FileMode.Create);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource) image));
            encoder.Save(fileStream);
            return true;
        }
        catch ( Exception e )
        {
            e.LogError("缩略图保存异常");
            return false;
        }
    }
}
