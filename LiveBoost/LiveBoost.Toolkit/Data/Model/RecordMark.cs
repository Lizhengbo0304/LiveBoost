// 创建时间：2023-09-07-10:33
// 修改时间：2023-10-13-16:17

#region

using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.Toolkit.Data;

public sealed class RecordMark : INotifyPropertyChanged
{
    #region Event

    /// <summary>
    ///     隐式转换操作符，将RecordMark对象转换为RecordFile对象
    /// </summary>
    /// <param name="mark"> 要转换的RecordMark对象 </param>
    public static implicit operator RecordFile(RecordMark mark)
    {
        // 如果mark.Parent为null，则使用mark的一部分属性初始化RecordFile对象
        // 否则，使用mark和mark.Parent的一部分属性初始化RecordFile对象
        var recordFile = new RecordFile
        {
            Id = mark.Parent is null ? mark.Id : mark.RecordId,
            IsSub = true,
            Url = mark.Parent?.Url ?? mark.Url,
            Thumb = mark.Thumb,
            Name = mark.Name,
            Type = mark.Parent?.Type ?? 2,
            Status = mark.Parent?.Status ?? 0,
            // 尝试解析时间戳并赋值到RecordFile对象的相应属性
            InPoint = ParseTimeSpan(mark.InPoint?.Substring(0, 8)),
            RealInPoint = ParseTimeSpan(mark.InPoint),
            OutPoint = ParseTimeSpan(mark.OutPoint?.Substring(0, 8)),
            RealOutPoint = ParseTimeSpan(mark.OutPoint)
        };

        // 如果mark.Parent不为null，则将mark.Parent的一部分属性赋值到RecordFile对象
        if (mark.Parent is null)
        {
            return recordFile;
        }

        recordFile.CreateDate = mark.Parent.CreateDate;
        recordFile.CreateUser = mark.Parent.CreateUser;
        recordFile.Stream = mark.Parent.Stream;
        recordFile.ParentId = mark.Parent.ParentId;
        recordFile.ParentIds = mark.Parent.ParentIds;

        return recordFile;
    }

    /// <summary>
    ///     尝试解析输入字符串为TimeSpan对象
    /// </summary>
    /// <param name="input"> 输入字符串 </param>
    /// <returns> 如果解析成功，则返回TimeSpan对象，否则返回null </returns>
    private static TimeSpan? ParseTimeSpan(string? input)
    {
        if (TimeSpan.TryParse(input, out var result))
        {
            return result;
        }

        return null;
    }

    #endregion

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
