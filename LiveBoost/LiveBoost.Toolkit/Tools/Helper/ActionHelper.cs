// 创建时间：2023-09-07-9:46
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Tools;

public static class ActionHelper
{
    /// <summary>
    ///     带有超时的任务执行函数
    /// </summary>
    /// <param name = "action" > 要执行的任务 </param>
    /// <param name = "timeout" > 超时时间（毫秒） </param>
    public static async Task RunWithTimeout(this Action action, int timeout = 5000)
    {
        // 创建取消标记源和任务
        var cts = new CancellationTokenSource();
        var task = Task.Run(action, cts.Token);

        try
        {
            // 使用 Task.WhenAny 来等待任务完成或超时
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));

            // 如果任务完成，检查是否有异常，如果有，将其传播出去
            if ( completedTask == task )
            {
                await task; // 等待任务完成，以便传播异常
            }
            else
            {
                // 如果是超时引发的完成，取消任务
                cts.Cancel();
                "任务超时取消".LogInfo();
            }
        }
        catch ( OperationCanceledException )
        {
            // 处理由取消引发的异常
            "任务超时取消".LogInfo();
        }
    }
    /// <summary>
    ///     带有超时的任务执行函数，接受带参数的 Action
    /// </summary>
    /// <typeparam name = "T" > Action 的参数类型 </typeparam>
    /// <param name = "action" > 要执行的任务 </param>
    /// <param name = "parameter" > 传递给 Action 的参数 </param>
    /// <param name = "timeout" > 超时时间（毫秒） </param>
    public static async Task RunWithTimeout<T>(this Action<T> action, T parameter, int timeout = 5000)
    {
        // 创建取消标记源和任务
        var cts = new CancellationTokenSource();
        var task = Task.Run(() => action(parameter), cts.Token);

        try
        {
            // 使用 Task.WhenAny 来等待任务完成或超时
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));

            // 如果任务完成，检查是否有异常，如果有，将其传播出去
            if ( completedTask == task )
            {
                await task; // 等待任务完成，以便传播异常
            }
            else
            {
                // 如果是超时引发的完成，取消任务
                cts.Cancel();
                "任务超时取消".LogInfo();
            }
        }
        catch ( OperationCanceledException )
        {
            // 处理由取消引发的异常
            "任务超时取消".LogInfo();
        }
    }
}
