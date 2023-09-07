// 创建时间：2023-06-06-15:50
// 修改时间：2023-07-18-9:37

#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class TimeSpanToMillSecondsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            TimeSpan span => span.TotalMilliseconds,
            Duration duration => duration.HasTimeSpan ? duration.TimeSpan.TotalMilliseconds : 0d,
            _ => 0d
        };
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ( value is double == false )
        {
            return 0d;
        }
        var result = TimeSpan.FromTicks(System.Convert.ToInt64(TimeSpan.TicksPerMillisecond * (double) value));

        // Do the conversion from visibility to bool
        if ( targetType == typeof(TimeSpan) )
        {
            return result;
        }
        return targetType == typeof(Duration) ? new Duration(result) : Activator.CreateInstance(targetType);
    }
}
