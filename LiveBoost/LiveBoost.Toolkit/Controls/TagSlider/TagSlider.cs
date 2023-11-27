using HandyControl.Tools;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace LiveBoost.Toolkit.Controls;

[DefaultEvent("ValueChanged")]
[DefaultProperty("Value")]
[TemplatePart(Name = TrackName, Type = typeof(TagTrack))]
public sealed class TagSlider : TagRangeBase
{
    #region Field

    /// <summary>
    ///     常量，用于标识轨道的名称。
    /// </summary>
    private const string TrackName = "PART_Track";

    /// <summary>
    ///     用于存储 TagTrack 的实例。
    /// </summary>
    private TagTrack? _tagTrack;

    /// <summary>
    ///     鼠标在屏幕上的上一个位置
    /// </summary>
    private Point _previousScreenCoordPosition;

    /// <summary>
    ///     Thumb拖拽前的原始位置
    /// </summary>
    private Point _originThumbPoint;

    /// <summary>
    ///     当前操作滑块所属的Track
    /// </summary>
    private TagRangeTrack? _currentTrack;

    /// <summary>
    ///     当前操作的滑块
    /// </summary>
    private ITagThumb? _currentThumb;

    #endregion

    #region Private-Events

    /// <summary>
    ///     移动到下一个刻度
    /// </summary>
    /// <param name="direction">移动的方向</param>
    private void MoveToNextTick(double direction)
    {
        // 检查方向是否接近于0，如果是则返回
        if (MathHelper.AreClose(direction, 0)) return;

        // 计算下一个刻度
        var next = CalculateNextTick(direction);

        // 检查新的值是否有效，如果是则设置为当前值
        if (IsValidValue(next)) SetCurrentValue(ValueProperty, next);
    }

    /// <summary>
    ///     计算下一个刻度
    /// </summary>
    /// <param name="direction">移动的方向</param>
    /// <returns>下一个刻度</returns>
    private double CalculateNextTick(double direction)
    {
        // 获取当前的值
        var value = Value;
        // 计算下一个刻度的预设值
        var next = SnapToTick(Math.Max(Minimum, Math.Min(Maximum, value + direction)));
        // 判断移动的方向
        var greaterThan = direction > 0;

        // 如果下一个刻度的预设值与当前值接近，或者当前值已经接近最大值或最小值，则直接返回下一个刻度的预设值
        if (!MathHelper.AreClose(next, value) || (greaterThan && MathHelper.AreClose(value, Maximum)) || (!greaterThan && MathHelper.AreClose(value, Minimum))) return next;
        // 如果刻度列表有元素，则在刻度列表中查找下一个刻度
        if (Ticks is { Count: > 0 })
            next = FindNextTickInTicks(value, greaterThan, next);
        // 如果刻度频率大于0，则根据刻度频率计算下一个刻度
        else if (MathHelper.GreaterThan(TickFrequency, 0.0)) next = CalculateNextTickByFrequency(value, greaterThan);

        return next;
    }

    /// <summary>
    ///     在刻度列表中查找下一个刻度
    /// </summary>
    /// <param name="value">当前的值</param>
    /// <param name="greaterThan">是否查找大于当前值的刻度</param>
    /// <param name="next">预设的下一个刻度</param>
    /// <returns>下一个刻度</returns>
    private double FindNextTickInTicks(double value, bool greaterThan, double next)
    {
        // 遍历刻度列表，找到符合条件的刻度
        foreach (var tick in Ticks!.Where(tick => (greaterThan && MathHelper.GreaterThan(tick, value) && (MathHelper.LessThan(tick, next) || MathHelper.AreClose(next, value)))
                                                  || (!greaterThan && MathHelper.LessThan(tick, value) && (MathHelper.GreaterThan(tick, next) || MathHelper.AreClose(next, value)))))
            // 更新下一个刻度
            next = tick;

        return next;
    }

    /// <summary>
    ///     根据刻度频率计算下一个刻度
    /// </summary>
    /// <param name="value">当前的值</param>
    /// <param name="greaterThan">是否查找大于当前值的刻度</param>
    /// <returns>下一个刻度</returns>
    private double CalculateNextTickByFrequency(double value, bool greaterThan)
    {
        // 计算当前刻度的位置
        var tickNumber = Math.Round((value - Minimum) / TickFrequency);

        // 根据移动的方向，增加或减少刻度位置
        if (greaterThan)
            tickNumber += 1.0;
        else
            tickNumber -= 1.0;

        // 计算下一个刻度
        var next = Minimum + tickNumber * TickFrequency;

        return next;
    }

    /// <summary>
    ///     检查值是否有效
    /// </summary>
    /// <param name="value">需要检查的值</param>
    /// <returns>如果值有效则返回true，否则返回false</returns>
    private bool IsValidValue(double value)
    {
        return !MathHelper.AreClose(value, Value);
    }

    /// <summary>
    ///     将给定的值捕捉到最近的刻度值。
    /// </summary>
    /// <param name="value">要捕捉的值。</param>
    /// <returns>捕捉到的最近刻度值。</returns>
    private double SnapToTick(double value)
    {
        // 如果不启用刻度捕捉，则直接返回原值
        if (!IsSnapToTickEnabled)
            return value;

        // 初始化前一个和后一个刻度值为最小值和最大值
        var previous = Minimum;
        var next = Maximum;

        // 如果存在刻度集合
        if (Ticks?.Count > 0)
        {
            // 获取当前值在刻度集合中的位置
            var tickIndex = Ticks.ToList().BinarySearch(value);

            // 如果找到了与当前值相等的刻度，直接返回原值
            if (tickIndex >= 0)
                return value;

            // 计算当前值在刻度集合中的插入点
            tickIndex = ~tickIndex;

            // 根据插入点获取前一个和后一个刻度值
            if (tickIndex > 0)
                previous = Ticks[tickIndex - 1];
            if (tickIndex < Ticks.Count)
                next = Ticks[tickIndex];
        }
        // 如果不存在刻度集合，但刻度频率大于零
        else if (MathHelper.GreaterThan(TickFrequency, 0.0))
        {
            // 计算当前值相对于最小值的刻度数量
            var relativeTicks = Math.Round((value - Minimum) / TickFrequency);

            // 计算前一个刻度值
            previous = Minimum + relativeTicks * TickFrequency;

            // 计算后一个刻度值，不超过最大值
            next = Math.Min(Maximum, previous + TickFrequency);
        }

        // 选择当前值与前后刻度值之间最近的值
        return MathHelper.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
    }


    /// <summary>
    ///     更新值并根据需要进行刻度捕捉。
    /// </summary>
    /// <param name="value">要更新的值。</param>
    private void UpdateValue(double value)
    {
        // 获取刻度捕捉后的值
        var snapValue = SnapToTick(value);

        // 如果刻度捕捉后的值与原值相等，更新当前值
        if (!MathHelper.AreClose(Value, snapValue)) SetCurrentValue(ValueProperty, Math.Max(Minimum, Math.Min(Maximum, snapValue)));
    }

    /// <summary>
    ///     更新范围轨道的值。
    /// </summary>
    /// <param name="value">新的值。</param>
    /// <param name="isStart">如果为 true，则更新起始值；否则，更新结束值。</param>
    private void UpdateValue(double value, bool isStart)
    {
        // 对新的值进行快照
        var snappedValue = SnapToTick(value);

        // 如果当前轨道为空，直接返回
        if (_currentTrack is null) return;

        // 根据 isStart 参数确定需要更新的属性和对应的值
        // 如果 isStart 为 true，则更新起始值；否则，更新结束值
        var currentProperty = isStart ? TagRangeTrack.ValueStartProperty : TagRangeTrack.ValueEndProperty;
        var otherProperty = isStart ? TagRangeTrack.ValueEndProperty : TagRangeTrack.ValueStartProperty;
        var currentValue = isStart ? _currentTrack.ValueStart : _currentTrack.ValueEnd;
        var otherValue = isStart ? _currentTrack.ValueEnd : _currentTrack.ValueStart;
        var currentThumb = isStart ? _currentTrack.ThumbStart : _currentTrack.ThumbEnd;
        var otherThumb = isStart ? _currentTrack.ThumbEnd : _currentTrack.ThumbStart;

        // 如果新的值接近当前的值，直接返回
        if (MathHelper.AreClose(snappedValue, currentValue!.Value)) return;

        // 计算新的值，新的值不能超出范围
        var newValue = Math.Max(Minimum, Math.Min(Maximum, snappedValue));

        // 如果新的值超出了其他值的范围，交换两个值的位置，并调整相应的滑块
        if (otherValue is not null && (isStart ? newValue > otherValue.Value : newValue < otherValue.Value))
        {
            _currentTrack.SetCurrentValue(currentProperty, otherValue);
            _currentTrack.SetCurrentValue(otherProperty, newValue);
            currentThumb?.CancelDrag();
            otherThumb?.StartDrag();
            _currentThumb = otherThumb;
        }
        // 否则，直接更新当前的值
        else
        {
            _currentTrack.SetCurrentValue(currentProperty, newValue);
        }
    }

    #endregion

    #region Override

    /// <summary>
    ///     在应用模板时执行的操作，用于获取模板中的特定元素。
    /// </summary>
    public override void OnApplyTemplate()
    {
        // 基类执行相应的操作
        base.OnApplyTemplate();

        // 获取模板中的特定元素
        _tagTrack = GetTemplateChild(TrackName) as TagTrack;
    }

    /// <summary>
    ///     在鼠标左键按下时预览操作，用于处理启用移动到点的情况。
    /// </summary>
    /// <param name="e">鼠标按钮事件参数</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        // 检查是否启用了移动到点功能，并且标记轨道不在拇指上
        if (IsMoveToPointEnabled && _tagTrack is not null && !(_tagTrack.Thumb?.IsMouseOver ?? false) && !(_tagTrack.SuperTagPanel?.FindVisualChildren<TagRangeTrack>().Any(it => (it.ThumbStart?.IsMouseOver ?? false) || (it.ThumbEnd?.IsMouseOver ?? false)) ?? false))
        {
            // 获取鼠标在标记轨道上的位置
            var point = e.MouseDevice.GetPosition(_tagTrack);

            // 根据鼠标位置计算新的值
            var newValue = _tagTrack.ValueFromPoint(point);

            // 验证新值是否在有效范围内
            if (ValidateHelper.IsInRangeOfDouble(newValue))
                // 更新值
                UpdateValue(newValue);

            // 标记事件已处理
            e.Handled = true;
        }

        // 基类执行相应的操作
        base.OnPreviewMouseLeftButtonDown(e);
    }

    /// <summary>
    ///     当用户释放鼠标左键时被调用。
    /// </summary>
    /// <param name="e">包含事件数据的MouseButtonEventArgs。</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonUp(e);

        // 当用户释放鼠标左键时，清除当前被操作的滑块和轨道。
        _currentThumb = null;
        _currentTrack = null;
    }

    /// <summary>
    ///     处理鼠标移动的事件
    /// </summary>
    /// <param name="e">事件参数</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        // 检查鼠标的左键是否被按下，如果没有，就返回
        if (e.MouseDevice.LeftButton != MouseButtonState.Pressed) return;
        // 检查滑块是否正在拖动，如果不是，就返回
        if (!((_currentThumb as Thumb)?.IsDragging ?? false)) return;

        // 获取滑块的坐标位置
        var thumbCoordPosition = e.GetPosition(_currentThumb as Thumb);
        // 转换为屏幕坐标
        var screenCoordPosition = PointFromScreen(thumbCoordPosition);
        // 检查这个位置是否与上一个位置相同，如果相同，就返回
        if (screenCoordPosition == _previousScreenCoordPosition) return;
        // 更新上一个位置
        _previousScreenCoordPosition = screenCoordPosition;
        // 触发滑块的DragDelta事件
        (_currentThumb as Thumb)?.RaiseEvent(new DragDeltaEventArgs(thumbCoordPosition.X - _originThumbPoint.X,
            thumbCoordPosition.Y - _originThumbPoint.Y));
    }

    #endregion

    #region 依赖属性

    /// <summary>
    ///     标签源属性
    /// </summary>
    public static readonly DependencyProperty TagsSourceProperty = DependencyProperty.Register(
        nameof(TagsSource),
        typeof(IEnumerable),
        typeof(TagSlider),
        new FrameworkPropertyMetadata(default(IEnumerable), FrameworkPropertyMetadataOptions.AffectsRender)
    );

    /// <summary>
    ///     获取或设置标签源
    /// </summary>
    public IEnumerable? TagsSource
    {
        // 获取标签源属性的值
        get => (IEnumerable?)GetValue(TagsSourceProperty);
        // 设置标签源属性的值
        set => SetValue(TagsSourceProperty, value);
    }


    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation), typeof(Orientation), typeof(TagSlider), new FrameworkPropertyMetadata(Orientation.Horizontal));

    /// <summary>
    ///     获取或设置滑块的方向。默认为水平方向。
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty DelayProperty = RepeatButton.DelayProperty.AddOwner(typeof(TagSlider), new FrameworkPropertyMetadata(GetKeyboardDelay()));

    /// <summary>
    ///     获取或设置滑动延迟时间。默认为键盘延迟时间。
    /// </summary>
    public int Delay
    {
        get => (int)GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    public static readonly DependencyProperty IntervalProperty = RepeatButton.IntervalProperty.AddOwner(typeof(TagSlider), new FrameworkPropertyMetadata(GetKeyboardSpeed()));

    /// <summary>
    ///     获取或设置滑动间隔时间。默认为键盘速度。
    /// </summary>
    public int Interval
    {
        get => (int)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register(
        nameof(AutoToolTipPlacement), typeof(AutoToolTipPlacement), typeof(TagSlider), new FrameworkPropertyMetadata(AutoToolTipPlacement.None), IsValidAutoToolTipPlacement);

    /// <summary>
    ///     获取或设置自动显示工具提示的位置。默认为不显示。
    /// </summary>
    public AutoToolTipPlacement AutoToolTipPlacement
    {
        get => (AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty);
        set => SetValue(AutoToolTipPlacementProperty, value);
    }

    public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
        nameof(AutoToolTipPrecision), typeof(int), typeof(TagSlider), new FrameworkPropertyMetadata(0), IsValidAutoToolTipPrecision);

    /// <summary>
    ///     获取或设置自动显示工具提示的精度。默认为0。
    /// </summary>
    public int AutoToolTipPrecision
    {
        get => (int)GetValue(AutoToolTipPrecisionProperty);
        set => SetValue(AutoToolTipPrecisionProperty, value);
    }

    public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register(
        nameof(IsSnapToTickEnabled), typeof(bool), typeof(TagSlider), new FrameworkPropertyMetadata(false));

    /// <summary>
    ///     获取或设置是否启用滑块跳格功能。默认为不启用。
    /// </summary>
    public bool IsSnapToTickEnabled
    {
        get => (bool)GetValue(IsSnapToTickEnabledProperty);
        set => SetValue(IsSnapToTickEnabledProperty, value);
    }

    public static readonly DependencyProperty TickPlacementProperty = DependencyProperty.Register(
        nameof(TickPlacement), typeof(TickPlacement), typeof(TagSlider), new FrameworkPropertyMetadata(TickPlacement.None), IsValidTickPlacement);

    /// <summary>
    ///     获取或设置滑块刻度的放置位置。默认为不放置。
    /// </summary>
    public TickPlacement TickPlacement
    {
        get => (TickPlacement)GetValue(TickPlacementProperty);
        set => SetValue(TickPlacementProperty, value);
    }

    public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
        nameof(TickFrequency), typeof(double), typeof(TagSlider), new FrameworkPropertyMetadata(1.0), IsValidDoubleValue);

    /// <summary>
    ///     获取或设置刻度间隔。
    /// </summary>
    public double TickFrequency
    {
        get => (double)GetValue(TickFrequencyProperty);
        set => SetValue(TickFrequencyProperty, value);
    }

    public static readonly DependencyProperty TicksProperty = DependencyProperty.Register(
        nameof(Ticks), typeof(DoubleCollection), typeof(TagSlider), new FrameworkPropertyMetadata(new DoubleCollection()));

    /// <summary>
    ///     获取或设置刻度集合。
    /// </summary>
    public DoubleCollection? Ticks
    {
        get => (DoubleCollection?)GetValue(TicksProperty);
        set => SetValue(TicksProperty, value);
    }

    public static readonly DependencyProperty IsMoveToPointEnabledProperty = DependencyProperty.Register(
        nameof(IsMoveToPointEnabled), typeof(bool), typeof(TagSlider), new FrameworkPropertyMetadata(false));


    /// <summary>
    ///     获取或设置一个布尔值，该值指示是否启用移动到点功能。
    /// </summary>
    public bool IsMoveToPointEnabled
    {
        get => (bool)GetValue(IsMoveToPointEnabledProperty);
        set => SetValue(IsMoveToPointEnabledProperty, value);
    }

    #region 依赖属性方法

    /// <summary>
    ///     获取键盘延迟时间
    /// </summary>
    /// <returns>键盘延迟时间（以毫秒为单位）</returns>
    internal static int GetKeyboardDelay()
    {
        var delay = SystemParameters.KeyboardDelay;

        // 如果延迟时间小于0或大于3，则将延迟时间设为0
        if (delay is < 0 or > 3)
            delay = 0;

        // 返回延迟时间加1后的值乘以250
        return (delay + 1) * 250;
    }

    /// <summary>
    ///     获取键盘速度。
    /// </summary>
    /// <returns>键盘速度，取值范围为0到31。</returns>
    internal static int GetKeyboardSpeed()
    {
        // 获取键盘速度。
        var speed = SystemParameters.KeyboardSpeed;

        // 如果速度小于0或大于31，则设置为31。
        if (speed is < 0 or > 31)
            speed = 31;

        // 根据速度计算每分钟按键次数，并返回结果。
        return (31 - speed) * (400 - 1000 / 30) / 31 + 1000 / 30;
    }


    /// <summary>
    ///     判断传入的对象是否为有效的自动工具提示放置位置
    /// </summary>
    /// <param name="o">要进行验证的对象</param>
    /// <returns>如果传入的对象是None、TopLeft或BottomRight其中之一，则返回true；否则返回false</returns>
    private static bool IsValidAutoToolTipPlacement(object o)
    {
        // 将传入的对象强制转换为AutoToolTipPlacement类型，并赋值给placement变量
        var placement = (AutoToolTipPlacement)o;

        // 判断placement的值是否为None、TopLeft或BottomRight其中之一
        return placement is AutoToolTipPlacement.None or AutoToolTipPlacement.TopLeft or AutoToolTipPlacement.BottomRight;
    }

    /// <summary>
    ///     判断传入的对象是否为有效的自动工具提示精度。
    /// </summary>
    /// <param name="o">要进行判断的对象。</param>
    /// <returns>如果传入的对象是大于等于0的整数，则返回true；否则返回false。</returns>
    private static bool IsValidAutoToolTipPrecision(object o)
    {
        return (int)o >= 0;
    }

    /// <summary>
    ///     判断给定的对象 o 是否是有效的刻度放置选项
    /// </summary>
    /// <param name="o">要判断的对象</param>
    /// <returns>如果对象是有效的刻度放置选项，返回 true；否则返回 false</returns>
    private static bool IsValidTickPlacement(object o)
    {
        var value = (TickPlacement)o;
        return value is TickPlacement.None or TickPlacement.TopLeft or TickPlacement.BottomRight or TickPlacement.Both;
    }


    /// <summary>
    ///     判断传入的对象是否为有效的双精度浮点数。
    /// </summary>
    /// <param name="value">要进行判断的对象。</param>
    /// <returns>如果传入的对象是双精度浮点数，则返回true；否则返回false。</returns>
    private static bool IsValidDoubleValue(object value)
    {
        // 将传入的对象转换为双精度浮点数类型
        var d = (double)value;

        // 判断转换后的值是否为NaN（不是一个数）或无穷大
        return !(double.IsNaN(d) || double.IsInfinity(d));
    }

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    ///     静态构造函数，用于初始化静态成员和注册事件处理程序等。
    /// </summary>
    static TagSlider()
    {
        // 1. 初始化命令
        InitializeCommands();

        // 2. 覆盖依赖属性的元数据

        // 设置 MinimumProperty 的默认元数据，指定默认值为 0.0d，同时影响布局
        MinimumProperty.OverrideMetadata(typeof(TagSlider), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // 设置 MaximumProperty 的默认元数据，指定默认值为 10.0d，同时影响布局
        MaximumProperty.OverrideMetadata(typeof(TagSlider), new FrameworkPropertyMetadata(10.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // 设置 ValueProperty 的默认元数据，指定默认值为 0.0d，同时影响布局
        ValueProperty.OverrideMetadata(typeof(TagSlider), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // 3. 注册类处理程序

        // 注册 Thumb.DragStartedEvent 事件处理程序，处理 Thumb 拖动开始事件
        EventManager.RegisterClassHandler(typeof(TagSlider), Thumb.DragStartedEvent, new DragStartedEventHandler(OnThumbDragStarted));

        // 注册 Thumb.DragDeltaEvent 事件处理程序，处理 Thumb 拖动过程中的变化事件
        EventManager.RegisterClassHandler(typeof(TagSlider), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));

        // 注册 Thumb.DragCompletedEvent 事件处理程序，处理 Thumb 拖动完成事件
        EventManager.RegisterClassHandler(typeof(TagSlider), Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnThumbDragCompleted));

        // 注册 Mouse.MouseDownEvent 事件处理程序，处理鼠标左键按下事件，使用隧道方式（捕获阶段）
        EventManager.RegisterClassHandler(typeof(TagSlider), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);

        // 重写DefaultStyleKeyProperty_metadata方法，设置TagSlider类的默认样式键属性，并且设置默认样式键属性的类型为TagSlider
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TagSlider), new FrameworkPropertyMetadata(typeof(TagSlider)));
    }

    /// <summary>
    ///     默认构造函数，用于初始化 <see cref="TagSlider" /> 实例。
    /// </summary>
    public TagSlider()
    {
        // 添加命令绑定

        // 绑定 IncreaseLarge 命令，关联到 OnIncreaseLarge 方法
        CommandBindings.Add(new CommandBinding(IncreaseLarge, OnIncreaseLarge));

        // 绑定 IncreaseSmall 命令，关联到 OnIncrementSmall 方法
        CommandBindings.Add(new CommandBinding(IncreaseSmall, OnIncrementSmall));

        // 绑定 DecreaseLarge 命令，关联到 OnDecreaseLarge 方法
        CommandBindings.Add(new CommandBinding(DecreaseLarge, OnDecreaseLarge));

        // 绑定 DecreaseSmall 命令，关联到 OnDecreaseSmall 方法
        CommandBindings.Add(new CommandBinding(DecreaseSmall, OnDecreaseSmall));

        // 绑定 MinimizeValue 命令，关联到 OnMinimizeValue 方法
        CommandBindings.Add(new CommandBinding(MinimizeValue, OnMinimizeValue));

        // 绑定 MaximizeValue 命令，关联到 OnMaximizeValue 方法
        CommandBindings.Add(new CommandBinding(MaximizeValue, OnMaximizeValue));
    }

    #endregion

    #region Commands

    /// <summary>
    ///     增加大值的路由命令
    /// </summary>
    public static RoutedCommand IncreaseLarge { get; private set; }

    /// <summary>
    ///     增加小值的路由命令
    /// </summary>
    public static RoutedCommand IncreaseSmall { get; private set; }

    /// <summary>
    ///     减少大值的路由命令
    /// </summary>
    public static RoutedCommand DecreaseLarge { get; private set; }

    /// <summary>
    ///     减少小值的路由命令
    /// </summary>
    public static RoutedCommand DecreaseSmall { get; private set; }

    /// <summary>
    ///     最小化值的路由命令
    /// </summary>
    public static RoutedCommand MinimizeValue { get; private set; }

    /// <summary>
    ///     最大化值的路由命令
    /// </summary>
    public static RoutedCommand MaximizeValue { get; private set; }

    #region Commands - Events

    /// <summary>
    ///     设置 ValueProperty 的值为最大值
    /// </summary>
    private void OnMaximizeValue()
    {
        // 将 ValueProperty 的值设置为 Maximum
        SetCurrentValue(ValueProperty, Maximum);
    }

    /// <summary>
    ///     执行 OnMaximizeValue 方法
    /// </summary>
    /// <param name="sender">事件的发送者</param>
    /// <param name="e">包含事件数据的 ExecutedRoutedEventArgs</param>
    private static void OnMaximizeValue(object sender, ExecutedRoutedEventArgs e)
    {
        // 将 sender 强制转换为 TagSlider 类型，然后调用 OnMaximizeValue 方法
        (sender as TagSlider)?.OnMaximizeValue();
    }

    /// <summary>
    ///     设置 ValueProperty 的值为最小值
    /// </summary>
    private void OnMinimizeValue()
    {
        // 将 ValueProperty 的值设置为 Minimum
        SetCurrentValue(ValueProperty, Minimum);
    }

    /// <summary>
    ///     执行 OnMinimizeValue 方法
    /// </summary>
    /// <param name="sender">事件的发送者</param>
    /// <param name="e">包含事件数据的 ExecutedRoutedEventArgs</param>
    private static void OnMinimizeValue(object sender, ExecutedRoutedEventArgs e)
    {
        // 将 sender 强制转换为 TagSlider 类型，然后调用 OnMinimizeValue 方法
        (sender as TagSlider)?.OnMinimizeValue();
    }

    /// <summary>
    ///     减少大量值
    /// </summary>
    private void OnDecreaseLarge()
    {
        // 调用 MoveToNextTick 方法，将 LargeChange 作为参数传入
        MoveToNextTick(-LargeChange);
    }

    /// <summary>
    ///     执行 OnDecreaseLarge 方法
    /// </summary>
    /// <param name="sender">事件的发送者</param>
    /// <param name="e">包含事件数据的 ExecutedRoutedEventArgs</param>
    private static void OnDecreaseLarge(object sender, ExecutedRoutedEventArgs e)
    {
        // 将 sender 强制转换为 TagSlider 类型，然后调用 OnDecreaseLarge 方法
        (sender as TagSlider)?.OnDecreaseLarge();
    }

    /// <summary>
    ///     减少小量值
    /// </summary>
    private void OnDecreaseSmall()
    {
        // 调用 MoveToNextTick 方法，将 SmallChange 作为参数传入
        MoveToNextTick(-SmallChange);
    }

    /// <summary>
    ///     执行 OnDecreaseSmall 方法
    /// </summary>
    /// <param name="sender">事件的发送者</param>
    /// <param name="e">包含事件数据的 ExecutedRoutedEventArgs</param>
    private static void OnDecreaseSmall(object sender, ExecutedRoutedEventArgs e)
    {
        // 将 sender 强制转换为 TagSlider 类型，然后调用 OnDecreaseSmall 方法
        (sender as TagSlider)?.OnDecreaseSmall();
    }

    /// <summary>
    ///     增加小量值
    /// </summary>
    private void OnIncrementSmall()
    {
        // 调用 MoveToNextTick 方法，将 SmallChange 作为参数传入
        MoveToNextTick(SmallChange);
    }

    /// <summary>
    ///     执行 OnIncrementSmall 方法
    /// </summary>
    /// <param name="sender">事件的发送者</param>
    /// <param name="e">包含事件数据的 ExecutedRoutedEventArgs</param>
    private static void OnIncrementSmall(object sender, ExecutedRoutedEventArgs e)
    {
        // 将 sender 强制转换为 TagSlider 类型，然后调用 OnIncrementSmall 方法
        (sender as TagSlider)?.OnIncrementSmall();
    }

    /// <summary>
    ///     增加大量值
    /// </summary>
    private void OnIncreaseLarge()
    {
        // 调用 MoveToNextTick 方法，将 LargeChange 作为参数传入
        MoveToNextTick(LargeChange);
    }

    /// <summary>
    ///     执行 OnIncreaseLarge 方法
    /// </summary>
    /// <param name="sender">事件的发送者</param>
    /// <param name="e">包含事件数据的 ExecutedRoutedEventArgs</param>
    private static void OnIncreaseLarge(object sender, ExecutedRoutedEventArgs e)
    {
        // 将 sender 强制转换为 TagSlider 类型，然后调用 OnIncreaseLarge 方法
        (sender as TagSlider)?.OnIncreaseLarge();
    }

    #endregion

    #endregion


    #region Static Event Handlers

    /// <summary>
    ///     初始化命令
    /// </summary>
    private static void InitializeCommands()
    {
        // 创建一个名为 IncreaseLarge 的 RoutedCommand 实例，所有者类型为 TagSlider
        IncreaseLarge = new RoutedCommand(nameof(IncreaseLarge), typeof(TagSlider));
// 创建一个名为 IncreaseSmall 的 RoutedCommand 实例，所有者类型为 TagSlider
        IncreaseSmall = new RoutedCommand(nameof(IncreaseSmall), typeof(TagSlider));
// 创建一个名为 DecreaseLarge 的 RoutedCommand 实例，所有者类型为 TagSlider
        DecreaseLarge = new RoutedCommand(nameof(DecreaseLarge), typeof(TagSlider));
// 创建一个名为 DecreaseSmall 的 RoutedCommand 实例，所有者类型为 TagSlider
        DecreaseSmall = new RoutedCommand(nameof(DecreaseSmall), typeof(TagSlider));
// 创建一个名为 MinimizeValue 的 RoutedCommand 实例，所有者类型为 TagSlider
        MinimizeValue = new RoutedCommand(nameof(MinimizeValue), typeof(TagSlider));
// 创建一个名为 MaximizeValue 的 RoutedCommand 实例，所有者类型为 TagSlider
        MaximizeValue = new RoutedCommand(nameof(MaximizeValue), typeof(TagSlider));

// 注册命令处理器
// 当用户按下 Home 键时，执行 MinimizeValue 命令
        CommandHelpers.RegisterCommandHandler(typeof(TagSlider), MinimizeValue, OnMinimizeValue,
            Key.Home);

// 当用户按下 End 键时，执行 MaximizeValue 命令
        CommandHelpers.RegisterCommandHandler(typeof(TagSlider), MaximizeValue, OnMaximizeValue,
            Key.End);
    }


    /// <summary>
    ///     处理鼠标左键按下事件。
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="e">鼠标按钮事件参数</param>
    private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 检查是否为左键按下事件
        if (e.ChangedButton is not MouseButton.Left) return;

        if (sender is not TagSlider slider) return;
        // 检查事件源是否为TagSlider，并且键盘焦点不在内部
        if (!slider.IsKeyboardFocusWithin) e.Handled = slider.Focus() || e.Handled;

        if (slider._tagTrack?.Thumb is { IsMouseOver: true })
        {
            slider._currentThumb = slider._tagTrack.Thumb;
            slider._currentTrack = null;
        }
        else if (slider._tagTrack?.SuperTagPanel?.FindVisualChildren<TagRangeTrack>().Find(it => (it.ThumbStart?.IsMouseOver ?? false) || (it.ThumbEnd?.IsMouseOver ?? false)) is { } rangeTrack)
        {
            slider._currentThumb = rangeTrack.ThumbStart?.IsMouseOver ?? false ? rangeTrack.ThumbStart : rangeTrack.ThumbEnd;
            slider._currentTrack = rangeTrack;
        }

        slider._currentThumb?.StartDrag();
    }


    /// <summary>
    ///     处理滑块拖动完成后的事件
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
    {
        // 检查事件的发送者是否是TagSlider类型，如果是，就调用TagSlider的OnThumbDragCompleted方法
        (sender as TagSlider)?.OnThumbDragCompleted();
    }

    /// <summary>
    ///     处理滑块拖动完成后的事件
    /// </summary>
    private void OnThumbDragCompleted()
    {
        // 在这个方法中，没有实现任何逻辑
    }

    /// <summary>
    ///     处理滑块在拖动过程中的事件
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
    {
        // 检查事件的发送者是否是TagSlider类型，如果是，就调用TagSlider的OnThumbDragDelta方法
        (sender as TagSlider)?.OnThumbDragDelta(e);
    }

    /// <summary>
    ///     处理滑块在拖动过程中的事件
    /// </summary>
    /// <param name="e">事件参数</param>
    private void OnThumbDragDelta(DragDeltaEventArgs e)
    {
        if (Equals(e.OriginalSource, _tagTrack?.Thumb))
        {
            // 计算新的值
            var newValue = Value + _tagTrack.ValueFromDistance(e.HorizontalChange, e.VerticalChange);
            // 如果新的值在有效范围内，就更新滑块的值
            if (ValidateHelper.IsInRangeOfDouble(newValue)) UpdateValue(newValue);
        }
        else if (e.OriginalSource is TagRangeThumb rangeThumb)
        {
            // 计算新的值
            var newValue = (rangeThumb.IsStart ? _currentTrack!.ValueStart!.Value : _currentTrack!.ValueEnd!.Value) + _currentTrack!.ValueFromDistance(e.HorizontalChange, e.VerticalChange);
            // 如果新的值在有效范围内，就更新滑块的值
            if (!ValidateHelper.IsInRangeOfDouble(newValue)) return;
            UpdateValue(newValue, (_currentThumb as TagRangeThumb)!.IsStart);
            _currentTrack.SetCurrentValue(rangeThumb.IsStart ? TagRangeTrack.ValueStartProperty : TagRangeTrack.ValueEndProperty, newValue);
        }
    }


    /// <summary>
    ///     处理滑块开始拖动的事件
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private static void OnThumbDragStarted(object sender, DragStartedEventArgs e)
    {
        // 检查事件的发送者是否是TagSlider类型，如果是，就调用TagSlider的OnThumbDragStarted方法
        (sender as TagSlider)?.OnThumbDragStarted(e);
    }

    /// <summary>
    ///     处理滑块开始拖动的事件
    /// </summary>
    /// <param name="e">事件参数</param>
    private void OnThumbDragStarted(DragStartedEventArgs e)
    {
        // 检查事件的原始源是否是TagThumb类型，如果不是，就返回
        if (e.OriginalSource is not ITagThumb thumb) return;
        // 获取鼠标的位置
        _originThumbPoint = Mouse.GetPosition(thumb as Thumb);
        // 开始拖动
        thumb.StartDrag();
    }

    #endregion
}
