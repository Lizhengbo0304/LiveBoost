// 创建时间：2023-09-05-14:38
// 修改时间：2023-09-06-9:03

using HandyControl.Controls;

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm : INotifyPropertyChanged
{
#region Ctor

    public CombinationMainWindowVm(MediaElement mediaElement,SimplePanel simple)
    {
        // 初始化串口
        InitializeSerialPort();

        // 初始化收录
        Task.Run(async () =>
        {
            await InitializeRecordAccessesAsync();
            await InitializePlayAccessesAsync();
        });
        // 播放器初始化
        MdActive = mediaElement;
        MdPanel = simple;
        MdElement = MdActive;
        MdElement.RenderingAudio += FfPlayOnRenderingAudio;
    }

#endregion
#region Properties

    /// <summary>
    ///     下位机
    /// </summary>
    public SerialPort? LiveBoostSerialPort { get; set; }

#endregion
#region Event

    /// <summary>
    ///     初始化串口
    /// </summary>
    private void InitializeSerialPort()
    {
        try
        {
            // 如果不支持串口小屏，直接返回
            if ( !AppConfig.Instance.IsPortSupported )
            {
                return;
            }
            // 如果串口名为空，直接返回
            if ( string.IsNullOrEmpty(AppConfig.Instance.SerialPort) )
            {
                return;
            }
            // 如果串口名不存在，直接返回
            if ( !AppConfig.Instance.SerialPort!.IsPortNameExists() )
            {
                return;
            }
            // 创建并配置串口
            LiveBoostSerialPort = new SerialPort(AppConfig.Instance.SerialPort, 9600, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                DtrEnable = true,
                RtsEnable = true
            };
            // 打开串口
            LiveBoostSerialPort.Open();
            // 检查串口是否真的打开了
            if ( !LiveBoostSerialPort.IsOpen )
            {
                MessageBox.Warning("无法打开串口", "串口初始化");
            }
        }
        catch ( Exception e )
        {
            MessageBox.Warning($"串口下位机初始化异常：{e.Message}", "初始化异常");
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
}
