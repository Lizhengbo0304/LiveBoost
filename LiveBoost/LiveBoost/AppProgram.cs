// 创建时间：2023-09-04-10:56
// 修改时间：2023-09-19-14:01

namespace LiveBoost;

public class AppProgram
{
    /// <summary>
    ///     程序入口
    /// </summary>
    [STAThread]
    private static void Main()
    {
        var startAsync = Instance.StartAsync();
        HandleStartExceptions(startAsync);

        Instance.App.Run();
    }
#region Event

    /// <summary>
    ///     开始程序（展示界面前的初始化）
    /// </summary>
    public async Task StartAsync()
    {
        // 检查当前进程是否是唯一的进程
        if ( Mutex.IsOnlyProcess() )
        {
            // 并行初始化App和配置文件
            await Task.WhenAll(InitAppAsync(), InitConfigAsync());
            // 初始化窗口
            InitWindow();
        }
        else
        {
            // 如果不是唯一的进程，关闭应用程序
            App.Shutdown();
        }
    }

// 初始化App
    private static Task InitAppAsync()
    {
        return Task.Run(() =>
        {
            // 等待ContainerLocator.Current初始化完成
            while ( ContainerLocator.Current == null )
            {
                // 添加延迟以避免CPU使用率过高
                Task.Delay(100);
            }
        });
    }

// 初始化配置文件
    private static async Task InitConfigAsync()
    {
        // 配置FlurlHttp的Json序列化器
        FlurlHttp.GlobalSettings.JsonSerializer = new NewtonsoftJsonSerializer();
        // 设置FFmpeg的目录
        Library.FFmpegDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dll");
        // 禁用WPF的多线程视频
        Library.EnableWpfMultiThreadedVideo = false;
        FlurlHttp.ConfigureClient(AppConfig.Instance.MamCommonIp, cli =>
            cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        // 阻止系统休眠
        SystemSleepHelper.PreventForCurrentThread(false);

        // 获取配置文件
        var result = await UrlHelper.GetConfig();
        AppConfig.Instance.IsInit = result.Item1;

        // 如果获取配置文件失败，显示警告消息
        if ( result.Item1 )
        {
            FlurlHttp.ConfigureClient(AppConfig.Instance.MamApiIp, cli =>
                cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        else
        {
            MessageBox.Warning(result.Item2, "获取配置文件");
        }
    }

    // 初始化窗口
    private void InitWindow()
    {
        // 在UI线程上操作，以避免线程冲突
        App.Dispatcher.Invoke(() =>
        {
            // 创建LoginWindow实例
            LoginWnd = ContainerLocator.Container.Resolve<LoginWindow>();
            // 设置主窗口
            App.MainWindow = LoginWnd;
            // 添加窗口关闭的事件处理器
            LoginWnd.Closed += AppMainWndOnClosed;
            // 显示窗口
            LoginWnd.Show();
        });
    }

    /// <summary>
    ///     窗口关闭事件
    /// </summary>
    private void AppMainWndOnClosed(object? sender, EventArgs e)
    {
        // 如果窗口关闭后主窗口为null，则关闭程序（如果最后关闭的为MainWnd，则执行Logout操作）
        switch ( sender )
        {
            case LoginWindow loginWnd:
            {
                loginWnd.Closed -= AppMainWndOnClosed;

                if ( App.MainWindow == null )
                {
                    App.Shutdown();
                }

                break;
            }
            case CombinationMainWindow mainWnd:
            {
                mainWnd.Closed -= AppMainWndOnClosed;
                if ( App.MainWindow == null )
                {
                    IsClosed = true;
                    GlobalEvent.Instance.GetEvent<CloseChildPlayerProcess>().Publish();
                    App.Shutdown();
                }
                else
                {
                    LoginWnd!.Closed += AppMainWndOnClosed;
                }
                break;
            }
        }
    }

    /// <summary>
    ///     初始化异常处理
    /// </summary>
    private static async void HandleStartExceptions(Task task)
    {
        try
        {
            await Task.Yield();
            await task;
        }
        catch ( Exception )
        {
            Instance.App.Shutdown();
        }
    }

#endregion
#region Global

    private static readonly Lazy<AppProgram> AppProgramLazy = new(() => new AppProgram());

    private AppProgram()
    {
        App = new App
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };
        App.InitializeComponent();
    }

    public static AppProgram Instance => AppProgramLazy.Value;

#endregion

#region Property

    /// <summary>
    ///     互斥锁
    /// </summary>
    private static readonly Mutex Mutex = new(true, "16231732-4B3A-4FA1-88B7-89E90C498E4E");

    /// <summary>
    ///     APP
    /// </summary>
    public readonly App App;

    public bool IsClosed { get; set; }
    /// <summary>
    ///     登录窗口
    /// </summary>
    public LoginWindow? LoginWnd { get; set; }
    /// <summary>
    ///     用户
    /// </summary>
    public LoginUser? LoginUser { get; set; }
    /// <summary>
    ///     主窗口
    /// </summary>
    public CombinationMainWindow? MainWnd { get; set; }

#endregion
#region Public - Event

    public void LoginInit()
    {
        if ( LoginWnd == null )
        {
            return;
        }

        MainWnd = ContainerLocator.Container.Resolve<CombinationMainWindow>();
        // 设置主窗口为RegionManager并展示主窗口，隐藏登录窗口
        App.MainWindow = MainWnd;
        MainWnd.Closed += AppMainWndOnClosed;
        LoginWnd?.Close();
        MainWnd.Show();
        LoginWnd = null;
        // 取消登录页的订阅
        if ( LoginWndInputBoxFocus.SubscriptionTokens.Any() )
        {
            LoginWndInputBoxFocus.SubscriptionTokens.ForEach(it => GlobalEvent.Instance.GetEvent<LoginWndInputBoxFocus>().Unsubscribe(it));
        }
    }

    public void LogoutInit() { }

#endregion
}
