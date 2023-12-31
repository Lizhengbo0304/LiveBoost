﻿// 创建时间：2023-09-07-10:21
// 修改时间：2023-10-13-16:16

#region

using LiveBoost.ToolKit.Data;

#endregion

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm
{
    #region Properties

    /// <summary>
    ///     推流通道列表
    /// </summary>
    public List<PushAccess>? PlayAccesses { get; set; }

    /// <summary>
    ///     当前播单
    /// </summary>
    public PushAccess? CurrentPlayList { get; set; }

    #endregion

    #region Event

    // 初始化 PlayAccesses
    private async Task InitializePlayAccessesAsync()
    {
        var playTemplates = await 10.GetShouluTemplates().ConfigureAwait(false);
        PlayAccesses = await UrlHelper.GetPushAccess().ConfigureAwait(false);

        if (PlayAccesses!.Any())
        {
            // 处理 PlayAccesses 不为空的情况
            PlayAccesses!.First()!.IsSelected = true;
            PlayAccesses!.ForEach(async it =>
            {
                if (playTemplates.Find(template => template.Id == it.AccessId) is { } recordTemplate)
                {
                    it.RecordFiles = recordTemplate.RecordFiles;
                }
                else
                {
                    // 创建播单
                    if (!await UrlHelper.NewPlayList(it.AccessId!, it.Title!, it.Mode))
                    {
                        MessageBox.Warning($"{it.Title}创建播单异常，无法存储", "推流通道");
                    }
                }

                it.RecordFiles.CollectionChanged += it.RecordFilesOnCollectionChanged;
            });
        }
    }

    #region 播单变动方法

    /// <summary>
    ///     当播单内容发生变化时触发的事件处理方法
    /// </summary>
    private async void RecordFilesOnCollectionChanged(PushAccess access)
    {
        // 如果当前播单为空或正在编辑中，则忽略变化
        if (access.IsEdit)
        {
            return;
        }

        // 根据当前播单的状态进行不同的处理
        if (!access.Status)
        {
            // 推流时： 处理推流中的通道文件列表变动
            await HandleStreamingPushAccessAsync(access);
        }
        else
        {
            // 空闲时：处理空闲的推流通道文件变动
            await HandleIdlePushAccessAsync(access);
        }
    }

    /// <summary>
    ///     处理推流中的通道文件列表变动
    /// </summary>
    private async Task HandleStreamingPushAccessAsync(PushAccess access)
    {
        // 设置当前播单为编辑状态
        access.IsEdit = true;

        // 向推流后台推送最新播单，如果编辑成功则初始化播放器并同步播单信息，否则恢复备份的记录文件
        if (await access.EditPushPlayList())
        {
            await InitializePlayerAsync(access);
            await SyncPlayListAsync(access);
        }
        else
        {
            access.RecordFiles.BackupRecover();
        }

        // 取消编辑状态
        access.IsEdit = false;
    }

    /// <summary>
    ///     处理空闲的推流通道文件变动
    /// </summary>
    private async Task HandleIdlePushAccessAsync(PushAccess access)
    {
        await InitializePlayerAsync(access);
        await SyncPlayListAsync(access);
    }

    /// <summary>
    ///     初始化播放器
    /// </summary>
    private async Task InitializePlayerAsync(PushAccess access)
    {
        // 如果当前播放模式为播单且播放文件列表与当前播单的记录文件列表相同，则并行初始化每个 MediaElement
        if ((PlayMode == PlayMode.PlayList) && (PlayFiles == access.RecordFiles))
        {
            await Task.WhenAll(PlayFiles.Select(InitializeMediaElementAsync));
        }
    }

// 初始化单个 MediaElement
    private async Task InitializeMediaElementAsync(RecordFile it)
    {
        // 如果该记录文件已经有关联的 MediaElement，则直接返回
        if (it.MediaElement is not null)
        {
            return;
        }

        // 使用依赖注入获取一个 MediaElement 实例
        it.MediaElement = CreateMediaElement();

        // 打开记录文件并设置实际的入点和出点
        await it.MediaElement.Open(new Uri(it.FullPath));

        // 如果记录文件的实际入点和出点为空，则使用 MediaElement 的播放开始时间和结束时间作为实际入点和出点
        if (it.RealInPoint is null || it.RealOutPoint is null)
        {
            it.RealInPoint = it.MediaElement.PlaybackStartTime;
            it.RealOutPoint = it.MediaElement.PlaybackEndTime;
        }

        // 将 MediaElement 添加到播放器面板中
        MdPanel!.Children.Add(it.MediaElement);
    }

// 创建 MediaElement 实例
    private MediaElement CreateMediaElement() =>
        new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = Brushes.Black,
            LoadedBehavior = MediaPlaybackState.Manual,
            IsHitTestVisible = false,
            Visibility = Visibility.Collapsed
        };

// 同步播单信息
    private async Task SyncPlayListAsync(PushAccess access)
    {
        // 创建一个 RecordTemplate 对象，并使用对象初始化器设置其属性值
        var recordTemplate = new RecordTemplate
        {
            Id = access.AccessId,
            Type = "10",
            Mode = access.Mode,
            RecordFiles = access.RecordFiles,
            Title = access.Title
        };

        // 编辑播单信息
        await recordTemplate.EditPlayList().ConfigureAwait(false);
    }

    #endregion

    #endregion

    #region Command-播单

    // 播单推流
    public DelegateCommand PushPlayList => new DelegateCommand(async () =>
    {
        if (CurrentPlayList is null)
        {
            MessageBox.Warning("请选择要推流的通道", "推流");
            return;
        }

        if (string.IsNullOrEmpty(CurrentPlayList.AccessId))
        {
            MessageBox.Warning("播单Id为空，请重新选择播单", "推流");
            return;
        }

        if (!CurrentPlayList.Status && !CurrentPlayList.IsPause)
        {
            MessageBox.Warning("播单正在推流，请重新选择播单", "推流");
            return;
        }

        if (CurrentPlayList.IsPause && !CurrentPlayList.Status)
        {
            if (!await CurrentPlayList.PlayListPush("restore"))
            {
                return;
            }

            CurrentPlayList.Status = false;
            CurrentPlayList.IsPause = false;
        }
        else
        {
            if (await CurrentPlayList.PlayListPush("start"))
            {
                CurrentPlayList.Status = false;
            }
        }
    }, () => CurrentPlayList is { Status: true } or { IsPause: true }).ObservesProperty(() => CurrentPlayList!.Status).ObservesProperty(() => CurrentPlayList!.IsPause);

    // 播单推流暂停
    public DelegateCommand PausePlayList => new DelegateCommand(async () =>
    {
        if (MessageBox.Ask("是否确定暂停推流", "推流") is not MessageBoxResult.OK)
        {
            return;
        }

        if (CurrentPlayList is null)
        {
            MessageBox.Warning("请选择要推流的通道", "暂停推流");
            return;
        }

        if (string.IsNullOrEmpty(CurrentPlayList.AccessId))
        {
            MessageBox.Warning("播单Id为空，请重新选择播单", "暂停推流");
            return;
        }

        if (await CurrentPlayList.PlayListPush("pause"))
        {
            CurrentPlayList.IsPause = true;
        }
    }, () => CurrentPlayList is { Status: false, IsPause: false }).ObservesProperty(() => CurrentPlayList!.Status).ObservesProperty(() => CurrentPlayList!.IsPause);

    // 播单推流停止
    public DelegateCommand StopPlayList => new DelegateCommand(async () =>
    {
        if (MessageBox.Ask("是否确定停止推流", "推流") is not MessageBoxResult.OK)
        {
            return;
        }

        if (CurrentPlayList is null)
        {
            MessageBox.Warning("请选择要推流的通道", "暂停推流");
            return;
        }

        if (string.IsNullOrEmpty(CurrentPlayList.AccessId))
        {
            MessageBox.Warning("播单Id为空，请重新选择播单", "暂停推流");
            return;
        }

        if (!await CurrentPlayList.PlayListPush("stop"))
        {
            return;
        }

        CurrentPlayList.Status = true;
        CurrentPlayList.IsPause = false;
        CurrentPlayList.RecordFiles.ForEach(it =>
        {
            it.IsPlaying = false;
            it.Progress = 0;
        });
    }, () => CurrentPlayList is { Status: false }).ObservesProperty(() => CurrentPlayList!.Status);

    // 播单推流预览
    public DelegateCommand PreviewPlayList => new DelegateCommand(async () => { await PlayPushAccessAsync(); }, () => CurrentPlayList is not null).ObservesProperty(() => CurrentPlayList);

    // 播单推流预览
    public DelegateCommand OutPlayList => new DelegateCommand(async () =>
    {
        if (CurrentPlayList is null)
        {
            MessageBox.Warning("请选择要导出的播单", "导出播单");
            return;
        }

        if (string.IsNullOrEmpty(CurrentPlayList.AccessId))
        {
            MessageBox.Warning("播单Id为空，请重新选择播单", "导出播单");
            return;
        }

        (string fileName, RecordTemplate outTemplate) outResult = CombinationPlayListOut.ShowOut();
        await CurrentPlayList!.AccessId!.OutPlayList2Video(outResult.fileName, outResult.outTemplate.Id!)
            .ConfigureAwait(false);
    }, () => CurrentPlayList is not null).ObservesProperty(() => CurrentPlayList);

    // 播单推流预览
    public DelegateCommand OutXml => new DelegateCommand(async () =>
    {
        if (CurrentPlayList is null)
        {
            MessageBox.Warning("请选择要导出的播单", "导出播单");
            return;
        }

        if (string.IsNullOrEmpty(CurrentPlayList.AccessId))
        {
            MessageBox.Warning("播单Id为空，请重新选择播单", "导出播单");
            return;
        }

        var outResult = CombinationPlayListOutXml.ShowOut();
        if (string.IsNullOrEmpty(outResult))
        {
            return;
        }

        await CurrentPlayList!.AccessId!.OutPlayList2Xml(outResult!)
            .ConfigureAwait(false);
    }, () => CurrentPlayList is not null).ObservesProperty(() => CurrentPlayList);

    // 播单推流预览
    public DelegateCommand OutAAF => new DelegateCommand(() =>
    {
        if (CurrentPlayList is null)
        {
            MessageBox.Warning("请选择要导出的播单", "导出播单");
            return;
        }

        if (string.IsNullOrEmpty(CurrentPlayList.AccessId))
        {
            MessageBox.Warning("播单Id为空，请重新选择播单", "导出播单");
        }
    }, () => CurrentPlayList is not null).ObservesProperty(() => CurrentPlayList);

    #endregion
}