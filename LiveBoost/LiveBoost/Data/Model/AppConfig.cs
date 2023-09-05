// 创建时间：2023-09-04-11:42
// 修改时间：2023-09-05-17:59

#region

using AutoProperties;

#endregion

namespace LiveBoost.Data;

public class AppConfig
{
#region SingleInstance

    private static readonly Lazy<AppConfig> AppConfigLazy = new(() => new AppConfig());

    private AppConfig() => _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "LiveBoost.config");

    public static AppConfig Instance => AppConfigLazy.Value;
    private readonly string _configPath;

#endregion

#region AutoPropertyEvent

    [GetInterceptor]
    // ReSharper disable once UnusedMember.Local
    private T? GetValue<T>(string name)
    {
        try
        {
            return (T?) Convert.ChangeType(_configPath.GetValueWithKey(name), typeof(T));
        }
        catch ( Exception )
        {
            return default;
        }
    }

    [SetInterceptor]
    // ReSharper disable once UnusedMember.Local
    private void SetValue(string name, object newValue)
    {
        _configPath.SetVaule(name, newValue.ToString());
    }

#endregion

#region ConfigProperty

    /// <summary>
    ///     媒资IP地址-qiao
    /// </summary>
    public string? MamCommonIp { get; set; }
    /// <summary>
    ///     串口名称
    /// </summary>
    public string? SerialPort { get; set; }
    /// <summary>
    ///     是否支持串口小屏
    /// </summary>
    public bool IsPortSupported { get; set; }
    /// <summary>
    ///     是否初始化完成
    /// </summary>
    [InterceptIgnore]
    public bool IsInit { get; set; }

#endregion

#region Property

    /// <summary>
    ///     收录存储地址
    /// </summary>
    [JsonProperty("windows.zyglq.shoulu.path")]
    [InterceptIgnore]
    public string? ShouluPath { get; set; }

    /// <summary>
    ///     微服务接口地址
    /// </summary>
    [JsonProperty("zyglq.micro.services.url")]
    [InterceptIgnore]
    public string? MamApiIp { get; set; }

    /// <summary>
    ///     收录WebSocket接口
    /// </summary>
    [JsonProperty("shoulu.websocket.url")]
    [InterceptIgnore]
    public string? ShouluWebSocket { get; set; }

#endregion
}
