// 创建时间：2023-09-21-14:24
// 修改时间：2023-10-13-16:17

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Tools;

public class LayoutIndex2ItemsPanelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 确保输入值是整数类型
        if (value is not int index)
        {
            return null;
        }

        return index switch
        {
            0 =>
                // 查找并返回对应的 ItemsPanelTemplate 资源
                Application.Current.FindResource("twoItemsPanelStyle") as ItemsPanelTemplate,
            1 => Application.Current.FindResource("threeItemsPanelStyle") as ItemsPanelTemplate,
            2 => Application.Current.FindResource("fourItemsPanelStyle") as ItemsPanelTemplate,
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}