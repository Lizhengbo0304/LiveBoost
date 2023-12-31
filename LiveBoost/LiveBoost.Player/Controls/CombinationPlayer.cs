﻿// 创建时间：2023-09-06-14:18
// 修改时间：2023-10-13-16:17

#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;
using LiveBoost.Player.Tools;
using LiveBoost.Toolkit.Data;
using LiveBoost.Toolkit.Tools;
using Prism.Commands;
using Unosquare.FFME.Common;

// ReSharper disable AsyncVoidLambda

#endregion

namespace LiveBoost.Player.Controls;

[TemplatePart(Name = "Part_ffPlay", Type = typeof(MediaElement))]
public sealed class CombinationPlayer : Control, ICombinationPlayer, INotifyPropertyChanged
{
    #region Ctor

    static CombinationPlayer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CombinationPlayer),
            new FrameworkPropertyMetadata(typeof(CombinationPlayer)));
    }

    public CombinationPlayer()
    {
        Resources.MergedDictionaries.Add(
            Application.LoadComponent(new Uri("LiveBoost.Player;component/Themes/CombinationStyle.xaml",
                UriKind.Relative)) as ResourceDictionary);

        // 创建一个容器服务器 Builder。
        var serverBuilder = new ContainerServerBuilder();
        serverBuilder.UseNamedPipe(AppProgram.Instance.GuidBack!).UseJsonSerializer();

        serverBuilder.Register<ICombinationPlayer>(() => this);
        var server = serverBuilder.Build();

        // 别忘了启动服务器哦！
        server.Start();
    }

    #endregion

    #region Field

    // 播放器
    private MediaElement? _ffPlay;

    // 收录闪烁
    private DispatcherTimer? _dispatcherTimerBlink;

    #endregion

    #region Property

    // 通道名称
    public string? AccessName { get; set; }

    // 协议
    public string? Protocol { get; set; }

    // 视频尺寸
    public string? VideoSize { get; set; }

    /// <summary>
    ///     开始时间
    /// </summary>
    public TimeSpan? StartTime { get; set; }

    /// <summary>
    ///     截止时间
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    // 打入出点图标
    public string PlayPauseImage { get; set; } = "pack://application:,,,/LiveBoost.ToolKit;component/Images/Recording1.png";

    #endregion

    #region Command

    // 切换频道
    public DelegateCommand ChangedChannelCmd => new(async () =>
    {
        try
        {
            await ActionHelper.RunWithTimeout(IpcClientHelper.CombinationPlayer.ChangedChannel);
        }
        catch (Exception e)
        {
            e.LogError("切换频道异常");
        }
    });

    // 删除频道
    public DelegateCommand DeleteChannelCmd => new(async () =>
    {
        try
        {
            await ActionHelper.RunWithTimeout(IpcClientHelper.CombinationPlayer.ClearChannel);
        }
        catch (Exception e)
        {
            e.LogError("清除频道异常");
        }
    });

    public DelegateCommand SetTimeCmd => new(async () =>
    {
        try
        {
            if (StartTime is null)
            {
                StartTime = _ffPlay!.Position;
                PlayPauseImage = "pack://application:,,,/LiveBoost.ToolKit;component/Images/Recording.png";
                await ActionHelper.RunWithTimeout(IpcClientHelper.CombinationPlayer.SetStartTime, StartTime.Value);
            }
            else
            {
                EndTime = _ffPlay!.Position;
                PlayPauseImage = "pack://application:,,,/LiveBoost.ToolKit;component/Images/Recording1.png";
                await ActionHelper.RunWithTimeout(IpcClientHelper.CombinationPlayer.SetStopTime, EndTime.Value);
                StartTime = null;
                EndTime = null;
            }
        }
        catch (Exception e)
        {
            e.LogError("设置开始、结束时间异常");
        }
    });

    #endregion

    #region Event

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _ffPlay = GetTemplateChild("Part_ffPlay") as MediaElement;
        if (_ffPlay != null)
        {
            _ffPlay.RenderingAudio += FfPlayOnRenderingAudio;
        }
    }

    #region RenderingAudioField

    public double DrawVuMeterLeftValue { get; set; }
    public double DrawVuMeterRightValue { get; set; }
    private short[]? _drawVuMeterLeftSamples;
    private short[]? _drawVuMeterRightSamples;
    private readonly object _drawVuMeterRmsLock = new();

    #endregion

    private void FfPlayOnRenderingAudio(object sender, RenderingAudioEventArgs e)
    {
        // 分析即将渲染的音频数据
        // 如果不存在音频，则不需要分析
        if (e.EngineState.HasAudio == false)
        {
            return;
        }

        // 把音频数据分为左右声道
        if ((_drawVuMeterLeftSamples == null) || (_drawVuMeterLeftSamples.Length != e.SamplesPerChannel))
        {
            _drawVuMeterLeftSamples = new short[e.SamplesPerChannel];
        }

        if ((_drawVuMeterRightSamples == null) || (_drawVuMeterRightSamples.Length != e.SamplesPerChannel))
        {
            _drawVuMeterRightSamples = new short[e.SamplesPerChannel];
        }

        var bufferData = e.GetBufferData();

        // 使用 Buffer.BlockCopy 替代循环和 BitConverter.ToInt16
        Buffer.BlockCopy(bufferData, 0, _drawVuMeterLeftSamples, 0, e.SamplesPerChannel * sizeof(short)); // 复制左声道数据
        Buffer.BlockCopy(bufferData, e.SamplesPerChannel * sizeof(short), _drawVuMeterRightSamples, 0, e.SamplesPerChannel * sizeof(short)); // 复制右声道数据

        // 使用 Parallel.Invoke 同时计算左右声道的 RMS 值
        double leftValue = 0.0, rightValue = 0.0;
        lock (_drawVuMeterRmsLock)
        {
            Parallel.Invoke(
                () => { leftValue = VolumeHelper.CalculateRms(_drawVuMeterLeftSamples); },
                () => { rightValue = VolumeHelper.CalculateRms(_drawVuMeterRightSamples); }
            );
            DrawVuMeterLeftValue = leftValue;
            DrawVuMeterRightValue = rightValue;
        }
    }


    protected override async void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        try
        {
            await ActionHelper.RunWithTimeout(IpcClientHelper.CombinationPlayer.Send2MainPlayer);
        }
        catch (Exception exception)
        {
            exception.LogError("双击播放窗口异常");
        }
    }

    #endregion

    #region ICombinationPlayer

    public void SetName(string accessName)
    {
        // 设置通道名称
        AccessName = accessName;
    }

    public async void SetPlayFile(string playFilePath)
    {
        _dispatcherTimerBlink?.Stop();
        StartBlinking();

        if (_ffPlay is null)
        {
            return;
        }

        await OpenFileAsync(playFilePath);
        SetVideoSize();
        await AdjustRemainingTimeAsync();
        await _ffPlay.Play();
    }

    private void StartBlinking()
    {
        _dispatcherTimerBlink = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _dispatcherTimerBlink.Tick += (_, _) => { };
        _dispatcherTimerBlink.Start();
    }

    private async Task OpenFileAsync(string playFilePath)
    {
        var openFile = false;
        while (!openFile)
        {
            if (!File.Exists(playFilePath))
            {
                $"{AccessName}收录文件：{playFilePath}不存在".LogFileInfo();
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                continue;
            }

            $"{AccessName}收录文件：{playFilePath}正在打开".LogFileInfo();
            openFile = await _ffPlay!.Open(new Uri(playFilePath));
            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }
    }

    private void SetVideoSize()
    {
        VideoSize = $"{_ffPlay!.NaturalVideoWidth}*{_ffPlay.NaturalVideoHeight}";
    }

    private async Task AdjustRemainingTimeAsync()
    {
        if (_ffPlay!.RemainingDuration is not null)
        {
            if (_ffPlay.RemainingDuration.Value.TotalSeconds > 20)
            {
                await _ffPlay.Seek(_ffPlay.RemainingDuration.Value.Add(TimeSpan.FromSeconds(-20)));
            }
            else
            {
                var delay = 20 - _ffPlay.RemainingDuration.Value.TotalSeconds;
                if (delay > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
            }
        }
    }


    public void SetStorageUse(double rate)
    {
    }

    public void SetStreamProtocal(string protocal)
    {
        Protocol = protocal.ToUpper() + " IN";
    }

    public async void StopPlay()
    {
        if (_ffPlay is not null)
        {
            await _ffPlay.Close();
        }

        VideoSize = string.Empty;
        _dispatcherTimerBlink?.Stop();
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}