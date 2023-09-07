// 创建时间：2023-09-07-9:46
// 修改时间：2023-09-07-9:46

namespace LiveBoost.Toolkit.Tools;

public static class ActionHelper
{
    /// <summary>
    /// 带有超时的任务执行函数
    /// </summary>
    /// <param name="action">要执行的任务</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    public static async Task RunWithTimeout(this Action action, int timeout)
    {
        var cts = new CancellationTokenSource();
        var task = Task.Run(action, cts.Token);
        await Task.Delay(timeout, cts.Token);
        if (!task.IsCompleted)
        {
            cts.Cancel();
        }
    }
}
