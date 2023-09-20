// 创建时间：2023-09-18-14:36
// 修改时间：2023-09-19-15:42

#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;
using LiveBoost.Player.Tools;
using LiveBoost.Toolkit.Data;
using LiveBoost.Toolkit.Tools;
using Prism.Commands;
using Unosquare.FFME.Common;

#endregion

namespace LiveBoost.Player.Controls;

[TemplatePart(Name = "Part_ffPlay", Type = typeof(MediaElement))]
public sealed class JggPlayer : Control, INotifyPropertyChanged, IJggPlayer
{
#region Field

    // 播放器
    private MediaElement? _ffPlay;

#endregion
#region Property

    /// <summary>
    ///     频道名称
    /// </summary>
    public string? ChannelName { get; set; }

#endregion
#region Command

    /// <summary>
    ///     清除频道信息
    /// </summary>
    public DelegateCommand ClearChannelCmd { get; }

#endregion
#region Ctor

    static JggPlayer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(JggPlayer),
            new FrameworkPropertyMetadata(typeof(JggPlayer)));
    }
    public JggPlayer()
    {
        Resources.MergedDictionaries.Add(
            Application.LoadComponent(new Uri("LiveBoost.Player;component/Themes/JggStyle.xaml",
                UriKind.Relative)) as ResourceDictionary);

        // 创建一个容器服务器 Builder。
        var serverBuilder = new ContainerServerBuilder();
        serverBuilder.UseNamedPipe(AppProgram.Instance.GuidBack!).UseJsonSerializer();

        serverBuilder.Register<IJggPlayer>(() => this);
        var server = serverBuilder.Build();

        // 别忘了启动服务器哦！
        server.Start();

        ClearChannelCmd = new DelegateCommand(ClearChannelExecute);
    }

#endregion
#region Event

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _ffPlay = GetTemplateChild("Part_ffPlay") as MediaElement;
        if ( _ffPlay != null )
        {
            _ffPlay.RenderingAudio += FfPlayOnRenderingAudio;
        }
    }

    /// <summary>
    /// 清除频道信息
    /// </summary>
    public async void ClearChannelExecute()
    {
        await ActionHelper.RunWithTimeout(IpcClientHelper.JggPlayer.ClearChannel);
    }

#region RenderingAudioField

    public double DrawVuMeterLeftValue { get; set; }
    public double DrawVuMeterRightValue { get; set; }
    private short[]? drawVuMeterLeftSamples;
    private short[]? drawVuMeterRightSamples;
    private readonly object drawVuMeterRmsLock = new();

#endregion
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
#region IJggPlayerEvent

    public void SetName(string accessName)
    {
        ChannelName = accessName;
    }

    public async void SetPlayFile(string playFilePath)
    {
        if ( _ffPlay is null )
        {
            return;
        }

        await OpenFileAsync(playFilePath);
        await AdjustRemainingTimeAsync();
        await _ffPlay.Play();
    }
    private async Task OpenFileAsync(string playFilePath)
    {
        var openFile = false;
        while ( !openFile )
        {
            if ( !File.Exists(playFilePath) )
            {
                $"{ChannelName}预览文件：{playFilePath}不存在".LogFileInfo();
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                continue;
            }
            $"{ChannelName}预览文件：{playFilePath}正在打开".LogFileInfo();
            openFile = await _ffPlay!.Open(new Uri(playFilePath));
            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }
    }

    private async Task AdjustRemainingTimeAsync()
    {
        if ( _ffPlay!.RemainingDuration is not null )
        {
            if ( _ffPlay.RemainingDuration.Value.TotalSeconds > 20 )
            {
                await _ffPlay.Seek(_ffPlay.RemainingDuration.Value.Add(TimeSpan.FromSeconds(-20)));
            }
            else
            {
                var delay = 20 - _ffPlay.RemainingDuration.Value.TotalSeconds;
                if ( delay > 0 )
                {
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
            }
        }
    }
    public async void StopPlay()
    {
        if ( _ffPlay is not null )
        {
            await _ffPlay.Close();
        }
    }

#endregion
}
