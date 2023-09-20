// 创建时间：2023-09-05-14:53
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     获取收录通道
    /// </summary>
    public static async Task<List<RecordServerConfig>?> GetShouluAccess()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/client/access/list";

        try
        {
            var result = await url
                .WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}")
                .GetJsonAsync<List<RecordServerConfig>>()
                .ConfigureAwait(false);

            return result ?? new List<RecordServerConfig>();
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取收录通道");
            e.LogUrlError("获取收录通道");
            return null;
        }
    }

    /// <summary>
    ///     获取收录频道
    /// </summary>
    /// <returns> 收录频道列表 </returns>
    public static async Task<List<RecordChannel>?> GetShouluChannels()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel/liststatus";

        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}")
                .GetStringAsync().ConfigureAwait(false);
            // 反序列化为收录频道列表
            var projects = JsonConvert.DeserializeObject<List<RecordChannel>>(result ?? "[]");
            return projects ?? new List<RecordChannel>();
        }
        catch ( Exception e )
        {
            // 异常处理
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取收录频道");
            e.LogUrlError("获取收录频道");
            return new List<RecordChannel>();
        }

    }
    /// <summary>
    ///     开始收录
    /// </summary>
    public static async Task<(string? filePath, string? taskId)> StartRecord(this RecordAccess access, string channelId)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/start";

        var para = new
        {
            taskId = Guid.NewGuid().ToString("N"),
            accessId = access.AccessId,
            channelId
        };
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                return ( jobj["data"]?.ToString(), para.taskId );
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "开始收录");
                return ( string.Empty, string.Empty );
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "开始收录");
                e.LogUrlError("开始收录");
                return ( string.Empty, string.Empty );
            });
    }
    /// <summary>
    ///     开始收录
    /// </summary>
    public static async Task<(string? filePath, string? taskId)> StartRecord(this string accessId, string channelId)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/start";

        var para = new
        {
            taskId = Guid.NewGuid().ToString("N"),
            accessId,
            channelId
        };
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                return ( jobj["data"]?.ToString(), para.taskId );
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "开始收录");
                return ( string.Empty, string.Empty );
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "开始收录");
                e.LogUrlError("开始收录");
                return ( string.Empty, string.Empty );
            });
    }
    /// <summary>
    ///     停止收录
    /// </summary>
    public static async Task<bool> StopRecord(this string taskId)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/task/stop";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        var para = new
        {
            taskId
        };
        return await url.Post(para,
            _ => true,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "停止收录");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "停止收录");
                e.LogUrlError("停止收录");
                return false;
            });
    }
}
