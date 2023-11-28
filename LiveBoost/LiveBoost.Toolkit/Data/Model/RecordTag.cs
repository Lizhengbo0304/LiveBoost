// ReSharper disable UnusedMember.Local
namespace LiveBoost.Toolkit.Data;

/// <summary>
/// 表示一个记录标签，实现了INotifyPropertyChanged接口。
/// </summary>
public sealed class RecordTag : INotifyPropertyChanged
{
    #region INotifyPropertyChangedEvent

    /// <summary>
    /// 属性更改事件。
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 属性更改通知方法。
    /// </summary>
    /// <param name="propertyName">属性名称。</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 设置字段值，并在值更改时触发属性更改通知。
    /// </summary>
    /// <typeparam name="T">字段类型。</typeparam>
    /// <param name="field">要设置的字段。</param>
    /// <param name="value">要设置的值。</param>
    /// <param name="propertyName">属性名称。</param>
    /// <returns>如果字段值发生更改，则为true；否则为false。</returns>
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region TagSlider

    /// <summary>
    /// 获取或设置标签的输入点值。
    /// </summary>
    public double? InPointValue { get; set; }

    /// <summary>
    /// 获取或设置标签的输出点值。
    /// </summary>
    public double? OutPointValue { get; set; }

    /// <summary>
    /// 获取或设置标签的画刷。
    /// </summary>
    public Brush TagBrush { get; set; } = BrushHelper.GetRandomColorBrush(Guid.NewGuid().ToString().GetHashAsInt());

    #endregion
}
