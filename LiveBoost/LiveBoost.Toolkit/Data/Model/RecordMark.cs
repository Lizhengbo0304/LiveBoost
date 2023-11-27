// 创建时间：2023-09-07-10:33
// 修改时间：2023-10-13-16:17

#region

using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.Toolkit.Data;

public sealed class RecordMark : INotifyPropertyChanged
{
    #region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region Ctor

    public RecordMark()
    {
        IconPath = new LazyProperty<ImageSource>(_ =>
        {
            return Task.Run(() => AppConfig.Instance.ShouluPath!.Combine(Thumb)
                .GetThumbnailByPath(AppConfig.Instance.DefaultIcon));
        }, AppConfig.Instance.DefaultIcon);
        Duration = new LazyProperty<TimeSpan>(_ =>
        {
            if (!TimeSpan.TryParse(InPoint, out var realinpotDateTime))
            {
                realinpotDateTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(OutPoint, out var realoutpotDateTime))
            {
                realoutpotDateTime = TimeSpan.Zero;
            }

            return Task.FromResult(realoutpotDateTime - realinpotDateTime);
        }, TimeSpan.Zero);
    }

    public RecordFile? Parent { get; set; }

    #endregion

    #region Property

    [JsonProperty("id")] public string? Id { get; set; }

    [JsonProperty("recordId")] public string? RecordId { get; set; }

    [JsonProperty("name")] public string? Name { get; set; }

    [JsonProperty("inPoint")] public string? InPoint { get; set; }

    [JsonProperty("outPoint")] public string? OutPoint { get; set; }

    [JsonProperty("duration")] public string? DurationStr { get; set; }

    [JsonProperty("description")] public string? Description { get; set; }
    [JsonProperty("url")] public string? Url { get; set; }
    [JsonProperty("channelId")] public string? ChannelId { get; set; }

    [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
    public string? Thumb { get; set; }

    public string ToolTip
    {
        get
        {
            if (!TimeSpan.TryParse(InPoint, out var inpoint))
            {
                inpoint = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(OutPoint, out var outpoint))
            {
                outpoint = TimeSpan.Zero;
            }

            var length = outpoint - inpoint;
            var inFrame =
                $"{(int)inpoint.TotalHours:00}:{inpoint.Minutes:00}:{inpoint.Seconds:00}:{inpoint.Milliseconds / 40:00}";
            var outFrame =
                $"{(int)outpoint.TotalHours:00}:{outpoint.Minutes:00}:{outpoint.Seconds:00}:{outpoint.Milliseconds / 40:00}";
            var lengTip =
                $"{(int)length.TotalHours:00}:{length.Minutes:00}:{length.Seconds:00}:{length.Milliseconds / 40:00}";
            return $"名称:{Name}\n\r入点:{inFrame}\n\r出点:{outFrame}\n\r时长:{lengTip}";
        }
    }

    #endregion

    #region UI - Property

    public LazyProperty<ImageSource> IconPath { get; set; }

    /// <summary>
    ///     图标真实路径
    /// </summary>
    public LazyProperty<TimeSpan> Duration { get; }

    #endregion
}
