// 创建时间：2023-09-04-11:23
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Toolkit.Tools;

public static class DateTimeHelper
{
    /// <summary>
    ///     将日期对象的毫秒部分截断，精确到秒
    /// </summary>
    /// <param name = "dt" > 需要精确到秒的日期对象 </param>
    /// <returns> 截断毫秒后的日期对象 </returns>
    private static DateTime CutOffMillisecond(this DateTime dt) => new(dt.Ticks - dt.Ticks % TimeSpan.TicksPerSecond, dt.Kind);

    /// <summary>
    ///     生成当前时间的时间戳
    /// </summary>
    /// <returns> 当前时间减去 1970-01-01 00:00:00 得到的秒数 </returns>
    public static string GetTimeStamp() => GetTimeStamp(DateTime.Now);

    /// <summary>
    ///     生成指定日期时间对象的时间戳
    /// </summary>
    /// <param name = "dateTime" > 需要获取时间戳的日期时间对象 </param>
    /// <returns> 指定日期时间减去 1970-01-01 00:00:00 得到的秒数 </returns>
    public static string GetTimeStamp(this DateTime dateTime)
    {
        var dateTimeSecond = CutOffMillisecond(dateTime);

        // 判断是否早于 1970-01-01
        if ( dateTimeSecond <= new DateTime(1970, 1, 1) )
        {
            return "0";
        }

        var ts = dateTimeSecond - new DateTime(1970, 1, 1);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
}
