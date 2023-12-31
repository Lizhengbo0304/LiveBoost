﻿// 创建时间：2023-09-20-9:14
// 修改时间：2023-10-13-16:16

#region

using LiveBoost.Controls;

#endregion

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm
{
    #region Command

    /// <summary>
    ///     收录通道翻页命令
    /// </summary>
    public DelegateCommand<FunctionEventArgs<int>> PageUpdatedCmd { get; }

    /// <summary>
    ///     展示九宫格界面
    /// </summary>
    public DelegateCommand ShowJggCmd { get; set; }

    #endregion

    #region Command-Event

    /// <summary>
    ///     收录通道翻页命令
    /// </summary>
    private void PageUpdatedExecute(FunctionEventArgs<int> info)
    {
        // 设置首页通道
        for (var i = 0; i < RecordItems.Count; i++)
        {
            var recordItem = RecordItems[i];
            recordItem.RecordAccess = TotalRecordAccesses?.Skip((info.Info - 1) * 4 + i).ToList()[0];
        }
    }

    /// <summary>
    ///     展示九宫格界面
    /// </summary>
    private void ShowJggExecute()
    {
        if (JggMainWindow is null)
        {
            JggMainWindow = new JggMainWindow(RecordChannels);
            JggMainWindow.Closed += OnJggMainWindowClosed;
            JggMainWindow.Show();
        }
        else
        {
            MessageBox.Warning("预览窗口已打开", "打开预览窗口");
        }
    }

    /// <summary>
    ///     预览窗口关闭事件
    /// </summary>
    private void OnJggMainWindowClosed(object sender, EventArgs e)
    {
        JggMainWindow!.Closed -= OnJggMainWindowClosed;
        GlobalEvent.Instance.GetEvent<CloseJggPlayerProcess>().Publish();
        JggMainWindow = null;
        GC.Collect();
    }

    #endregion

    #region 初始化收录通道

    private async Task InitializeRecordChannelsAsync()
    {
        RecordChannels = await UrlHelper.GetShouluChannels().ConfigureAwait(false);
    }

    /// <summary>
    ///     初始化收录通道
    /// </summary>
    private async Task InitializeRecordAccessesAsync()
    {
        // 获取收录服务器配置
        var recordConfigs = await UrlHelper.GetShouluAccess().ConfigureAwait(false);

        if (recordConfigs is null || !recordConfigs.Any())
        {
            // 处理收录服务器配置为空的情况
            HandleEmptyRecordConfigs();
        }
        else
        {
            // 处理收录服务器配置不为空的情况
            HandleNonEmptyRecordConfigs(recordConfigs);
        }

        PageUpdatedCmd.Execute(new FunctionEventArgs<int>(1));
    }

    // 处理配置为空的情况
    private void HandleEmptyRecordConfigs()
    {
        TotalPage = 1;
        // 初始化收录通道
        InitializeRecordAccesses(0);
    }

    /// <summary>
    ///     处理非空的记录配置
    /// </summary>
    /// <param name="configs"> 记录配置列表 </param>
    private void HandleNonEmptyRecordConfigs(List<RecordServerConfig> configs)
    {
        // 计算所有收录通道数量（包含SDI、IP）
        var total = CalculateTotalChannels(configs);
        TotalPage = (int)Math.Ceiling(total / 4.0);
        CurrentPage = 1;
        InitializeRecordAccesses(total);
    }

    /// <summary>
    ///     计算总的通道数
    /// </summary>
    /// <param name="configs"> 记录配置列表 </param>
    /// <returns> 总的通道数 </returns>
    private static int CalculateTotalChannels(IEnumerable<RecordServerConfig> configs)
    {
        return configs.Where(config => config.AccessConfig != null)
            .Sum(config => config.AccessConfig!.Sdi + config.AccessConfig!.Ip);
    }

    /// <summary>
    ///     初始化记录访问
    /// </summary>
    /// <param name="total"> 总的通道数 </param>
    private void InitializeRecordAccesses(int total)
    {
        TotalRecordAccesses = Enumerable.Range(0, TotalPage * 4)
            .Select(i => i >= total
                ? null
                : new RecordAccess
                {
                    AccessId = Guid.NewGuid().ToString("N"),
                    AccessName = $"R{i + 1}"
                }).ToList();
    }

    #endregion

    #region Properties

    /// <summary>
    ///     所有收录通道
    /// </summary>
    public List<RecordAccess?>? TotalRecordAccesses { get; set; }

    /// <summary>
    ///     当前页收录通道
    /// </summary>
    public List<CombinationItem> RecordItems { get; set; } = new()
    {
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem()
    };

    public List<RecordChannel>? RecordChannels { get; set; }

    /// <summary>
    ///     通道总页数
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    ///     当前页
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    ///     预览窗口
    /// </summary>
    private JggMainWindow? JggMainWindow { get; set; }

    #endregion
}