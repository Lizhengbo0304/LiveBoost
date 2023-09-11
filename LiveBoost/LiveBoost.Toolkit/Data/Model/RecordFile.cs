// 创建时间：2023-09-07-10:31
// 修改时间：2023-09-07-12:02

using LiveBoost.ToolKit.Tools;

namespace LiveBoost.Toolkit.Data;

public sealed class RecordFile : INotifyPropertyChanged, IFileHierarchy
{
    public RecordFile()
    {
        IconPath = new LazyProperty<ImageSource>(_ =>
        {
            return Task.Run(() =>
            {
                switch ( Type )
                {
                    case 0:
                    case 1:
                        return AppConfig.Instance.FolderIcon;
                    default:
                        return string.IsNullOrEmpty(AppConfig.Instance.ShouluPath?.Combine(Thumb)) ?
                            AppConfig.Instance.DefaultIcon :
                            AppConfig.Instance.ShouluPath!.Combine(Thumb).GetThumbnailByPath(AppConfig.Instance.DefaultIcon);
                }
            });
        }, AppConfig.Instance.DefaultIcon);
        Duration = new LazyProperty<TimeSpan>(async _ =>
        {
            if ( RealInPoint is not null && RealOutPoint is not null )
            {
                return RealOutPoint.Value - RealInPoint.Value;
            }
            var duration = await FullPath.GetMediaInfoAsync("General;%Duration%");
            if ( !int.TryParse(duration, out var result) )
            {
                result = 0;
            }
            return TimeSpan.FromMilliseconds(result);
        }, TimeSpan.Zero);
    }
#region Event

    public RecordFile Clone()
    {
        var newFile = new RecordFile
        {
            Id = Guid.NewGuid().ToString("N"),
            IsSub = true,
            Url = Url,
            Thumb = Thumb,
            CreateDate = CreateDate, CreateUser = CreateUser,
            Type = Type, Name = Name, Status = Status, Stream = Stream,
            ParentIds = ParentIds,
            ParentId = ParentId,
            OutPoint = OutPoint, InPoint = InPoint, RealInPoint = RealInPoint, RealOutPoint = RealOutPoint
        };
        return newFile;
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
        if ( EqualityComparer<T>.Default.Equals(field, value) )
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

#endregion

#region IFileHierarchyEvent

    /// <inheritdoc />
    public IFileHierarchy? ParentFile { get; set; }

    /// <inheritdoc />
    public List<IFileHierarchy> Parents
    {
        get
        {
            var list = new List<IFileHierarchy> {this};
            var item = ParentFile;
            while ( item != null )
            {
                list.Add(item);
                item = item.ParentFile;
            }

            list.Reverse();
            return list;
        }
    }

#endregion
#region UI - Property

    public string? ToolTip => Name;
    //完整路径
    public string FullPath => AppConfig.Instance.ShouluPath!.Combine(Url);

    public int SearchType { get; set; }
    //播放进度
    public double Progress { get; set; }

    /// <summary>
    ///     图标真实路径
    /// </summary>
    public LazyProperty<ImageSource> IconPath { get; }

    /// <summary>
    ///     图标真实路径
    /// </summary>
    public LazyProperty<TimeSpan> Duration { get; }

    /// <summary>
    ///     是否展开节点
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    ///     子集
    /// </summary>
    public ObservableList<RecordFile>? Children { get; set; }

    /// <summary>
    ///     是否处于收录中
    /// </summary>
    public bool IsRecording => Type != 1 && Status != 0;

    /// <summary>
    ///     父节点
    /// </summary>
    public RecordFile? Parent { get; set; }

    /// <summary>
    ///     是否为片段
    /// </summary>
    [JsonProperty("isSub")]
    public bool IsSub { get; set; }

    /// <summary>
    ///     入点
    /// </summary>
    [JsonProperty("inPoint")]
    public TimeSpan? InPoint { get; set; }

    /// <summary>
    ///     出点
    /// </summary>
    [JsonProperty("outPoint")]
    public TimeSpan? OutPoint { get; set; }

    /// <summary>
    ///     入点
    /// </summary>
    [JsonProperty("realInPoint")]
    public TimeSpan? RealInPoint { get; set; }

    /// <summary>
    ///     出点
    /// </summary>
    [JsonProperty("realOutPoint")]
    public TimeSpan? RealOutPoint { get; set; }

    public bool IsPlaying { get; set; }

    public Unosquare.FFME. MediaElement? MediaElement { get; set; }

#endregion

#region Property

    [JsonProperty("id")] public string? Id { get; set; }

    [JsonProperty("parentId")] public string? ParentId { get; set; }

    [JsonProperty("createUser")] public string? CreateUser { get; set; }

    [JsonProperty("parentIds")] public string? ParentIds { get; set; }

//对应的流地址
    [JsonProperty("stream")] public string? Stream { get; set; }

//收录状态 0-未收录 1-收录中
    [JsonProperty("status")] public long Status { get; set; }

    [JsonProperty("name")] public string? Name { get; set; }

//1=文件夹 2=文件
    [JsonProperty("type")] public long Type { get; set; }

    [JsonProperty("createDate")] public DateTime? CreateDate { get; set; }

    [JsonProperty("thumb")] public string? Thumb { get; set; }

    //收录文件路径
    [JsonProperty("url")] public string? Url { get; set; }

    [JsonProperty("markers", NullValueHandling = NullValueHandling.Ignore)]
    public ObservableList<RecordMark>? Markers { get; set; }

#endregion
}
