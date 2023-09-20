// 创建时间：2023-09-17-13:00
// 修改时间：2023-09-19-14:01

using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;

namespace LiveBoost.Controls;

public sealed class JggItem : ListViewItem, INotifyPropertyChanged,IJggItem
{
#region Event
    public void OnLoaded(object? sender, RoutedEventArgs? e)
    {
        if ( this.FindVisualParent<JggListView>() is not { } jggListView )
        {
            return; // 如果无法找到CombinationListView的父级元素，则直接返回
        }
        if ( jggListView.FindVisualChild<UniformGrid>() is not { } uniformGrid )
        {
            return; // 如果无法找到UniformGrid，直接返回
        }

        var index = jggListView.ItemContainerGenerator.IndexFromContainer(this); // 获取当前元素在CombinationListView中的索引
        Margin = uniformGrid.Rows switch
        {
            2 => new Thickness(5,5, index % 2 == 1 ? 5 : 0 ,index >= 2 ? 5 : 0),
            3 =>new Thickness(5,5, index % 3 == 2 ? 5 :0 ,index >= 6 ? 5 : 0),
            4 =>new Thickness(5,5, index % 4 == 3 ? 5 :0 ,index >= 12 ? 5 : 0),
            _ => Margin
        };
    }

    // 初始化播放器
    private async Task InitPlayer()
    {
        await Task.Run(async () =>
        {
                try
                {
                    PlayProcess = LoadPluginHelper.LoadPlugin("LiveBoost.Player.exe", Guid, "1"); // 加载播放器进程
                    PlayProcess.Start(); // 启动播放器进程
                    await Task.Run(() => PlayProcess?.WaitForExit()); // 等待播放器进程退出
                }
                catch ( Exception ex )
                {
                    // 处理异常，例如记录日志或者进行其他操作
                    MessageBox.Warning(ex.Message, "初始化播放器时发生异常"); // 显示异常信息
                }
        });
    }

#endregion
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if ( propertyName == nameof(Channel) )
        {
            if ( Channel == null )
            {
                PlayProcess?.Kill();
            } else
            {
                Task.Run(InitPlayer);
            }
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
#region Ctor

    static JggItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(JggItem),
            new FrameworkPropertyMetadata(typeof(JggItem)));
    }

    public JggItem()
    {
        Loaded += OnLoaded;
        // 初始化Ipc
        IpcLazy = new Lazy<IJggPlayer>(() =>
        {
            var clientBuilder = new ContainerClientBuilder();
            clientBuilder.UseNamedPipe(Guid + "back").UseJsonSerializer();
            var client = clientBuilder.Build();
            // 从以上构建好的 client 中取用合同实例。
            var same = client.Resolve<IJggPlayer>();
            return same;
        });
        // 创建一个容器服务器 Builder。
        var serverBuilder = new ContainerServerBuilder();
        serverBuilder.UseNamedPipe(Guid).UseJsonSerializer();

        serverBuilder.Register<IJggItem>(() => this);
        var server = serverBuilder.Build();

        // 别忘了启动服务器哦！
        server.Start();
        SourceMonitorDragEvent.SubscriptionTokens.Add(GlobalEvent.Instance.GetEvent<SourceMonitorDragEvent>().Subscribe(( isDragging =>
        {
            this.IsDragging = isDragging;
        })));
    }



#endregion
#region Property
    /// <summary>
    /// 是否正在拖拽
    /// </summary>
    public bool IsDragging { get; set; }

    /// <summary>
    /// 播放器进程通信
    /// </summary>
    private Lazy<IJggPlayer> IpcLazy { get; }
    /// <summary>
    /// Guid
    /// </summary>
    private readonly string Guid = System.Guid.NewGuid().ToString("N");
    /// <summary>
    /// 播放进程
    /// </summary>
    public Process? PlayProcess { get; set; }
    public ViewHost? PlayerHost { get; set; }

    /// <summary>
    /// 频道
    /// </summary>
    public RecordChannel? Channel { get; set; }
#endregion

#region IJggItem
    private static readonly SemaphoreSlim _controlSemaphoreSlim = new(1, 1);

    /// <inheritdoc />
    public async void SendPlayer(int handle)
    {
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        try
        {
            // 创建一个 ViewHost 实例用于播放
            var content = new ViewHost(new IntPtr(handle));
            PlayerHost = content;
            Content = PlayerHost;
        }
        finally
        {
            _controlSemaphoreSlim.Release();
        }
    }

    /// <summary>
    /// 清除频道信息
    /// </summary>
    public void ClearChannel()
    {

    }
#endregion
}
