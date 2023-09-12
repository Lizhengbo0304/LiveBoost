// 创建时间：2023-09-05-14:39
// 修改时间：2023-09-11-17:55

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
    public DelegateCommand<FunctionEventArgs<int>> PageUpdatedCmd { get; set; }

#endregion
#region Command-Event

    /// <summary>
    ///     收录通道翻页命令
    /// </summary>
    private void PageUpdatedExecute(FunctionEventArgs<int> info)
    {
        // 设置首页通道
        for ( var i = 0; i < RecordItems.Count; i++ )
        {
            var recordItem = RecordItems[i];
            recordItem.RecordAccess = TotalRecordAccesses?.Skip(( info.Info - 1 ) * 4 + i).ToList()[0];
        }
    }
#endregion
    #region 初始化收录通道

        /// <summary>
        ///     初始化收录通道
        /// </summary>
        private async Task InitializeRecordAccessesAsync()
        {
            // 获取收录服务器配置
            var RecordConfigs = await UrlHelper.GetShouluAccess().ConfigureAwait(false);

            if ( RecordConfigs is null || !RecordConfigs.Any() )
            {
                // 处理收录服务器配置为空的情况
                HandleEmptyRecordConfigs();
            }
            else
            {
                // 处理收录服务器配置不为空的情况
                HandleNonEmptyRecordConfigs(RecordConfigs);
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
        /// <param name = "configs" > 记录配置列表 </param>
        private void HandleNonEmptyRecordConfigs(List<RecordServerConfig> configs)
        {
            // 计算所有收录通道数量（包含SDI、IP）
            var total = CalculateTotalChannels(configs);
            TotalPage = (int) Math.Ceiling(total / 4.0);
            CurrentPage = 1;
            InitializeRecordAccesses(total);
        }

        /// <summary>
        ///     计算总的通道数
        /// </summary>
        /// <param name = "configs" > 记录配置列表 </param>
        /// <returns> 总的通道数 </returns>
        private static int CalculateTotalChannels(IEnumerable<RecordServerConfig> configs)
        {
            return configs.Where(config => config.AccessConfig != null)
                .Sum(config => config.AccessConfig!.Sdi + config.AccessConfig!.Ip);
        }

        /// <summary>
        ///     初始化记录访问
        /// </summary>
        /// <param name = "total" > 总的通道数 </param>
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
    /// <summary>
    ///     通道总页数
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    ///     当前页
    /// </summary>
    public int CurrentPage { get; set; }

#endregion
}
