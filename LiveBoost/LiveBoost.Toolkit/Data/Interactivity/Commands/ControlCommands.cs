// 创建时间：2023-09-04-16:50
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Toolkit.Data;

public static class ControlCommands
{
    /// <summary>
    ///     关闭窗口
    /// </summary>
    public static CloseWindowCommand CloseWindowCommand { get; } = new();

    /// <summary>
    ///     最小化窗口
    /// </summary>
    public static MinWindowCommand MinWindowCommand { get; } = new();

    /// <summary>
    ///     最大化或者恢复窗口
    /// </summary>
    public static MaxOrNorWindowCommand MaxOrNorWindowCommand { get; } = new();
}
