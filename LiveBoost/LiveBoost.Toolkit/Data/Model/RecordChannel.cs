// 创建时间：2023-09-05-14:16
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Data;

public sealed class RecordChannel : INotifyPropertyChanged,IIndex
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
    [JsonProperty("streamURL")]
    public string? StreamURL { get; set; }

    // 属性表示了JSON数据中的ndiGroup字段
    [JsonProperty("ndiGroup")]
    public string? NdiGroup { get; set; }

    // 属性表示了JSON数据中的ndiName字段
    [JsonProperty("ndiName")]
    public string? NdiName { get; set; }
// 属性表示了JSON数据中的status字段
    [JsonProperty("status")]
    public bool Status { get; set; }
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
    /// <inheritdoc />
    public int Index { get; set; }
}
