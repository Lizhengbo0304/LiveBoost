// 创建时间：2023-06-07-16:14
// 修改时间：2023-07-18-9:37

#region

using System;
using HandyIpc;
using HandyIpc.NamedPipe;
using HandyIpc.Serializer.Json;
using LiveBoost.Toolkit.Data;

#endregion

namespace LiveBoost.Player.Tools;

public static class IpcClientHelper
{
    private static readonly Lazy<ICombinationItem> IpcSameLazy = new(() =>
    {
        var clientBuilder = new ContainerClientBuilder();
        clientBuilder.UseNamedPipe(AppProgram.Instance.Guid!).UseJsonSerializer();
        var client = clientBuilder.Build();
        // 从以上构建好的 client 中取用合同实例。
        var same = client.Resolve<ICombinationItem>();
        return same;
    });

    public static ICombinationItem ChildPlayer => IpcSameLazy.Value;
}
