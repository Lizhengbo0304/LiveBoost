namespace LiveBoost.Toolkit.Controls;

/// <summary>
///     表示一个带有标签的可拖动控件。
/// </summary>
public class TagThumb : Thumb, ITagThumb
{
    #region 事件处理

    /// <summary>
    ///     处理鼠标移动事件。
    /// </summary>
    /// <param name="e">The MouseEventArgs.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        // 在这里添加自定义的鼠标移动逻辑
        // 如果有需要处理的鼠标移动逻辑，请在此添加
    }

    /// <summary>
    ///     启动拖动操作。
    /// </summary>
    public void StartDrag()
    {
        // 设置拖动标志
        IsDragging = true;

        // 设置焦点，以确保拖动操作的正确处理
        Focus();

        // 捕获鼠标，开始拖动
        CaptureMouse();

        // 触发预览鼠标左键按下事件
        RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = PreviewMouseLeftButtonDownEvent,
            Source = this
        });
    }

    /// <summary>
    ///     取消拖动操作。
    /// </summary>
    public new void CancelDrag()
    {
        // 调用基类的取消拖动方法
        base.CancelDrag();

        // 触发预览鼠标左键释放事件
        RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = PreviewMouseLeftButtonUpEvent
        });
    }

    #endregion
}
