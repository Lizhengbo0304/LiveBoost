// 创建时间：2023-09-04-14:36
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Toolkit.Controls;

public class ForegroundSwitchElement
{
    /// <summary>
    ///     选中字体颜色
    /// </summary>
    public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.RegisterAttached(
        "SelectedForeground", typeof(Brush), typeof(ForegroundSwitchElement), new PropertyMetadata(default(Brush)));

    /// <summary>
    ///     鼠标悬浮字体颜色
    /// </summary>
    public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached(
        "MouseOverForeground", typeof(Brush), typeof(ForegroundSwitchElement), new PropertyMetadata(default(Brush)));

    /// <summary>
    ///     获取选中字体颜色
    /// </summary>
    public static Brush GetSelectedForeground(DependencyObject element) => (Brush) element.GetValue(SelectedForegroundProperty);

    /// <summary>
    ///     设置选中字体颜色
    /// </summary>
    public static void SetSelectedForeground(DependencyObject element, Brush value)
    {
        element.SetValue(SelectedForegroundProperty, value);
    }

    /// <summary>
    ///     获取鼠标悬浮字体颜色
    /// </summary>
    public static Brush GetMouseOverForeground(DependencyObject element) => (Brush) element.GetValue(MouseOverForegroundProperty);

    /// <summary>
    ///     设置鼠标悬浮字体颜色
    /// </summary>
    public static void SetMouseOverForeground(DependencyObject element, Brush value)
    {
        element.SetValue(MouseOverForegroundProperty, value);
    }
}
