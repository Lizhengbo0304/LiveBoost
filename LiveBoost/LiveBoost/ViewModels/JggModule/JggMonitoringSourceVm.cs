// 创建时间：2023-09-15-14:50
// 修改时间：2023-09-15-14:50

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm
{
#region Properties

    /// <summary>
    /// 收录源列表
    /// </summary>
    public List<RecordChannel>? RecordChannels { get; set; }

#endregion
#region Event

    /// <summary>
    ///     初始化收录通道
    /// </summary>
    private async Task InitializeRecordChannelsAsync()
    {
        RecordChannels = await UrlHelper.GetShouluChannels().ConfigureAwait(false);
    }

#endregion
}
