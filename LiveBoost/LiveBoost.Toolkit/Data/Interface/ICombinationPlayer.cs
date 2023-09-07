// 创建时间：2023-06-07-16:08
// 修改时间：2023-07-18-9:37

#region

using HandyIpc;

#endregion

namespace LiveBoost.Toolkit.Data;

[IpcContract]
public interface ICombinationPlayer
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
    ///     设置存储使用率
    /// </summary>
    void SetStorageUse(double rate);

    /// <summary>
    ///     设置流协议
    /// </summary>
    void SetStreamProtocal(string protocal);

    void StopPlay();
}
