// 创建时间：2023-09-07-11:57
// 修改时间：2023-09-07-14:08

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
}
