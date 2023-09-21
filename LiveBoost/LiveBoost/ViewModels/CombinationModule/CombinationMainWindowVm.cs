// 创建时间：2023-09-05-14:38
// 修改时间：2023-09-19-14:01

#region

using System.Net.WebSockets;
using HandyControl.Controls;
using Websocket.Client;

#endregion

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm : INotifyPropertyChanged
{
#region Ctor

    public CombinationMainWindowVm(MediaElement mediaElement, SimplePanel simple)
    {
        // 初始化串口
        InitializeSerialPort();
        // 初始化命令
        PageUpdatedCmd = new DelegateCommand<FunctionEventArgs<int>>(PageUpdatedExecute);
        RecordFileDoubleClickCmd = new DelegateCommand<MouseButtonEventArgs>(RecordFileDoubleClickExecute);
        ShowJggCmd = new DelegateCommand(ShowJggExecute);
        // 初始化收录
        Task.Run(async () =>
        {
            //    初始化收录通道
            await InitializeRecordAccessesAsync();
            await InitializeRecordChannelsAsync();
            // 初始化推流通道
            await InitializePlayAccessesAsync();
            // 初始化我的收录列表
            await InitializeMyRecordFileAsync();
            // 初始化公共收录列表
            await InitializePublicRecordFileAsync();
        });
        PushAccessRecordFilesChanged.SubscriptionTokens.Add(GlobalEvent.Instance.GetEvent<PushAccessRecordFilesChanged>().Subscribe(RecordFilesOnCollectionChanged));

        // 播放器初始化
        MdActive = mediaElement;
        MdPanel = simple;
        MdElement = MdActive;
        MdElement.RenderingAudio += FfPlayOnRenderingAudio;

        StartSocket().ConfigureAwait(false);
    }

#endregion
#region Properties

    /// <summary>
    ///     下位机
    /// </summary>
    public SerialPort? LiveBoostSerialPort { get; set; }

#endregion
#region Event

    /// <summary>
    ///     初始化串口
    /// </summary>
    private void InitializeSerialPort()
    {
        try
        {
            // 如果不支持串口小屏，直接返回
            if ( !AppConfig.Instance.IsPortSupported )
            {
                return;
            }
            // 如果串口名为空，直接返回
            if ( string.IsNullOrEmpty(AppConfig.Instance.SerialPort) )
            {
                return;
            }
            // 如果串口名不存在，直接返回
            if ( !AppConfig.Instance.SerialPort!.IsPortNameExists() )
            {
                return;
            }
            // 创建并配置串口
            LiveBoostSerialPort = new SerialPort(AppConfig.Instance.SerialPort, 9600, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                DtrEnable = true,
                RtsEnable = true
            };
            // 打开串口
            LiveBoostSerialPort.Open();
            // 检查串口是否真的打开了
            if ( !LiveBoostSerialPort.IsOpen )
            {
                MessageBox.Warning("无法打开串口", "串口初始化");
            }
        }
        catch ( Exception e )
        {
            MessageBox.Warning($"串口下位机初始化异常：{e.Message}", "初始化异常");
        }
    }

#endregion
#region WebSocket

    private WebsocketClient? socket;

    /// <summary> 开起Socket </summary>
    public async Task StartSocket()
    {
        if ( string.IsNullOrEmpty(AppConfig.Instance.ShouluWebSocket) )
        {
            // 如果收录WebSocket设置为空，无法连接，弹出警告框提示用户
            MessageBox.Warning("收录WebSocket设置为空，无法连接", "开启Socket");
            return;
        }
        // 初始化socket
        InitializeSocket();
        await socket!.StartOrFail().ConfigureAwait(false);
        CanSendHeart = true;
        // 发送心跳
        await SendHeart().ConfigureAwait(false);
    }
    /// <summary>
    ///     socket初始化
    /// </summary>
    private void InitializeSocket()
    {
        // 创建新的WebsocketClient实例
        socket = new WebsocketClient(new Uri(AppConfig.Instance.ShouluWebSocket!));
        // 禁用自动重连
        socket.IsReconnectionEnabled = false;
        // 订阅消息接收事件
        socket.MessageReceived.Subscribe(HandleSocketMessage);
    }
    /// <summary>
    ///     处理socket消息
    /// </summary>
    private void HandleSocketMessage(ResponseMessage info)
    {
        try
        {
            if ( info.MessageType != WebSocketMessageType.Text )
            {
                return;
            }

            var response = JObject.Parse(info.Text);
            if ( response is null )
            {
                return;
            }
            // 处理收录停止逻辑
            HandleRecordStop(response);
            // 处理推流结束逻辑
            HandlePushStop(response);
            // 处理推流进度逻辑
            HandlePushProgress(response);
            // 处理小屏信息逻辑
            HandlePortServerInfo(response);
        }
        catch ( Exception ex )
        {
            // 记录异常信息
            ex.LogError("Socket消息格式有误");
        }
    }

    /// <summary>
    ///     处理收录停止逻辑
    /// </summary>
    private void HandleRecordStop(JObject response)
    {
        // 处理收录停止逻辑
        // 收录停止
        if ( response["type"]?.ToString() != "recored" ||
             response["info"]?["accessid"]?.ToString() is not { } accessid ||
             response["info"]?["type"]?.ToString() != "stop" )
        {
            return;
        }
        // 查找对应的收录通道
        var targetAccess = TotalRecordAccesses?.Find(it => it?.AccessId == accessid);
        if ( targetAccess is null )
        {
            return;
        }
        // 清空通道频道
        targetAccess.Channel = null;
        if ( RecordItems.Find(it => it.RecordAccess == targetAccess) is not { } targetItem )
        {
            return;
        }
        // 停止播放组合
        targetItem.Combination.StopPlay();
        // 在UI线程上隐藏播放进程窗口
        targetItem.Dispatcher.Invoke(() => { targetItem.Content = null; });
    }

    /// <summary>
    ///     处理推流结束逻辑
    /// </summary>
    private void HandlePushStop(JObject response)
    {
        // 处理推流结束逻辑
        // 推流结束
        if ( response["type"]?.ToString() != "push" ||
             response["info"]?["accessid"]?.ToString() is not { } pushAccessid ||
             response["info"]?["type"]?.ToString() != "stop" )
        {
            return;
        }
        // 查找对应的推流通道
        var targetAccess = PlayAccesses?.Find(it => it.AccessId == pushAccessid);
        if ( targetAccess is null )
        {
            return;
        }
        // 重置当前索引、暂停状态和状态
        targetAccess.CurrentIndex = -1;
        targetAccess.IsPause = false;
        targetAccess.Status = true;
        // 将所有记录文件的播放状态和进度重置
        targetAccess.RecordFiles.ForEach(it =>
        {
            it.IsPlaying = false;
            it.Progress = 0;
        });
    }

    /// <summary>
    ///     处理推流进度逻辑
    /// </summary>
    private void HandlePushProgress(JObject response)
    {
        // 处理推流进度逻辑
        // 推流进度
        if ( response["type"]?.ToString() != "push" ||
             response["info"]?["accessid"]?.ToString() is not { } pushingAccessid ||
             response["info"]?["type"]?.ToString() != "schedule" ||
             response["info"]?["id"]?.Value<int>() is not { } index ||
             response["info"]?["time"]?.Value<int>() is not { } time )
        {
            return;
        }
        // 查找对应的推流通道
        var targetAccess = PlayAccesses?.Find(it => it.AccessId == pushingAccessid);
        if ( targetAccess is null )
        {
            return;
        }
        // 更新当前索引和当前时间
        targetAccess.CurrentIndex = index;
        targetAccess.CurrentTime = TimeSpan.FromMilliseconds(time);
        // 更新每个记录文件的播放状态和进度
        for ( var i = 0; i < targetAccess.RecordFiles.Count; i++ )
        {
            var file = targetAccess.RecordFiles[i];
            file.IsPlaying = i == index;
            file.Progress = file.IsPlaying
                ? Math.Round(
                    ( targetAccess.CurrentTime.TotalMilliseconds - file.RealInPoint!.Value.TotalMilliseconds ) * 100 /
                    ( file.RealOutPoint!.Value.TotalMilliseconds - file.RealInPoint.Value.TotalMilliseconds ), 2,
                    MidpointRounding.AwayFromZero)
                : 0;
        }
    }

    /// <summary>
    ///     处理下位机信息逻辑
    /// </summary>
    private void HandlePortServerInfo(JObject response)
    {
        // 处理下位机信息逻辑
        // 下位机信息
        if ( response["type"]?.ToString() != "server" ||
             response["accept"]?.ToString() != "request" ||
             response["info"]?["data"]?.ToString() is not { } sendInfo )
        {
            return;
        }
        // 如果串口未打开，则尝试打开串口
        if ( !( LiveBoostSerialPort?.IsOpen ?? false ) )
        {
            try { LiveBoostSerialPort?.Open(); }
            catch ( Exception e )
            {
                e.LogError("下位机重连异常");
            }
        }
        // 如果串口仍未打开，则返回
        if ( !( LiveBoostSerialPort?.IsOpen ?? false ) )
        {
            return;
        }
        try
        {
            // 向串口发送信息
            LiveBoostSerialPort.WriteLine(sendInfo + "|");
        }
        catch ( TimeoutException )
        {
            try
            {
                // 发生超时异常时，尝试重新打开串口并发送信息
                LiveBoostSerialPort.Close();
                LiveBoostSerialPort.Open();
                LiveBoostSerialPort.WriteLine(sendInfo + "|");
            }
            catch ( Exception exception )
            {
                exception.LogError("下位机连接发送消息异常");
            }
        }
    }
    /// <summary>
    ///     关闭Socket链接
    /// </summary>
    public async Task StopSocket()
    {
        // 停止发送心跳
        CanSendHeart = false;
        // 如果 WebSocket 正在运行，则发送正常关闭消息
        if ( socket is {IsRunning: true} )
        {
            await socket.StopOrFail(WebSocketCloseStatus.NormalClosure, string.Empty);
        }
    }

    private bool CanSendHeart { get; set; }

    /// <summary> 发送心跳 </summary>
    public async Task SendHeart()
    {
        while ( CanSendHeart )
        {
            await Task.Delay(5000);
            socket?.Send(AppProgram.Instance.LoginUser?.LoginName);
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
}
