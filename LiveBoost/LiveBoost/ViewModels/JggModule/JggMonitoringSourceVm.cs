// 创建时间：2023-09-15-14:50
// 修改时间：2023-09-19-14:01

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm
{
#region Properties

    /// <summary>
    ///     收录源列表
    /// </summary>
    public List<RecordChannel>? RecordChannels { get; set; }

    /// <summary>
    /// 拖拽事件
    /// </summary>
    public JggDragHandler JggDragHandler { get; set; } = new();
    public JggDropHandler JggDropHandler { get; set; } = new();
#endregion
#region Event

    /// <summary>
    ///     初始化收录通道
    /// </summary>
    private async Task InitializeRecordChannelsAsync()
    {
        Debug.WriteLine(DateTime.Now);
        RecordChannels = await UrlHelper.GetShouluChannels().ConfigureAwait(false);
        Debug.WriteLine(DateTime.Now);
    }

#endregion
}
