using HandyControl.Tools;

namespace LiveBoost.Toolkit.Controls;

/// <summary>
///     表示TagSlider的基类。
/// </summary>
public class TagRangeBase : Control
{
    #region Constructors

    protected TagRangeBase()
    {
    }

    #endregion

    #region 依赖属性

    /// <summary>
    ///     定义一个名为Minimum的依赖属性，表示TagRangeBase的最小值。
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(double),
        typeof(TagRangeBase),
        new PropertyMetadata(0.0d, OnMinimumChanged),
        ValidateHelper.IsInRangeOfDouble);

    /// <summary>
    ///     当Minimum属性的值发生改变时，此回调函数会被调用。
    /// </summary>
    /// <param name="d">发生属性值改变的TagRangeBase元素。</param>
    /// <param name="e">包含属性值改变的详细信息。</param>
    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 将DependencyObject对象转换为TagRangeBase对象
        var ctrl = (TagRangeBase)d;

        // 调用CoerceValue方法，强制重新计算MaximumProperty和ValueProperty的值
        ctrl.CoerceValue(MaximumProperty);
        ctrl.CoerceValue(ValueProperty);

        // 调用OnMinimumChanged方法，通知子类Minimum属性的值已经改变
        ctrl.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary>
    ///     定义一个虚方法，当Minimum属性的值发生改变时，子类可以重写此方法来实现自定义的行为。
    /// </summary>
    /// <param name="oldMinimum">属性值改变前的旧值。</param>
    /// <param name="newMinimum">属性值改变后的新值。</param>
    protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
        // 子类可以在此方法中添加自定义的行为
    }

    /// <summary>
    ///     定义Minimum属性的CLR属性访问器。
    /// </summary>
    public double Minimum
    {
        // 获取MinimumProperty的值
        get => (double)GetValue(MinimumProperty);

        // 设置MinimumProperty的值
        set => SetValue(MinimumProperty, value);
    }


    /// <summary>
    ///     Maximum 的依赖属性
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(double),
        typeof(TagRangeBase),
        new PropertyMetadata(10.0, OnMaximumChanged, CoerceMaximum),
        ValidateHelper.IsInRangeOfDouble);

    /// <summary>
    ///     强制重新计算MaximumProperty的值。
    /// </summary>
    /// <param name="d">发生属性值改变的TagRangeBase元素。</param>
    /// <param name="basevalue">待强制计算的值。</param>
    /// <returns>强制计算后的值。</returns>
    private static object CoerceMaximum(DependencyObject d, object basevalue)
    {
        var ctrl = (TagRangeBase)d;
        var min = ctrl.Minimum;
        // 如果新的值小于Minimum属性的值，则将新的值设为Minimum属性的值
        return (double)basevalue < min ? min : basevalue;
    }

    /// <summary>
    ///     当Maximum属性的值发生改变时，此回调函数会被调用。
    /// </summary>
    /// <param name="d">发生属性值改变的TagRangeBase元素。</param>
    /// <param name="e">包含属性值改变的详细信息。</param>
    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (TagRangeBase)d;
        // 调用CoerceValue方法，强制重新计算ValueProperty的值
        ctrl.CoerceValue(ValueProperty);
        // 调用OnMaximumChanged方法，通知子类Maximum属性的值已经改变
        ctrl.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary>
    ///     定义一个虚方法，当Maximum属性的值发生改变时，子类可以重写此方法来实现自定义的行为。
    /// </summary>
    /// <param name="oldMaximum">属性值改变前的旧值。</param>
    /// <param name="newMaximum">属性值改变后的新值。</param>
    protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
        // 子类可以在此方法中添加自定义的行为
    }

    /// <summary>
    ///     定义Maximum属性的CLR属性访问器。
    /// </summary>
    public double Maximum
    {
        // 获取MaximumProperty的值
        get => (double)GetValue(MaximumProperty);

        // 设置MaximumProperty的值
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    ///     定义一个名为Value的依赖属性，表示TagRangeBase的当前值。
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(TagRangeBase),
        new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnValueChanged, ConstrainToRange),
        ValidateHelper.IsInRangeOfDouble);

    /// <summary>
    ///     强制重新计算ValueProperty的值。
    /// </summary>
    /// <param name="d">发生属性值改变的TagRangeBase元素。</param>
    /// <param name="basevalue">待强制计算的值。</param>
    /// <returns>强制计算后的值。</returns>
    private static object ConstrainToRange(DependencyObject d, object basevalue)
    {
        var ctrl = (TagRangeBase)d;
        var min = ctrl.Minimum;
        var v = (double)basevalue;
        // 如果新的值小于Minimum属性的值，则将新的值设为Minimum属性的值
        if (v < min)
        {
            return min;
        }

        var max = ctrl.Maximum;
        // 如果新的值大于Maximum属性的值，则将新的值设为Maximum属性的值
        return v > max ? max : basevalue;
    }

    /// <summary>
    ///     当Value属性的值发生改变时，此回调函数会被调用。
    /// </summary>
    /// <param name="d">发生属性值改变的TagRangeBase元素。</param>
    /// <param name="e">包含属性值改变的详细信息。</param>
    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (TagRangeBase)d;
        // 调用OnValueChanged方法，通知子类Value属性的值已经改变
        ctrl.OnValueChanged((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary>
    ///     定义一个虚方法，当Value属性的值发生改变时，子类可以重写此方法来实现自定义的行为。
    /// </summary>
    /// <param name="oldValue">属性值改变前的旧值。</param>
    /// <param name="newValue">属性值改变后的新值。</param>
    protected virtual void OnValueChanged(double oldValue, double newValue)
    {
        // 创建一个RoutedPropertyChangedEventArgs<double>对象，用于描述属性值的改变
        var args = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
        {
            RoutedEvent = ValueChangedEvent
        };
        // 调用RaiseEvent方法，触发ValueChangedEvent路由事件
        RaiseEvent(args);
    }

    /// <summary>
    ///     定义Value属性的CLR属性访问器。
    /// </summary>
    public double Value
    {
        // 获取ValueProperty的值
        get => (double)GetValue(ValueProperty);

        // 设置ValueProperty的值
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    ///     定义一个名为LargeChange的依赖属性，表示TagRangeBase的大变化步长。
    /// </summary>
    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
        nameof(LargeChange),
        typeof(double),
        typeof(TagRangeBase),
        new PropertyMetadata(1.0),
        ValidateHelper.IsInRangeOfPosDoubleIncludeZero);

    /// <summary>
    ///     定义LargeChange属性的CLR属性访问器。
    /// </summary>
    public double LargeChange
    {
        // 获取LargeChangeProperty的值
        get => (double)GetValue(LargeChangeProperty);

        // 设置LargeChangeProperty的值
        set => SetValue(LargeChangeProperty, value);
    }

    /// <summary>
    ///     定义一个名为SmallChange的依赖属性，表示TagRangeBase的小变化步长。
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
        nameof(SmallChange),
        typeof(double),
        typeof(TagRangeBase),
        new PropertyMetadata(0.1),
        ValidateHelper.IsInRangeOfPosDoubleIncludeZero);

    /// <summary>
    ///     定义SmallChange属性的CLR属性访问器。
    /// </summary>
    public double SmallChange
    {
        // 获取SmallChangeProperty的值
        get => (double)GetValue(SmallChangeProperty);

        // 设置SmallChangeProperty的值
        set => SetValue(SmallChangeProperty, value);
    }

    #endregion

    #region 路由事件

    /// 定义一个名为ValueChanged的路由事件
    /// <summary>
    ///     此路由事件在TagRangeBase的值发生变化时触发。
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(TagRangeBase));

    /// 定义一个名为ValueChanged的CLR事件封装器
    /// <summary>
    ///     当TagRangeBase的值发生变化时，此事件会被触发。
    /// </summary>
    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        // <summary>
        //  添加一个事件处理程序到ValueChanged事件。
        // </summary>
        // <param name="value">要添加的事件处理程序。</param>
        add => AddHandler(ValueChangedEvent, value);

        // <summary>
        //  从ValueChanged事件中移除一个事件处理程序。
        // </summary>
        // <param name="value">要移除的事件处理程序。</param>
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    #endregion
}