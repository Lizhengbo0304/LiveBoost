// 创建时间：2023-09-04-17:08
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     登录
    /// </summary>
    /// <param name = "loginName" > 用户名 </param>
    /// <param name = "password" > 密码 </param>
    /// <returns> 结果 </returns>
    public static async Task<bool> Login(string loginName, string? password)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/auth/client/login";

        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            froms = "liveBoostWindows",
            loginName,
            password = password!.AesEncrypt(Aes),
            stamp,
            sign = Hash.Content(Md5Str + stamp)
        };
        try
        {
            var result = await url.PostJsonAsync(para).ReceiveString().ConfigureAwait(false);
            var jobj = JObject.Parse(result);

            // Success = 0 : 请求失败
            if ( jobj["code"]?.Value<int>() == 200 )
            {
                if ( jobj["data"]?["entity"]?.ToString() is { } entity &&
                     jobj["data"]?["access_token"]?.ToString() is { } token )
                {
                    AppProgram.Instance.LoginUser = JsonConvert.DeserializeObject<LoginUser>(entity);
                    AppProgram.Instance.LoginUser.Token = token;
                    return true;
                }

                MessageBox.Warning("返回用户为空", "用户查询异常");
                return false;
            }

            MessageBox.Warning(jobj["msg"]?.Value<string>(), "登录失败");
            return false;
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "登录失败");
            e.LogUrlError("【用户】媒资用户认证接口\r\n登录失败");
            return false;
        }
    }
}
