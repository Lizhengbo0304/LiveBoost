// 创建时间：2023-09-07-10:22
// 修改时间：2023-10-13-16:17

#region

using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.Toolkit.Data;

public sealed class PushAccess : INotifyPropertyChanged
{
#region Ctor

    public PushAccess() => RecordFiles = new ObservableList<RecordFile>();

#endregion
#region Event

    /// <summary>
    ///     当播单内容发生变化时触发的事件处理方法
    /// </summary>
    public void RecordFilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        GlobalEvent.Instance.GetEvent<PushAccessRecordFilesChanged>().Publish(this);
    }

#endregion
#region Command

    /// <summary>
    ///     删除选定的记录文件。
    /// </summary>
    public DelegateCommand<IList> DeleteCmd => new(DeleteItems);
    /// <summary>
    ///     删除选定的记录文件。
    /// </summary>
    /// <param name = "items" > 要删除的记录文件的集合。 </param>
    private void DeleteItems(IList items)
    {
        var selectedItems = items.ToList<RecordFile>();
        if ( selectedItems.Count != 1 )
        {
            return;
        }
        var selectedItem = selectedItems.First();
        var selectedIndex = RecordFiles.IndexOf(selectedItem);
        if ( selectedIndex <= CurrentIndex + 1 && !Status )
        {
            // 如果选定的项已锁定，则显示警告消息框
            MessageBox.Show("此项已锁定，无法删除", "播单项删除", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            if ( RecordFiles.Contains(selectedItem) )
            {
                RecordFiles.RemoveItem(selectedItem);
            }
        }
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
#region Properties

    /// <summary>
    ///     通道ID
    /// </summary>
    [JsonProperty("accessId")]
    public string? AccessId { get; set; }
    /// <summary>
    ///     通道名称
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }
    /// <summary>
    ///     0=横版 1=竖版
    /// </summary>
    [JsonProperty("mode")]
    public int Mode { get; set; }
    /// <summary>
    ///     通道状态，true可用，false不可用
    /// </summary>
    [JsonProperty("status")]
    public bool Status { get; set; }

#endregion
#region UI-Properties

    /// <summary>
    ///     通道图标
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
