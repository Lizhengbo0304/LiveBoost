// 创建时间：2023-09-07 11:35
// 修改时间：2023-11-06 11:17

#region

using System.Drawing;
using HandyControl.Controls;
using LiveBoost.ToolKit.Tools;
using Brushes = System.Windows.Media.Brushes;

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
    public bool IsPlayListMode => PlayMode is PlayMode.PlayList;

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
        if (e.EngineState.HasAudio == false)
        {
            return;
        }

        // 把音频数据分为左右声道
        if ((drawVuMeterLeftSamples == null) || (drawVuMeterLeftSamples.Length != e.SamplesPerChannel))
        {
            drawVuMeterLeftSamples = new short[e.SamplesPerChannel];
        }

        if ((drawVuMeterRightSamples == null) || (drawVuMeterRightSamples.Length != e.SamplesPerChannel))
        {
            drawVuMeterRightSamples = new short[e.SamplesPerChannel];
        }

        var bufferData = e.GetBufferData();

        // 使用 Buffer.BlockCopy 替代循环和 BitConverter.ToInt16
        Buffer.BlockCopy(bufferData, 0, drawVuMeterLeftSamples, 0, e.SamplesPerChannel * sizeof(short)); // 复制左声道数据
        Buffer.BlockCopy(bufferData, e.SamplesPerChannel * sizeof(short), drawVuMeterRightSamples, 0, e.SamplesPerChannel * sizeof(short)); // 复制右声道数据

        // 使用 Parallel.Invoke 同时计算左右声道的 RMS 值
        double leftValue = 0.0, rightValue = 0.0;
        lock (drawVuMeterRmsLock)
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
    }

    /// <summary>
    ///     添加出点
    /// </summary>
    private async void RecordPlaybackFfPlayerAddOutPoint()
    {
        await RecordPlaybackFfPlayerAddPoint(false);
    }

    /// <summary>
    ///     添加标记点
    /// </summary>
    /// <param name="isIn"> 是否为入点 </param>
    private async Task RecordPlaybackFfPlayerAddPoint(bool isIn)
    {
        switch (PlayMode)
        {
            // 收录完成文件、收录预览ts设置标记点
            case PlayMode.RecordFile or PlayMode.Access when isIn:
                // 设置入点
                switch (IsOutSet)
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
                switch (IsInSet)
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
    ///     添加新的标记点，入出点一致
    /// </summary>
    /// <param name="isIn"> 是否为入点 </param>
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
    ///     存在入点的情况设置出点（或存在出点的情况设置入点）
    /// </summary>
    /// <param name="isIn"> 是否为入点 </param>
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
    ///     获取帧名称
    /// </summary>
    /// <param name="targetDir"> 目标目录 </param>
    /// <param name="fileName"> 文件名 </param>
    /// <returns> 缩略图名称 </returns>
    private async Task<string> GetFrameName(string targetDir, string fileName) =>
        // 使用目标目录和文件名生成新的缩略图名称
        await targetDir.GetNewNameInPathWithoutExtensionAsync(2, $"{fileName}_子片段", ".png");

    /// <summary>
    ///     保存缩略图
    /// </summary>
    /// <param name="targetDir"> 目标目录 </param>
    /// <param name="frameName"> 帧名称 </param>
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
    ///     保存缩略图
    /// </summary>
    /// <param name="thumbnailName"> 缩略图的完整路径 </param>
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
    ///     创建标记点
    /// </summary>
    /// <param name="frameName"> 缩略图名称 </param>
    /// <returns> 标记点 </returns>
    private RecordMark CreateRecordMark(string frameName)
    {
        var playerPath = Path.GetFullPath(MdElement.Source.LocalPath);
        var basePath = Path.GetFullPath(AppConfig.Instance.ShouluPath);
        if (PlayMode == PlayMode.Access)
            // 如果收录通道预览模式
        {
            return new RecordMark
            {
                Name = frameName,
                InPoint = TimeSpan.FromMilliseconds(SliderIn).ToString(@"hh\:mm\:ss\.ffff"),
                OutPoint = TimeSpan.FromMilliseconds(SliderOut).ToString(@"hh\:mm\:ss\.ffff"),
                Thumb = Path.Combine(Path.GetDirectoryName(playerPath.Replace(basePath, string.Empty))!, $"{frameName}.png")
                    .Replace('\\', '/'),
                Url = playerPath.Replace(basePath, string.Empty).Replace('\\', '/'),
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
    ///     新增标记点
    /// </summary>
    /// <param name="mark"> 标记点 </param>
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
    ///     添加片段
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
        $"MdElement.Source.LocalPath = {MdElement.Source.LocalPath}\n\ttargetDir = {targetDir}\n\tfileName = {fileName}\n\tframeName = {frameName}\n\tmark = {JsonConvert.SerializeObject(mark)}".LogFileInfo();
    }

    /// <summary>
    ///     获取缩略图的名称
    /// </summary>
    /// <returns> 缩略图的名称 </returns>
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
    ///     创建记录标记
    /// </summary>
    /// <returns> 创建的记录标记 </returns>
    private RecordMark CreateRecordMark() =>
        new()
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

    /// <summary>
    ///     添加记录标记
    /// </summary>
    /// <param name="mark"> 要添加的记录标记 </param>
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

    #region 播放设置

    /// <summary>
    ///     播单预览
    /// </summary>
    public async Task PlayPushAccessAsync()
    {
        try
        {
            await CleanupPlayer();
            if (CurrentPlayList is null)
            {
                return;
            }

            // 设置播放标题、重置Mark点列表、设置播放模式、播放列表
            PlayName = CurrentPlayList.Title;
            CurrentMarks = new ObservableList<RecordMark>();
            PlayMode = PlayMode.PlayList;
            PlayFiles = CurrentPlayList.RecordFiles;
            if (!CurrentPlayList.RecordFiles.Any())
            {
                MessageBox.Warning("该播单条目为空，无法预览", "播单预览");
                return;
            }

            MdPanel!.Visibility = Visibility.Visible;
            MdActive.Visibility = Visibility.Collapsed;
            foreach (var recordFile in PlayFiles)
            {
                recordFile.MediaElement = new MediaElement
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.Black,
                    LoadedBehavior = MediaPlaybackState.Manual,
                    IsHitTestVisible = false,
                    Visibility = Visibility.Collapsed
                };
                recordFile.MediaElement.RenderingAudio += FfPlayOnRenderingAudio;
                await recordFile.MediaElement.Open(new Uri(recordFile.FullPath));
                if (recordFile.RealInPoint is null || recordFile.RealOutPoint is null)
                {
                    recordFile.RealInPoint = recordFile.MediaElement.PlaybackStartTime;
                    recordFile.RealOutPoint = recordFile.MediaElement.PlaybackEndTime;
                }

                await recordFile.MediaElement.Seek(recordFile.RealInPoint!.Value);
                MdPanel.Children.Add(recordFile.MediaElement);
            }

            CurrentIndex = 0;
            CurrentPlayFile = CurrentPlayList.RecordFiles[CurrentIndex];
            CurrentPlayFile.MediaElement!.Visibility = Visibility.Visible;
            PlayerInpoint = CurrentPlayFile.RealInPoint!.Value;
            PlayerOutpoint = CurrentPlayFile.RealOutPoint!.Value;
            MdElement = CurrentPlayFile.MediaElement!;
            await MdElement.Play();
        }
        catch (Exception e)
        {
            MessageBox.Warning($"播单播放器初始化异常：{e.Message}", "播单预览");
            await CleanupPlayer();
        }
    }

    /// <summary>
    ///     播放收录文件
    /// </summary>
    /// <param name="file"> 要播放的收录文件 </param>
    public async Task PlayRecordFileAsync(RecordFile file)
    {
        await CleanupPlayer();
        // 切换显示的播放器元素
        MdPanel!.Visibility = Visibility.Collapsed;
        MdActive.Visibility = Visibility.Visible;
        MdElement = MdActive;

        // 设置播放文件相关的属性
        PlayName = file.Name;
        PlayMode = PlayMode.RecordFile;
        PlayFile = file;

        // 检查文件是否存在
        if (!File.Exists(file.FullPath))
        {
            MessageBox.Warning("文件丢失，无法预览", "播放");
            return;
        }

        PlayerInpoint = null;
        PlayerOutpoint = null;
        // 打开并播放文件
        await MdElement.Open(new Uri(file.FullPath));
        await MdElement.Play();

        CurrentMarks = file.Markers ??= new ObservableList<RecordMark>();
        CurrentTags = CurrentMarks?.Select(x =>new RecordTag()
        {
            InPointValue = x.InPoint?.ParseTimeSpan()?.TotalMilliseconds,
            OutPointValue = x.OutPoint?.ParseTimeSpan()?.TotalMilliseconds,
        }).ToObservableList();
    }

    public async Task PlayRecordAccess(RecordAccess access)
    {
        await CleanupPlayer();
        // 切换显示的播放器元素
        MdPanel!.Visibility = Visibility.Collapsed;
        MdActive.Visibility = Visibility.Visible;
        MdElement = MdActive;
        // 设置播放模式为 Access，并设置相关属性
        PlayMode = PlayMode.Access;
        PlayName = access.AccessName;
        CurrentMarks = await access.Channel!.ChannelId!.GetMarks(access.VideoPath!);
        PlayChannel = access.Channel;
        PlayerInpoint = null;
        PlayerOutpoint = null;
        // 打开视频文件
        if (!await MdElement.Open(new Uri(access.VideoPath!)))
        {
            MessageBox.Warning("收录预览失败", "预览");
            return;
        }

        // 调整视频播放位置
        await AdjustVideoPlayback();

        // 播放视频
        await MdElement.Play();

        // 获取更新后的标记
        CurrentMarks = await GetUpdatedMarks(access);
    }

// 调整视频播放位置
    private async Task AdjustVideoPlayback()
    {
        if (MdElement.RemainingDuration is not null)
        {
            var remainingDuration = MdElement.RemainingDuration.Value;

            if (remainingDuration.TotalSeconds > 20)
            {
                // 如果剩余时长大于 20 秒，则从剩余时长的倒数第 20 秒开始播放
                await MdElement.Seek(remainingDuration.Add(TimeSpan.FromSeconds(-20)));
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
    private async Task<ObservableList<RecordMark>> GetUpdatedMarks(RecordAccess access)
    {
        var videoPath = MdElement.Source.LocalPath.Replace(
            AppConfig.Instance.ShouluPath!.ToLower().Replace('/', '\\'),
            string.Empty).Replace('\\', '/');

        return await access.Channel!.ChannelId!.GetMarks(videoPath);
    }

    /// <summary>
    ///     播放Mark点
    /// </summary>
    /// <param name="mark"> 要播放的Mark点 </param>
    public async Task PlayMarkAsync(RecordMark mark)
    {
        // 清除播放器的标记点
        RecordPlaybackFfPlayerCleanPointsCmd.Execute();

        // 暂停播放
        await MdElement.Pause();

        // 解析标记点的时间
        var inpoint = TimeSpan.TryParse(mark.InPoint, out var inPointTime) ? inPointTime : TimeSpan.Zero;
        SliderIn = inpoint.TotalMilliseconds;
        var outpoint = TimeSpan.TryParse(mark.OutPoint, out var outPointTime) ? outPointTime : TimeSpan.Zero;
        SliderOut = outpoint.TotalMilliseconds;
        IsInSet = true;
        IsOutSet = true;

        // 设置播放文件相关的属性
        PlayName = mark.Name;
        PlayMode = PlayMode.SubRecordFile;
        PlayMark = mark;

        // 跳转到标记点的时间
        await MdElement.Seek(inpoint);
    }

    /// <summary>
    ///     清理播放器状态和资源
    /// </summary>
    private async Task CleanupPlayer()
    {
        // 关闭当前打开的播放器
        if (MdElement.IsOpen)
        {
            await MdElement.Close();
        }

        // 清空播放MdPanel所有播放器
        MdPanel?.Children.Clear();
        // 关闭播放列表模式下的所有播放文件
        if (PlayFiles?.Any() ?? false)
        {
            foreach (var it in PlayFiles)
            {
                it.MediaElement?.Dispose();
            }
        }

        // 清除播放器的标记点
        RecordPlaybackFfPlayerCleanPointsCmd.Execute();
    }

    #endregion

    #endregion

    #region 飞轮外设

    private int intervalTime = 5;

    /// <summary>
    ///     设置intervalTime的值
    /// </summary>
    public DelegateCommand<string> IntervalTimeCmd => new(interval =>
    {
        if (int.TryParse(interval, out var time))
        {
            intervalTime = time;
        }
    });

    /// <summary>
    ///     调整位置的通用方法。根据指定的时间间隔和方向来调整播放位置。
    /// </summary>
    /// <param name="interval"> 时间间隔 </param>
    /// <param name="isForward"> 如果为 true，则向前调整位置；如果为 false，则向后调整位置。 </param>
    private void AdjustPosition(TimeSpan interval, bool isForward)
    {
        // 根据方向选择合适的限制点（向前调整选择 PlayerOutpoint 或 PlaybackEndTime，向后调整选择 PlayerInpoint 或 PlaybackStartTime）。
        var limit = isForward ? PlayerOutpoint ?? MdElement.PlaybackEndTime : PlayerInpoint ?? MdElement.PlaybackStartTime;

        // 检查是否存在限制点。
        if (limit is null)
        {
            return;
        }

        // 根据方向执行相应的位置调整。
        if (isForward)
        {
            // 向前调整位置。
            if ((MdElement.Position + interval) <= limit)
            {
                MdElement.Position += interval;
            }
            else
                // 如果超过了限制点，则将位置设置为限制点。
            {
                MdElement.Position = limit.Value;
            }
        }
        else
        {
            // 向后调整位置。
            if ((MdElement.Position - interval) >= limit)
            {
                MdElement.Position -= interval;
            }
            else
                // 如果超过了限制点，则将位置设置为限制点。
            {
                MdElement.Position = limit.Value;
            }
        }
    }

    /// <summary>
    ///     向前调整指定秒数
    /// </summary>
    public DelegateCommand BackwardCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(intervalTime), false); });

    /// <summary>
    ///     向前整0.5秒
    /// </summary>
    public DelegateCommand Backward500msCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(0.5), false); });

    /// <summary>
    ///     向前调整1秒
    /// </summary>
    public DelegateCommand Backward1sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(1), false); });

    /// <summary>
    ///     向前调整2秒
    /// </summary>
    public DelegateCommand Backward2sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(2), false); });

    /// <summary>
    ///     向前调整5秒
    /// </summary>
    public DelegateCommand Backward5sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(5), false); });

    /// <summary>
    ///     向前调整10秒
    /// </summary>
    public DelegateCommand Backward10sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(10), false); });

    /// <summary>
    ///     向前调整15秒
    /// </summary>
    public DelegateCommand Backward15sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(15), false); });

    /// <summary>
    ///     向前调整20秒
    /// </summary>
    public DelegateCommand Backward20sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(20), false); });

    /// <summary>
    ///     向后调整指定秒数
    /// </summary>
    public DelegateCommand ForwardCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(intervalTime), true); });

    /// <summary>
    ///     向后调整0.5秒
    /// </summary>
    public DelegateCommand Forward500msCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(0.5), true); });

    /// <summary>
    ///     向后调整1秒
    /// </summary>
    public DelegateCommand Forward1sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(1), true); });

    /// <summary>
    ///     向后调整2秒
    /// </summary>
    public DelegateCommand Forward2sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(2), true); });

    /// <summary>
    ///     向后调整5秒
    /// </summary>
    public DelegateCommand Forward5sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(5), true); });

    /// <summary>
    ///     向后调整10秒
    /// </summary>
    public DelegateCommand Forward10sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(10), true); });

    /// <summary>
    ///     向后调整15秒
    /// </summary>
    public DelegateCommand Forward15sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(15), true); });

    /// <summary>
    ///     向后调整20秒
    /// </summary>
    public DelegateCommand Forward20sCmd => new(() => { AdjustPosition(TimeSpan.FromSeconds(20), true); });

    #endregion

    #region Command-播放器

    /// <summary>
    ///     上一帧
    /// </summary>
    public DelegateCommand RecordPlaybackFfPlayerStepBackwardCmd => new(async () =>
    {
        if (PlayerInpoint != null)
        {
            if ((MdElement.Position - MdElement.PositionStep) > PlayerInpoint.Value)
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
        if (!MdElement.IsSeekable)
        {
            return;
        }

        if (MdElement.IsPlaying)
        {
            await MdElement.Pause();
        }
        else
        {
            if (PlayerOutpoint is not null)
            {
                if (MdElement.Position < PlayerOutpoint)
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
        if (PlayerOutpoint is not null)
        {
            if ((MdElement.Position + MdElement.PositionStep) <=
                PlayerOutpoint)
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
        if (isInternal)
        {
            return;
        }

        // 非播单预览模式不处理
        if (!IsPlayListMode)
        {
            return;
        }

        // 播单条目为空不处理
        if (PlayFiles is not { Count: > 0 })
        {
            return;
        }

        if (CurrentPlayFile is null)
        {
            return;
        }

        if (PlayerInpoint == null)
        {
            return;
        }

        if (PlayerOutpoint == null)
        {
            return;
        }

        var playerOutpoint = PlayerOutpoint.Value;

        // 当前时间未超过出点不处理
        if (arg.NewValue < playerOutpoint.TotalMilliseconds)
        {
            return;
        }

        await MdElement.Pause();

        if (Equals(MdElement, MdActive))
        {
            return;
        }

        CurrentIndex++;
        if (CurrentPlayFile is null)
        {
            return;
        }

        if (CurrentIndex >= PlayFiles.Count)
        {
            // 当前条目为最后一条时，停止在出点
            await MdElement.Seek(playerOutpoint);
            return;
        }

        CurrentPlayFile = PlayFiles[CurrentIndex];

        isInternal = true;
        PlayerInpoint = CurrentPlayFile.RealInPoint!.Value;
        PlayerOutpoint = CurrentPlayFile.RealOutPoint!.Value;
        isInternal = false;
        PlayFiles.ForEach(it => { it.MediaElement!.Visibility = it == CurrentPlayFile ? Visibility.Visible : Visibility.Collapsed; });
        MdElement = CurrentPlayFile.MediaElement!;
        await MdElement.Play();
    });

    #endregion
}
