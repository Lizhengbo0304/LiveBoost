namespace LiveBoost.Toolkit.Tools;

/// <summary>
///     提供一组用于处理双精度浮点数的数学辅助方法。
/// </summary>
public static class MathHelper
{
    /// <summary>
    ///     浮点数比较时的很小的常数值。
    /// </summary>
    internal const double DblEpsilon = 2.2204460492503131e-016;

    /// <summary>
    ///     检查两个浮点数是否在非常小的范围内相等。
    /// </summary>
    /// <param name="value1">第一个浮点数。</param>
    /// <param name="value2">第二个浮点数。</param>
    /// <returns>如果两个浮点数在非常小的范围内相等，则为 true；否则为 false。</returns>
    public static bool AreClose(double value1, double value2) =>
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        (value1 == value2) || IsVerySmall(value1 - value2);

    /// <summary>
    ///     在两个值之间执行线性插值。
    /// </summary>
    /// <param name="x">起始值。</param>
    /// <param name="y">结束值。</param>
    /// <param name="alpha">插值参数，通常在 [0, 1] 范围内。</param>
    /// <returns>插值结果。</returns>
    public static double Lerp(double x, double y, double alpha) => x * (1.0 - alpha) + y * alpha;

    /// <summary>
    ///     检查一个浮点数是否非常小。
    /// </summary>
    /// <param name="value">要检查的浮点数。</param>
    /// <returns>如果浮点数非常小，则为 true；否则为 false。</returns>
    public static bool IsVerySmall(double value) => Math.Abs(value) < 1E-06;

    /// <summary>
    ///     检查一个浮点数是否接近零。
    /// </summary>
    /// <param name="value">要检查的浮点数。</param>
    /// <returns>如果浮点数接近零，则为 true；否则为 false。</returns>
    public static bool IsZero(double value) => Math.Abs(value) < (10.0 * DblEpsilon);

    /// <summary>
    ///     检查一个浮点数是否有限。
    /// </summary>
    /// <param name="x">要检查的浮点数。</param>
    /// <returns>如果浮点数有限，则为 true；否则为 false。</returns>
    public static bool IsFiniteDouble(double x) => !double.IsInfinity(x) && !double.IsNaN(x);

    /// <summary>
    ///     使用指定的底数和指数计算浮点数的值。
    /// </summary>
    /// <param name="x">底数。</param>
    /// <param name="exp">指数。</param>
    /// <returns>计算得到的浮点数值。</returns>
    public static double DoubleFromMantissaAndExponent(double x, int exp) => x * Math.Pow(2.0, exp);

    /// <summary>
    ///     检查一个浮点数是否大于另一个浮点数。
    /// </summary>
    /// <param name="value1">第一个浮点数。</param>
    /// <param name="value2">第二个浮点数。</param>
    /// <returns>如果第一个浮点数大于第二个浮点数，则为 true；否则为 false。</returns>
    public static bool GreaterThan(double value1, double value2) => (value1 > value2) && !AreClose(value1, value2);

    /// <summary>
    ///     检查一个浮点数是否大于或非常接近于另一个浮点数。
    /// </summary>
    /// <param name="value1">第一个浮点数。</param>
    /// <param name="value2">第二个浮点数。</param>
    /// <returns>如果第一个浮点数大于或非常接近于第二个浮点数，则为 true；否则为 false。</returns>
    public static bool GreaterThanOrClose(double value1, double value2) => !(value1 <= value2) || AreClose(value1, value2);

    /// <summary>
    ///     计算直角三角形的斜边长度。
    /// </summary>
    /// <param name="x">第一个边的长度。</param>
    /// <param name="y">第二个边的长度。</param>
    /// <returns>斜边的长度。</returns>
    public static double Hypotenuse(double x, double y) => Math.Sqrt(x * x + y * y);

    /// <summary>
    ///     检查一个浮点数是否小于另一个浮点数。
    /// </summary>
    /// <param name="value1">第一个浮点数。</param>
    /// <param name="value2">第二个浮点数。</param>
    /// <returns>如果第一个浮点数小于第二个浮点数，则为 true；否则为 false。</returns>
    public static bool LessThan(double value1, double value2) => (value1 < value2) && !AreClose(value1, value2);

    /// <summary>
    ///     检查一个浮点数是否小于或非常接近于另一个浮点数。
    /// </summary>
    /// <param name="value1">第一个浮点数。</param>
    /// <param name="value2">第二个浮点数。</param>
    /// <returns>如果第一个浮点数小于或非常接近于第二个浮点数，则为 true；否则为 false。</returns>
    public static bool LessThanOrClose(double value1, double value2) => !(value1 >= value2) || AreClose(value1, value2);

    /// <summary>
    ///     确保一个值在指定的范围内，如果超出范围，则返回边界值。
    /// </summary>
    /// <param name="value">要确保的值。</param>
    /// <param name="min">范围的下限，如果为 null，则不限制下限。</param>
    /// <param name="max">范围的上限，如果为 null，则不限制上限。</param>
    /// <returns>在指定范围内的值。</returns>
    public static double EnsureRange(double value, double? min, double? max)
    {
        if (min.HasValue && (value < min.Value))
        {
            return min.Value;
        }

        if (max.HasValue && (value > max.Value))
        {
            return max.Value;
        }

        return value;
    }

    /// <summary>
    ///     安全地执行除法操作，避免除数为零的情况。
    /// </summary>
    /// <param name="lhs">被除数。</param>
    /// <param name="rhs">除数。</param>
    /// <param name="fallback">当除数为零时返回的默认值。</param>
    /// <returns>执行除法操作的结果。</returns>
    public static double SafeDivide(double lhs, double rhs, double fallback)
    {
        if (!IsVerySmall(rhs))
        {
            return lhs / rhs;
        }

        return fallback;
    }
}