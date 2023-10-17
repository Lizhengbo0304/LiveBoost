// 创建时间：2023-09-07-14:02
// 修改时间：2023-10-13-16:17

#region

#endregion

namespace LiveBoost.ToolKit.Tools;

public static class MediaHelper
{
    /// <summary>
    ///     获取媒体信息
    /// </summary>
    /// <param name = "videoPath" > 视频路径 </param>
    /// <param name = "param" > 参数 </param>
    /// <returns> 媒体信息 </returns>
    public static string GetMediaInfo(this string videoPath, string param)
    {
        if ( string.IsNullOrWhiteSpace(videoPath) || string.IsNullOrWhiteSpace(param) )
        {
            return string.Empty;
        }

        if ( !File.Exists(videoPath) )
        {
            return string.Empty;
        }

        try
        {
            using var mi = new MediaInfo.MediaInfo();
            mi.Open(videoPath);
            mi.Option("Inform", param);
            return mi.Inform();
        }
        catch ( Exception )
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///     异步获取媒体信息
    /// </summary>
    /// <param name = "videoPath" > 视频路径 </param>
    /// <param name = "param" > 参数 </param>
    /// <returns> 媒体信息 </returns>
    public static Task<string> GetMediaInfoAsync(this string videoPath, string param)
    {
        return Task.Run(() => videoPath.GetMediaInfo(param));
    }
}
