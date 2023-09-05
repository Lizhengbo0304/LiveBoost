// 创建时间：2023-09-04-15:52
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Data;

public class FlyItemsSourceChangedEventArgs : RoutedEventArgs
{
    public FlyItemsSourceChangedEventArgs(RoutedEvent routedEvent, object source, object? oldValue, object? newValue) :
        base(routedEvent, source)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
