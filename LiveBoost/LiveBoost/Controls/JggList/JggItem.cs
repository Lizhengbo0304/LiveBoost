// 创建时间：2023-09-20-9:14
// 修改时间：2023-10-13-16:16

#region

using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;
using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.Controls;

public sealed class JggItem : ListViewItem, INotifyPropertyChanged, IJggItem
{
    #region Event

    /// <summary>
    ///     当元素加载完成时的事件处理程序。
    /// </summary>
    /// <param name="sender"> 触发事件的对象。 </param>
    /// <param name="e"> 事件参数。 </param>
    public void OnLoaded(object? sender, RoutedEventArgs? e)
    {
        // 尝试查找父级元素 JggListView
        if (this.FindVisualParent<JggListView>() is not { } jggListView)
        {
            return; // 如果无法找到 CombinationListView 的父级元素，则直接返回
        }

        // 尝试查找 UniformGrid
        if (jggListView.FindVisualChild<UniformGrid>() is not { } uniformGrid)
        {
            return; // 如果无法找到 UniformGrid，直接返回
        }

        // 获取当前元素在 CombinationListView 中的索引
        var index = jggListView.ItemContainerGenerator.IndexFromContainer(this);

        // 根据 UniformGrid 的行数设置 Margin 属性
        Margin = uniformGrid.Rows switch
        {
            2 => new Thickness(5, 5, (index % 2) == 1 ? 5 : 0, index >= 2 ? 5 : 0),
            3 => new Thickness(5, 5, (index % 3) == 2 ? 5 : 0, index >= 6 ? 5 : 0),
            4 => new Thickness(5, 5, (index % 4) == 3 ? 5 : 0, index >= 12 ? 5 : 0),
            _ => Margin // 默认情况下不修改 Margin
        };

        // 如果索引大于等于 UniformGrid 的总元素数，调用 ClearPreView 方法
        if (index >= (uniformGrid.Rows * uniformGrid.Columns))
        {
            Content = null;
        }
    }


    /// <summary>
    ///     异步初始化播放器进程的方法。
    /// </summary>
    private async void InitPlayer()
    {
        while (!IsOwnerWindowClosed) // 循环，直到拥有者窗口关闭
        {
            try
            {
                // 加载播放器进程
                PlayProcess = LoadPluginHelper.LoadPlugin("LiveBoost.Player.exe", Guid, "1");

                // 启动播放器进程
                PlayProcess.Start();
                await Task.Yield();
                // 等待播放器进程退出
                await Task.Run(() => PlayProcess?.WaitForExit()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // 处理异常，例如记录日志或者进行其他操作
                MessageBox.Warning(ex.Message, "初始化播放器时发生异常"); // 显示异常信息
            }
        }
    }


    /// <summary>
    ///     启动预览或收录操作的方法。
    /// </summary>
    private async void StartPreview()
    {
        if (PlayProcess is null)
            // 如果播放器进程尚未初始化，异步启动播放器初始化
        {
            InitPlayer();
        }

        if (string.IsNullOrEmpty(AppConfig.Instance.ShouluPath))
        {
            // 如果收录路径为空，无法选择频道，显示警告消息并返回
            MessageBox.Warning("收录路径为空，无法选择频道", "添加收录频道");
            return;
        }

        // 屏蔽channel为空或channelID为空的情况
        if (Channel is null || string.IsNullOrEmpty(Channel.ChannelId))
        {
            return;
        }

        // 开启收录任务
        var result = await Guid.StartRecord(Channel.ChannelId!).ConfigureAwait(false);

        // 如果收录任务开启失败，则重置频道
        if (string.IsNullOrEmpty(result.taskId))
        {
            Channel = null;
            return;
        }

        if (string.IsNullOrEmpty(result.filePath))
        {
            // 如果收录路径为空，显示警告消息并返回
            MessageBox.Warning("收录路径为空", "开始收录");
            return;
        }

        // 构建视频路径
        VideoPath = AppConfig.Instance.ShouluPath.Combine(result.filePath!);
        TaskId = result.taskId;
        Dispatcher.Invoke(() => Content = PlayerHost);

        try
        {
            // 设置播放文件和名称到子进程
            await ActionHelper.RunWithTimeout(() =>
            {
                Jgg.SetPlayFile(VideoPath!);
                Jgg.SetName(Channel.ChannelName!);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            // 处理设置播放路径、名称、收录协议时的异常，并记录日志
            e.LogError("向子进程设置播放路径、名称、收录协议时发生异常");
        }
    }


    /// <summary>
    ///     清理预览相关状态和资源的方法。
    /// </summary>
    private async void ClearPreView()
    {
        // 停止播放，超时处理
        try
        {
            await ActionHelper.RunWithTimeout(Jgg.StopPlay);
        }
        catch (Exception e)
        {
            e.LogError("停止播放，超时处理异常");
        }

        if (!string.IsNullOrEmpty(TaskId))
        {
            // 停止任务
            if (!await TaskId!.StopRecord())
            {
                return; // 如果停止任务失败，直接返回
            }

            TaskId = null; // 任务停止后将 TaskId 置为 null
        }

        // 重置视频路径、频道和内容
        VideoPath = null;
        Channel = null;
        Dispatcher.Invoke(() => Content = null);

        if (PlayProcess is { HasExited: false } && IsOwnerWindowClosed)
            // 如果播放器进程未退出且拥有者窗口已关闭，强制终止播放器进程
        {
            PlayProcess.Kill();
        }
    }

    #endregion

    #region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if (propertyName != nameof(Channel))
        {
            return;
        }

        if (Channel == null)
        {
            ClearPreView();
        }
        else
        {
            StartPreview();
        }
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

    #region Ctor

    static JggItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(JggItem),
            new FrameworkPropertyMetadata(typeof(JggItem)));
    }

    /// <summary>
    ///     JggItem 类的构造函数。
    /// </summary>
    public JggItem()
    {
        // 注册 Loaded 事件处理程序
        Loaded += OnLoaded;

        // 初始化 Lazy<IJggPlayer>
        IpcLazy = new Lazy<IJggPlayer>(() =>
        {
            // 创建一个容器客户端 Builder
            var clientBuilder = new ContainerClientBuilder();
            clientBuilder.UseNamedPipe(Guid + "back").UseJsonSerializer();
            var client = clientBuilder.Build();

            // 从以上构建好的 client 中取用合同实例。
            var same = client.Resolve<IJggPlayer>();
            return same;
        });

        // 创建一个容器服务器 Builder
        var serverBuilder = new ContainerServerBuilder();
        serverBuilder.UseNamedPipe(Guid).UseJsonSerializer();

        // 注册 JggItem 实例到容器
        serverBuilder.Register<IJggItem>(() => this);
        var server = serverBuilder.Build();

        // 启动服务器
        server.Start();

        // 订阅 SourceMonitorDragEvent 事件
        SourceMonitorDragEvent.SubscriptionTokens.Add(GlobalEvent.Instance.GetEvent<SourceMonitorDragEvent>().Subscribe(isDragging => { IsDragging = isDragging; }));

        // 订阅 CloseJggPlayerProcess 事件
        CloseJggPlayerProcess.SubscriptionTokens.Add(GlobalEvent.Instance.GetEvent<CloseJggPlayerProcess>().Subscribe(() =>
        {
            IsOwnerWindowClosed = true;
            Channel = null;
        }));
    }

    #endregion

    #region Property

    /// <summary>
    ///     是否正在拖拽
    /// </summary>
    public bool IsDragging { get; set; }

    /// <summary>
    ///     播放器进程通信
    /// </summary>
    private Lazy<IJggPlayer> IpcLazy { get; }

    private IJggPlayer Jgg => IpcLazy.Value;

    /// <summary>
    ///     Guid
    /// </summary>
    private readonly string Guid = System.Guid.NewGuid().ToString("N");

    /// <summary>
    ///     播放进程
    /// </summary>
    public Process? PlayProcess { get; set; }

    /// <summary>
    ///     播放器子进程界面
    /// </summary>
    public ViewHost? PlayerHost { get; set; }

    /// <summary>
    ///     频道
    /// </summary>
    public RecordChannel? Channel { get; set; }

    /// <summary>
    ///     视频地址
    /// </summary>
    public string? VideoPath { get; set; }

    /// <summary>
    ///     任务ID
    /// </summary>
    public string? TaskId { get; set; }

    public bool IsOwnerWindowClosed { get; set; }

    #endregion

    #region IJggItem

    private static readonly SemaphoreSlim _controlSemaphoreSlim = new(1, 1);

    public async void SendPlayer(int handle)
    {
        if (_controlSemaphoreSlim.CurrentCount <= 0)
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

    /// <summary>
    ///     清除频道信息
    /// </summary>
    public async void ClearChannel()
    {
        if (_controlSemaphoreSlim.CurrentCount <= 0)
        {
            return;
        }

        await _controlSemaphoreSlim.WaitAsync();

        try
        {
            if (string.IsNullOrEmpty(TaskId))
            {
                return;
            }

            if (MessageBox.Ask("是否确定停止收录", "收录") is not MessageBoxResult.OK)
            {
                return;
            }

            // 停止任务
            if (!await TaskId!.StopRecord())
            {
                return;
            }

            TaskId = null;
            VideoPath = null;
            Channel = null;
        }
        finally
        {
            _controlSemaphoreSlim.Release();
        }
    }

    #endregion
}