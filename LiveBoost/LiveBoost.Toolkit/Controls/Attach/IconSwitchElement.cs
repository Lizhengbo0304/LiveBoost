// 创建时间：2023-09-04-14:36
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Controls;

public class IconSwitchElement : IconElement
{
    /// <summary>
    ///     切换图标
    /// </summary>
    public static readonly DependencyProperty SourceSelectedProperty = DependencyProperty.RegisterAttached(
        "SourceSelected",
        typeof(ImageSource),
        typeof(IconSwitchElement),
        new PropertyMetadata(default(ImageSource)));

    /// <summary>
    ///     选中后的图标宽度
    /// </summary>
    public static readonly DependencyProperty WidthSelectedProperty = DependencyProperty.RegisterAttached(
        "WidthSelected", typeof(double), typeof(IconSwitchElement), new PropertyMetadata(default(double)));

    /// <summary>
    ///     选中后的图标高度
    /// </summary>
    public static readonly DependencyProperty HeightSelectedProperty = DependencyProperty.RegisterAttached(
        "HeightSelected", typeof(double), typeof(IconSwitchElement), new PropertyMetadata(default(double)));

    /// <summary>
    ///     获取切换图标
    /// </summary>
    public static ImageSource GetSourceSelected(DependencyObject element) => (ImageSource) element.GetValue(SourceSelectedProperty);

    /// <summary>
    ///     设置切换图标
    /// </summary>
    public static void SetSourceSelected(DependencyObject element, ImageSource value)
    {
        element.SetValue(SourceSelectedProperty, value);
    }

    /// <summary>
    ///     获取选中后的图标宽度
    /// </summary>
    public static double GetWidthSelected(DependencyObject element) => (double) element.GetValue(WidthSelectedProperty);

    /// <summary>
    ///     设置选中后的图标宽度
    /// </summary>
    public static void SetWidthSelected(DependencyObject element, double value)
    {
        element.SetValue(WidthSelectedProperty, value);
    }

    /// <summary>
    ///     获取选中后的图标高度
    /// </summary>
    public static double GetHeightSelected(DependencyObject element) => (double) element.GetValue(HeightSelectedProperty);

    /// <summary>
    ///     设置选中后的图标高度
    /// </summary>
    public static void SetHeightSelected(DependencyObject element, double value)
    {
        element.SetValue(HeightSelectedProperty, value);
    }
}
