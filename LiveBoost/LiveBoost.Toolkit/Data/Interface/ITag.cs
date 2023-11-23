namespace LiveBoost.Toolkit.Data;

/// <summary>
///     标记点接口
/// </summary>
public interface ITag : INotifyPropertyChanged
{
    /// <summary>
    ///     入点值
    /// </summary>
    public double? InPointValue { get; set; }

    /// <summary>
    ///     出点值
    /// </summary>
    public double? OutPointValue { get; set; }

    /// <summary>
    ///     标签颜色
    /// </summary>
    public Brush TagBrush { get; set; }

    /// <summary>
    ///     更新标签
    /// </summary>
    public void UpdateTag();
}