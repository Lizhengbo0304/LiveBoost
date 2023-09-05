// 创建时间：2023-09-04-12:03
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     获取配置文件
    /// </summary>
    /// <returns> 配置文件实例 </returns>
    public static async Task<(bool, string?)> GetConfig()
    {
        // 检查接口地址是否为空
        if ( string.IsNullOrEmpty(AppConfig.Instance.MamCommonIp) )
        {
            return ( false, "接口地址为空,获取配置信息异常" );
        }

        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            froms = "liveBoostWindows",
            stamp,
            sign = Hash.Content(Md5Str + stamp)
        };

        try
        {
            var url = $"{AppConfig.Instance.MamCommonIp}/a/rest/mam/config";

            // 发送异步的 POST 请求并接收字符串响应
            var result = await url.WithTimeout(5).PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            // 解析响应字符串为 JObject
            var jobj = JObject.Parse(result);
            if ( jobj["success"]?.Value<int>() != 1 )
            {
                // 返回错误消息
                return ( false, $"{jobj["msg"]?.Value<string>()},获取配置信息异常" );
            }

            // 将 jobj 中的数据反序列化到 AppConfig.Instance 对象中
            JsonConvert.PopulateObject(jobj["data"]?.ToString() ?? "{}", AppConfig.Instance);
            return ( true, null );
        }
        catch ( Exception e )
        {
            // 返回异常消息
            return ( false, $"{e.InnerException?.Message ?? e.Message},获取配置信息异常" );
        }
    }
}
