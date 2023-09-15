// 创建时间：2023-09-07-11:57
// 修改时间：2023-09-07-16:22

#region

using LiveBoost.ToolKit.Data;

#endregion

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     查询模板列表
    /// </summary>
    /// <param name = "templateType" >
    ///     1=任务模板，2=导出模板，3=推流模板，10-播单
    /// </param>
    /// <returns> </returns>
    public static async Task<List<RecordTemplate>> GetShouluTemplates(this int templateType)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/list/{templateType}";

        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .GetStringAsync().ConfigureAwait(false);

            var projects = JsonConvert.DeserializeObject<List<RecordTemplate>>(result ?? "[]");
            return projects ?? new List<RecordTemplate>();
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "查询模板列表");
            e.LogUrlError("查询模板列表");
            return new List<RecordTemplate>();
        }
    }
    /// <summary>
    ///     获取推流通道
    /// </summary>
    public static async Task<List<PushAccess>?> GetPushAccess()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/access/liststatus";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }

        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}")
                .GetStringAsync().ConfigureAwait(false);

            var projects = JsonConvert.DeserializeObject<List<PushAccess>>(result ?? "[]");
            return projects ?? new List<PushAccess>();
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取推流通道");
            e.LogUrlError("获取推流通道");
            return new List<PushAccess>();
        }
    }
    /// <summary>
    ///     新建播单
    /// </summary>
    public static async Task<bool> NewPlayList(string id, string title, int mode)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        var para = new
        {
            id, title, type = 10, mode, info = "[]"
        };
        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .PostJsonAsync(para).ReceiveString().ConfigureAwait(false);
            var jobj = JObject.Parse(result);

            // Success = 0 : 请求失败
            if ( jobj["code"]?.Value<int>() == 200 )
            {
                return true;
            }

            MessageBox.Warning(jobj["msg"]?.Value<string>(), "新增播单");
            return false;
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "新增播单");
            e.LogUrlError("新增播单");
            return false;
        }
    }
    /// <summary>
    ///     播单推流，start,stop,pause,restore
    /// </summary>
    public static async Task<bool> PlayListPush(this PushAccess pushAccess, string operate)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/access/push/{operate}";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        var para = new
        {
            accessId = pushAccess.AccessId,
            playList = pushAccess.RecordFiles.ToJson()
        };
        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jobj = JObject.Parse(result);

            // Success = 0 : 请求失败
            if ( jobj["code"]?.Value<int>() == 200 )
            {
                return true;
            }

            MessageBox.Warning(jobj["msg"]?.Value<string>(), "播单操作");
            return false;
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "播单操作");
            e.LogUrlError("播单操作");
            return false;
        }
    }
    /// <summary>
    ///     导出视频
    /// </summary>
    public static async Task OutPlayList2Video(this string playListId, string exportName, string templateId)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/export";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        var para = new
        {
            exportName, playListId, templateId
        };
        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jobj = JObject.Parse(result);
            if ( jobj["code"]?.Value<int>() == 200 )
            {
                MessageBox.Success("导出视频成功", "导出视频");
            }
            else
            {
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "导出视频");
            }
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "导出视频");
            e.LogUrlError("导出视频");
        }
    }

    /// <summary>
    ///     导出Xml
    /// </summary>
    public static async Task OutPlayList2Xml(this string playListId, string exportName)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/exportxml";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        var para = new
        {
            exportName, playListId
        };
        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jobj = JObject.Parse(result);
            if ( jobj["code"]?.Value<int>() == 200 )
            {
                MessageBox.Success("导出Xml成功", "导出Xml");
            }
            else
            {
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "导出Xml");
            }
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "导出Xml");
            e.LogUrlError("导出Xml");
        }
    }

    /// <summary>
    ///     客户端播单推流播单修改
    /// </summary>
    public static async Task<bool> EditPushPlayList(this PushAccess pushAccess)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/access/push/modify";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
        var para = new
        {
            accessId = pushAccess.AccessId,
            playList = pushAccess.RecordFiles.ToJson()
        };
        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

                var jobj = JObject.Parse(result);

                // Success = 0 : 请求失败
                if ( jobj["code"]?.Value<int>() == 200 )
                {
                    return true;
                }

                MessageBox.Warning(jobj["msg"]?.Value<string>(), "编辑推流播单");
                return false;
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "编辑推流播单");
            e.LogUrlError("编辑推流播单");
            return false;
        }
    }

    /// <summary>
    ///     编辑播单
    /// </summary>
    public static async Task<bool> EditPlayList(this RecordTemplate recordTemplate)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template";
        if ( url.StartsWith("https") || url.StartsWith("http") )
        {
            FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }

        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .PutJsonAsync(recordTemplate).ReceiveString().ConfigureAwait(false);

                var jobj = JObject.Parse(result);

                // Success = 0 : 请求失败
                if ( jobj["code"]?.Value<int>() == 200 )
                {
                    return true;
                }

                MessageBox.Warning(jobj["msg"]?.Value<string>(), "编辑播单");
                return false;
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "编辑播单");
            e.LogUrlError("编辑播单");
            return false;
        }
    }
}
