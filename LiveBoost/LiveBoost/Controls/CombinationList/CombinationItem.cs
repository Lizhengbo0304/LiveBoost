// 创建时间：2023-09-05-11:17
// 修改时间：2023-09-19-14:01

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
        try
        {
            await ActionHelper.RunWithTimeout(() =>
            {
                Combination.SetPlayFile(RecordAccess!.VideoPath!);
                Combination.SetName(RecordAccess.AccessName!);
                Combination.SetStreamProtocal(RecordAccess.Channel!.Protocol!);
            });
        }
        catch ( Exception e )
        {
            e.LogError("向子进程设置播放路径、名称、收录协议时发生异常");
        }
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
                try
                {
                    await ActionHelper.RunWithTimeout(Combination.StopPlay);
                }
                catch ( Exception e )
                {
                    e.LogError("向子进程发送停止播放命令异常");
                }
                return;
            }
            try
            {
                await ActionHelper.RunWithTimeout(() =>
                {
                    Combination.SetPlayFile(RecordAccess!.VideoPath!);
                    Combination.SetName(RecordAccess.AccessName!);
                    Combination.SetStreamProtocal(RecordAccess.Channel!.Protocol!);
                });
            }
            catch ( Exception e )
            {
                e.LogError("向子进程设置播放路径、名称、收录协议时发生异常");
            }

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
        if ( combination.FindVisualChild<UniformGrid>() is not { } uniformGrid )
        {
            return; // 如果无法找到UniformGrid，直接返回
        }

        var index = combination.ItemContainerGenerator.IndexFromContainer(this); // 获取当前元素在CombinationListView中的索引
        Margin = uniformGrid.Rows switch
        {
            2 =>
                // 根据索引设置Margin属性
                index switch
                {
                    1 => new Thickness(-5, 0, 0, 0),
                    2 => new Thickness(0, -5, 0, 0),
                    3 => new Thickness(-5, -5, 0, 0),
                    _ => new Thickness(0)
                },
            3 =>
                // 根据索引设置Margin属性
                index switch
                {
                    1 => new Thickness(-5, 0, 0, 0),
                    2 => new Thickness(-5, 0, 0, 0),
                    3 => new Thickness(0, -5, 0, 0),
                    4 => new Thickness(-5, -5, 0, 0),
                    5 => new Thickness(-5, -5, 0, 0),
                    6 => new Thickness(0, -5, 0, 0),
                    7 => new Thickness(-5, -5, 0, 0),
                    8 => new Thickness(-5, -5, 0, 0),
                    _ => new Thickness(0)
                },
            4 =>
                // 根据索引设置Margin属性
                index switch
                {
                    1 => new Thickness(-5, 0, 0, 0),
                    2 => new Thickness(-5, 0, 0, 0),
                    3 => new Thickness(-5, 0, 0, 0),
                    4 => new Thickness(0, -5, 0, 0),
                    5 => new Thickness(-5, -5, 0, 0),
                    6 => new Thickness(-5, -5, 0, 0),
                    7 => new Thickness(-5, -5, 0, 0),
                    8 => new Thickness(0, -5, 0, 0),
                    9 => new Thickness(-5, -5, 0, 0),
                    10 => new Thickness(-5, -5, 0, 0),
                    11 => new Thickness(-5, -5, 0, 0),
                    12 => new Thickness(0, -5, 0, 0),
                    13 => new Thickness(-5, -5, 0, 0),
                    14 => new Thickness(-5, -5, 0, 0),
                    15 => new Thickness(-5, -5, 0, 0),
                    _ => new Thickness(0)
                },
            _ => Margin
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
        GlobalEvent.Instance.GetEvent<CloseCombinationPlayerProcess>().Subscribe(() =>
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
        try
        {
            // 创建一个 ViewHost 实例用于播放
            var content = new ViewHost(new IntPtr(handle));
            PlayerHost = content;
        }
        finally
        {
            _controlSemaphoreSlim.Release();
        }
    }

    /// 关闭收录
    public async void ClearChannel()
    {
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        try
        {
            if ( RecordAccess == null || string.IsNullOrEmpty(RecordAccess.TaskId) )
            {
                return;
            }

            if ( MessageBox.Ask("是否确定停止收录", "收录") is not MessageBoxResult.OK )
            {
                return;
            }

            // 停止任务
            if ( !await RecordAccess.TaskId!.StopRecord() )
            {
                return;
            }

            RecordAccess.Channel = null;
            RecordAccess.TaskId = null;
            RecordAccess.VideoPath = null;

            try
            {
                await ActionHelper.RunWithTimeout(Combination.StopPlay);
            }
            catch ( Exception e )
            {
                e.LogError("向子进程发送停止播放命令异常");
            }
            Content = null;
        }
        finally
        {
            _controlSemaphoreSlim.Release();
        }
    }

    /// <inheritdoc />
    public async void ChangedChannel()
    {
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        try
        {
            // 修改频道
            if ( MessageBox.Ask("是否确定修改收录频道", "收录") is not MessageBoxResult.OK )
            {
                return;
            }

            AddShouluChannelCommand.Execute();
        }
        finally
        {
            _controlSemaphoreSlim.Release();
        }
    }

    public async void Send2MainPlayer()
    {
        // 检查控制信号量是否已被占用
        if ( _controlSemaphoreSlim.CurrentCount <= 0 )
        {
            return;
        }
        await _controlSemaphoreSlim.WaitAsync();
        try
        {
            // 检查 CombinationMainWindowVm 是否为空
            if ( CombinationMainWindowVm is null )
            {
                return;
            }

            // 检查 RecordAccess 的 ChannelId 是否为空
            if ( string.IsNullOrEmpty(RecordAccess?.Channel?.ChannelId) )
            {
                return;
            }

            // 检查 RecordAccess 的 VideoPath 是否为空
            if ( string.IsNullOrEmpty(RecordAccess?.VideoPath) )
            {
                return;
            }

            // 检查 VideoPath 对应的文件是否存在
            if ( !File.Exists(RecordAccess!.VideoPath) )
            {
                return;
            }

            await CombinationMainWindowVm.PlayRecordAccess(RecordAccess);
        }
        finally
        {
            _controlSemaphoreSlim.Release();
        }
    }

#endregion
}
