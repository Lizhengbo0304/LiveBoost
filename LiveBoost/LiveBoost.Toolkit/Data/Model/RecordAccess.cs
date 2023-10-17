// 创建时间：2023-09-05-14:21
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public sealed class RecordAccess : INotifyPropertyChanged
{
#region Property

    /// <summary>
    ///     通道ID
    /// </summary>
    public string? AccessId { get; set; }
    /// <summary>
    ///     通道名称
    /// </summary>
    public string? AccessName { get; set; }

    /// <summary>
    ///     当前频道
    /// </summary>
    public RecordChannel? Channel { get; set; }

    /// <summary>
    ///     视频地址
    /// </summary>
    public string? VideoPath { get; set; }

    /// <summary>
    ///     任务ID
    /// </summary>
    public string? TaskId { get; set; }

#endregion

#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if ( EqualityComparer<T>.Default.Equals(field, value) )
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

#endregion
}
