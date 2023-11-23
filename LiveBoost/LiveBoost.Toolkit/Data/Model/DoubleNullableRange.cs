using HandyControl.Data;

namespace LiveBoost.Toolkit.Data;

/// <summary>
///     表示一个可以为空的双精度浮点数范围。
/// </summary>
public struct DoubleNullableRange : IValueRange<double?>
{
    /// <summary>
    ///     获取或设置范围的开始值。
    /// </summary>
    public double? Start { get; set; }

    /// <summary>
    ///     获取或设置范围的结束值。
    /// </summary>
    public double? End { get; set; }
}