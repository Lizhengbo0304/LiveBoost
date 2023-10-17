// 创建时间：2023-09-04-17:08
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     登录用户的方法。
    /// </summary>
    /// <param name = "loginName" > 用户名。 </param>
    /// <param name = "password" > 密码（可为null）。 </param>
    /// <returns> 登录是否成功。 </returns>
    public static async Task<bool> Login(string loginName, string? password)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/auth/client/login";

        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            froms = "liveBoostWindows",
            loginName,
            password = password!.AesEncrypt(Aes), // 使用AES加密密码
            stamp,
            sign = Hash.Content(Md5Str + stamp) // 计算签名
        };

        // 调用通用的Post方法来执行登录操作
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                if ( jobj["data"]?["entity"]?.ToString() is { } entity && jobj["data"]?["access_token"]?.ToString() is { } token )
                {
                    // 登录成功，将用户信息反序列化并存储
                    AppProgram.Instance.LoginUser = JsonConvert.DeserializeObject<LoginUser>(entity);
                    AppProgram.Instance.LoginUser.Token = token;
                    return true;
                }
                MessageBox.Warning("返回用户为空", "用户查询异常");
                return false;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                // 登录失败，显示错误消息
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "登录失败");
                return false;
            },
            e =>
            {
                // 处理登录时的异常情况
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "登录失败");
                e.LogUrlError("【用户】媒资用户认证接口\r\n登录失败");
                return false;
            });
    }
}
