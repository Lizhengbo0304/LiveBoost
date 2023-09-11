// 创建时间：2023-09-05-11:17
// 修改时间：2023-09-11-10:31

#region

using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;
using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.Controls;

public sealed class CombinationItem : ListViewItem, INotifyPropertyChanged, ICombinationItem
{
#region Command

    /// <summary>
    ///     添加收录频道命令
    /// </summary>
    public DelegateCommand AddShouluChannelCommand => new(async () =>
    {
        // 如果CombinationMainWindowVm或RecordAccess为null则直接返回
        if ( CombinationMainWindowVm is null || RecordAccess is null )
        {
            MessageBox.Warning("无法选择频道", "添加收录频道");
            return;
        }
        if ( string.IsNullOrEmpty(AppConfig.Instance.ShouluPath) )
        {
            MessageBox.Warning("收录路径为空，无法选择频道", "添加收录频道");
            return;
        }
        var channel = await CombinationChooseRecordChannelWindow.ShowChooseWindow(CombinationMainWindowVm.TotalRecordAccesses);
        // 屏蔽channel为空或channelID为空的情况
        if ( channel is null || string.IsNullOrEmpty(channel.ChannelId) )
        {
            return;
        }
        RecordAccess.Channel = channel;
        // 设置通道协议
        // 开启收录任务
        var result = await RecordAccess.StartRecord(RecordAccess.Channel.ChannelId!);
        // 如果收录任务开启失败，则重置频道
        if ( string.IsNullOrEmpty(result.taskId) )
        {
            RecordAccess.Channel = null;
            return;
        }

        if ( string.IsNullOrEmpty(result.filePath) )
        {
            MessageBox.Warning("收录路径为空", "开始收录");
            return;
        }

        RecordAccess.VideoPath = AppConfig.Instance.ShouluPath!.Combine(result.filePath!);
        RecordAccess.TaskId = result.taskId;
        Content = PlayerHost;

        await ActionHelper.RunWithTimeout(() =>
        {
            Combination.SetPlayFile(RecordAccess.VideoPath);
            Combination.SetName(RecordAccess.AccessName!);
            Combination.SetStreamProtocal(RecordAccess.Channel.Protocol!);
        }, 5000);

    });

#endregion
#region PropertyChaneged

    /// <summary>
    ///     收录通道改变时，设置是否可用、Content是否隐藏、与子进程通信
    /// </summary>
    private async void RecordAccessChanged()
    {
        await Dispatcher.Invoke(async () =>
        {
            // 设置IsEnabled属性
            IsEnabled = RecordAccess is not null;
            Content = IsEnabled ? string.IsNullOrEmpty(RecordAccess?.VideoPath) ? null : PlayerHost : null;
            if ( Content == null )
            {
                await ActionHelper.RunWithTimeout(Combination.StopPlay, 5000);
                return;
            }
            await ActionHelper.RunWithTimeout(() =>
            {
                Combination.SetPlayFile(RecordAccess!.VideoPath!);
                Combination.SetName(RecordAccess.AccessName!);
                Combination.SetStreamProtocal(RecordAccess.Channel!.Protocol!);
            }, 5000);
        });
    }

#endregion
#region Event

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded; // 从Loaded事件中移除当前方法，以确保只执行一次

        if ( this.FindVisualParent<CombinationListView>() is not { } combination )
        {
            return; // 如果无法找到CombinationListView的父级元素，则直接返回
        }

        var index = combination.ItemContainerGenerator.IndexFromContainer(this); // 获取当前元素在CombinationListView中的索引

        // 根据索引设置Margin属性
        Margin = index switch
        {
            1 => new Thickness(-5, 0, 0, 0),
            2 => new Thickness(0, -5, 0, 0),
            3 => new Thickness(-5, -5, 0, 0),
            _ => new Thickness(0)
        };

        IsEnabled = RecordAccess is not null; // 根据RecordAccess属性是否为null来设置IsEnabled属性

        CombinationMainWindowVm ??= combination.DataContext as CombinationMainWindowVm; // 如果CombinationMainWindowVm为null，则将其赋值为combination.DataContext
    }

    // 初始化播放器
    private async Task InitPlayer()
    {
        while ( !AppProgram.Instance.IsClosed )
        {
            try
            {
                PlayProcess = LoadPluginHelper.LoadPlugin("LiveBoost.Player.exe", Guid); // 加载播放器进程
                PlayProcess.Start(); // 启动播放器进程
                await Task.Run(() => PlayProcess?.WaitForExit()); // 等待播放器进程退出
            }
            catch ( Exception ex )
            {
                // 处理异常，例如记录日志或者进行其他操作
                MessageBox.Warning(ex.Message, "初始化播放器时发生异常"); // 显示异常信息
                break; // 终止循环
            }
        }
    }

#endregion
#region Construct

    static CombinationItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CombinationItem),
            new FrameworkPropertyMetadata(typeof(CombinationItem)));
    }

    public CombinationItem()
    {
        Loaded += OnLoaded;
        // 初始化Ipc
        IpcLazy = new Lazy<ICombinationPlayer>(() =>
        {
            var clientBuilder = new ContainerClientBuilder();
            clientBuilder.UseNamedPipe(Guid + "back").UseJsonSerializer();
            var client = clientBuilder.Build();
            // 从以上构建好的 client 中取用合同实例。
            var same = client.Resolve<ICombinationPlayer>();
            return same;
        });
        GlobalEvent.Instance.GetEvent<CloseChildPlayerProcess>().Subscribe(() =>
        {
            {
                try
                {
                    PlayProcess?.Kill();
                }
                catch ( Exception )
                {
                    // ignored
                }
            }
        });
        // 创建一个容器服务器 Builder。
        var serverBuilder = new ContainerServerBuilder();
        serverBuilder.UseNamedPipe(Guid).UseJsonSerializer();

        serverBuilder.Register<ICombinationItem>(() => this);
        var server = serverBuilder.Build();

        // 别忘了启动服务器哦！
        server.Start();
        Task.Run(async () =>
        {
            await InitPlayer();
        });
    }

#endregion
#region Property

    /// <summary>
    ///     父级ListView的DataContext
    /// </summary>
    public CombinationMainWindowVm? CombinationMainWindowVm { get; set; }
    /// <summary>
    ///     收录通道
    /// </summary>
    public RecordAccess? RecordAccess { get; set; }
    public ViewHost? PlayerHost { get; set; }

    private readonly string Guid = System.Guid.NewGuid().ToString("N");
    private Lazy<ICombinationPlayer> IpcLazy { get; }
    public Process? PlayProcess { get; set; }

    public ICombinationPlayer Combination => IpcLazy.Value;

#endregion
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        switch ( propertyName )
        {
            case nameof(RecordAccess):
                RecordAccessChanged();
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
#region ICombinationItemEventHandler

    private static readonly SemaphoreSlim _controlSemaphoreSlim = new(1, 1);

    /// <inheritdoc />
    public async void SendPlayer(int handle)
    {
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        var content = new ViewHost(new IntPtr(handle));
        PlayerHost = content;
        _controlSemaphoreSlim.Release();
    }

    /// 关闭收录
    public async void ClearChannel()
    {
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        if ( RecordAccess == null || string.IsNullOrEmpty(RecordAccess.TaskId) )
        {
            _controlSemaphoreSlim.Release();
            return;
        }

        if ( MessageBox.Ask("是否确定停止收录", "收录") is not MessageBoxResult.OK )
        {
            return;
        }

        // 停止任务
        if ( !await RecordAccess.TaskId!.StopRecord() )
        {
            _controlSemaphoreSlim.Release();
            return;
        }

        RecordAccess.Channel = null;
        RecordAccess.TaskId = null;
        RecordAccess.VideoPath = null;

        await ActionHelper.RunWithTimeout(Combination.StopPlay, 5000);
        Content = null;
        _controlSemaphoreSlim.Release();
    }

    /// <inheritdoc />
    public async void ChangedChannel()
    {
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        // 修改频道
        if ( MessageBox.Ask("是否确定修改收录频道", "收录") is not MessageBoxResult.OK )
        {
            return;
        }
        AddShouluChannelCommand.Execute();
        _controlSemaphoreSlim.Release();
    }

public async void Send2MainPlayer()
{
    // 检查控制信号量是否已被占用
    if (_controlSemaphoreSlim.CurrentCount <= 0)
    {
        return;
    }
    await _controlSemaphoreSlim.WaitAsync();

    try
    {
        // 检查 CombinationMainWindowVm 是否为空
        if (CombinationMainWindowVm is null)
        {
            return;
        }

        // 检查 RecordAccess 的 ChannelId 是否为空
        if (string.IsNullOrEmpty(RecordAccess?.Channel?.ChannelId))
        {
            return;
        }

        // 检查 RecordAccess 的 VideoPath 是否为空
        if (string.IsNullOrEmpty(RecordAccess?.VideoPath))
        {
            return;
        }

        // 检查 VideoPath 对应的文件是否存在
        if (!File.Exists(RecordAccess!.VideoPath))
        {
            return;
        }

        // 如果 CombinationMainWindowVm 处于播放列表模式且存在播放文件，则将所有文件的 IsPlaying 属性设置为 false
        if (CombinationMainWindowVm.IsPlayListMode && (CombinationMainWindowVm.PlayFiles?.Any() ?? false))
        {
            CombinationMainWindowVm.PlayFiles.ForEach(it => it.IsPlaying = false);
            CombinationMainWindowVm.IsPlayListMode = false;
            CombinationMainWindowVm.MdPanel!.Visibility = Visibility.Collapsed;
        }

        // 清除播放器的标记点
        CombinationMainWindowVm.RecordPlaybackFfPlayerCleanPointsCmd.Execute();

        // 设置播放模式为 Access，并设置相关属性
        CombinationMainWindowVm.PlayMode = PlayMode.Access;
        CombinationMainWindowVm.PlayName = RecordAccess.AccessName;
        CombinationMainWindowVm.CurrentMarks = await RecordAccess.Channel!.ChannelId!.GetMarks(RecordAccess.VideoPath!);
        CombinationMainWindowVm.PlayChannel = RecordAccess.Channel;

        // 打开视频文件
        var openFile = await OpenVideoFile();
        if (!openFile)
        {
            return;
        }

        // 调整视频播放位置
        await AdjustVideoPlayback();

        // 播放视频
        await CombinationMainWindowVm.MdElement.Play();

        // 获取更新后的标记
        CombinationMainWindowVm.CurrentMarks = await GetUpdatedMarks();
    }
    finally
    {
        _controlSemaphoreSlim.Release();
    }
}

// 打开视频文件
private async Task<bool> OpenVideoFile()
{
    var openFile = false;
    while (!openFile)
    {
        openFile = await CombinationMainWindowVm!.MdElement.Open(new Uri(RecordAccess!.VideoPath!));
    }
    return openFile;
}

// 调整视频播放位置
private async Task AdjustVideoPlayback()
{
    if (CombinationMainWindowVm!.MdElement.RemainingDuration is not null)
    {
        var remainingDuration = CombinationMainWindowVm.MdElement.RemainingDuration.Value;

        if (remainingDuration.TotalSeconds > 20)
        {
            // 如果剩余时长大于 20 秒，则从剩余时长的倒数第 20 秒开始播放
            await CombinationMainWindowVm.MdElement.Seek(remainingDuration.Add(TimeSpan.FromSeconds(-20)));
        }
        else if (remainingDuration.TotalSeconds > 0)
        {
            // 如果剩余时长小于等于 20 秒，则延迟播放剩余时长的秒数
            var delay = remainingDuration.TotalSeconds;
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
    }
}

// 获取更新后的标记
private async Task<ObservableList<RecordMark>> GetUpdatedMarks()
{
    var videoPath = CombinationMainWindowVm!.MdElement.Source.LocalPath.Replace(
        AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'),
        string.Empty).Replace('\\', '/');

    return await RecordAccess!.Channel!.ChannelId!.GetMarks(videoPath);
}

#endregion
}
