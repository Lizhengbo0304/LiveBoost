// 创建时间：2023-09-07-10:22
// 修改时间：2023-09-07-10:22

using LiveBoost.Toolkit.Tools;

namespace LiveBoost.Toolkit.Data;

public class PushAccess : INotifyPropertyChanged
{
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if ( EqualityComparer<T>.Default.Equals(field, value) ) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

#endregion
#region Ctor

    public PushAccess()
    {
        RecordFiles = new ObservableList<RecordFile>();
    }

#endregion
#region Properties
/// <summary>
/// 通道ID
/// </summary>
    [JsonProperty("accessId")] public string? AccessId { get; set; }
/// <summary>
/// 通道名称
/// </summary>
    [JsonProperty("title")] public string? Title { get; set; }
    /// <summary>
    /// 0=横版 1=竖版
    /// </summary>
    [JsonProperty("mode")] public int Mode { get; set; }
    /// <summary>
    ///     通道状态，true可用，false不可用
    /// </summary>
    [JsonProperty("status")]
    public bool Status { get; set; }
#endregion
#region UI-Properties
    /// <summary>
    /// 通道图标
    /// </summary>
    public string ModeImage =>
        Mode == 0 ? "pack://application:,,,/LiveBoost.Toolkit;component/Images/PlayListModule/computer.png" : "pack://application:,,,/LiveBoost.Toolkit;component/Images/PlayListModule/mobile.png";
    /// <summary>
    ///     播单列表
    /// </summary>
    public ObservableList<RecordFile> RecordFiles { get; set; }
    // 是否选中
    public bool IsSelected { get; set; }
    // 拖放
    public PlayListDragHandler PlayListDragHandler => new();
    public PlayListDropHandler PlayListDropHandler => new();
    public bool IsPause { get; set; }
    //当前播放进度
    public int CurrentIndex { get; set; }
    // 当前播放时间
    public TimeSpan CurrentTime { get; set; }
    // 是否正在编辑
    public bool IsEdit { get; set; }

#endregion
}
