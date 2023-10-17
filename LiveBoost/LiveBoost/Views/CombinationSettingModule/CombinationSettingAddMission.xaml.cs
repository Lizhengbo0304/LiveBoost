// 创建时间：2023-10-08-16:20
// 修改时间：2023-10-13-16:16

#region

using System.Text.RegularExpressions;

#endregion

namespace LiveBoost.Views;

public sealed partial class CombinationSettingAddMission : INotifyPropertyChanged
{
    private void ButtonGroup_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ( !IsLoaded )
        {
            return;
        }
        if ( e.NewValue is true )
        {
            Height += 160;
        }
        else
        {
            Height -= 160;
        }
    }

    private void Frame_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ( !IsLoaded )
        {
            return;
        }
        if ( e.NewValue is true )
        {
            Height += 40;
        }
        else
        {
            Height -= 40;
        }
    }

    private void ResolutionX_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ( !IsLoaded )
        {
            return;
        }
        if ( e.NewValue is true )
        {
            Height += 80;
        }
        else
        {
            Height -= 80;
        }
    }
#region Init-Events

    private CombinationSettingAddMission()
    {
        InitializeComponent();
        SubmitCommand = new DelegateCommand(SubmitCommandExecute);
        ResetCommand = new DelegateCommand(ResetCommandExecute);
        Task.Run(InitChannels);
        Task.Run(InitServers);
        SelectedMissionType = MissionTypes.First();
        SelectedResolution = Resolution.Resolutions.First();
        SelectedInterlaces = Interlaces.First();
        SelectedVideoFrame = VideoFrames.First();
        SegmentTimePicker.SelectedTime = new DateTime(1, 1, 1, 1, 0, 0);
        BitRate = "0";
        TaskStartTimePicker.SelectedTime = TaskStartTimePicker.DisplayTime;
        TaskEndTimePicker.SelectedTime = TaskEndTimePicker.DisplayTime;
    }
    private CombinationSettingAddMission(RecordMission recordMission)
    {
        InitializeComponent();

        SubmitCommand = new DelegateCommand(SubmitCommandExecute);
        ResetCommand = new DelegateCommand(ResetCommandExecute);
        Title = "修改收录任务配置";
        mission = recordMission;
        Task.Run(InitChannels);
        Task.Run(InitServers);

        SelectedMissionType = mission.MissionFlag == "loop" ? MissionTypes.First() : MissionTypes.Last();
        MissionName = mission.MissionName ?? string.Empty;
        SelectedFormat = ShouluFormat.ShouluFormats.Find(it => string.Equals(it.Format, mission.VideoType, StringComparison.OrdinalIgnoreCase));
        SelectedVideoRecordCodec = SelectedFormat.VideoRecordCodec?.Find(it => it == mission.VideoCodec);
        SelectedAudioRecordCodec = SelectedFormat.AudioRecordCodec?.Find(it => it == mission.AudioCodec);
        SelectedResolution = Resolution.Resolutions.Find(it => it.ResolutionValue == mission.Resolution);
        if ( SelectedResolution is {ResolutionValue: "其他"} )
        {
            SelectedResolutionX = mission.Width;
            SelectedResolutionY = mission.Height;
        }
        SelectedInterlaces = string.IsNullOrEmpty(mission.Interlaced) ? Interlaces.First() : mission.Interlaced == "i" ? Interlaces[1] : Interlaces.Last();
        SelectedVideoFrame = string.IsNullOrEmpty(mission.VideoFramerate) ? VideoFrames.First() : VideoFrames.Find(it => string.Equals(it, mission.VideoFramerate, StringComparison.OrdinalIgnoreCase));
        if ( SelectedVideoFrame is "其他" )
        {
            Frame = mission.Framerate;
        }
        SegmentTimePicker.SelectedTime = mission.SegmentTime;
        BitRate = string.IsNullOrEmpty(mission.BitRate) ? "0" : mission.BitRate;
        if ( mission.StartDate is not null )
        {
            TaskStartDate = mission.StartDate.Value;
        }
        if ( mission.EndDate is not null )
        {
            TaskEndDate = mission.EndDate.Value;
        }
        if ( mission.StartTime is not null )
        {
            TaskStartTimePicker.SelectedTime = mission.StartTime;
        }
        if ( mission.EndTime is not null )
        {
            TaskEndTimePicker.SelectedTime = mission.EndTime;
        }
        foreach ( ToggleButton buttonGroupItem in ButtonGroup.Items )
        {
            if ( buttonGroupItem.Content is string week )
            {
                buttonGroupItem.IsChecked = mission.Weeks?.Contains(week);
            }
        }
        Height = 915 - ( SelectedMissionType == MissionTypes.First() ? 0 : 160 ) + ( SelectedResolution is {ResolutionValue: "其他"} ? 80 : 0 ) + ( SelectedVideoFrame is "其他" ? 40 : 0 );
    }
    public static bool Show(Window owner)
    {
        var addChannel = new CombinationSettingAddMission
        {
            Owner = owner
        };
        addChannel.ShowDialog();
        return addChannel.AddResult;
    }

    public static bool Show(Window owner, RecordMission mission)
    {
        var addChannel = new CombinationSettingAddMission(mission)
        {
            Owner = owner
        };
        addChannel.ShowDialog();
        return addChannel.AddResult;
    }

#endregion
#region Events

    /// <summary>
    ///     初始化频道列表
    /// </summary>
    private async Task InitChannels()
    {
        Channels = await UrlHelper.GetShouluChannels().ConfigureAwait(false);
        if ( mission is not null )
        {
            SelectedChannel = Channels.Find(it => it.ChannelId == mission.ChannelId);
        }
    }

    /// <summary>
    ///     初始化服务器列表
    /// </summary>
    private async Task InitServers()
    {
        Servers = await UrlHelper.GetShouluServers().ConfigureAwait(false);
        if ( mission is not null )
        {
            SelectedServer = Servers.Find(it => it.ClientId == mission.ClientId);
        }
    }

#endregion
#region Properties

    private readonly RecordMission? mission;
    /// <summary>
    ///     添加任务结果
    /// </summary>
    private bool AddResult { get; set; }
    /// <summary>
    ///     任务类型
    /// </summary>
    public List<string> MissionTypes { get; } = new()
    {
        "周期任务", "即时任务"
    };
    /// <summary>
    ///     选中的任务类型
    /// </summary>
    public string SelectedMissionType { get; set; }
    /// <summary>
    ///     任务名称
    /// </summary>
    public string MissionName { get; set; } = string.Empty;
    /// <summary>
    ///     频道列表
    /// </summary>
    public List<RecordChannel>? Channels { get; set; }
    /// <summary>
    ///     选中的频道
    /// </summary>
    public RecordChannel? SelectedChannel { get; set; }
    /// <summary>
    ///     服务器列表
    /// </summary>
    public List<RecordServer>? Servers { get; set; }
    /// <summary>
    ///     选中的服务器
    /// </summary>
    public RecordServer? SelectedServer { get; set; }
    /// <summary>
    ///     选中的封装格式
    /// </summary>
    public ShouluFormat? SelectedFormat { get; set; }
    /// <summary>
    ///     视频编码格式
    /// </summary>
    public string? SelectedVideoRecordCodec { get; set; }
    /// <summary>
    ///     音频编码格式
    /// </summary>
    public string? SelectedAudioRecordCodec { get; set; }
    /// <summary>
    ///     选中的分辨率
    /// </summary>
    public Resolution? SelectedResolution { get; set; }

    /// <summary>
    ///     分辨率X
    /// </summary>
    public string? SelectedResolutionX { get; set; }
    /// <summary>
    ///     分辨率Y
    /// </summary>
    public string? SelectedResolutionY { get; set; }

    /// <summary>
    ///     场扫描方式
    /// </summary>
    public List<string> Interlaces { get; } = new()
    {
        "自动", "隔行扫描", "逐行扫描"
    };
    /// <summary>
    ///     选中的场扫描方式
    /// </summary>
    public string? SelectedInterlaces { get; set; }

    /// <summary>
    ///     视频帧率
    /// </summary>
    public List<string> VideoFrames { get; } = new()
    {
        "自动", "25", "30", "50", "60", "其他"
    };
    /// <summary>
    ///     选中帧率
    /// </summary>
    public string? SelectedVideoFrame { get; set; }

    /// <summary>
    ///     手动设置帧率
    /// </summary>
    public string? Frame { get; set; }

    /// <summary>
    ///     任务开始日期
    /// </summary>
    public DateTime TaskStartDate { get; set; } = DateTime.Today;
    /// <summary>
    ///     任务截止日期
    /// </summary>
    public DateTime TaskEndDate { get; set; } = DateTime.Today;

    /// <summary>
    ///     比特率
    /// </summary>
    public string? BitRate { get; set; }

#endregion
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        switch ( propertyName )
        {
            case nameof(MissionName):
                if ( MissionName.Length > 64 )
                {
                    MissionName = MissionName.Substring(0, 64);
                }
                break;
            case nameof(SelectedResolutionX):
                SelectedResolutionX = Regex.Replace(SelectedResolutionX ?? string.Empty, "[^0-9]", "");
                break;
            case nameof(SelectedResolutionY):
                SelectedResolutionY = Regex.Replace(SelectedResolutionY ?? string.Empty, "[^0-9]", "");
                break;
            case nameof(Frame):
                Frame = Regex.Replace(Frame ?? string.Empty, "[^0-9]", "");
                break;
            case nameof(BitRate):
                BitRate = Regex.Replace(BitRate ?? string.Empty, "[^0-9]", "");
                break;
            case nameof(SelectedFormat):
                if ( SelectedFormat is {VideoRecordCodec.Count: > 0, AudioRecordCodec.Count: > 0} )
                {
                    SelectedVideoRecordCodec = SelectedFormat.VideoRecordCodec.First();
                    SelectedAudioRecordCodec = SelectedFormat.AudioRecordCodec.First();
                }
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
#region Commands

    /// <summary>
    ///     提交命令
    /// </summary>
    public DelegateCommand? SubmitCommand { get; set; }

    /// <summary>
    ///     重置命令
    /// </summary>
    public DelegateCommand? ResetCommand { get; set; }

#endregion

#region Command-Events

    /// <summary>
    ///     提交
    /// </summary>
    private async void SubmitCommandExecute()
    {
        // 验证输入是否合法
        var errorMessage = ValidateInput();

        if ( !string.IsNullOrEmpty(errorMessage) )
        {
            MessageBox.Warning(errorMessage, "提交");
            return;
        }
        // 创建频道参数对象
        var channelParams = CreateMissionParams();
        bool success;
        // 添加频道并检查是否成功
        if ( mission is null )
        {
            success = await channelParams.AddMission().ConfigureAwait(false);
        }
        else
        {
            success = await channelParams.EditMission().ConfigureAwait(false);
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
    /// <summary>
    ///     验证用户输入的数据是否合法。如果有任何不合法的情况，将返回相应的错误消息，否则返回 null 表示验证通过。
    /// </summary>
    /// <returns> 如果数据不合法，返回相应的错误消息；否则返回 null 表示验证通过。 </returns>
    private string? ValidateInput() =>
        // 对每个字段进行验证，如果有任何字段不合法，立即返回错误消息
        ValidateMissionName() ??
        ValidateSelectedChannel() ??
        ValidateSelectedServer() ??
        ValidateFormat() ??
        ValidateResolution() ??
        ValidateVideoFrame() ??
        ValidateSegmentTime() ??
        ValidateBitRate() ??
        ValidateMissionType();
    /// <summary>
    ///     验证任务名称。如果任务名称为空或空白，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果任务名称为空或空白，返回错误消息；否则返回 null。 </returns>
    private string? ValidateMissionName() =>
        // 如果任务名称为空或空白，返回错误消息
        string.IsNullOrWhiteSpace(MissionName) ? "任务名称不能为空，请输入任务名称" : null;

    /// <summary>
    ///     验证选定的频道。如果频道为空，或者频道的协议不是 "NDI" 且频道的流URL为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果频道为空，或者频道的协议不是 "NDI" 且频道的流URL为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateSelectedChannel()
    {
        // 如果频道为空，返回错误消息
        if ( SelectedChannel is null )
        {
            return "频道不能为空，请选择频道";
        }
        // 如果频道的协议不是 "NDI" 且频道的流URL为空，返回错误消息
        if ( !string.Equals(SelectedChannel.Protocol, "NDI", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(SelectedChannel.StreamURL) )
        {
            return "频道地址不能为空，请重新选择其他频道";
        }
        // 否则，返回 null
        return null;
    }

    /// <summary>
    ///     验证选定的服务器。如果服务器为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果服务器为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateSelectedServer() =>
        // 如果服务器为空，返回错误消息
        SelectedServer is null ? "收录路径不能为空，请选择收录路径" : null;

    /// <summary>
    ///     验证选定的格式。如果格式为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果格式为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateFormat() =>
        // 如果格式为空，返回错误消息
        SelectedFormat is null ? "封装格式不能为空，请选择封装格式" : null;

    /// <summary>
    ///     验证选定的分辨率。如果分辨率为空，或者分辨率的宽和高不在0~4096的范围内，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果分辨率为空，或者分辨率的宽和高不在0~4096的范围内，返回错误消息；否则返回 null。 </returns>
    private string? ValidateResolution()
    {
        // 如果分辨率为空，返回错误消息
        if ( SelectedResolution is {ResolutionValue: "其他"} && ( string.IsNullOrWhiteSpace(SelectedResolutionX) || string.IsNullOrWhiteSpace(SelectedResolutionY) ) )
        {
            return "请输入分辨率的宽和高";
        }
        // 如果分辨率的宽度不在0~4096的范围内，返回错误消息
        if ( int.TryParse(SelectedResolutionX, out var width) && ( width < 0 || width > 4096 ) )
        {
            return "分辨率范围0~4096";
        }
        // 如果分辨率的高度不在0~4096的范围内，返回错误消息
        if ( int.TryParse(SelectedResolutionY, out var height) && ( height < 0 || height > 4096 ) )
        {
            return "分辨率范围0~4096";
        }
        return null;
    }
    /// <summary>
    ///     验证视频帧。如果视频帧为 "其他" 且帧值为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果视频帧为 "其他" 且帧值为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateVideoFrame() =>
        // 如果视频帧为 "其他" 且帧值为空，返回错误消息
        SelectedVideoFrame is "其他" && string.IsNullOrWhiteSpace(Frame) ? "请输入帧率的值" : null;

    /// <summary>
    ///     验证切片时长。如果切片时长为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果切片时长为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateSegmentTime() => SegmentTimePicker.SelectedTime is null ? "请输入切片时长" : null;
    /// <summary>
    ///     验证比特率。如果比特率为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果比特率为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateBitRate() =>
        // 如果比特率为空，返回错误消息
        string.IsNullOrWhiteSpace(BitRate) ? "请输入收录码率" : null;

    /// <summary>
    ///     验证任务类型。如果任务类型为 "周期任务" 且开始时间或结束时间为空，返回错误消息；否则返回 null。
    /// </summary>
    /// <returns> 如果任务类型为 "周期任务" 且开始时间或结束时间为空，返回错误消息；否则返回 null。 </returns>
    private string? ValidateMissionType()
    {
        // 如果任务类型为 "周期任务" 且开始时间或结束时间为空，返回错误消息
        if ( SelectedMissionType == "周期任务" )
        {
            if ( TaskStartTimePicker.SelectedTime is null )
            {
                return "请输入收录开始时间";
            }
            if ( TaskEndTimePicker.SelectedTime is null )
            {
                return "请输入收录结束时间";
            }
        }
        // 否则，返回 null
        return null;
    }

// 创建频道参数对象的方法
    private object CreateMissionParams()
    {
        var weeks = string.Empty;
        foreach ( ToggleButton buttonGroupItem in ButtonGroup.Items )
        {
            if ( buttonGroupItem.Content is string week && ( buttonGroupItem.IsChecked ?? false ) )
            {
                weeks += week;
            }
        }
        return SelectedMissionType switch
        {
            "周期任务" => new
            {
                missionId = mission?.MissionId,
                missionName = MissionName,
                missionFlag = SelectedMissionType == "周期任务" ? "loop" : "real",
                clientId = SelectedServer?.ClientId,
                clientName = SelectedServer?.ClientName,
                recordPath = SelectedServer?.RecordPath,
                videoType = SelectedFormat?.Format,
                videoCodec = SelectedVideoRecordCodec,
                audioCodec = SelectedAudioRecordCodec,
                resolution = SelectedResolution?.ResolutionValue,
                width = SelectedResolution?.ResolutionValue switch
                {
                    "其他" => SelectedResolutionX,
                    _ => SelectedResolution?.Width
                },
                height = SelectedResolution?.ResolutionValue switch
                {
                    "其他" => SelectedResolutionY,
                    _ => SelectedResolution?.Height
                },
                interlaced = SelectedInterlaces switch
                {
                    "隔行扫描" => "i",
                    "逐行扫描" => "p",
                    _ => string.Empty
                },
                videoFramerate = SelectedVideoFrame,
                framerate = SelectedVideoFrame switch
                {
                    "其他" => Frame,
                    "自动" => string.Empty,
                    _ => SelectedVideoFrame
                },
                segmentTime = SegmentTimePicker.SelectedTime!.Value.ToString("hh:mm:ss"),
                channelId = SelectedChannel?.ChannelId,
                channelName = SelectedChannel?.ChannelName,
                streamURL = SelectedChannel?.StreamURL,
                bitRate = BitRate,
                startDate = SelectedMissionType switch
                {
                    "周期任务" => TaskStartDate.Date.ToString("yyyy-MM-dd"),
                    _ => string.Empty
                },
                endDate = SelectedMissionType switch
                {
                    "周期任务" => TaskEndDate.Date.ToString("yyyy-MM-dd"),
                    _ => string.Empty
                },
                startTime = SelectedMissionType switch
                {
                    "周期任务" => TaskStartTimePicker.SelectedTime!.Value.ToString("hh:mm:ss"),
                    _ => string.Empty
                },
                endTime = SelectedMissionType switch
                {
                    "周期任务" => TaskEndTimePicker.SelectedTime!.Value.ToString("hh:mm:ss"),
                    _ => string.Empty
                },
                weeks = SelectedMissionType switch
                {
                    "周期任务" => weeks,
                    _ => string.Empty
                },
                protocol = SelectedChannel?.Protocol
            },
            _ => new
            {
                missionId = mission?.MissionId,
                missionName = MissionName,
                missionFlag = SelectedMissionType == "周期任务" ? "loop" : "real",
                clientId = SelectedServer?.ClientId,
                clientName = SelectedServer?.ClientName,
                recordPath = SelectedServer?.RecordPath,
                videoType = SelectedFormat?.Format,
                videoCodec = SelectedVideoRecordCodec,
                audioCodec = SelectedAudioRecordCodec,
                resolution = SelectedResolution?.ResolutionValue,
                width = SelectedResolution?.ResolutionValue switch
                {
                    "其他" => SelectedResolutionX,
                    _ => SelectedResolution?.Width
                },
                height = SelectedResolution?.ResolutionValue switch
                {
                    "其他" => SelectedResolutionY,
                    _ => SelectedResolution?.Height
                },
                interlaced = SelectedInterlaces switch
                {
                    "隔行扫描" => "i",
                    "逐行扫描" => "p",
                    _ => string.Empty
                },
                videoFramerate = SelectedVideoFrame,
                framerate = SelectedVideoFrame switch
                {
                    "其他" => Frame,
                    "自动" => string.Empty,
                    _ => SelectedVideoFrame
                },
                segmentTime = SegmentTimePicker.SelectedTime!.Value.ToString("hh:mm:ss"),
                channelId = SelectedChannel?.ChannelId,
                channelName = SelectedChannel?.ChannelName,
                streamURL = SelectedChannel?.StreamURL,
                bitRate = BitRate,
                protocol = SelectedChannel?.Protocol
            }
        };
    }
    private void ResetCommandExecute()
    {
        SelectedMissionType = MissionTypes.First();
        MissionName = string.Empty;
        SelectedChannel = null;
        SelectedServer = null;
        SelectedFormat = null;
        SelectedResolution = Resolution.Resolutions.First();
        SelectedResolutionX = string.Empty;
        SelectedResolutionY = string.Empty;
        SelectedInterlaces = Interlaces.First();
        SelectedVideoFrame = VideoFrames.First();
        Frame = string.Empty;
        SegmentTimePicker.SelectedTime = new DateTime(1, 1, 1, 1, 0, 0);
        BitRate = "0";
        TaskStartDate = TaskEndDate = DateTime.Today;
        TaskStartTimePicker.SelectedTime = TaskEndTimePicker.SelectedTime = null;
        foreach ( ToggleButton buttonGroupItem in ButtonGroup.Items )
        {
            buttonGroupItem.IsChecked = false;
        }
    }

#endregion
}
