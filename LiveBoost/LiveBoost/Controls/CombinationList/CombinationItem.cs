// 创建时间：2023-09-05-11:17
// 修改时间：2023-09-06-10:47

using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;
using LiveBoost.ToolKit.Tools;

namespace LiveBoost.Controls;

public sealed class CombinationItem : ListViewItem, INotifyPropertyChanged,ICombinationItem
{
#region Event
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
        IsEnabled = RecordAccess is not null;
        CombinationMainWindowVm ??= combination.DataContext as CombinationMainWindowVm;
    }
    // 初始化播放器
    private async Task InitPlayer()
    {

        while ( !AppProgram.Instance.IsClosed )
        {
           PlayProcess = LoadPluginHelper.LoadPlugin("LiveBoost.Player.exe", Guid);
            PlayProcess.Start();
            await Task.Run(() => PlayProcess.WaitForExit());
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

        RecordAccess.VideoPath = AppConfig.Instance.ShouluPath!.Combine( result.filePath!);
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
    /// 收录通道改变时，设置是否可用、Content是否隐藏、与子进程通信
    /// </summary>
    private async void RecordAccessChanged()
    {
        await  this.Dispatcher.Invoke(async () =>
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
       AddShouluChannelCommand.Execute();
        _controlSemaphoreSlim.Release();
    }

    /// <inheritdoc />
    public async void Send2MainPlayer()
    {
            if ( _controlSemaphoreSlim.CurrentCount <= 0 )
            {
                return;
            }
            await _controlSemaphoreSlim.WaitAsync();

            if ( CombinationMainWindowVm is null )
            {
                _controlSemaphoreSlim.Release();
                return;
            }

            if ( string.IsNullOrEmpty(RecordAccess?.Channel?.ChannelId) )
            {
                _controlSemaphoreSlim.Release();
                return;
            }

            if ( string.IsNullOrEmpty(RecordAccess?.VideoPath) )
            {
                _controlSemaphoreSlim.Release();
                return;
            }

            if ( !File.Exists(RecordAccess!.VideoPath) )
            {
                _controlSemaphoreSlim.Release();
                return;
            }
    }
#endregion

}
