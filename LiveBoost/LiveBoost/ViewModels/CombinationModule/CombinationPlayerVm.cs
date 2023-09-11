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
    public DelegateCommand RecordPlaybackFfPlayerAddInPointCmd => new DelegateCommand(async () =>
    {
        if ( PlayMode is PlayMode.RecordFile or PlayMode.Access )
        {
            switch ( IsOutSet )
            {
                case true when IsInSet:
                    RecordPlaybackFfPlayerCleanPointsCmd.Execute();
                    IsInSet = true;
                    SliderIn = MdElement.Position.TotalMilliseconds;
                    SliderOut = MdElement.Position.TotalMilliseconds;
                    MarkerThumb = await MdElement.CaptureBitmapAsync();
                    break;
                case true:
                {
                    var inValue = MdElement.Position.TotalMilliseconds;
                    if ( inValue < SliderOut )
                    {
                        SliderIn = inValue;
                        MarkerThumb = await MdElement.CaptureBitmapAsync();
                    }
                    else
                    {
                        SliderIn = SliderOut;
                        SliderOut = inValue;
                    }

                    IsInSet = true;
                    // 添加片段
                    //1.获取缩略图名称
                    RecordMark mark;
                    if ( PlayMode == PlayMode.Access )
                    {
                        if ( string.IsNullOrEmpty(MdElement.Source.LocalPath) )
                        {
                            return;
                        }

                        var targetDir = Path.GetDirectoryName(MdElement.Source.LocalPath);
                        if ( string.IsNullOrEmpty(targetDir) )
                        {
                            return;
                        }

                        if ( !Directory.Exists(targetDir) )
                        {
                            return;
                        }

                        var fileName = Path.GetFileNameWithoutExtension(MdElement.Source.LocalPath);
                        if ( string.IsNullOrEmpty(fileName) )
                        {
                            return;
                        }
                        var frameName = await targetDir.GetNewNameInPathWithoutExtensionAsync(2, $"{fileName}_子片段",
                            ".png");
                        try
                        {
                            MarkerThumb?.Save(Path.Combine(targetDir, $"{frameName}.png"));
                        }
                        catch ( Exception e )
                        {
                            e.LogError("保存收录中ts缩略图异常");
                            await MdElement.Source.LocalPath.GetFrameWithTimeSpan(Path.Combine(targetDir, $"{frameName}.png"),
                                TimeSpan.FromMilliseconds(SliderIn));
                        }
                        mark = new RecordMark
                        {
                            Name = frameName,
                            InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                            OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                            Thumb = Path
                                .Combine(
                                    Path.GetDirectoryName(
                                        MdElement.Source.LocalPath.Replace(
                                            AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'),
                                            string.Empty))!, $"{frameName}.png")
                                .Replace('\\', '/'),
                            Url = MdElement.Source.LocalPath.Replace(
                                AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'),
                                string.Empty).Replace('\\', '/'),
                            ChannelId = PlayChannel!.ChannelId
                        };
                    }
                    else
                    {
                        if ( string.IsNullOrEmpty(PlayFile?.FullPath) )
                        {
                            return;
                        }

                        var targetDir = Path.GetDirectoryName(PlayFile?.FullPath);
                        if ( string.IsNullOrEmpty(targetDir) )
                        {
                            return;
                        }

                        if ( !Directory.Exists(targetDir) )
                        {
                            return;
                        }
                        var fileName = Path.GetFileNameWithoutExtension(MdElement.Source.LocalPath);
                        if ( string.IsNullOrEmpty(fileName) )
                        {
                            return;
                        }
                        var frameName = await targetDir.GetNewNameInPathWithoutExtensionAsync(2, $"{fileName}_子片段",
                            ".png");
                        try
                        {
                            MarkerThumb?.Save(Path.Combine(targetDir, $"{frameName}.png"));
                        }
                        catch ( Exception e )
                        {
                            e.LogError("保存缩略图异常");
                        }
                        mark = new RecordMark
                        {
                            Name = frameName,
                            RecordId = PlayFile?.Id,
                            InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                            OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                            Thumb = Path.Combine(Path.GetDirectoryName(PlayFile?.Url)!, $"{frameName}.png")
                                .Replace('\\', '/')
                        };
                    }


                    var result = await mark.SaveMark().ConfigureAwait(false);
                    if ( !string.IsNullOrEmpty(result) )
                    {
                        mark.Id = result;
                        mark.Parent = PlayFile;
                        CurrentMarks ??= new ObservableList<RecordMark>();
                        CurrentMarks.AddItem(mark);
                    }

                    break;
                }
                default:
                    IsInSet = true;
                    SliderIn = MdElement.Position.TotalMilliseconds;
                    SliderOut = MdElement.Position.TotalMilliseconds;
                    MarkerThumb = await MdElement.CaptureBitmapAsync();
                    break;
            }
        }
        else if ( PlayMode == PlayMode.SubRecordFile )
        {
            var inValue = MdElement.Position.TotalMilliseconds;
            if ( inValue < SliderOut )
            {
                SliderIn = inValue;
                MarkerThumb = await MdElement.CaptureBitmapAsync();
            }
            else
            {
                SliderIn = SliderOut;
                SliderOut = inValue;
            }

            IsInSet = true;
            // 添加片段
            //1.获取缩略图名称
            if ( string.IsNullOrEmpty(PlayFile?.FullPath) )
            {
                return;
            }

            var targetDir = Path.GetDirectoryName(PlayFile?.FullPath);
            if ( string.IsNullOrEmpty(targetDir) )
            {
                return;
            }

            if ( !Directory.Exists(targetDir) )
            {
                return;
            }
            if ( PlayMark is null )
            {
                return;
            }
            try
            {
                MarkerThumb?.Save(AppConfig.Instance.ShouluPath!.Combine(PlayMark.Thumb));
            }
            catch ( Exception e )
            {
                e.LogError("保存缩略图异常");
            }
            var mark = new RecordMark
            {
                Name = PlayMark.Name,
                RecordId = PlayMark.RecordId,
                InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                Thumb = PlayMark.Thumb,
                Id = PlayMark.Id,
                Url = PlayMark.Url,
                ChannelId = PlayMark.ChannelId
            };

            var result = await mark.SaveMark().ConfigureAwait(false);
            if ( string.IsNullOrEmpty(result) )
            {
                if ( !TimeSpan.TryParse(PlayMark.InPoint, out var inpoint) )
                {
                    inpoint = TimeSpan.Zero;
                }
                SliderIn = inpoint.TotalMilliseconds;
                if ( !TimeSpan.TryParse(PlayMark.OutPoint, out var outpoint) )
                {
                    outpoint = TimeSpan.Zero;
                }
                SliderOut = outpoint.TotalMilliseconds;
                return;
            }

            PlayMark.InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff");
            PlayMark.OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff");
        }
    }, () => !IsPlayListMode).ObservesProperty(() => IsPlayListMode);

    /// <summary>
    ///     添加出点
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerAddOutPointCmd => new DelegateCommand(async () =>
    {
        switch ( PlayMode )
        {
            case PlayMode.RecordFile or PlayMode.Access:
                switch ( IsInSet )
                {
                    case true when IsOutSet:
                        RecordPlaybackFfPlayerCleanPointsCmd.Execute();
                        IsOutSet = true;
                        SliderOut = MdElement.Position.TotalMilliseconds;
                        SliderIn = MdElement.Position.TotalMilliseconds;
                        MarkerThumb = await MdElement.CaptureBitmapAsync();
                        break;
                    case true:
                    {
                        var outValue = MdElement.Position.TotalMilliseconds;
                        if ( outValue >= SliderIn )
                        {
                            SliderOut = outValue;
                        }
                        else
                        {
                            SliderOut = SliderIn;
                            SliderIn = outValue;
                            MarkerThumb = await MdElement.CaptureBitmapAsync();
                        }

                        IsOutSet = true;
                        // 添加片段
                        //1.获取缩略图名称
                        RecordMark mark;
                        if ( PlayMode == PlayMode.Access )
                        {
                            if ( string.IsNullOrEmpty(MdElement.Source.LocalPath) )
                            {
                                return;
                            }

                            var targetDir = Path.GetDirectoryName(MdElement.Source.LocalPath);
                            if ( string.IsNullOrEmpty(targetDir) )
                            {
                                return;
                            }

                            if ( !Directory.Exists(targetDir) )
                            {
                                return;
                            }
                            var fileName = Path.GetFileNameWithoutExtension(MdElement.Source.LocalPath);
                            if ( string.IsNullOrEmpty(fileName) )
                            {
                                return;
                            }
                            var frameName = await targetDir.GetNewNameInPathWithoutExtensionAsync(2, $"{fileName}_子片段",
                                ".png");
                            try
                            {
                                MarkerThumb?.Save(Path.Combine(targetDir, $"{frameName}.png"));
                            }
                            catch ( Exception e )
                            {
                                e.LogError("保存缩略图异常");
                                await MdElement.Source.LocalPath.GetFrameWithTimeSpan(Path.Combine(targetDir, $"{frameName}.png"),
                                    TimeSpan.FromMilliseconds(SliderIn));
                            }
                            mark = new RecordMark
                            {
                                Name = frameName,
                                InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                                OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                                Thumb = Path
                                    .Combine(
                                        Path.GetDirectoryName(
                                            MdElement.Source.LocalPath.Replace(
                                                AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'),
                                                string.Empty))!, $"{frameName}.png")
                                    .Replace('\\', '/'),
                                Url = MdElement.Source.LocalPath.Replace(
                                    AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'),
                                    string.Empty).Replace('\\', '/'),
                                ChannelId = PlayChannel!.ChannelId
                            };
                        }
                        else
                        {
                            if ( string.IsNullOrEmpty(PlayFile?.FullPath) )
                            {
                                return;
                            }

                            var targetDir = Path.GetDirectoryName(PlayFile?.FullPath);
                            if ( string.IsNullOrEmpty(targetDir) )
                            {
                                return;
                            }

                            if ( !Directory.Exists(targetDir) )
                            {
                                return;
                            }

                            var fileName = Path.GetFileNameWithoutExtension(MdElement.Source.LocalPath);
                            if ( string.IsNullOrEmpty(fileName) )
                            {
                                return;
                            }
                            var frameName = await targetDir.GetNewNameInPathWithoutExtensionAsync(2, $"{fileName}_子片段",
                                ".png");
                            try
                            {
                                MarkerThumb?.Save(Path.Combine(targetDir, $"{frameName}.png"));
                            }
                            catch ( Exception e )
                            {
                                e.LogError("保存缩略图异常");
                            }
                            mark = new RecordMark
                            {
                                Name = frameName,
                                RecordId = PlayFile?.Id,
                                InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                                OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                                Thumb = Path.Combine(Path.GetDirectoryName(PlayFile?.Url)!, $"{frameName}.png")
                                    .Replace('\\', '/')
                            };
                        }

                        var result = await mark.SaveMark().ConfigureAwait(false);
                        if ( !string.IsNullOrEmpty(result) )
                        {
                            mark.Id = result;
                            mark.Parent = PlayFile;
                            CurrentMarks ??= new ObservableList<RecordMark>();
                            CurrentMarks.AddItem(mark);
                        }

                        break;
                    }
                    default:
                        IsOutSet = true;
                        SliderOut = MdElement.Position.TotalMilliseconds;
                        SliderIn = MdElement.Position.TotalMilliseconds;
                        MarkerThumb = await MdElement.CaptureBitmapAsync();
                        break;
                }
                break;
            case PlayMode.SubRecordFile:
            {
                var outValue = MdElement.Position.TotalMilliseconds;
                if ( outValue >= SliderIn )
                {
                    SliderOut = outValue;
                }
                else
                {
                    SliderOut = SliderIn;
                    SliderIn = outValue;
                    MarkerThumb = await MdElement.CaptureBitmapAsync();
                }

                IsInSet = true;
                // 添加片段
                //1.获取缩略图名称
                if ( string.IsNullOrEmpty(PlayFile?.FullPath) )
                {
                    return;
                }

                var targetDir = Path.GetDirectoryName(PlayFile?.FullPath);
                if ( string.IsNullOrEmpty(targetDir) )
                {
                    return;
                }

                if ( !Directory.Exists(targetDir) )
                {
                    return;
                }
                if ( PlayMark is null )
                {
                    return;
                }
                try
                {
                    MarkerThumb?.Save(AppConfig.Instance.ShouluPath!.Combine(PlayMark.Thumb));
                }
                catch ( Exception e )
                {
                    e.LogError("保存缩略图异常");
                }

                var mark = new RecordMark
                {
                    Name = PlayMark.Name,
                    RecordId = PlayMark.RecordId,
                    InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                    OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                    Thumb = PlayMark.Thumb,
                    Id = PlayMark.Id,
                    Url = PlayMark.Url,
                    ChannelId = PlayMark.ChannelId
                };

                var result = await mark.SaveMark().ConfigureAwait(false);
                if ( string.IsNullOrEmpty(result) )
                {
                    if ( !TimeSpan.TryParse(PlayMark.InPoint, out var inpoint) )
                    {
                        inpoint = TimeSpan.Zero;
                    }
                    SliderIn = inpoint.TotalMilliseconds;
                    if ( !TimeSpan.TryParse(PlayMark.OutPoint, out var outpoint) )
                    {
                        outpoint = TimeSpan.Zero;
                    }
                    SliderOut = outpoint.TotalMilliseconds;
                    return;
                }

                PlayMark.InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff");
                PlayMark.OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff");
                break;
            }
        }
    }, () => !IsPlayListMode).ObservesProperty(() => IsPlayListMode);

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
        // CurrentPlayFile.Progress =
        //     Math.Round(
        //         ( MdElement!.Position.TotalMilliseconds - PlayerInpoint.Value.TotalMilliseconds ) * 100 /
        //         ( PlayerOutpoint.Value.TotalMilliseconds - PlayerInpoint.Value.TotalMilliseconds ), 2,
        //         MidpointRounding.AwayFromZero);
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

        if ( CurrentPlayFile is not null )
        {
            // CurrentPlayFile.IsPlaying = false;
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

        if ( CurrentPlayFile is not null )
        {
            // CurrentPlayFile.IsPlaying = true;
        }
    });

#endregion
}
