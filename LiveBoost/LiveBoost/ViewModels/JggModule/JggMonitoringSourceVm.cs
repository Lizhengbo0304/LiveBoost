// 创建时间：2023-09-15-14:50
// 修改时间：2023-10-13-16:16

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm
{
#region Event

    /// <summary>
    ///     初始化收录通道
    /// </summary>
    private async void InitializeRecordChannelsAsync()
    {
        RecordChannels = await UrlHelper.GetShouluChannels().ConfigureAwait(false);
    }

#endregion
#region Properties

    /// <summary>
    ///     收录源列表
    /// </summary>
    public List<RecordChannel>? RecordChannels { get; set; }

    /// <summary>
    ///     拖拽事件
    /// </summary>
    public JggDragHandler JggDragHandler { get; set; }
    public JggDropHandler JggDropHandler { get; set; }

#endregion
}
