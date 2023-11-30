// 创建时间：2023-09-20-16:59
// 修改时间：2023-10-13-16:17

#region

using Quartz;
using Quartz.Impl;

#endregion

namespace LiveBoost.Tools;

// JobTool 类用于管理定时任务
public static class JobTool
{
    /// <summary>
    /// 检查QR状态的JobKey
    /// </summary>
    private static readonly JobKey RenewalJobKey = JobKey.Create("RenewalJob", "RenewalJobGroup");

    /// <summary>
    /// 检查QR状态的Job
    /// </summary>
    private static IJobDetail? _renewalJob;

    /// <summary>
    /// 启动定时任务
    /// </summary>
    public static async Task StartRenewalJob()
    {
        // 获取调度器
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        // 如果调度器未启动，启动它
        if (!scheduler.IsStarted)
        {
            await scheduler.Start();
        }

        // 如果 RenewalJob 存在，删除它
        if (_renewalJob is not null)
        {
            await scheduler.DeleteJob(RenewalJobKey);
        }

        // 创建 TokenRenewalJob 任务
        _renewalJob = JobBuilder.Create<TokenRenewalJob>()
            .WithIdentity("RenewalJob", "RenewalJobGroup").Build();

        // 创建触发器，每10分钟触发一次
        var trigger = TriggerBuilder.Create()
            .WithIdentity("TokenRenewalJobTrigger", "TokenRenewalJobTriggers").StartNow()
            .WithSimpleSchedule(
                x => x.WithIntervalInMinutes(10).RepeatForever()
                    .WithMisfireHandlingInstructionNextWithRemainingCount()).Build();

        // 将任务和触发器添加到调度器
        await scheduler.ScheduleJob(_renewalJob, trigger);
    }

    /// <summary>
    /// 停止定时任务
    /// </summary>
    public static async Task StopRenewalJob()
    {
        // 如果 RenewalJob 存在，删除它
        if (_renewalJob != null)
        {
            await (await StdSchedulerFactory.GetDefaultScheduler()).DeleteJob(RenewalJobKey);
        }
    }
}
