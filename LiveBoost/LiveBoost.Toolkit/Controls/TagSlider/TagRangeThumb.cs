namespace LiveBoost.Toolkit.Controls;

/// <summary>
///     表示一个可以拖动的控件，用于表示一个范围的开始或结束。
/// </summary>
public class TagRangeThumb : Thumb, ITagThumb
{
    /// <summary>
    ///     表示这个控件是否表示范围的开始。
    /// </summary>
    public static readonly DependencyProperty IsStartProperty = DependencyProperty.Register(
        nameof(IsStart), typeof(bool), typeof(TagRangeThumb), new PropertyMetadata(default(bool)));

    /// <summary>
    ///     获取或设置这个控件是否表示范围的开始。
    /// </summary>
    public bool IsStart
    {
        get => (bool)GetValue(IsStartProperty);
        set => SetValue(IsStartProperty, value);
    }

    /// <summary>
    ///     开始拖动操作，获取焦点，捕获鼠标，并触发一个鼠标左键按下的事件。
    /// </summary>
    public void StartDrag()
    {
        // 设置IsDragging属性为true，表示开始拖动操作
        IsDragging = true;

        // 获取焦点，这样用户可以通过键盘来操作这个控件
        Focus();

        // 捕获鼠标，这样只有这个控件可以接收到鼠标事件
        CaptureMouse();

        // 创建一个新的MouseButtonEventArgs对象，表示一次鼠标左键按下的事件
        // 设置这个事件的源为这个控件
        // 触发这个事件，任何注册到这个事件的事件处理程序都会被调用
        RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = PreviewMouseLeftButtonDownEvent,
            Source = this
        });
    }

    /// <summary>
    ///     在鼠标移动时执行的操作。
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        // 这个方法可能是空的，因为它是在鼠标移动时执行的操作，
        // 但在这个类中，我们不需要在鼠标移动时执行任何操作。
    }

    /// <summary>
    ///     取消当前的拖动操作，并模拟一次鼠标左键释放的事件。
    /// </summary>
    public new void CancelDrag()
    {
        // 调用基类的CancelDrag方法，取消当前的拖动操作
        base.CancelDrag();

        // 创建一个新的MouseButtonEventArgs对象，表示一次鼠标左键释放的事件
        // 触发这个事件，任何注册到这个事件的事件处理程序都会被调用
        RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = PreviewMouseLeftButtonUpEvent
        });
    }
}