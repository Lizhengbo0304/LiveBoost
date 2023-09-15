// 创建时间：2023-09-05-14:16
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Toolkit.Data;

public sealed class RecordChannel : INotifyPropertyChanged
{
#region UI-Property

    /// <summary>
    ///     是否显示（用于检索过滤时的数据隐藏）
    /// </summary>
    public bool IsShow { get; set; } = true;

#endregion
#region Property

    //频道ID
    [JsonProperty("channelId")] public string? ChannelId { get; set; }

//频道名称
    [JsonProperty("channelName")] public string? ChannelName { get; set; }
//协议类型
    [JsonProperty("protocol")] public string? Protocol { get; set; }

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
