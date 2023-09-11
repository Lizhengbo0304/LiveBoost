// 创建时间：2023-09-07-11:35
// 修改时间：2023-09-07-16:24

#region

using System.Drawing;
using HandyControl.Controls;
using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm
{
#region Properties

    // 当前播放器
    public MediaElement MdElement { get; set; }
    public MediaElement MdActive { get; set; }
    // 播放器Panel
    public SimplePanel? MdPanel { get; set; }
    // 播放名称
    public string? PlayName { get; set; }
    public double DrawVuMeterLeftValue { get; set; }
    public double DrawVuMeterRightValue { get; set; }
    // 播放模式
    public PlayMode PlayMode { get; set; }
    // 播放通道
    public RecordChannel? PlayChannel { get; set; }

    // 播放标记点
    public RecordMark? PlayMark { get; set; }
    public bool IsPlayListMode { get; set; }

    // 播放标播单
    public ObservableList<RecordFile>? PlayFiles { get; set; }
    public int CurrentIndex { get; set; }
    public RecordFile? CurrentPlayFile { get; set; }

    // 播放文件
    public RecordFile? PlayFile { get; set; }

    // 播放器入点
    public TimeSpan? PlayerInpoint { get; set; }

    // 播放器出点
    public TimeSpan? PlayerOutpoint { get; set; }

    // 入出点缩略图
    public Bitmap? MarkerThumb { get; set; }

    // 入出点是否设置
    public bool IsInSet { get; set; }
    public bool IsOutSet { get; set; }

    // 入出点的值
    public double SliderIn { get; set; }
    public double SliderOut { get; set; }

#endregion
#region Event

    private short[]? drawVuMeterLeftSamples;
    private short[]? drawVuMeterRightSamples;
    private readonly object drawVuMeterRmsLock = new();
    private void FfPlayOnRenderingAudio(object sender, RenderingAudioEventArgs e)
    {
        // 分析即将渲染的音频数据
        // 如果不存在音频，则不需要分析
        if ( e.EngineState.HasAudio == false )
        {
            return;
        }
        // 把音频数据分为左右声道
        if ( drawVuMeterLeftSamples == null || drawVuMeterLeftSamples.Length != e.SamplesPerChannel )
        {
            drawVuMeterLeftSamples = new short[e.SamplesPerChannel];
        }

        if ( drawVuMeterRightSamples == null || drawVuMeterRightSamples.Length != e.SamplesPerChannel )
        {
            drawVuMeterRightSamples = new short[e.SamplesPerChannel];
        }

        var bufferData = e.GetBufferData();

        // 使用 Buffer.BlockCopy 替代循环和 BitConverter.ToInt16
        Buffer.BlockCopy(bufferData, 0, drawVuMeterLeftSamples, 0, e.SamplesPerChannel * sizeof(short)); // 复制左声道数据
        Buffer.BlockCopy(bufferData, e.SamplesPerChannel * sizeof(short), drawVuMeterRightSamples, 0, e.SamplesPerChannel * sizeof(short)); // 复制右声道数据

        // 使用 Parallel.Invoke 同时计算左右声道的 RMS 值
        double leftValue = 0.0, rightValue = 0.0;
        lock ( drawVuMeterRmsLock )
        {
            Parallel.Invoke(
                () => { leftValue = VolumeHelper.CalculateRms(drawVuMeterLeftSamples); },
                () => { rightValue = VolumeHelper.CalculateRms(drawVuMeterRightSamples); }
            );
            DrawVuMeterLeftValue = leftValue;
            DrawVuMeterRightValue = rightValue;
        }
    }
#region 添加标记点

    /// <summary>
    ///     添加入点
    /// </summary>
    private async void RecordPlaybackFfPlayerAddInPoint()
    {
        await RecordPlaybackFfPlayerAddPoint(true);
    } /// <summary>
    ///     添加出点
    /// </summary>
    private async void RecordPlaybackFfPlayerAddOutPoint()
    {
        await RecordPlaybackFfPlayerAddPoint(false);
    }
    /// <summary>
    /// 添加标记点
    /// </summary>
    /// <param name="isIn">是否为入点</param>
    private async Task RecordPlaybackFfPlayerAddPoint(bool isIn)
    {
        switch ( PlayMode )
        {
            // 收录完成文件、收录预览ts设置标记点
            case PlayMode.RecordFile or PlayMode.Access when isIn:
                // 设置入点
                switch ( IsOutSet )
                {
                    case true when IsInSet:
                        // 添加新的标记点，入出点一致
                        await RecordPlaybackFfPlayerSetNewPoint(isIn);
                        return;
                    case true:
                        // 出点存在，设置入点
                        await RecordPlaybackFfPlayerSetNewPointWithExistPoint(isIn);
                        // 调用方法
                        await AddFragment();
                        return;
                    default:
                        await RecordPlaybackFfPlayerSetNewPoint(isIn);
                        return;
                }
            case PlayMode.RecordFile or PlayMode.Access:
                // 设置出点
                switch ( IsInSet )
                {
                    case true when IsOutSet:
                        // 添加新的标记点，入出点一致
                        await RecordPlaybackFfPlayerSetNewPoint(isIn);
                        return;
                    case true:
                        // 入点存在，设置出点
                        await RecordPlaybackFfPlayerSetNewPointWithExistPoint(isIn);
                        // 调用方法
                        await AddFragment();
                        return;
                    default:
                        await RecordPlaybackFfPlayerSetNewPoint(isIn);
                        return;
                }
            case PlayMode.SubRecordFile:
                // 修改入点
                await RecordPlaybackFfPlayerSetNewPointWithExistPoint(isIn);
                // 获取缩略图名称
                var thumbnailName = GetThumbnailName();

                // 保存缩略图
                SaveThumbnail(thumbnailName);

                // 创建记录标记
                var mark = CreateRecordMark();

                // 添加记录标记
                await AddRecordMarkWithPlayMark(mark);
                return;
        }
    }

    /// <summary>
    /// 添加新的标记点，入出点一致
    /// </summary>
    /// <param name="isIn">是否为入点</param>
    private async Task RecordPlaybackFfPlayerSetNewPoint(bool isIn)
    {
        // 执行清除标记点的命令
        RecordPlaybackFfPlayerCleanPointsCmd.Execute();

        // 设置入点和出点的值为当前位置的毫秒数
        SliderOut = MdElement.Position.TotalMilliseconds;
        SliderIn = MdElement.Position.TotalMilliseconds;

        // 获取标记点的缩略图
        MarkerThumb = await MdElement.CaptureBitmapAsync();

        // 根据参数设置相应的标记点状态
        if (isIn)
        {
            IsInSet = true;
        }
        else
        {
            IsOutSet = true;
        }
    }
    /// <summary>
    /// 存在入点的情况设置出点（或存在出点的情况设置入点）
    /// </summary>
    /// <param name="isIn">是否为入点</param>
    private async Task RecordPlaybackFfPlayerSetNewPointWithExistPoint(bool isIn)
    {
        var position = MdElement.Position.TotalMilliseconds;

        if (isIn)
        {
            // 如果当前位置小于出点位置，则将当前位置设置为入点，并获取标记点的缩略图
            if (position < SliderOut)
            {
                SliderIn = position;
                MarkerThumb = await MdElement.CaptureBitmapAsync();
            }
            else
            {
                // 如果当前位置大于等于出点位置，则将出点位置设置为入点（缩略图以获取），当前位置设置为出点
                SliderIn = SliderOut;
                SliderOut = position;
            }

            IsInSet = true; // 设置入点已设置的状态为true
        }
        else
        {
            // 如果当前位置大于等于入点位置，则将当前位置设置为出点
            if (position >= SliderIn)
            {
                SliderOut = position;
            }
            else
            {
                // 如果当前位置小于入点位置，则将当前位置设置为入点，入点位置设置为出点，并获取标记点的缩略图
                SliderOut = SliderIn;
                SliderIn = position;
                MarkerThumb = await MdElement.CaptureBitmapAsync();
            }

            IsOutSet = true; // 设置出点已设置的状态为true
        }
    }
    /// <summary>
    /// 获取帧名称
    /// </summary>
    /// <param name="targetDir">目标目录</param>
    /// <param name="fileName">文件名</param>
    /// <returns>缩略图名称</returns>
private async Task<string> GetFrameName(string targetDir, string fileName)
{
    // 使用目标目录和文件名生成新的缩略图名称
    return await targetDir.GetNewNameInPathWithoutExtensionAsync(2, $"{fileName}_子片段", ".png");
}

/// <summary>
/// 保存缩略图
/// </summary>
/// <param name="targetDir">目标目录</param>
/// <param name="frameName">帧名称</param>
private void SaveThumbnail(string targetDir, string frameName)
{
    try
    {
        // 保存缩略图到目标目录
        MarkerThumb?.Save(Path.Combine(targetDir, $"{frameName}.png"));
    }
    catch (Exception e)
    {
        e.LogError("保存缩略图异常");
    }
}
/// <summary>
/// 保存缩略图
/// </summary>
/// <param name="thumbnailName">缩略图的完整路径</param>
private void SaveThumbnail(string thumbnailName)
{
    try
    {
        MarkerThumb?.Save(thumbnailName);
    }
    catch (Exception e)
    {
        e.LogError("保存缩略图异常");
    }
}

/// <summary>
/// 创建标记点
/// </summary>
/// <param name="frameName">缩略图名称</param>
/// <returns>标记点</returns>
private RecordMark CreateRecordMark(string frameName)
{
    if (PlayMode == PlayMode.Access)
    {
        // 如果收录通道预览模式
        return new RecordMark
        {
            Name = frameName,
            InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
            OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
            Thumb = Path.Combine(Path.GetDirectoryName(MdElement.Source.LocalPath.Replace(AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'), string.Empty))!, $"{frameName}.png").Replace('\\', '/'),
            Url = MdElement.Source.LocalPath.Replace(AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'), string.Empty).Replace('\\', '/'),
            ChannelId = PlayChannel!.ChannelId
        };
    }
    return new RecordMark
    {
        Name = frameName,
        RecordId = PlayFile?.Id,
        InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
        OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
        Thumb = Path.Combine(Path.GetDirectoryName(PlayFile?.Url)!, $"{frameName}.png").Replace('\\', '/')
    };
}

/// <summary>
/// 新增标记点
/// </summary>
/// <param name="mark">标记点</param>
private async Task AddRecordMark(RecordMark mark)
{
    // 保存记录标记并获取结果
    var result = await mark.SaveMark().ConfigureAwait(false);

    if (!string.IsNullOrEmpty(result))
    {
        // 如果结果不为空，设置标记标识和父级，并将标记添加到当前标记列表
        mark.Id = result;
        mark.Parent = PlayFile;
        CurrentMarks ??= new ObservableList<RecordMark>();
        CurrentMarks.AddItem(mark);
    }
}

/// <summary>
/// 添加片段
/// </summary>
private async Task AddFragment()
{
    string? targetDir;
    string? fileName;

    if (PlayMode == PlayMode.Access)
    {
        if (string.IsNullOrEmpty(MdElement.Source.LocalPath))
        {
            return;
        }

        // 获取目标目录和文件名
        targetDir = Path.GetDirectoryName(MdElement.Source.LocalPath);
        fileName = Path.GetFileNameWithoutExtension(MdElement.Source.LocalPath);
    }
    else
    {
        if (string.IsNullOrEmpty(PlayFile?.FullPath))
        {
            return;
        }

        // 获取目标目录和文件名
        targetDir = Path.GetDirectoryName(PlayFile?.FullPath);
        fileName = Path.GetFileNameWithoutExtension(MdElement.Source.LocalPath);
    }

    if (string.IsNullOrEmpty(targetDir) || !Directory.Exists(targetDir) || string.IsNullOrEmpty(fileName))
    {
        return;
    }

    // 获取缩略图名称
    var frameName = await GetFrameName(targetDir, fileName);

    // 保存缩略图
    SaveThumbnail(targetDir, frameName);

    // 创建记录标记
    var mark = CreateRecordMark(frameName);

    // 添加记录标记
    await AddRecordMark(mark);
}

/// <summary>
/// 获取缩略图的名称
/// </summary>
/// <returns>缩略图的名称</returns>
private string GetThumbnailName()
{
    if (string.IsNullOrEmpty(PlayFile?.FullPath))
    {
        return string.Empty;
    }

    // 获取目标目录
    var targetDir = Path.GetDirectoryName(PlayFile?.FullPath);
    if (string.IsNullOrEmpty(targetDir) || !Directory.Exists(targetDir))
    {
        return string.Empty;
    }

    if (PlayMark is null)
    {
        return string.Empty;
    }

    // 构建缩略图的完整路径
    return AppConfig.Instance.ShouluPath!.Combine(PlayMark.Thumb);
}
/// <summary>
/// 创建记录标记
/// </summary>
/// <returns>创建的记录标记</returns>
private RecordMark CreateRecordMark()
{
    return new RecordMark
    {
        Name = PlayMark!.Name,
        RecordId = PlayMark.RecordId,
        InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
        OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
        Thumb = PlayMark.Thumb,
        Id = PlayMark.Id,
        Url = PlayMark.Url,
        ChannelId = PlayMark.ChannelId
    };
}
/// <summary>
/// 添加记录标记
/// </summary>
/// <param name="mark">要添加的记录标记</param>
private async Task AddRecordMarkWithPlayMark(RecordMark mark)
{
    var result = await mark.SaveMark().ConfigureAwait(false);

    if (string.IsNullOrEmpty(result))
    {
        // 如果保存失败，则将滑块的值重置为原始值
        if (!TimeSpan.TryParse(mark.InPoint, out var inpoint))
        {
            inpoint = TimeSpan.Zero;
        }
        SliderIn = inpoint.TotalMilliseconds;
        if (!TimeSpan.TryParse(mark.OutPoint, out var outpoint))
        {
            outpoint = TimeSpan.Zero;
        }
        SliderOut = outpoint.TotalMilliseconds;
        return;
    }

    // 更新记录标记的入点和出点
    PlayMark!.InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff");
    PlayMark.OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff");
}

#endregion
#endregion

#region 播放器

    /// <summary>
    ///     前5秒
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerBackwardCmd => new(() =>
    {
        if ( PlayerInpoint is not null )
        {
            if ( MdElement.Position - TimeSpan.FromSeconds(5) >
                 PlayerOutpoint )
            {
                MdElement.Position -= TimeSpan.FromSeconds(5);
            }
            else
            {
                MdElement.Position = PlayerInpoint!.Value;
            }
        }
        else if ( MdElement.PlaybackStartTime is not null )
        {
            if ( MdElement.Position - TimeSpan.FromSeconds(5) >
                 MdElement.PlaybackStartTime )
            {
                MdElement.Position -= TimeSpan.FromSeconds(5);
            }
            else
            {
                MdElement.Position = MdElement.PlaybackStartTime!.Value;
            }
        }
        else
        {
            MdElement.Position = MdElement.PlaybackStartTime!.Value;
        }
    });
    /// <summary>
    ///     上一帧
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerStepBackwardCmd => new(async () =>
    {
        if ( PlayerInpoint != null )
        {
            if ( MdElement.Position - MdElement.PositionStep > PlayerInpoint.Value )
            {
                await MdElement.StepBackward();
            }
        }
        else
        {
            await MdElement.StepBackward();
        }
    });

    /// <summary>
    ///     播放、暂停
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerPlayPauseCmd => new(async () =>
    {
        if ( !MdElement.IsSeekable )
        {
            return;
        }

        if ( MdElement.IsPlaying )
        {
            await MdElement.Pause();
        }
        else
        {
            if ( PlayerOutpoint is not null )
            {
                if ( MdElement.Position < PlayerOutpoint )
                {
                    await MdElement.Play();
                }
            }
            else
            {
                await MdElement.Play();
            }
        }
    });

    /// <summary>
    ///     下一帧
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerStepForwardCmd => new(async () =>
    {
        if ( PlayerOutpoint is not null )
        {
            if ( MdElement.Position + MdElement.PositionStep <=
                 PlayerOutpoint )
            {
                await MdElement.StepForward();
            }
        }
        else
        {
            await MdElement.StepForward();
        }
    });

    /// <summary>
    ///     向后5秒
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerForwardCmd => new(() =>
    {
        if ( PlayerOutpoint is not null )
        {
            if ( MdElement.Position + TimeSpan.FromSeconds(5) <=
                 PlayerOutpoint )
            {
                MdElement.Position += TimeSpan.FromSeconds(5);
            }
            else
            {
                MdElement.Position = PlayerOutpoint!.Value;
            }
        }
        else if ( MdElement.PlaybackEndTime is not null )
        {
            if ( MdElement.Position + TimeSpan.FromSeconds(5) <=
                 MdElement.PlaybackEndTime )
            {
                MdElement.Position += TimeSpan.FromSeconds(5);
            }
            else
            {
                MdElement.Position = MdElement.PlaybackEndTime!.Value;
            }
        }
        else
        {
            MdElement.Position = MdElement.PlaybackEndTime!.Value;
        }
    });
    /// <summary>
    ///     清除入出点
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerCleanPointsCmd => new(() =>
    {
        IsInSet = false;
        IsOutSet = false;
        SliderIn = 0;
        SliderOut = 0;
    });
    /// <summary>
    ///     添加入点
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerAddInPointCmd => new DelegateCommand(RecordPlaybackFfPlayerAddInPoint, () => !IsPlayListMode).ObservesProperty(() => IsPlayListMode);

    /// <summary>
    ///     添加出点
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerAddOutPointCmd => new DelegateCommand(RecordPlaybackFfPlayerAddOutPoint, () => !IsPlayListMode).ObservesProperty(() => IsPlayListMode);

    private bool isInternal;

    //播放进度
    public DelegateCommand<RoutedPropertyChangedEventArgs<double>> SliderValueChangedCmd => new(async arg =>
    {
        // 内部Seek不处理
        if ( isInternal )
        {
            return;
        }
        // 非播单预览模式不处理
        if ( !IsPlayListMode )
        {
            return;
        }
        // 播单条目为空不处理
        if ( PlayFiles is not {Count: > 0} )
        {
            return;
        }

        if ( CurrentPlayFile is null )
        {
            return;
        }

        if ( PlayerInpoint == null )
        {
            return;
        }
        if ( PlayerOutpoint == null )
        {
            return;
        }

        var playerOutpoint = PlayerOutpoint;

        // 当前时间未超过出点不处理
        if ( playerOutpoint is null || !( arg.NewValue >= playerOutpoint.Value.TotalMilliseconds ) )
        {
            return;
        }

        await MdElement.Pause();

        if ( Equals(MdElement, MdActive) )
        {
            return;
        }
        CurrentIndex++;
        if ( CurrentPlayFile is null )
        {
            return;
        }
        if ( CurrentIndex >= PlayFiles.Count )
        {
            // 当前条目为最后一条时，停止在出点
            await MdElement.Seek(playerOutpoint.Value);
            return;
        }

        CurrentPlayFile = PlayFiles[CurrentIndex];

        isInternal = true;
        PlayerInpoint = CurrentPlayFile.RealInPoint!.Value;
        PlayerOutpoint = CurrentPlayFile.RealOutPoint!.Value;
        isInternal = false;
        PlayFiles.ForEach(it =>
        {
            it.MediaElement!.Visibility = it == CurrentPlayFile ? Visibility.Visible : Visibility.Collapsed;
        });
        MdElement = CurrentPlayFile.MediaElement!;
        await MdElement.Play();
    });

#endregion
}
