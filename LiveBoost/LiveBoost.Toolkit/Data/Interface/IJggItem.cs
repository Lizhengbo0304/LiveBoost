// 创建时间：2023-09-18-15:55
// 修改时间：2023-10-13-16:17

#region

using HandyIpc;

#endregion

namespace LiveBoost.Toolkit.Data;

[IpcContract]
public interface IJggItem
{
    /// <summary>
    ///     发送播放器
    /// </summary>
    void SendPlayer(int handle);
    /// <summary>
    ///     删除频道
    /// </summary>
    void ClearChannel();
}
