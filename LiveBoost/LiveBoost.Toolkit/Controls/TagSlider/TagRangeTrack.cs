using HandyControl.Tools;

// ReSharper disable UnusedParameter.Local

namespace LiveBoost.Toolkit.Controls;

/// <summary>
///     表示滑块范围的轨道。
/// </summary>
public sealed class TagRangeTrack : FrameworkElement
{
    #region Ctor

    /// <summary>
    ///     静态构造函数，用于初始化 TagRangeTrack 类型的静态成员。
    /// </summary>
    static TagRangeTrack()
    {
        // 覆盖 IsEnabledProperty 的元数据，指定属性更改事件为 OnIsEnabledChanged 方法
        IsEnabledProperty.OverrideMetadata(typeof(TagRangeTrack), new UIPropertyMetadata(OnIsEnabledChanged));
    }

    #endregion

    #region 依赖属性

    /// <summary>
    ///     标识布局方向的依赖属性。
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation), typeof(Orientation), typeof(TagRangeTrack),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    ///     获取或设置布局方向。
    /// </summary>
    /// <remarks>
    ///     默认值为水平方向。
    /// </remarks>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }


    /// <summary>
    ///     标识最小值的依赖属性。
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum), typeof(double), typeof(TagRangeTrack),
        new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsArrange, OnMinimumChanged), ValidateHelper.IsInRangeOfDouble);


    /// <summary>
    ///     获取或设置最小值。
    /// </summary>
    /// <remarks>
    ///     默认值为 0.0。
    /// </remarks>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    ///     标识最大值的依赖属性。
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum), typeof(double), typeof(TagRangeTrack),
        new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.AffectsArrange, OnMaximumChanged, CoerceMaximum), ValidateHelper.IsInRangeOfDouble);

    /// <summary>
    ///     获取或设置最大值。
    /// </summary>
    /// <remarks>
    ///     默认值为 1.0。
    /// </remarks>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    ///     标识起始值依赖属性。
    /// </summary>
    public static readonly DependencyProperty ValueStartProperty = DependencyProperty.Register(
        nameof(ValueStart), typeof(double?), typeof(TagRangeTrack), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange, OnValueStartChanged, ConstrainToRange));

    /// <summary>
    ///     获取或设置起始值。
    /// </summary>
    public double? ValueStart
    {
        get => (double?)GetValue(ValueStartProperty);
        set => SetValue(ValueStartProperty, value);
    }

    /// <summary>
    ///     标识结束值依赖属性。
    /// </summary>
    public static readonly DependencyProperty ValueEndProperty = DependencyProperty.Register(
        nameof(ValueEnd), typeof(double?), typeof(TagRangeTrack), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange, OnValueEndChanged, ConstrainToRange));

    /// <summary>
    ///     获取或设置结束值。
    /// </summary>
    public double? ValueEnd
    {
        get => (double?)GetValue(ValueEndProperty);
        set => SetValue(ValueEndProperty, value);
    }

    /// <summary DoubleNullableRange="，拥有该路由事件的类型为 TagRangeTrack。">
    ///     定义一个自定义的路由事件 ValueChangedEvent，使用冒泡策略，事件处理函数类型为 RoutedPropertyChangedEventHandler
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DoubleNullableRange>), typeof(TagRangeTrack));

    /// <summary>
    ///     为 ValueChangedEvent 事件添加事件处理器。
    /// </summary>
    public event RoutedPropertyChangedEventHandler<DoubleNullableRange> ValueChanged
    {
        // 使用 AddHandler 方法添加事件处理器
        add => AddHandler(ValueChangedEvent, value);
        // 使用 RemoveHandler 方法删除事件处理器
        remove => RemoveHandler(ValueChangedEvent, value);
    }


    #region 依赖属性-方法

    /// <summary>
    ///     依赖属性的值改变处理函数，在最小值改变时被调用。
    /// </summary>
    /// <param name="d">依赖属性所在的对象。</param>
    /// <param name="e">依赖属性改变的详细信息。</param>
    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 将 DependencyObject 强制转换为 TagRangeTrack 类型
        var track = (TagRangeTrack)d;

        // 强制重新计算 MaximumProperty、ValueStartProperty 和 ValueEndProperty 的值
        track.CoerceValue(MaximumProperty);
        track.CoerceValue(ValueStartProperty);
        track.CoerceValue(ValueEndProperty);

        // 处理最小值的改变
        track.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary>
    ///     处理最小值的改变。
    /// </summary>
    /// <param name="oldMinimum">旧的最小值。</param>
    /// <param name="newMinimum">新的最小值。</param>
    private void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
        // 这个方法可以被子类重写，用于处理最小值的改变
    }

    /// <summary>
    ///     最大值的强制值回调函数，用于在依赖属性的值即将改变时强制重新计算新的值。
    /// </summary>
    /// <param name="d">依赖属性所在的对象。</param>
    /// <param name="basevalue">新的值。</param>
    /// <returns>强制重新计算后的新值。</returns>
    private static object CoerceMaximum(DependencyObject d, object basevalue)
    {
        // 将 DependencyObject 强制转换为 TagRangeTrack 类型
        var track = (TagRangeTrack)d;

        // 将新的值强制转换为 double 类型，并获取最小值
        var maximum = (double)basevalue;
        var minimum = track.Minimum;

        // 如果新的值小于最小值，则返回最小值，否则返回新的值
        return maximum < minimum ? minimum : maximum;
    }

    /// <summary>
    ///     依赖属性的值改变处理函数，在最大值改变时被调用。
    /// </summary>
    /// <param name="d">依赖属性所在的对象。</param>
    /// <param name="e">依赖属性改变的详细信息。</param>
    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 将 DependencyObject 强制转换为 TagRangeTrack 类型
        var track = (TagRangeTrack)d;

        // 强制重新计算 ValueStartProperty 和 ValueEndProperty 的值
        track.CoerceValue(ValueStartProperty);
        track.CoerceValue(ValueEndProperty);

        // 处理最大值的改变
        track.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
    }

    /// <summary>
    ///     处理最大值的改变。
    /// </summary>
    /// <param name="oldMaximum">旧的最大值。</param>
    /// <param name="newMaximum">新的最大值。</param>
    private void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
        // 这个方法可以被子类重写，用于处理最大值的改变
    }


    /// <summary>
    ///     限制值在指定范围内的强制值回调函数。
    /// </summary>
    /// <param name="d">依赖属性所在的对象。</param>
    /// <param name="basevalue">新的值。</param>
    /// <returns>如果新的值超出范围，则返回限制在范围内的值，否则返回新的值。</returns>
    private static object ConstrainToRange(DependencyObject d, object basevalue)
    {
        // 将 DependencyObject 强制转换为 TagRangeTrack 类型
        // 这是因为我们知道这个对象一定是 TagRangeTrack 类型的，所以可以安全地进行强制转换
        var track = (TagRangeTrack)d;

        // 获取最小值和新的值
        var minimum = track.Minimum;
        var value = (double?)basevalue;

        // 如果新的值小于最小值，则返回最小值
        if (value < minimum)
        {
            return minimum;
        }

        // 获取最大值
        var maximum = track.Maximum;

        // 如果新的值大于最大值，则返回最大值，否则返回新的值
        return value > maximum ? maximum : basevalue;
    }


    /// <summary>
    ///     当值变化时触发的事件。
    /// </summary>
    /// <param name="oldValue">变化前的值。</param>
    /// <param name="newValue">变化后的值。</param>
    private void OnValueChanged(DoubleNullableRange oldValue, DoubleNullableRange newValue)
    {
        // 创建一个新的RoutedPropertyChangedEventArgs<T>实例，传入旧值和新值
        var args = new RoutedPropertyChangedEventArgs<DoubleNullableRange>(oldValue, newValue)
        {
            // 设置RoutedEvent属性
            RoutedEvent = ValueChangedEvent
        };
        // 触发事件
        RaiseEvent(args);
        Console.Out.WriteLine($"OldValue:{oldValue.Start},{oldValue.End}\n\rNewValue:{newValue.Start},{newValue.End}");
    }


    /// <summary>
    ///     当ValueStart属性值改变时的回调函数。
    /// </summary>
    /// <param name="d">依赖对象。</param>
    /// <param name="e">依赖属性值改变的事件参数。</param>
    private static void OnValueStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 将依赖对象转换为TagRangeTrack对象
        var track = (TagRangeTrack)d;
        // 创建一个新的DoubleNullableRange对象，将旧值和新值设置为Start属性和End属性
        var oldValue = new DoubleNullableRange
        {
            Start = (double?)e.OldValue,
            End = track.ValueEnd
        };
        var newValue = new DoubleNullableRange
        {
            Start = (double?)e.NewValue,
            End = track.ValueEnd
        };
        // 调用OnValueChanged方法，将旧值和新值传递给OnValueChanged方法
        track.OnValueChanged(oldValue, newValue);
    }

    /// <summary>
    ///     当ValueEnd属性值改变时的回调函数。
    /// </summary>
    /// <param name="d">依赖对象。</param>
    /// <param name="e">依赖属性值改变的事件参数。</param>
    private static void OnValueEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 将依赖对象转换为TagRangeTrack对象
        var track = (TagRangeTrack)d;
        // 创建一个新的DoubleNullableRange对象，将旧值和新值设置为End属性和Start属性
        var oldValue = new DoubleNullableRange
        {
            Start = track.ValueStart,
            End = (double?)e.OldValue
        };
        var newValue = new DoubleNullableRange
        {
            Start = track.ValueStart,
            End = (double?)e.NewValue
        };
        // 调用OnValueChanged方法，将旧值和新值传递给OnValueChanged方法
        track.OnValueChanged(oldValue, newValue);
    }

    #endregion

    #endregion

    #region 属性、字段

    /// <summary>
    ///     表示增大按钮组件。
    /// </summary>
    private RepeatButton? _increaseButton;

    /// <summary>
    ///     获取或设置IncreaseRepeatButton。
    /// </summary>
    /// <exception cref="NotSupportedException">如果_decreaseButton或_centerButton等于输入的值，会抛出这个异常。</exception>
    public RepeatButton? IncreaseRepeatButton
    {
        get => _increaseButton;
        set
        {
            // 如果_decreaseButton或_centerButton等于输入的值，抛出NotSupportedException异常
            if (Equals(_decreaseButton, value) || Equals(_centerButton, value))
            {
                throw new NotSupportedException("SameButtons");
            }

            // 调用UpdateComponent方法更新组件
            UpdateComponent(_increaseButton, value);

            // 将_increaseButton设置为输入的值
            _increaseButton = value;

            // 如果_increaseButton不为null，调用CommandManager.InvalidateRequerySuggested方法更新命令的状态
            if (_increaseButton != null)
            {
                CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
            }
        }
    }

    /// <summary>
    ///     表示中间按钮组件。
    /// </summary>
    private RepeatButton? _centerButton;

    /// <summary>
    ///     获取或设置CenterRepeatButton。
    /// </summary>
    /// <exception cref="NotSupportedException">如果_increaseButton或_decreaseButton等于输入的值，会抛出这个异常。</exception>
    public RepeatButton? CenterRepeatButton
    {
        get => _centerButton;
        set
        {
            // 如果_increaseButton或_decreaseButton等于输入的值，抛出NotSupportedException异常
            if (Equals(_increaseButton, value) || Equals(_decreaseButton, value))
            {
                throw new NotSupportedException("SameButtons");
            }

            // 调用UpdateComponent方法更新组件
            UpdateComponent(_centerButton, value);

            // 将_centerButton设置为输入的值
            _centerButton = value;

            // 如果_centerButton不为null，调用CommandManager.InvalidateRequerySuggested方法更新命令的状态
            if (_centerButton != null)
            {
                CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
            }
        }
    }

    /// <summary>
    ///     表示减小按钮组件。
    /// </summary>
    private RepeatButton? _decreaseButton;

    /// <summary>
    ///     获取或设置DecreaseRepeatButton。
    /// </summary>
    /// <exception cref="NotSupportedException">如果_increaseButton或_centerButton等于输入的值，会抛出这个异常。</exception>
    public RepeatButton? DecreaseRepeatButton
    {
        get => _decreaseButton;
        set
        {
            // 如果_increaseButton或_centerButton等于输入的值，抛出NotSupportedException异常
            if (Equals(_increaseButton, value) || Equals(_centerButton, value))
            {
                throw new NotSupportedException("SameButtons");
            }

            // 调用UpdateComponent方法更新组件
            UpdateComponent(_decreaseButton, value);

            // 将_decreaseButton设置为输入的值
            _decreaseButton = value;

            // 如果_decreaseButton不为null，调用CommandManager.InvalidateRequerySuggested方法更新命令的状态
            if (_decreaseButton != null)
            {
                CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
            }
        }
    }

    /// <summary>
    ///     表示起始滑块组件
    /// </summary>
    private TagRangeThumb? _thumbStart;

    /// <summary>
    ///     获取或设置ThumbStart。
    /// </summary>
    public TagRangeThumb? ThumbStart
    {
        get => _thumbStart;
        set
        {
            // 调用UpdateComponent方法更新组件
            UpdateComponent(_thumbStart, value);

            // 将_thumbStart设置为输入的值
            _thumbStart = value;
            if (_thumbStart != null)
            {
                _thumbStart.ToolTip = ValueStart;
            }
        }
    }

    /// <summary>
    ///     表示结束滑块组件
    /// </summary>
    private TagRangeThumb? _thumbEnd;

    /// <summary>
    ///     获取或设置ThumbEnd。
    /// </summary>
    public TagRangeThumb? ThumbEnd
    {
        get => _thumbEnd;
        set
        {
            // 调用UpdateComponent方法更新组件
            UpdateComponent(_thumbEnd, value);

            // 将_thumbEnd设置为输入的值
            _thumbEnd = value;
            if (_thumbEnd != null)
            {
                _thumbEnd.ToolTip = ValueEnd;
            }
        }
    }

    /// <summary>
    ///     表示滑块组件的 Visual 数组。
    /// </summary>
    private Visual?[]? VisualChildren { get; set; }

    /// <summary>
    ///     表示滑块的密度。用于将值映射到长度。
    /// </summary>
    private double Density { get; set; } = double.NaN;

    /// <summary>
    ///     标记点
    /// </summary>
    private ITag? TagModel { get; set; }

    #endregion

    #region 静态方法

    /// <summary>
    ///     当 IsEnabled 属性发生变化时调用的处理方法。
    /// </summary>
    /// <param name="d">发生属性更改的依赖对象。</param>
    /// <param name="e">包含事件数据的参数。</param>
    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 当 IsEnabled 属性的新值为 true 时执行
        if ((bool)e.NewValue)
            // 同步鼠标状态
        {
            Mouse.Synchronize();
        }
    }

    /// <summary>
    ///     强制调整给定的组件长度，确保在合理的范围内。
    /// </summary>
    /// <param name="componentLength">要强制调整的组件长度。</param>
    /// <param name="trackLength">轨道的长度。</param>
    private static void CoerceLength(ref double? componentLength, double trackLength)
    {
        switch (componentLength)
        {
            case null:
                return;
            // 如果组件长度小于 0，将其设置为 0
            case < 0.0:
                componentLength = 0.0;
                break;
            default:
            {
                // 如果组件长度大于轨道长度或为 NaN，将其设置为轨道长度
                if ((componentLength.Value > trackLength) || double.IsNaN(componentLength.Value))
                {
                    componentLength = trackLength;
                }

                break;
            }
        }
    }

    /// <summary>
    ///     强制调整给定的组件长度，确保在合理的范围内。
    /// </summary>
    /// <param name="componentLength">要强制调整的组件长度。</param>
    /// <param name="trackLength">轨道的长度。</param>
    private static void CoerceLength(ref double componentLength, double trackLength)
    {
        // 如果组件长度小于 0，将其设置为 0
        if (componentLength < 0.0)
        {
            componentLength = 0.0;
        }
        else
        {
            // 如果组件长度大于轨道长度或为 NaN，将其设置为轨道长度
            if ((componentLength > trackLength) || double.IsNaN(componentLength))
            {
                componentLength = trackLength;
            }
        }
    }

    #endregion

    #region 共有方法

    /// <summary>
    ///     基于鼠标的位置返回一个值，这个值表示在滑块上的相对位置。
    ///     如果滑块是水平的，这个值基于鼠标的X坐标；
    ///     如果滑块是垂直的，这个值基于鼠标的Y坐标。
    ///     如果滑块的方向被反转，返回的值将被反转。
    /// </summary>
    /// <param name="pt">一个点，表示鼠标的位置。</param>
    /// <returns>一个值，表示在滑块上的相对位置。</returns>
    public double ValueFromPoint(Point pt) =>
        // 如果滑块是水平的
        Orientation == Orientation.Horizontal
            ?
            // 如果滑块的方向没有被反转，返回一个值，这个值基于鼠标的X坐标
            pt.X / RenderSize.Width * Maximum
            // 如果滑块是垂直的
            :
            // 如果滑块的方向没有被反转，返回一个值，这个值基于鼠标的Y坐标
            pt.Y / RenderSize.Height * Maximum;


    /// <summary>
    ///     根据水平和垂直距离计算对应的值。
    /// </summary>
    /// <param name="horizontal">水平距离。</param>
    /// <param name="vertical">垂直距离。</param>
    /// <returns>计算得到的值。</returns>
    public double ValueFromDistance(double horizontal, double vertical) =>
        // 根据方向选择计算方式，水平方向使用水平距离乘以密度，垂直方向使用负值的垂直距离乘以密度
        Orientation == Orientation.Horizontal ? horizontal * Density : -1.0 * vertical * Density;

    #endregion

    #region Override

    /// <summary>
    ///     根据父容器分配的可用空间，计算并返回该容器根据其子元素大小确定的在布局过程中所需的大小。
    /// </summary>
    /// <param name="arrangeSize">父容器分配的可用空间。</param>
    /// <returns>该容器根据其子元素大小确定的在布局过程中所需的大小。</returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        // 判断是否为垂直方向
        var isVertical = Orientation == Orientation.Vertical;

        // 计算各个按钮和滑块的长度
        ComputeLengths(arrangeSize, isVertical, out var decreaseButtonLength, out var centerButtonLength,
            out var increaseButtonLength, out var thumbStartLength, out var thumbEndLength);

        // 初始化偏移量和尺寸
        var offset = new Point();
        var pieceSize = arrangeSize;

        // 若为垂直方向
        if (isVertical)
        {
            // 制约按钮长度
            CoerceLength(ref decreaseButtonLength, arrangeSize.Height);
            CoerceLength(ref centerButtonLength, arrangeSize.Height);
            CoerceLength(ref increaseButtonLength, arrangeSize.Height);
            CoerceLength(ref thumbStartLength, arrangeSize.Height);
            CoerceLength(ref thumbEndLength, arrangeSize.Height);

            // 计算偏移量和设置尺寸
            offset.Y = 0;
            pieceSize.Height = increaseButtonLength;

            // 布局增加按钮
            IncreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            offset.Y = increaseButtonLength + (thumbStartLength ?? 0);
            pieceSize.Height = centerButtonLength;

            // 布局中心按钮
            CenterRepeatButton?.Arrange(new Rect(offset, pieceSize));

            offset.Y = increaseButtonLength + (thumbStartLength ?? 0) + centerButtonLength + (thumbEndLength ?? 0);
            pieceSize.Height = decreaseButtonLength;

            // 布局减少按钮
            DecreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            offset.Y = increaseButtonLength + (thumbStartLength ?? 0) + centerButtonLength;
            pieceSize.Height = thumbStartLength ?? 0;
            if (ThumbStart is not null)
            {
                ThumbStart.Visibility = thumbStartLength is null ? Visibility.Collapsed : Visibility.Visible;
            }

            // 布局滑块（未滑动部分）
            ArrangeThumb(false, offset, pieceSize);

            offset.Y = increaseButtonLength;
            pieceSize.Height = thumbEndLength ?? 0;
            if (ThumbEnd is not null)
            {
                ThumbEnd.Visibility = thumbEndLength is null ? Visibility.Collapsed : Visibility.Visible;
            }

            // 布局滑块（已滑动部分）
            ArrangeThumb(true, offset, pieceSize);
        }
        // 若为水平方向
        else
        {
            // 制约按钮长度
            CoerceLength(ref decreaseButtonLength, arrangeSize.Width);
            CoerceLength(ref centerButtonLength, arrangeSize.Width);
            CoerceLength(ref increaseButtonLength, arrangeSize.Width);
            CoerceLength(ref thumbStartLength, arrangeSize.Width);
            CoerceLength(ref thumbEndLength, arrangeSize.Width);

            // 计算偏移量和设置尺寸
            offset.X = decreaseButtonLength + (thumbEndLength ?? 0) + centerButtonLength + (thumbStartLength ?? 0);
            pieceSize.Width = increaseButtonLength;

            // 布局增加按钮
            IncreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            offset.X = decreaseButtonLength + (thumbEndLength ?? 0);
            pieceSize.Width = centerButtonLength;

            // 布局中心按钮
            CenterRepeatButton?.Arrange(new Rect(offset, pieceSize));

            offset.X = 0;
            pieceSize.Width = decreaseButtonLength;

            // 布局减少按钮
            DecreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            offset.X = decreaseButtonLength;
            pieceSize.Width = thumbStartLength ?? 0;
            if (ThumbStart is not null)
            {
                ThumbStart.Visibility = thumbStartLength is null ? Visibility.Collapsed : Visibility.Visible;
            }

            // 布局滑块（未滑动部分）
            ArrangeThumb(false, offset, pieceSize);

            offset.X = decreaseButtonLength + (thumbEndLength ?? 0) + centerButtonLength;
            pieceSize.Width = thumbEndLength ?? 0;
            if (ThumbEnd is not null)
            {
                ThumbEnd.Visibility = thumbEndLength is null ? Visibility.Collapsed : Visibility.Visible;
            }

            // 布局滑块（已滑动部分）
            ArrangeThumb(true, offset, pieceSize);
        }

        // 返回大小
        return arrangeSize;
    }

    /// <summary>
    ///     根据父容器分配的可用空间，计算并返回该容器根据其子元素大小确定的在布局过程中所需的大小。
    /// </summary>
    /// <param name="availableSize">父容器分配的可用空间。</param>
    /// <returns>该容器根据其子元素大小确定的在布局过程中所需的大小。</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        // 创建一个新的Size对象
        var desiredSize = new Size();

        // 如果_thumbStart不为null
        if (_thumbStart != null)
        {
            // 调用_thumbStart.Measure方法
            _thumbStart.Measure(availableSize);

            // 将_thumbStart.DesiredSize赋值给desiredSize
            desiredSize = _thumbStart.DesiredSize;
        }

        // 如果_thumbEnd不为null
        if (_thumbEnd == null)
        {
            return desiredSize;
        }

        // 调用_thumbEnd.Measure方法
        _thumbEnd.Measure(availableSize);

        // 将_thumbEnd.DesiredSize和desiredSize中的最大值分别赋值给desiredSize.Width和desiredSize.Height
        desiredSize = new Size(Math.Max(_thumbEnd.DesiredSize.Width, desiredSize.Width),
            Math.Max(_thumbEnd.DesiredSize.Height, desiredSize.Height));

        // 返回desiredSize
        return desiredSize;
    }

    /// <summary>
    ///     获取可视子元素的数量。
    ///     如果VisualChildren为null，则返回0。
    ///     如果VisualChildren中存在null元素，那么返回null元素的索引。
    ///     否则，返回VisualChildren的长度。
    /// </summary>
    protected override int VisualChildrenCount
    {
        get
        {
            // 如果VisualChildren为null，返回0
            if (VisualChildren == null)
            {
                return 0;
            }

            // 遍历VisualChildren
            for (var i = 0; i < VisualChildren.Length; i++)
                // 如果VisualChildren中存在null元素，返回该元素的索引
            {
                if (VisualChildren[i] == null)
                {
                    return i;
                }
            }

            // 如果VisualChildren中不存在null元素，返回VisualChildren的长度
            return VisualChildren.Length;
        }
    }

    /// <summary>
    ///     根据索引获取可视子元素。
    /// </summary>
    /// <param name="index">子元素的索引。</param>
    /// <returns>在索引位置的可视子元素。</returns>
    protected override Visual? GetVisualChild(int index)
    {
        // 如果 VisualChildren 不为 null，且索引有效，返回对应索引处的元素
        if ((VisualChildren != null) && (index >= 0) && (index < VisualChildren.Length) && (VisualChildren[index] != null))
        {
            return VisualChildren[index];
        }

        // 如果索引无效，抛出参数异常
        throw new ArgumentOutOfRangeException(nameof(index), index, @"Visual_ArgumentOutOfRange");
    }

    #endregion

    #region 私有实例方法

    /// <summary>
    ///     计算滑动条的长度。
    /// </summary>
    /// <param name="arrangeSize">布局大小。</param>
    /// <param name="isVertical">是否是垂直滑动条。</param>
    /// <param name="decreaseButtonLength">减小按钮的长度。</param>
    /// <param name="centerButtonLength">中心按钮的长度。</param>
    /// <param name="increaseButtonLength">增大按钮的长度。</param>
    /// <param name="thumbStartLength">开始滑块的长度。</param>
    /// <param name="thumbEndLength">结束滑块的长度。</param>
    private void ComputeLengths(Size arrangeSize, bool isVertical, out double decreaseButtonLength,
        out double centerButtonLength, out double increaseButtonLength, out double? thumbStartLength,
        out double? thumbEndLength)
    {
        // 计算滑动范围和滑动起始和结束的位置
        var min = Minimum;
        var range = Math.Max(0.0, Maximum - min);
        double? offsetStart = ValueStart is null ? null : Math.Min(range, ValueStart.Value - min);
        double? offsetEnd = ValueEnd is null ? null : Math.Min(range, ValueEnd.Value - min);

        double trackLength;

        // 根据isVertical的值，计算出滑动条长度和滑块的长度
        if (isVertical)
        {
            trackLength = arrangeSize.Height;
            thumbStartLength = ValueStart is null ? null : _thumbStart?.DesiredSize.Height;
            thumbEndLength = ValueEnd is null ? null : _thumbEnd?.DesiredSize.Height;
        }
        else
        {
            trackLength = arrangeSize.Width;
            thumbStartLength = ValueStart is null ? null : _thumbStart?.DesiredSize.Width;
            thumbEndLength = ValueEnd is null ? null : _thumbEnd?.DesiredSize.Width;
        }

        // 调用CoerceLength方法来调整滑块的长度
        CoerceLength(ref thumbStartLength, trackLength);
        CoerceLength(ref thumbEndLength, trackLength);

        // 计算剩余的滑动条长度
        var remainingTrackLength = trackLength - (thumbStartLength ?? 0) - (thumbEndLength ?? 0);

        // 计算decreaseButtonLength，centerButtonLength和increaseButtonLength的长度
        decreaseButtonLength = offsetStart is null ? offsetEnd is null ? 0 : remainingTrackLength * offsetEnd.Value / range : remainingTrackLength * offsetStart.Value / range;
        CoerceLength(ref decreaseButtonLength, remainingTrackLength);

        centerButtonLength = offsetStart is null ? 0 : offsetEnd is null ? 0 : remainingTrackLength * offsetEnd.Value / range - decreaseButtonLength;
        CoerceLength(ref centerButtonLength, remainingTrackLength);

        increaseButtonLength = remainingTrackLength - decreaseButtonLength - centerButtonLength;
        CoerceLength(ref increaseButtonLength, remainingTrackLength);

        // 计算滑动密度
        Density = range / remainingTrackLength;
    }

    /// <summary>
    ///     安排滑块的位置。
    /// </summary>
    /// <param name="isStart">指示是否是开始。</param>
    /// <param name="offset">滑块的偏移量。</param>
    /// <param name="pieceSize">滑块的大小。</param>
    private void ArrangeThumb(bool isStart, Point offset, Size pieceSize)
    {
        // 如果isStart为true
        if (isStart)
        {
            ThumbEnd?.Arrange(new Rect(offset, pieceSize));
        }
        // 如果isStart为false
        else
        {
            ThumbStart?.Arrange(new Rect(offset, pieceSize));
        }
    }

    /// <summary>
    ///     更新组件，替换旧的控件为新的控件。
    /// </summary>
    /// <param name="oldValue">要替换的旧控件。</param>
    /// <param name="newValue">新的控件。</param>
    private void UpdateComponent(Visual? oldValue, Visual? newValue)
    {
        // 如果新旧控件相同，无需更新
        if (oldValue == newValue)
        {
            return;
        }

        // 初始化 VisualChildren 数组，用于存储子视觉元素
        VisualChildren ??= new Visual?[5];

        // 移除旧控件
        if (oldValue != null)
        {
            // 从可视子元素中移除旧控件
            RemoveVisualChild(oldValue);

            // 在 VisualChildren 数组中查找并移除旧控件的引用
            var indexToRemove = Array.IndexOf(VisualChildren, oldValue);
            if (indexToRemove != -1)
            {
                // 将数组中的元素向左移动一位
                Array.Copy(VisualChildren, indexToRemove + 1, VisualChildren, indexToRemove, VisualChildren.Length - indexToRemove - 1);

                // 将最后一个元素设为null
                VisualChildren[VisualChildren.Length - 1] = null;
            }
        }

        // 找到第一个空的位置插入新控件
        var indexToAdd = Array.IndexOf(VisualChildren, null);
        if (indexToAdd != -1)
        {
            // 将新控件插入数组
            VisualChildren[indexToAdd] = newValue;

            // 添加新控件到可视子元素
            AddVisualChild(newValue);
        }

        // 通知布局需要重新测量
        InvalidateMeasure();

        // 通知布局需要重新排列
        InvalidateArrange();
    }

    #endregion
}