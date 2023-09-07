// 创建时间：2023-06-06-14:15
// 修改时间：2023-07-18-9:37

using LiveBoost.ToolKit.Tools;

namespace LiveBoost.ToolKit.Data;

[JsonConverter(typeof(RecordTemplateConverter))]
public sealed class RecordTemplate : INotifyPropertyChanged
{
    public RecordTemplate() => RecordFiles = new ObservableList<RecordFile>();

    #region Command

    public DelegateCommand<IList> DeleteCmd => new(items =>
    {
        items.ToList<RecordFile>().ForEach(it =>
        {
            if ( RecordFiles.Contains(it) )
            {
                RecordFiles.RemoveItem(it);
            }
        });
    });

    #endregion

    #region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        switch ( propertyName )
        {
            case nameof(Status):
                IsPush = Status == "2";
                break;
        }
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

    #region Property

    [JsonProperty("id")] public string? Id { get; set; }

    [JsonProperty("type")] public string? Type { get; set; }

    [JsonProperty("title")] public string? Title { get; set; }

    [JsonProperty("info")] public string? Info { get; set; }
    [JsonProperty("status")] public string? Status { get; set; }

    /// <summary>
    ///     0=横屏 1=竖屏
    /// </summary>
    [JsonProperty("mode")]
    public int Mode { get; set; }

    [JsonProperty("createTime")] public DateTime? CreateTime { get; set; }

    #endregion

    #region UI - Property

    /// <summary>
    ///     播单列表
    /// </summary>
    public ObservableList<RecordFile> RecordFiles { get; set; }

    /// <summary>
    ///     是否选中
    /// </summary>
    [JsonProperty]
    public bool IsSelected { get; set; }

    public bool IsPush { get; set; }

    // 拖放
    public PlayListDragHandler PlayListDragHandler => new();
    public PlayListDropHandler PlayListDropHandler => new();

    #endregion
}
