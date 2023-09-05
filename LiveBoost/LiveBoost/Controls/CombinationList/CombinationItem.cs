// 创建时间：2023-09-05-11:17
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Controls;

public sealed class CombinationItem : ListViewItem, INotifyPropertyChanged
{
#region Construct

    static CombinationItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CombinationItem),
            new FrameworkPropertyMetadata(typeof(CombinationItem)));
    }

    public CombinationItem() => Loaded += OnLoaded;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        if ( this.FindVisualParent<CombinationListView>() is not { } combination )
        {
            return;
        }
        var index = combination.ItemContainerGenerator.IndexFromContainer(this);
        Margin = index switch
        {
            1 => new Thickness(-5, 0, 0, 0),
            2 => new Thickness(0, -5, 0, 0),
            3 => new Thickness(-5, -5, 0, 0),
            _ => new Thickness(0)
        };
        IsEnabled = DataContext is not null;
    }

#endregion
#region Property

    /// <summary>
    ///     是否选中：未选中状态：添加按钮  选中状态：预览界面
    /// </summary>
    public bool IsChecked { get; set; }
    /// <summary>
    ///     通道名称
    /// </summary>
    public string? AccessName { get; set; }
    /// <summary>
    ///     通道协议
    /// </summary>
    public string? Protocol { get; set; }
    /// <summary>
    ///     视频分辨率
    /// </summary>
    public string? VideoSize { get; set; }
    /// <summary>
    ///     左声道值
    /// </summary>
    public double DrawVuMeterLeftValue { get; set; }
    /// <summary>
    ///     右声道值
    /// </summary>
    public double DrawVuMeterRightValue { get; set; }

#endregion
#region Command

    /// <summary>
    ///     添加收录频道命令
    /// </summary>
    public DelegateCommand AddShouluChannelCommand => new(() => { });
    /// <summary>
    ///     切换频道
    /// </summary>
    public DelegateCommand ChangedChannelCmd => new(() => { });

    /// <summary>
    ///     删除频道
    /// </summary>
    public DelegateCommand DeleteChannelCmd => new(() => { });

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
