// 创建时间：2023-06-06-15:50
// 修改时间：2023-07-18-9:37

#region

using System.Globalization;
using System.Windows.Data;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class TimeSpanToMillSecondsConverter : IValueConverter
{
    // 将值转换为目标类型
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            TimeSpan span => span.TotalMilliseconds, // 如果值是 TimeSpan 类型，则返回总毫秒数
            Duration duration => duration.HasTimeSpan ? duration.TimeSpan.TotalMilliseconds : 0d, // 如果值是 Duration 类型且包含有效的 TimeSpan，则返回 TimeSpan 的总毫秒数；否则返回 0
            _ => 0d // 其他情况返回 0
        };
    }

    // 将转换后的值反向转换回原始类型
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!double.TryParse(value.ToString(), out var doubleValue))
        {
            return 0d; // 如果值无法转换为 double 类型，则返回 0
        }

        try
        {
            var result = TimeSpan.FromTicks(System.Convert.ToInt64(TimeSpan.TicksPerMillisecond * doubleValue)); // 根据输入的值计算对应的 TimeSpan

            if (targetType == typeof(TimeSpan))
            {
                return result; // 如果目标类型是 TimeSpan，则返回计算得到的 TimeSpan
            }
            return targetType == typeof(Duration) ? new Duration(result) : Activator.CreateInstance(targetType); // 如果目标类型是 Duration，则创建一个新的 Duration 对象，并使用计算得到的 TimeSpan 作为参数；否则，使用 Activator.CreateInstance 创建目标类型的实例并返回
        }
        catch (OverflowException)
        {
            // 处理超出 TimeSpan 类型范围的异常情况
            return targetType == typeof(TimeSpan) ? TimeSpan.MaxValue : Duration.Automatic;
        }
    }
}
