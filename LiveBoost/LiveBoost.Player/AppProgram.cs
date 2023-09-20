﻿// 创建时间：2023-09-06-14:16
// 修改时间：2023-09-19-14:02

#region

global using MediaElement = Unosquare.FFME.MediaElement;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LiveBoost.Player.Controls;
using LiveBoost.Player.Tools;
using LiveBoost.Toolkit.Tools;
using Unosquare.FFME;

#endregion

namespace LiveBoost.Player;

public class AppProgram
{
    /// <summary>
    ///     程序入口
    /// </summary>
    [STAThread]
    private static void Main(string?[] args)
    {
        try
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherSynchronizationContext());

            VolumeHelper.SetVolume(0);

            Library.FFmpegDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dll");
            Library.EnableWpfMultiThreadedVideo = false;

            switch ( args.Length )
            {
                case 1 when args[0] is { } guid1:
                    Instance.Guid = guid1;
                    Instance.GuidBack = guid1 + "back";
                    var combinationPlayer = new CombinationPlayer();
                    try
                    {
                        ActionHelper.RunWithTimeout(IpcClientHelper.CombinationPlayer.SendPlayer, combinationPlayer.ViewToHwnd()).Wait();
                    }
                    catch ( Exception e )
                    {
                        e.LogError("发送播放器句柄异常");
                    }
                    break;
                case 2 when args[0] is { } guid2 && args[1] is not null:
                    Instance.Guid = guid2;
                    Instance.GuidBack = guid2 + "back";
                    var jggPlayer = new JggPlayer();
                    try
                    {
                        ActionHelper.RunWithTimeout(IpcClientHelper.JggPlayer.SendPlayer, jggPlayer.ViewToHwnd()).Wait();
                    }
                    catch ( Exception e )
                    {
                        e.LogError("发送播放器句柄异常");
                    }
                    break;
                default:
                    return;
            }
            Instance.App.Run();
        }
        catch ( Exception e )
        {
            MessageBox.Show(e.Message);
        }
    }
#region Global

    private static readonly Lazy<AppProgram> ProgramLazy = new(() => new AppProgram());

    private AppProgram()
    {
        App = new App
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };
        App.InitializeComponent();
    }
    public static AppProgram Instance => ProgramLazy.Value;

#endregion

#region Property

    /// <summary>
    ///     APP
    /// </summary>
    public readonly App App;

    public string? Guid;
    public string? GuidBack;

#endregion
}
