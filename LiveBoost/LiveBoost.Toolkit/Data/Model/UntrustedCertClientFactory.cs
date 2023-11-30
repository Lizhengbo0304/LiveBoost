// 创建时间：2023-09-04-11:28
// 修改时间：2023-10-13-16:17

#region

using System.Net.Http;
using Flurl.Http.Configuration;

#endregion

namespace LiveBoost.Toolkit.Data;

public class UntrustedCertClientFactory : DefaultHttpClientFactory
{
    // 重写CreateMessageHandler方法，创建自定义的HttpMessageHandler
    public override HttpMessageHandler CreateMessageHandler()
    {
        return new HttpClientHandler
        {
            // 设置ServerCertificateCustomValidationCallback回调函数，始终返回true，即信任所有的服务器证书
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    }
}