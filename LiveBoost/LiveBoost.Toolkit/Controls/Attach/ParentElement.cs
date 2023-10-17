// 创建时间：2023-09-04-14:36
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Controls;

public class ParentElement
{
    /// <summary>
    ///     父级Element
    /// </summary>
    public static readonly DependencyProperty ParentElementProperty = DependencyProperty.RegisterAttached(
        nameof(ParentElement),
        typeof(FrameworkElement),
        typeof(ParentElement),
        new PropertyMetadata(default(FrameworkElement)));

    /// <summary>
    ///     获取父级Element
    /// </summary>
    public static FrameworkElement? GetParentElement(DependencyObject element) => (FrameworkElement?) element.GetValue(ParentElementProperty);

    /// <summary>
    ///     设置父级Element
    /// </summary>
    public static void SetParentElement(DependencyObject element, FrameworkElement value)
    {
        element.SetValue(ParentElementProperty, value);
    }
}
