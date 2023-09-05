// 创建时间：2023-09-05-14:53
// 修改时间：2023-09-05-17:59

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
}
