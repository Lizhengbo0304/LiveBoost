// 创建时间：2023-10-08-10:44
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public sealed class RecordMission : INotifyPropertyChanged, IIndex
{
    /// <summary>
    ///     任务ID
    /// </summary>
    [JsonProperty("missionId")]
    public string? MissionId { get; set; }

    /// <summary>
    ///     任务名称
    /// </summary>
    [JsonProperty("missionName")]
    public string? MissionName { get; set; }

    /// <summary>
    ///     任务类型：real 即时任务 loop周期任务
    /// </summary>
    [JsonProperty("missionFlag")]
    public string? MissionFlag { get; set; }

    /// <summary>
    ///     服务器ID
    /// </summary>
    [JsonProperty("clientId")]
    public string? ClientId { get; set; }

    /// <summary>
    ///     服务器名称
    /// </summary>
    [JsonProperty("clientName")]
    public string? ClientName { get; set; }

    /// <summary>
    ///     收录路径
    /// </summary>
    [JsonProperty("recordPath")]
    public string? RecordPath { get; set; }

    /// <summary>
    ///     收录格式
    /// </summary>
    [JsonProperty("videoType")]
    public string? VideoType { get; set; }

    /// <summary>
    ///     视频编码
    /// </summary>
    [JsonProperty("videoCodec")]
    public string? VideoCodec { get; set; }

    /// <summary>
    ///     音频编码
    /// </summary>
    [JsonProperty("audioCodec")]
    public string? AudioCodec { get; set; }

    /// <summary>
    ///     分辨率
    /// </summary>
    [JsonProperty("resolution")]
    public string? Resolution { get; set; }

    [JsonProperty("width")] public string? Width { get; set; }

    [JsonProperty("height")] public string? Height { get; set; }

    /// <summary>
    ///     场扫描
    /// </summary>
    [JsonProperty("interlaced")]
    public string? Interlaced { get; set; }

    [JsonProperty("videoFramerate")] public string? VideoFramerate { get; set; }

    /// <summary>
    ///     帧率
    /// </summary>
    [JsonProperty("framerate")]
    public string? Framerate { get; set; }

    /// <summary>
    ///     切片时长
    /// </summary>
    [JsonProperty("segmentTime")]
    public DateTime SegmentTime { get; set; }

    /// <summary>
    ///     频道ID
    /// </summary>
    [JsonProperty("channelId")]
    public string? ChannelId { get; set; }

    /// <summary>
    ///     频道名称
    /// </summary>
    [JsonProperty("channelName")]
    public string? ChannelName { get; set; }

    /// <summary>
    ///     频道地址
    /// </summary>
    [JsonProperty("streamURL")]
    public string? StreamUrl { get; set; }

    /// <summary>
    ///     频道协议
    /// </summary>
    [JsonProperty("protocol")]
    public string? Protocol { get; set; }

    /// <summary>
    ///     NDI
    /// </summary>
    [JsonProperty("ndiGroup")]
    public string? NdiGroup { get; set; }

    /// <summary>
    ///     NDI名称
    /// </summary>
    [JsonProperty("ndiName")]
    public string? NdiName { get; set; }

    /// <summary>
    ///     码率
    /// </summary>
    [JsonProperty("bitRate")]
    public string? BitRate { get; set; }

    /// <summary>
    ///     开始日期
    /// </summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    ///     截止日期
    /// </summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    ///     开始时间
    /// </summary>
    [JsonProperty("startTime")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    ///     结束时间
    /// </summary>
    [JsonProperty("endTime")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     制定周期
    /// </summary>
    [JsonProperty("weeks")]
    public string? Weeks { get; set; }

    /// <summary>
    ///     状态
    /// </summary>
    [JsonProperty("status")]
    public bool Status { get; set; }

    /// <inheritdoc />
    public int Index { get; set; }

    #region UI-Property

    /// <summary>
    ///     UI展示的封装格式
    /// </summary>
    public string Format => $"{VideoType} ( {VideoCodec} + {AudioCodec} )";

    /// <summary>
    ///     场扫描
    /// </summary>
    public string InterlacedString => string.IsNullOrEmpty(Interlaced) ? string.Empty : string.Equals(Interlaced, "i", StringComparison.OrdinalIgnoreCase) ? "隔行" : "逐行";

    /// <summary>
    ///     起止日期
    /// </summary>
    public string DateRange => $"{StartDate?.ToString("yyyy-MM-dd") ?? "***"}至{EndDate?.ToString("yyyy-MM-dd") ?? "***"}";

    /// <summary>
    ///     起止时间
    /// </summary>
    public string TimeRange => $"{StartTime?.ToString("HH:mm:ss") ?? "***"}至{EndTime?.ToString("HH:mm:ss") ?? "***"}";

    public string Size => string.IsNullOrEmpty(Width) ? string.Empty : $"{Width} x {Height}";

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
}