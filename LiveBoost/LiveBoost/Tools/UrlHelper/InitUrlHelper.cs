// 创建时间：2023-09-04-12:03
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     获取配置文件的方法。
    /// </summary>
    /// <returns> 包含成功标志和配置文件实例的元组。成功时标志为true，异常时标志为false。 </returns>
    public static async Task<(bool, string?)> GetConfig()
    {
        // 检查接口地址是否为空
        if ( string.IsNullOrEmpty(AppConfig.Instance.MamCommonIp) )
        {
            return ( false, "接口地址为空,获取配置信息异常" );
        }
        var url = $"{AppConfig.Instance.MamCommonIp}/a/rest/mam/config";

        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            froms = "liveBoostWindows",
            stamp,
            sign = Hash.Content(Md5Str + stamp)
        };

        // 调用通用的Post方法来执行获取配置文件操作，如果为空则返回失败标志和异常消息
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);

                // 将 jobj 中的数据反序列化到 AppConfig.Instance 对象中
                JsonConvert.PopulateObject(jobj["data"]?.ToString() ?? "{}", AppConfig.Instance);
                return ( true, string.Empty );
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                // 返回错误消息
                return ( false, $"{jobj["msg"]?.Value<string>()},获取配置信息异常" );
            },
            e => ( false, $"{e.InnerException?.Message ?? e.Message},获取配置信息异常" ));
    }
}
