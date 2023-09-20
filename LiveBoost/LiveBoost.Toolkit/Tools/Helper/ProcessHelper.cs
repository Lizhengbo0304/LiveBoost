// 创建时间：2023-09-04-11:59
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Tools;

public static class ProcessHelper
{
    /// <summary>
    ///     判断该进程是否唯一
    /// </summary>
    /// <param name = "Mutex" > 互斥体 </param>
    /// <returns> 如果该进程是唯一的，则返回true，否则返回false </returns>
    public static bool IsOnlyProcess(this Mutex Mutex)
    {
        try
        {
            // 尝试获取互斥体的所有权，如果成功，则说明该进程是唯一的
            return Mutex.WaitOne(TimeSpan.Zero, true);
        }
        catch ( AbandonedMutexException )
        {
            // 如果互斥体被遗弃（例如，另一个进程已经终止），则我们可以认为该进程是唯一的
            return true;
        }
    }

    /// <summary>
    ///     根据进程名称判断该进程是否存在
    /// </summary>
    /// <param name = "processName" > 进程名称 </param>
    /// <returns> 如果进程存在，则返回true，否则返回false </returns>
    public static bool IsProcessExist(this string processName)
    {
        if ( string.IsNullOrWhiteSpace(processName) )
        {
            return false;
        }

        var processes = Process.GetProcessesByName(processName);
        return processes.Length > 0;
    }

    /// <summary>
    ///     根据进程名称获取进程
    /// </summary>
    /// <param name = "processName" > 进程名称 </param>
    /// <returns> 如果进程存在，则返回进程，否则返回null </returns>
    public static Process? GetProcess(this string processName)
    {
        if ( string.IsNullOrWhiteSpace(processName) )
        {
            return null;
        }

        var processes = Process.GetProcessesByName(processName);
        return processes.FirstOrDefault();
    }
}
