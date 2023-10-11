// 创建时间：2023-09-06-9:05
// 修改时间：2023-10-11-11:00

#region

using System.Runtime.InteropServices;

#endregion

namespace LiveBoost.Toolkit.Tools;

public static class VolumeHelper
{
    [DllImport("winmm.dll")]
    private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

    [DllImport("winmm.dll")]
    private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

    /// <summary>
    ///     获取当前系统音量级别。
    /// </summary>
    /// <returns> 音量级别，范围从0（最小）到10（最大）。 </returns>
    public static int GetVolume()
    {
        waveOutGetVolume(IntPtr.Zero, out var currentVolume);
        var volume = (ushort) ( currentVolume & 0xFFFF );
        var volumeLevel = volume / ( ushort.MaxValue / 10 );
        return volumeLevel;
    }

    /// <summary>
    ///     设置系统音量级别。
    /// </summary>
    /// <param name = "volume" > 音量级别，范围从0（最小）到10（最大）。 </param>
    public static void SetVolume(int volume)
    {
        var newVolume = (ushort) ( ushort.MaxValue / 10 * volume );
        var newVolumeAllChannels = (uint) newVolume << 16 | newVolume;
        waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
    }

    /// <summary>
    ///     计算音频信号的均方根（RMS）分贝值。
    /// </summary>
    /// <param name = "samples" > 音频样本数据。 </param>
    /// <returns> RMS分贝值。 </returns>
    public static double CalculateRms(short[] samples)
    {
        var sum = samples.Aggregate<short, double>(0, (current, t) => current + t * t);
        var rms = Math.Sqrt(sum / ( samples.Length / 2.0 ));
        var db = 20 * Math.Log10(rms);
        return db;
    }
}
