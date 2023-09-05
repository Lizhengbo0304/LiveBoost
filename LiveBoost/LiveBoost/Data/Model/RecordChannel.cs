// 创建时间：2023-09-05-14:16
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Data;

public sealed class RecordChannel : INotifyPropertyChanged
{
#region Property

    //频道ID
    [JsonProperty("channelId")] public string? ChannelId { get; set; }

//频道名称
    [JsonProperty("channelName")] public string? ChannelName { get; set; }

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
