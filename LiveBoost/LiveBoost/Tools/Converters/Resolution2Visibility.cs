// 创建时间：2023-10-10-14:05
// 修改时间：2023-10-13-16:17

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Tools;

public class Resolution2Visibility : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is Resolution {DisplayName: "其他"} ? Visibility.Visible : Visibility.Collapsed;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
