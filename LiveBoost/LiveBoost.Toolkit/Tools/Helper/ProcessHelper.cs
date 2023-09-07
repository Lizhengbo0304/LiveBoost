// 创建时间：2023-09-04-11:59
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Toolkit.Tools;

public static class ProcessHelper
{
    /// <summary>
    ///     该进程是否唯一
    /// </summary>
    /// <param
    ///     name = "Mutex" >
    /// </param>
    /// <returns>
    ///     True
    ///     该进程唯一
    ///     FALSE
    ///     该进程不唯一
    /// </returns>
    public static bool IsOnlyProcess(this Mutex Mutex)
    {
        try
        {
            return Mutex.WaitOne(TimeSpan.Zero, true);
        }
        catch ( Exception )
        {
            return true;
        }
    }

    /// <summary>
    ///     根据进程名称判断该进程是否存在
    /// </summary>
    /// <param name = "processName" > 进程名称 </param>
    /// <returns> 是否存在 </returns>
    public static bool IsProcessExist(this string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        return processes.Length > 0;
    }

    /// <summary>
    ///     根据进程名称获取进程
    /// </summary>
    /// <param name = "processName" > 进程名称 </param>
    /// <returns> 进程 </returns>
    public static Process? GetProcess(this string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        return processes.Any() ? processes.First() : null;
    }
}
