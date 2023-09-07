// 创建时间：2023-06-07-16:05
// 修改时间：2023-07-18-9:37

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

            if ( args.Length == 1 && args[0] is { } guid1 )
            {
                Instance.Guid = guid1;
                Instance.GuidBack = guid1 + "back";
                Instance.ChannelPlayer = new CombinationPlayer();
                IpcClientHelper.ChildPlayer.SendPlayer(Instance.ChannelPlayer.ViewToHwnd());
            }
            else
            {
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
    public CombinationPlayer? ChannelPlayer { get; set; }

    #endregion
}
