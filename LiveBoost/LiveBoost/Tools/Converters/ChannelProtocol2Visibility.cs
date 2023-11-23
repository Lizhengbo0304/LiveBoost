// 创建时间：2023-10-10-9:50
// 修改时间：2023-10-13-16:17

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Tools;

public class ChannelProtocol2Visibility : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RecordChannel { } recordChannel)
        {
            return string.Equals(recordChannel.Protocol, "ndi", StringComparison.OrdinalIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}