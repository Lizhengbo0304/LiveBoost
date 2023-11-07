// 创建时间：2023-09-27-15:16
// 修改时间：2023-10-13-16:17

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Tools;

public class AddChannelProtocol2Visibility : IValueConverter
{
    /// <summary>
    ///     将协议名称和参数转换为 Visibility 属性，用于控制界面元素的可见性。
    /// </summary>
    /// <param name = "value" > 协议名称（字符串）。 </param>
    /// <param name = "targetType" > 目标类型。 </param>
    /// <param name = "parameter" > 参数（字符串），用于指定期望的协议和参数组合。 </param>
    /// <param name = "culture" > 区域信息。 </param>
    /// <returns> 根据协议名称和参数计算的 Visibility 属性。 </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 验证输入是否为字符串
        if ( value is not string protocol )
        {
            // 如果输入不是字符串，则根据参数返回 Visibility
            return parameter is "Normal" ? Visibility.Visible : Visibility.Collapsed;
        }

        // 根据协议名称和参数计算 Visibility
        return protocol switch
        {
            "SDI" => parameter is "SDI" ? Visibility.Visible : Visibility.Collapsed,
            "NDI" => parameter is "NDI" ? Visibility.Visible : Visibility.Collapsed,
            _ => parameter is "Normal" ? Visibility.Visible : Visibility.Collapsed
        };
    }

    /// <summary>
    ///     不支持从 Visibility 转换回协议名称和参数的操作，因此此方法返回 null。
    /// </summary>
    /// <param name = "value" > Visibility 值。 </param>
    /// <param name = "targetType" > 目标类型。 </param>
    /// <param name = "parameter" > 参数。 </param>
    /// <param name = "culture" > 区域信息。 </param>
    /// <returns> null。 </returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}
