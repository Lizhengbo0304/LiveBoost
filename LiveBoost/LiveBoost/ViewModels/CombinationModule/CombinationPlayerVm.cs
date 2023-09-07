// 创建时间：2023-09-07-11:35
// 修改时间：2023-09-07-11:35

using HandyControl.Controls;

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm
{
#region Properties

    // 当前播放器
    public MediaElement MdElement { get; set; }
    public MediaElement MdActive { get; set; }
    // 播放器Panel
    public SimplePanel? MdPanel { get; set; }
    // 播放名称
    public string? PlayName { get; set; }
#endregion
#region Event

    private void FfPlayOnRenderingAudio(object sender, RenderingAudioEventArgs e)
    {

    }

#endregion
}
