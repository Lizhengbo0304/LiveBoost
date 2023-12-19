using System.Globalization;
using System.Windows.Data;

namespace LiveBoost.Tools;

public class Boolean2PlayPauseToolTipConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value != null && (bool)value ? "暂停" : "播放";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>null;
}
