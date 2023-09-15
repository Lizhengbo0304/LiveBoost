// 创建时间：2023-09-07-10:47
// 修改时间：2023-09-15-15:41

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class TimeSpanFormatter : IMultiValueConverter
{
    // 默认的时间格式字符串
    private const string DefaultTimeFormat = "00:00:00:00";

    // 将多个输入值转换为格式化的时间字符串
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // 验证输入值的数量
        if ( values.Length != 2 && values.Length != 3 )
        {
            return DefaultTimeFormat;
        }

        TimeSpan start = TimeSpan.Zero, positionStep = TimeSpan.Zero;

        // 获取开始时间和时间步长
        if ( values.Length == 2 )
        {
            positionStep = values[1] as TimeSpan? ?? TimeSpan.Zero;
        }
        else if ( values.Length == 3 )
        {
            start = values[1] as TimeSpan? ?? TimeSpan.Zero;
            positionStep = values[2] as TimeSpan? ?? TimeSpan.Zero;
        }

        var duration = values[0] as TimeSpan?;

        // 如果持续时间为 null，则尝试从 Duration 类型的输入值中获取有效的 TimeSpan 值
        if ( duration == null )
        {
            var durationObject = values[0] as Duration? ?? default;
            if ( durationObject.HasTimeSpan )
            {
                duration = durationObject.TimeSpan;
            }
            else
            {
                return DefaultTimeFormat;
            }
        }

        var position = duration.Value;

        // 如果时间步长为零，则返回默认的时间格式字符串
        if ( positionStep == TimeSpan.Zero )
        {
            return DefaultTimeFormat;
        }

        position -= start;

        // 根据时间步长计算毫秒数的除数
        var millisecondsDivisor = positionStep.Milliseconds < 1 ? 40 : positionStep.Milliseconds;

        // 格式化时间字符串
        var formattedTime = $"{(int) position.TotalHours:00}:{position.Minutes:00}:{position.Seconds:00}:{position.Milliseconds / millisecondsDivisor:00}";
        return formattedTime;
    }

    // 将格式化的时间字符串转换回原始值（不支持反向转换）
    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
}
