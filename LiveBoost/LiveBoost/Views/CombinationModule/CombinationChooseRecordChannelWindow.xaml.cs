// 创建时间：2023-09-05-17:19
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Views;

public sealed partial class CombinationChooseRecordChannelWindow : INotifyPropertyChanged
{
#region Ctors

    public CombinationChooseRecordChannelWindow()
    {
        InitializeComponent();
    }
    public static async Task<RecordChannel?> ShowChooseWindow(List<RecordAccess?>? recordAccesses)
    {
        var wnd = new CombinationChooseRecordChannelWindow
        {
            Owner = AppProgram.Instance.App.MainWindow,
            Channels = await UrlHelper.GetShouluChannels()
        };
        if ( wnd.Channels?.Any() ?? false )
        {
            wnd.Channels.ForEach(it => it.IsShow = recordAccesses?.Find(access => access?.Channel?.ChannelId == it.ChannelId) is null);
        }
        wnd.ShowDialog();
        return wnd.IsDone ? wnd.SelectedChannel : null;
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
#region Command

    /// <summary>
    ///     确定命令
    /// </summary>
    public DelegateCommand DoneCmd => new(() =>
    {
        if ( SelectedChannel is null )
        {
            MessageBox.Warning("请选择要收录的频道", "选择频道");
            return;
        }
        if ( string.IsNullOrEmpty(SelectedChannel.ChannelId) )
        {
            MessageBox.Warning("选择的频道Id为空，请重新选择", "选择频道");
            return;
        }
        IsDone = true;
        Close();
    });
    /// <summary>
    ///     检索命令
    /// </summary>
    public DelegateCommand<string> SearchCmd => new(keyword =>
    {
        if ( Channels?.Any() ?? false )
        {
            Channels.ForEach(it => it.IsShow = string.IsNullOrEmpty(keyword) || it.ChannelName!.Contains(keyword));
        }
    });

#endregion
#region Property

    /// <summary>
    ///     频道列表
    /// </summary>
    public List<RecordChannel>? Channels { get; set; }
    /// <summary>
    ///     选择的频道
    /// </summary>
    public RecordChannel? SelectedChannel { get; set; }
    /// <summary>
    ///     是否确定选中
    /// </summary>
    public bool IsDone { get; set; }

#endregion
}
