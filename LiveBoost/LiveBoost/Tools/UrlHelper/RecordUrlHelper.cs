// 创建时间：2023-09-05-14:53
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     获取收录通道
    /// </summary>
    public static async Task<List<RecordServerConfig>> GetShouluAccess()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/client/access/list";
        return await url.Get(response => JsonConvert.DeserializeObject<List<RecordServerConfig>>(response ?? "[]") ?? new List<RecordServerConfig>(),
            _ => new List<RecordServerConfig>(),
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取收录通道");
                e.LogUrlError("获取收录通道");
                return new List<RecordServerConfig>();
            }) ?? new List<RecordServerConfig>();
    }

    /// <summary>
    ///     获取收录服务器列表
    /// </summary>
    public static async Task<List<RecordServer>> GetShouluServers()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/client/liststatus";
        return await url.Get(response => JsonConvert.DeserializeObject<List<RecordServer>>(response ?? "[]") ?? new List<RecordServer>(),
            _ => new List<RecordServer>(),
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取收录服务列表");
                e.LogUrlError("获取收录服务列表");
                return new List<RecordServer>();
            }) ?? new List<RecordServer>();
    }

    /// <summary>
    ///     获取收录服务器列表
    /// </summary>
    public static async Task<List<RecordServer>> GetShouluServersAll()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/client/listall";
        return await url.Get(response => JsonConvert.DeserializeObject<List<RecordServer>>(response ?? "[]") ?? new List<RecordServer>(),
            _ => new List<RecordServer>(),
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取收录服务列表");
                e.LogUrlError("获取收录服务列表");
                return new List<RecordServer>();
            }) ?? new List<RecordServer>();
    }

    /// <summary>
    ///     获取收录频道
    /// </summary>
    /// <returns> 收录频道列表 </returns>
    public static async Task<List<RecordChannel>> GetShouluChannels()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel/liststatus";
        return await url.Get(response => JsonConvert.DeserializeObject<List<RecordChannel>>(response ?? "[]") ?? new List<RecordChannel>(),
            _ => new List<RecordChannel>(),
            e =>
            {
                // 异常处理
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取收录频道");
                e.LogUrlError("获取收录频道");
                return new List<RecordChannel>();
            }) ?? new List<RecordChannel>();
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
    ///     新增频道
    /// </summary>
    public static async Task<bool> AddChannel(this object para)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel";

        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "新增频道");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "新增频道");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "新增频道");
                e.LogUrlError("新增频道");
                return false;
            });
    }
    /// <summary>
    ///     修改频道
    /// </summary>
    public static async Task<bool> EditChannel(this object para)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel";

        return await url.Put(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "修改频道");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "修改频道");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "修改频道");
                e.LogUrlError("修改频道");
                return false;
            });
    }
    /// <summary>
    ///     修改频道状态
    /// </summary>
    public static async Task<bool> EditChannelStatus(this RecordChannel channel)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel/status";
        var para = new
        {
            channelId = channel.ChannelId,
            status = channel.Status
        };
        return await url.Put(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "修改频道状态");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "修改频道状态");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "修改频道状态");
                e.LogUrlError("修改频道");
                return false;
            });
    }
    /// <summary>
    ///     删除频道
    /// </summary>
    public static async Task<bool> DeleteChannel(this RecordChannel channel)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel/{channel.ChannelId}";

        return await url.Delete(
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "删除频道");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "删除频道");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "删除频道");
                e.LogUrlError("删除频道");
                return false;
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
