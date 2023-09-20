// 创建时间：2023-09-06-14:08
// 修改时间：2023-09-19-14:01

#region

using HandyIpc;

#endregion

namespace LiveBoost.Toolkit.Data;

[IpcContract]
public interface ICombinationItem
{
    /// <summary>
    ///     发送播放器
    /// </summary>
    void SendPlayer(int handle);

    /// <summary>
    ///     删除频道
    /// </summary>
    void ClearChannel();
    /// <summary>
    ///     修改频道
    /// </summary>
    void ChangedChannel();

    /// <summary>
    ///     发送到主播放器
    /// </summary>
    void Send2MainPlayer();
}
