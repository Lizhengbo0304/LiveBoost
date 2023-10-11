// 创建时间：2023-09-05-10:51
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Toolkit.Data;

[Flags]
public enum ExecutionState : uint
{
    /// <summary>
    ///     表示系统需要处于工作状态，通过重置系统空闲计时器来实现。
    /// </summary>
    SystemRequired = 0x01,

    /// <summary>
    ///     表示需要保持显示器开启，通过重置显示器空闲计时器来实现。
    /// </summary>
    DisplayRequired = 0x02,

    /// <summary>
    ///     [已过时] 用户当前处于活动状态。
    /// </summary>
    [Obsolete("This value is not supported.")]
    UserPresent = 0x04,

    /// <summary>
    ///     启用离开模式，必须与 Continuous 一起使用。
    ///     离开模式应该仅由需要在计算机休眠时执行关键后台处理的媒体录制和媒体分发应用程序使用。
    /// </summary>
    AwaymodeRequired = 0x40,

    /// <summary>
    ///     指示设置的状态将保持有效，直到下一次使用 Continuous 和其他状态标志之一的调用将其清除。
    /// </summary>
    Continuous = 0x80000000
}
