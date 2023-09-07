// 创建时间：2023-06-06-15:50
// 修改时间：2023-07-18-9:37

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class TimeSpanFormatter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        const string DefaultTimeFormat = "00:00:00:00";

        if (values.Length != 2 && values.Length != 3)
            return DefaultTimeFormat;

        TimeSpan start = TimeSpan.Zero, positionStep = TimeSpan.Zero;

        if (values.Length == 2)
        {
            positionStep = values[1] as TimeSpan? ?? TimeSpan.Zero;
        }
        else if (values.Length == 3)
        {
            start = values[1] as TimeSpan? ?? TimeSpan.Zero;
            positionStep = values[2] as TimeSpan? ?? TimeSpan.Zero;
        }

        var duration = values[0] as TimeSpan?;
        if (duration == null)
        {
            var durationObject = values[0] as Duration? ?? default;
            if (durationObject.HasTimeSpan)
                duration = durationObject.TimeSpan;
            else
                return DefaultTimeFormat;
        }

        var position = duration.Value;

        if (positionStep == TimeSpan.Zero)
        {
            return DefaultTimeFormat;
        }

        position -= start;

        var millisecondsDivisor = positionStep.Milliseconds < 1 ? 40 : positionStep.Milliseconds;
        var formattedTime = $"{(int)position.TotalHours:00}:{position.Minutes:00}:{position.Seconds:00}:{position.Milliseconds / millisecondsDivisor:00}";
        return formattedTime;
    }

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
}
