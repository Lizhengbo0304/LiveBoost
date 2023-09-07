// 创建时间：2023-06-06-14:22
// 修改时间：2023-07-18-9:37

#region

using System;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace LiveBoost.ToolKit.Tools;

public static class MediaHelper
{
    public static string? GetMediaInfo(this string videoPath, string param)
    {
        try
        {
            if ( string.IsNullOrEmpty(videoPath) )
            {
                return string.Empty;
            }

            if ( !File.Exists(videoPath) )
            {
                return string.Empty;
            }

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

    public static Task<string> GetMediaInfoAsync(this string videoPath, string param)
    {
        return Task.Run(() =>
        {
            try
            {
                if ( string.IsNullOrEmpty(videoPath) )
                {
                    return string.Empty;
                }

                if ( !File.Exists(videoPath) )
                {
                    return string.Empty;
                }

                using var mi = new MediaInfo.MediaInfo();
                mi.Open(videoPath);
                mi.Option("Inform", param);
                return mi.Inform();
            }
            catch ( Exception )
            {
                return string.Empty;
            }
        });
    }
}
