// 创建时间：2023-09-04-14:36
// 修改时间：2023-10-11-10:59

#region

#endregion

namespace LiveBoost.Toolkit.Controls;

public class IconElement
{
    /// <summary>
    ///     图标高度
    /// </summary>
    public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
        "Height",
        typeof(double),
        typeof(IconElement),
        new PropertyMetadata(double.NaN));

    /// <summary>
    ///     图标宽度
    /// </summary>
    public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
        "Width",
        typeof(double),
        typeof(IconElement),
        new PropertyMetadata(double.NaN));

    /// <summary>
    ///     图标Margin
    /// </summary>
    public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
        "Margin",
        typeof(Thickness),
        typeof(IconElement),
        new PropertyMetadata(default(Thickness)));

    /// <summary>
    ///     图标方向
    /// </summary>
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached(
        "Placement",
        typeof(PlacementType),
        typeof(IconElement),
        new PropertyMetadata(PlacementType.Top));

    /// <summary>
    ///     图标
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
        "Source",
        typeof(ImageSource),
        typeof(IconElement),
        new PropertyMetadata(default(ImageSource)));

    /// <summary>
    ///     图标显示模式
    /// </summary>
    public static readonly DependencyProperty StretchProperty = DependencyProperty.RegisterAttached(
        "Stretch",
        typeof(Stretch),
        typeof(IconElement),
        new PropertyMetadata(Stretch.Uniform));

    /// <summary>
    ///     获取图标显示模式
    /// </summary>
    public static Stretch GetStretch(DependencyObject element) => (Stretch) element.GetValue(StretchProperty);

    /// <summary>
    ///     设置图标显示模式
    /// </summary>
    public static void SetStretch(DependencyObject element, Stretch value)
    {
        element.SetValue(StretchProperty, value);
    }

    /// <summary>
    ///     获取图标高度
    /// </summary>
    public static double GetHeight(DependencyObject element) => (double) element.GetValue(HeightProperty);

    /// <summary>
    ///     获取图标宽度
    /// </summary>
    public static double GetWidth(DependencyObject element) => (double) element.GetValue(WidthProperty);

    /// <summary>
    ///     设置图标宽度
    /// </summary>
    public static void SetWidth(DependencyObject element, double value)
    {
        element.SetValue(WidthProperty, value);
    }

    /// <summary>
    ///     获取图标Margin
    /// </summary>
    public static Thickness GetMargin(DependencyObject element) => (Thickness) element.GetValue(MarginProperty);

    /// <summary>
    ///     设置图标Margin
    /// </summary>
    public static void SetMargin(DependencyObject element, Thickness value)
    {
        element.SetValue(MarginProperty, value);
    }

    /// <summary>
    ///     设置图标高度
    /// </summary>
    public static void SetHeight(DependencyObject element, double value)
    {
        element.SetValue(HeightProperty, value);
    }

    /// <summary>
    ///     获取图标方向
    /// </summary>
    public static PlacementType GetPlacement(DependencyObject element) => (PlacementType) element.GetValue(PlacementProperty);

    /// <summary>
    ///     设置图标方向
    /// </summary>
    public static void SetPlacement(DependencyObject element, PlacementType value)
    {
        element.SetValue(PlacementProperty, value);
    }

    /// <summary>
    ///     获取图标
    /// </summary>
    public static ImageSource GetSource(DependencyObject element) => (ImageSource) element.GetValue(SourceProperty);

    /// <summary>
    ///     设置图标
    /// </summary>
    public static void SetSource(DependencyObject element, ImageSource value)
    {
        element.SetValue(SourceProperty, value);
    }
}
