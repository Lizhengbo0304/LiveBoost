// 创建时间：2023-09-05-14:44
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Tools;

/// <summary>
///     端口帮助类
/// </summary>
public static class PortHelper
{
    /// <summary>
    ///     检查指定的端口名是否存在
    /// </summary>
    /// <param name = "portName" > 要检查的端口名 </param>
    /// <returns> 如果端口名存在，则返回true，否则返回false </returns>
    public static bool IsPortNameExists(this string portName)
    {
        try
        {
            // 获取所有的端口名
            string[] portNames = SerialPort.GetPortNames();
            // 检查指定的端口名是否在所有的端口名中
            return portNames.Any(port => string.Equals(port, portName, StringComparison.OrdinalIgnoreCase));
        }
        catch ( Exception exception )
        {
            // 如果在查询端口时发生异常，记录异常并返回false
            exception.LogError("端口查询异常");
            return false;
        }
    }
}
