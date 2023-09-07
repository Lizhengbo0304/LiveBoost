﻿// 创建时间：2023-09-04-11:28
// 修改时间：2023-09-06-9:03

#region

using System.Net.Http;
using Flurl.Http.Configuration;

#endregion

namespace LiveBoost.Toolkit.Data;

public class UntrustedCertClientFactory : DefaultHttpClientFactory
{
    public override HttpMessageHandler CreateMessageHandler()
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    }
}
