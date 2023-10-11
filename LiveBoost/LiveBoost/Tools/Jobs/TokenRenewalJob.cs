// 创建时间：2023-09-20-16:59
// 修改时间：2023-10-11-10:59

#region

using Quartz;

#endregion

namespace LiveBoost.Tools;

// ReSharper disable once ClassNeverInstantiated.Global
// TokenRenewalJob 类用于执行特定的任务
public class TokenRenewalJob : IJob
{
    /// <inheritdoc />
    public Task Execute(IJobExecutionContext context)
    {
        return Task.Run(async () =>
        {
            // 检查登录用户的令牌是否为空
            if ( string.IsNullOrEmpty(AppProgram.Instance.LoginUser?.Token) )
            {
                return;
            }

            // 异步调用获取收录频道的方法
            await UrlHelper.GetShouluChannels().ConfigureAwait(false);
        });
    }
}
