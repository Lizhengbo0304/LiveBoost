// 创建时间：2023-09-07-15:35
// 修改时间：2023-10-13-16:17

namespace LiveBoost.ToolKit.Tools;

public static class FfMpegTool
{
    /// <summary> 获取首帧图片作为缩略图 </summary>
    /// <param name = "videoPath" > 视频路径 </param>
    /// <param name = "picName" > 图片路径 </param>
    /// <param name = "timeSpan" > 指定时间 </param>
    public static async Task GetFrameWithTimeSpan(this string videoPath, string picName, TimeSpan timeSpan)
    {
        try
        {
            var folder = Path.GetDirectoryName(picName);
            if ( string.IsNullOrEmpty(folder) )
            {
                return;
            }
            if ( !Directory.Exists(folder) )
            {
                Directory.CreateDirectory(folder);
            }

            var ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dll", "ffmpeg.exe");
            var arguments = $"""
                              -i "{videoPath}" -f image2 -vframes 1 -ss {timeSpan} -y "{picName}"
                             """;

            using var process = new Process( );
            process.StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            var errorData = new StringBuilder();
            process.ErrorDataReceived += (_, e) => { errorData.AppendLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await Task.Run(() => process.WaitForExit());

            if ( process.ExitCode != 0 )
            {
                throw new Exception($"ffmpeg failed with exit code {process.ExitCode}: {errorData}");
            }
        }
        catch ( Exception e )
        {
            e.LogError("缩略图截取失败");
        }
    }
}
