// 创建时间：2023-09-18-15:13
// 修改时间：2023-10-13-16:17

#region

using HandyIpc;

#endregion

namespace LiveBoost.Toolkit.Data;

[IpcContract]
public interface IJggPlayer
{
    /// <summary>
    ///     设置通道名称
    /// </summary>
    void SetName(string accessName);
    /// <summary>
    ///     设置播放路径
    /// </summary>
    void SetPlayFile(string playFilePath);
    /// <summary>
    ///     停止播放
    /// </summary>
    void StopPlay();
}
