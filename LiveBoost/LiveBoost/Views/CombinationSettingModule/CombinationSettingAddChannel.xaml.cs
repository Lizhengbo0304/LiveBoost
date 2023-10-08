// 创建时间：2023-09-26-17:22
// 修改时间：2023-09-28-14:17

namespace LiveBoost.Views;

public sealed partial class CombinationSettingAddChannel : INotifyPropertyChanged
{
    public CombinationSettingAddChannel()
    {
        InitializeComponent();
        SubmitCommand = new DelegateCommand(SubmitCommandExecute);
        ResetCommand = new DelegateCommand(ResetCommandExecute);
        Task.Run(GetServers);
        ContentRendered += (_, _) =>
        {
            // 频道名称输入框获取输入焦点
            ChannelNameTextBox.Focus();
        };
    }

    public static bool Show(Window owner)
    {
        var addChannel = new CombinationSettingAddChannel
        {
            Owner = owner
        };
        addChannel.ShowDialog();
        return addChannel.AddResult;
    }

    public static bool Show(Window owner,RecordChannel channel)
    {
        var addChannel = new CombinationSettingAddChannel
        {
            Owner = owner,
            ChannelName = channel.ChannelName ?? string.Empty,
            ChannelUrl = channel.StreamURL ?? string.Empty,
            NdiGroup = channel.NdiGroup ?? string.Empty,
            NdiName = channel.NdiName ?? string.Empty,
            Channel = channel,
            Title = "修改频道"
        };
        addChannel.SelectedProtocol = addChannel.Protocols.SingleOrDefault(it =>string.Equals(it,channel.Protocol,StringComparison.OrdinalIgnoreCase));

        addChannel.ShowDialog();
        return addChannel.AddResult;
    }
#region Properties

    private RecordChannel? Channel;
    /// <summary>
    ///     频道名称
    /// </summary>
    public string ChannelName { get; set; } = string.Empty;
    /// <summary>
    ///     视频流协议类型
    /// </summary>
    public List<string> Protocols { get; } = new()
    {
        "HTTPS", "UDP", "RTMP", "SDI", "NDI", "其他"
    };
    /// <summary>
    ///     选中的协议类型
    /// </summary>
    public string? SelectedProtocol { get; set; }

    /// <summary>
    ///     频道地址
    /// </summary>
    public string ChannelUrl { get; set; } = string.Empty;
    /// <summary>
    ///     频道地址是否必须
    /// </summary>
    public bool IsChannelUrlNecessary { get; set; } = true;
    /// <summary>
    ///     收录服务器列表
    /// </summary>
    public List<RecordServer>? ServerList { get; set; }
    /// <summary>
    ///     选中的服务器
    /// </summary>
    public RecordServer? SelectedServer { get; set; }
    /// <summary>
    ///     NDI组名称
    /// </summary>
    public string NdiGroup { get; set; } = string.Empty;
    /// <summary>
    ///     NDI流名称
    /// </summary>
    public string NdiName { get; set; } = string.Empty;
    /// <summary>
    ///     添加频道结果
    /// </summary>
    private bool AddResult { get; set; }

#endregion
#region Commands

    /// <summary>
    ///     提交命令
    /// </summary>
    public DelegateCommand SubmitCommand { get; set; }

    /// <summary>
    ///     重置命令
    /// </summary>
    public DelegateCommand ResetCommand { get; set; }

#endregion
#region Event Handlers

    public async void SubmitCommandExecute()
    {
        // 验证输入是否合法
        var errorMessage = ValidateInput();

        if ( !string.IsNullOrEmpty(errorMessage) )
        {
            MessageBox.Warning(errorMessage, "提交");

            return;
        }

        // 创建频道参数对象
        var channelParams = CreateChannelParams();
        bool success;
        // 添加频道并检查是否成功
        if ( Channel is null )
        {
            success = await channelParams.AddChannel().ConfigureAwait(false);
        }
        else
        {
            success = await channelParams.EditChannel().ConfigureAwait(false);
        }


        if ( success )
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddResult = true;
                Close();
            });
        }
    }

// 验证输入是否合法的方法
    private string? ValidateInput()
    {
        if ( string.IsNullOrEmpty(ChannelName) )
        {
            return "频道名称不能为空，请输入频道名称";
        }

        if ( string.IsNullOrEmpty(SelectedProtocol) )
        {
            return "协议类型不能为空，请选择协议类型";
        }

        switch ( SelectedProtocol )
        {
            case "SDI" when SelectedServer == null:
                return "服务器不能为空，请选择服务器";
            case "NDI" when string.IsNullOrEmpty(NdiName):
                return "流名称不能为空，请输入流名称";
        }

        if ( string.IsNullOrEmpty(ChannelUrl) && SelectedProtocol != "SDI" && SelectedProtocol != "NDI" )
        {
            return "频道地址不能为空，请输入频道地址";
        }

        return null; // 验证通过
    }
// 创建频道参数对象的方法
    private object CreateChannelParams()
    {
        return SelectedProtocol switch
        {
            "SDI" => new
            {
                channelId = Channel?.ChannelId,
                channelName = ChannelName,
                streamURL = ChannelUrl,
                protocol = SelectedProtocol,
                clientName = SelectedServer!.ClientName,
                clientSdiPort = ""
            },
            "NDI" => new
            {
                channelId = Channel?.ChannelId,
                channelName = ChannelName,
                streamURL = ChannelUrl,
                protocol = SelectedProtocol,
                ndiName = NdiName,
                ndiGroup = NdiGroup
            },
            _ => new
            {
                channelId = Channel?.ChannelId,
                channelName = ChannelName,
                streamURL = ChannelUrl,
                protocol = SelectedProtocol
            }
        };
    }

    public void ResetCommandExecute()
    {
        ChannelName = string.Empty;
        SelectedProtocol = null;
        ChannelUrl = string.Empty;
        SelectedServer = null;
        NdiGroup = string.Empty;
        NdiName = string.Empty;
    }

    /// <summary>
    ///     获取服务器列表
    /// </summary>
    public async void GetServers()
    {
        ServerList = await UrlHelper.GetShouluServers().ConfigureAwait(false);
    }

#endregion
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // 根据属性名称执行相应操作
        switch ( propertyName )
        {
            case nameof(SelectedProtocol):
                HandleSelectedProtocolChange();
                break;
            case nameof(ChannelName):
                HandleChannelNameChange();
                break;
            case nameof(ChannelUrl):
                HandleChannelUrlChange();
                break;
        }
    }

    /// <summary>
    ///     处理 SelectedProtocol 属性的更改。
    /// </summary>
    private void HandleSelectedProtocolChange()
    {
        // 根据 SelectedProtocol 更新窗口高度和 IsChannelUrlNecessary
        switch ( SelectedProtocol )
        {
            case "SDI":
            case "NDI":
                Height = 330;
                IsChannelUrlNecessary = false;
                break;
            default:
                Height = 340;
                IsChannelUrlNecessary = true;
                break;
        }
    }

    /// <summary>
    ///     处理 ChannelName 属性的更改，限制其长度为 64 个字符。
    /// </summary>
    private void HandleChannelNameChange()
    {
        // 限制 ChannelName 长度为 64 个字符
        if ( ChannelName.Length > 64 )
        {
            ChannelName = ChannelName.Substring(0, 64);
        }
    }

    /// <summary>
    ///     处理 ChannelUrl 属性的更改，限制其长度为 255 个字符。
    /// </summary>
    private void HandleChannelUrlChange()
    {
        // 限制 ChannelName 长度为 64 个字符
        if ( ChannelUrl.Length > 255 )
        {
            ChannelUrl = ChannelUrl.Substring(0, 255);
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
}
