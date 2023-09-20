// 创建时间：2023-09-18-15:13
// 修改时间：2023-09-19-14:01

using HandyIpc;

namespace LiveBoost.Toolkit.Data;
[IpcContract]
public interface IJggPlayer
{
    /// <summary>
    ///     设置播放路径
    /// </summary>
    void SetPlayFile(string playFilePath);
    /// <summary>
    ///     停止播放
    /// </summary>
    void StopPlay();
}
