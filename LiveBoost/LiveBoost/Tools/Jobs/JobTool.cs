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
    // 检查QR状态的JobKey
    private static readonly JobKey RenewalJobKey = JobKey.Create("RenewalJob", "RenewalJobGroup");

    // 检查QR状态的Job
    private static IJobDetail? _renewalJob;

    // 启动定时任务
    public static async Task StartRenewalJob()
    {
        var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
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

    // 停止定时任务
    public static async Task StopRenewalJob()
    {
        if (_renewalJob != null)
        {
            await (await StdSchedulerFactory.GetDefaultScheduler()).DeleteJob(RenewalJobKey);
        }
    }
}