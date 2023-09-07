// 创建时间：2023-09-04-11:23
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Toolkit.Tools;

public static class DateTimeHelper
{
    /// <summary>
    ///     时间精确到秒
    /// </summary>
    /// <param
    ///     name = "dt" >
    ///     需要精确到秒的日期对象
    /// </param>
    /// <returns> 日期结果 </returns>
    private static DateTime CutOffMillisecond(this DateTime dt) => new(dt.Ticks - dt.Ticks % TimeSpan.TicksPerSecond, dt.Kind);

    /// <summary>
    ///     生成时间戳
    /// </summary>
    /// <returns>
    ///     当前时间减去
    ///     1970-01-01
    ///     00.00.00
    ///     得到的秒数
    /// </returns>
    public static string GetTimeStamp() => GetTimeStamp(DateTime.Now);

    /// <summary>
    ///     生成时间戳
    /// </summary>
    /// <param
    ///     name = "dateTime" >
    ///     需要获取时间戳的对象
    /// </param>
    /// <returns>
    ///     当前时间减去
    ///     1970-01-01
    ///     00.00.00
    ///     得到的秒数
    /// </returns>
    public static string GetTimeStamp(this DateTime dateTime)
    {
        var dateTimeSecond = CutOffMillisecond(dateTime);
        var startTime1 = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
        var ts = dateTimeSecond - startTime1;
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
}
