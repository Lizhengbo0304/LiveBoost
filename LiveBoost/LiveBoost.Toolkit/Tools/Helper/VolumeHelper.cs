// 创建时间：2023-07-12-11:19
// 修改时间：2023-07-18-9:37

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

    public static int GetVolume()
    {
        waveOutGetVolume(IntPtr.Zero, out var currentVolume);
        var volume = (ushort) ( currentVolume & 0xFFFF );
        var volumeLevel = volume / ( ushort.MaxValue / 10 );
        return volumeLevel;
    }

    public static void SetVolume(int volume)
    {
        var newVolume = (ushort) ( ushort.MaxValue / 10 * volume );
        var newVolumeAllChannels = (uint) newVolume << 16 | newVolume;
        waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
    }

    public static double CalculateRms(short[] samples)
    {
        var sum = samples.Aggregate<short, double>(0, (current, t) => current + t * t);
        var rms = Math.Sqrt(sum / (samples.Length / 2.0));
        var db = 20 * Math.Log10(rms);
        return db;
    }

}
