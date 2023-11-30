// 创建时间：2023-09-06-12:39
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Controls;

public class FlyListView : ListView
{
    #region 是否可以点击空白处

    /// <summary>
    ///     是否可以点击空白处
    /// </summary>
    public static readonly DependencyProperty CanClickItemOutProperty = DependencyProperty.Register(
        nameof(CanClickItemOut), typeof(bool), typeof(FlyListView), new PropertyMetadata(false, CanClickItemOutPropertyChanged));

    private static void CanClickItemOutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FlyListView flyListView)
        {
            return;
        }

        if (flyListView.CanClickItemOut)
        {
            flyListView.PreviewMouseDown += flyListView.OnPreviewMouseDown;
        }
        else
        {
            flyListView.PreviewMouseDown -= flyListView.OnPreviewMouseDown;
        }
    }

    /// <summary>
    ///     是否可以点击空白处
    /// </summary>
    public bool CanClickItemOut
    {
        get => (bool)GetValue(CanClickItemOutProperty);
        set => SetValue(CanClickItemOutProperty, value);
    }

    // 预览鼠标按下事件处理方法
    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        // 如果点击的不是 ListViewItem，则将 SelectedItem 设置为 null
        if (!HitTestUtilities.HitTest4Type<ListViewItem>(sender, e.GetPosition(this)))
        {
            SelectedItem = null;
        }
    }

    #endregion

    #region 鼠标滚动设置

    /// <summary>
    ///     设置是否为滚动视图的子控件
    /// </summary>
    public static readonly DependencyProperty IsScrollChildProperty = DependencyProperty.Register(
        nameof(IsScrollChild), typeof(bool), typeof(FlyListView),
        new PropertyMetadata(false, IsScrollChildPropertyChanged));

    // IsScrollChild 属性变更时的回调方法
    private static void IsScrollChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FlyListView flyListView)
        {
            return;
        }

        if (flyListView.IsScrollChild)
        {
            flyListView.PreviewMouseWheel += flyListView.ItemsHost_PreviewMouseWheel;
        }
        else
        {
            flyListView.PreviewMouseWheel -= flyListView.ItemsHost_PreviewMouseWheel;
        }
    }

    // ItemsHost_PreviewMouseWheel 事件处理方法
    private void ItemsHost_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = MouseWheelEvent, Source = sender
        };
        ((ListView)sender).RaiseEvent(eventArg);
    }

    /// <summary>
    ///     设置是否为滚动视图的子控件
    /// </summary>
    public bool IsScrollChild
    {
        get => (bool)GetValue(IsScrollChildProperty);
        set => SetValue(IsScrollChildProperty, value);
    }

    #endregion

    #region ItemsChanged

    // ItemsSourceChanged 事件
    private static readonly RoutedEvent ItemsSourceChangedEvent = EventManager.RegisterRoutedEvent("ItemsSourceChanged",
        RoutingStrategy.Direct,
        typeof(EventHandler<FlyItemsSourceChangedEventArgs>),
        typeof(ListView));

// ItemsSourceChanged 事件处理程序
    public event RoutedEventHandler ItemsSourceChangedEventHandler
    {
        add => AddHandler(ItemsSourceChangedEvent, value);

        remove => RemoveHandler(ItemsSourceChangedEvent, value);
    }

// ItemsSource 变更时的回调方法
    protected override void OnItemsSourceChanged(IEnumerable? oldValue, IEnumerable? newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);
        RaiseEvent(new FlyItemsSourceChangedEventArgs(ItemsSourceChangedEvent, this, oldValue, newValue));
    }

// ItemsChanged 事件
    private static readonly RoutedEvent ItemsChangedEvent = EventManager.RegisterRoutedEvent("ItemsChanged",
        RoutingStrategy.Direct,
        typeof(EventHandler<FlyItemsSourceChangedEventArgs>),
        typeof(ListView));

// ItemsChanged 事件处理程序
    public event RoutedEventHandler ItemsChangedEventHandler
    {
        add => AddHandler(ItemsChangedEvent, value);

        remove => RemoveHandler(ItemsChangedEvent, value);
    }

// Items 变更时的回调方法
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);
        RaiseEvent(new FlyItemsSourceChangedEventArgs(ItemsChangedEvent, this, e.OldItems, e.NewItems));
    }

    #endregion
}