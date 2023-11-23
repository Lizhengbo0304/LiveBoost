// 创建时间：2023-10-10-18:05
// 修改时间：2023-10-13-16:17

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Tools;

public class Int2VisibilityReConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}