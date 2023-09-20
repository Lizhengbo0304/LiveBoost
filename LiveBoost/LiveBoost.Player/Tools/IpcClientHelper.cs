// 创建时间：2023-09-06-14:18
// 修改时间：2023-09-19-14:02

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
    private static readonly Lazy<ICombinationItem> IpcCombinationLazy = new(() =>
    {
        var clientBuilder = new ContainerClientBuilder();
        clientBuilder.UseNamedPipe(AppProgram.Instance.Guid!).UseJsonSerializer();
        var client = clientBuilder.Build();
        // 从以上构建好的 client 中取用合同实例。
        var same = client.Resolve<ICombinationItem>();
        return same;
    });

    public static ICombinationItem CombinationPlayer => IpcCombinationLazy.Value;

    private static readonly Lazy<IJggItem> IpcJggLazy = new(() =>
    {
        var clientBuilder = new ContainerClientBuilder();
        clientBuilder.UseNamedPipe(AppProgram.Instance.Guid!).UseJsonSerializer();
        var client = clientBuilder.Build();
        // 从以上构建好的 client 中取用合同实例。
        var same = client.Resolve<IJggItem>();
        return same;
    });

    public static IJggItem JggPlayer => IpcJggLazy.Value;
}
