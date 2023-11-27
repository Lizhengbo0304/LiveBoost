namespace LiveBoost.Toolkit.Controls;

public sealed class TagTrack : FrameworkElement
{
    #region Ctor

    /// <summary>
    ///     静态构造函数，用于初始化 TagTrack 类型的静态成员。
    /// </summary>
    static TagTrack()
    {
        // 覆盖 IsEnabledProperty 的元数据，指定属性更改事件为 OnIsEnabledChanged 方法
        IsEnabledProperty.OverrideMetadata(typeof(TagTrack), new UIPropertyMetadata(OnIsEnabledChanged));
    }

    #endregion

    #region 私有、内部方法

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
            Mouse.Synchronize();
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
            if (componentLength > trackLength || double.IsNaN(componentLength)) componentLength = trackLength;
        }
    }

    #endregion

    #region 依赖属性

    /// <summary>
    ///     标记点的数据源
    /// </summary>
    public static readonly DependencyProperty TagsSourceProperty = DependencyProperty.Register(
        nameof(TagsSource), typeof(IEnumerable), typeof(TagTrack), new PropertyMetadata(default(IEnumerable)));

    /// <summary>
    ///     获取或设置标记点的数据源。
    /// </summary>
    public IEnumerable TagsSource
    {
        get => (IEnumerable)GetValue(TagsSourceProperty);
        set => SetValue(TagsSourceProperty, value);
    }

    /// <summary>
    ///     标识布局方向的依赖属性。
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation), typeof(Orientation), typeof(TagTrack),
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
        nameof(Minimum), typeof(double), typeof(TagTrack),
        new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

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
        nameof(Maximum), typeof(double), typeof(TagTrack),
        new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

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
    ///     标识值的依赖属性。
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(double), typeof(TagTrack),
        new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     获取或设置值。
    /// </summary>
    /// <remarks>
    ///     默认值为 0.0。
    /// </remarks>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion

    #region Field

    /// <summary>
    ///     用于表示增大按钮的 RepeatButton。
    /// </summary>
    private RepeatButton? _increaseButton;

    /// <summary>
    ///     获取或设置增大按钮的 RepeatButton。
    /// </summary>
    /// <remarks>
    ///     如果设置的按钮与减小按钮相同，则会抛出 <see cref="NotSupportedException" /> 异常。
    /// </remarks>
    public RepeatButton? IncreaseRepeatButton
    {
        get => _increaseButton;
        set
        {
            // 如果设置的按钮与减小按钮相同，则抛出异常
            if (_decreaseButton == value)
                throw new NotSupportedException("Track_SameButtons");

            // 更新组件
            UpdateComponent(_increaseButton, value);

            // 设置增大按钮
            _increaseButton = value;

            // 如果增大按钮为 null，则不进行后续操作
            if (_increaseButton == null)
                return;

            // 通知命令管理器重新评估命令
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    ///     用于表示减小按钮的 RepeatButton。
    /// </summary>
    private RepeatButton? _decreaseButton;

    /// <summary>
    ///     获取或设置减小按钮的 RepeatButton。
    /// </summary>
    /// <remarks>
    ///     如果设置的按钮与增大按钮相同，则会抛出 <see cref="NotSupportedException" /> 异常。
    /// </remarks>
    public RepeatButton? DecreaseRepeatButton
    {
        get => _decreaseButton;
        set
        {
            // 如果设置的按钮与增大按钮相同，则抛出异常
            if (Equals(_increaseButton, value))
                throw new NotSupportedException("SameButtons");

            // 更新组件
            UpdateComponent(_decreaseButton, value);

            // 设置减小按钮
            _decreaseButton = value;

            // 如果减小按钮为 null，则不进行后续操作
            if (_decreaseButton == null)
                return;

            // 通知命令管理器重新评估命令
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    ///     用于表示滑块拇指的 TagThumb。
    /// </summary>
    private TagThumb? _thumb;

    /// <summary>
    ///     获取或设置滑块拇指的 TagThumb。
    /// </summary>
    public TagThumb? Thumb
    {
        get => _thumb;

        set
        {
            // 更新组件
            UpdateComponent(_thumb, value);

            // 设置滑块拇指
            _thumb = value;
        }
    }

    /// <summary>
    ///     用于表示标记点的父级Panel
    /// </summary>
    private TagListView? _superTagPanel;

    /// <summary>
    ///     获取或设置标记点的父级Panel
    /// </summary>
    public TagListView? SuperTagPanel
    {
        get => _superTagPanel;
        set
        {
            // 更新组件
            UpdateComponent(_superTagPanel, value);

            // 设置滑块拇指
            _superTagPanel = value;
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
    ///     表示滑块拇指的中心偏移。用于定位拇指的中心位置。
    /// </summary>
    private double ThumbCenterOffset { get; set; } = double.NaN;

    #endregion

    #region Events

    /// <summary>
    ///     根据给定的点计算对应的值。
    /// </summary>
    /// <param name="pt">要计算值的点。</param>
    /// <returns>计算得到的值，确保在最小值和最大值之间。</returns>
    public double ValueFromPoint(Point pt)
    {
        // 根据方向选择计算方式，计算得到水平和垂直距离
        var horizontalDistance = pt.X - (Orientation != Orientation.Horizontal ? RenderSize.Width * 0.5 : ThumbCenterOffset);
        var verticalDistance = pt.Y - (Orientation == Orientation.Horizontal ? RenderSize.Height * 0.5 : ThumbCenterOffset);

        // 使用 ValueFromDistance 方法将距离转换为值，同时确保在最小值和最大值之间
        var calculatedValue = Value + ValueFromDistance(horizontalDistance, verticalDistance);
        return Math.Max(Minimum, Math.Min(Maximum, calculatedValue));
    }


    /// <summary>
    ///     根据水平和垂直距离计算对应的值。
    /// </summary>
    /// <param name="horizontal">水平距离。</param>
    /// <param name="vertical">垂直距离。</param>
    /// <returns>计算得到的值。</returns>
    public double ValueFromDistance(double horizontal, double vertical)
    {
        // 根据方向选择计算方式，水平方向使用水平距离乘以密度，垂直方向使用负值的垂直距离乘以密度
        return Orientation == Orientation.Horizontal ? horizontal * Density : -1.0 * vertical * Density;
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
            return;

        // 初始化 _visualChildren 数组，用于存储子视觉元素
        VisualChildren ??= new Visual?[4];

        // 移除旧控件
        if (oldValue != null)
        {
            // 从可视子元素中移除旧控件
            RemoveVisualChild(oldValue);

            // 在 _visualChildren 数组中查找并移除旧控件的引用
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

    /// <summary>
    ///     计算滑块各组件的长度，包括减小按钮、滑块拇指和增大按钮。
    /// </summary>
    /// <param name="arrangeSize">滑块的排列大小。</param>
    /// <param name="isVertical">指示滑块是否为垂直方向的布局。</param>
    /// <param name="superTagPanelLength">输出参数，表示superTagPanel的长度。</param>
    /// <param name="decreaseButtonLength">输出参数，表示减小按钮的长度。</param>
    /// <param name="thumbLength">输出参数，表示滑块拇指的长度。</param>
    /// <param name="increaseButtonLength">输出参数，表示增大按钮的长度。</param>
    private void ComputerLengths(Size arrangeSize, bool isVertical, out double superTagPanelLength, out double decreaseButtonLength, out double thumbLength, out double increaseButtonLength)
    {
        // 获取最小值
        var min = Minimum;

        // 计算值范围
        var range = Math.Max(0.0, Maximum - min);

        // 计算偏移值
        var offsetValue = Math.Min(range, Value - min);

        // 轨道长度
        double trackLength;

        if (isVertical)
        {
            // 如果为垂直方向，获取垂直方向的排列大小和滑块拇指的高度
            trackLength = arrangeSize.Height;
            thumbLength = Thumb?.DesiredSize.Height ?? 0.0;
        }
        else
        {
            // 如果为水平方向，获取水平方向的排列大小和滑块拇指的宽度
            trackLength = arrangeSize.Width;
            thumbLength = Thumb?.DesiredSize.Width ?? 0.0;
        }

        superTagPanelLength = trackLength;
        CoerceLength(ref superTagPanelLength, trackLength);

        // 根据滑块的长度限制滑块拇指的长度
        CoerceLength(ref thumbLength, trackLength);

        // 剩余轨道长度
        var remainingTrackLength = trackLength - thumbLength;

        // 计算减小按钮的长度
        decreaseButtonLength = remainingTrackLength * offsetValue / range;
        CoerceLength(ref decreaseButtonLength, remainingTrackLength);

        // 计算增大按钮的长度
        increaseButtonLength = remainingTrackLength - decreaseButtonLength;
        CoerceLength(ref increaseButtonLength, remainingTrackLength);

        // 计算滑块的密度，用于将值映射到长度
        Density = range / remainingTrackLength;
    }

    #endregion

    #region Override

    /// <summary>
    ///     根据索引获取可视子元素。
    /// </summary>
    /// <param name="index">子元素的索引。</param>
    /// <returns>在索引位置的可视子元素。</returns>
    protected override Visual? GetVisualChild(int index)
    {
        // 如果 _visualChildren 不为 null，且索引有效，返回对应索引处的元素
        if (VisualChildren != null && index >= 0 && index < VisualChildren.Length && VisualChildren[index] != null) return VisualChildren[index];

        // 如果索引无效，抛出参数异常
        throw new ArgumentOutOfRangeException(nameof(index), index, @"Visual_ArgumentOutOfRange");
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
            if (VisualChildren == null) return 0;

            // 遍历VisualChildren
            for (var i = 0; i < VisualChildren.Length; i++)
                // 如果VisualChildren中存在null元素，返回该元素的索引
                if (VisualChildren[i] == null)
                    return i;

            // 如果VisualChildren中不存在null元素，返回VisualChildren的长度
            return VisualChildren.Length;
        }
    }

    /// <summary>
    ///     重写 MeasureOverride 方法，用于确定此元素在布局过程中所需的大小。
    /// </summary>
    /// <param name="availableSize">此元素可以给定的可用大小，可以是无穷大。</param>
    /// <returns>此元素在布局过程中所需的大小。</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        // 初始化大小为 (0.0, 0.0)
        var size = new Size(0.0, 0.0);

        // 如果 Thumb 不为 null，测量 Thumb 并获取其期望大小
        if (Thumb == null) return size;
        Thumb.Measure(availableSize);
        size = Thumb.DesiredSize;
        // 返回计算得到的大小
        return size;
    }

    /// <summary>
    ///     重写 ArrangeOverride 方法，确定滑块及其子组件的最终排列。
    /// </summary>
    /// <param name="arrangeSize">安排的大小。</param>
    /// <returns>实际使用的大小。</returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        // 判断滑块的布局方向是否为垂直
        var isVertical = Orientation == Orientation.Vertical;

        // 计算各组件的长度
        ComputerLengths(arrangeSize, isVertical, out var superTagPanelLength, out var decreaseButtonLength, out var thumbLength, out var increaseButtonLength);

        // 初始化偏移量和子组件大小
        var offset = new Point();
        var pieceSize = arrangeSize;

        if (isVertical)
        {
            // 如果是垂直布局
            CoerceLength(ref superTagPanelLength, arrangeSize.Height);
            CoerceLength(ref decreaseButtonLength, arrangeSize.Height);
            CoerceLength(ref increaseButtonLength, arrangeSize.Height);
            CoerceLength(ref thumbLength, arrangeSize.Height);
            SuperTagPanel?.Arrange(new Rect(offset, pieceSize));
            // 设置增大按钮的偏移和大小
            offset.Y = 0.0;
            pieceSize.Height = increaseButtonLength;
            IncreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            // 设置减小按钮的偏移和大小
            offset.Y = increaseButtonLength + thumbLength;
            pieceSize.Height = decreaseButtonLength;
            DecreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            // 设置滑块拇指的偏移和大小
            offset.Y = increaseButtonLength;
            pieceSize.Height = thumbLength;
            Thumb?.Arrange(new Rect(offset, pieceSize));

            // 更新滑块拇指中心偏移
            ThumbCenterOffset = offset.Y + thumbLength / 2;
        }
        else
        {
            // 如果是水平布局
            CoerceLength(ref superTagPanelLength, arrangeSize.Width);
            CoerceLength(ref decreaseButtonLength, arrangeSize.Width);
            CoerceLength(ref increaseButtonLength, arrangeSize.Width);
            CoerceLength(ref thumbLength, arrangeSize.Width);
            SuperTagPanel?.Arrange(new Rect(offset, pieceSize));

            // 设置减小按钮的偏移和大小
            offset.X = 0.0;
            pieceSize.Width = decreaseButtonLength;
            DecreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            // 设置增大按钮的偏移和大小
            offset.X = decreaseButtonLength + thumbLength;
            pieceSize.Width = increaseButtonLength;
            IncreaseRepeatButton?.Arrange(new Rect(offset, pieceSize));

            // 设置滑块拇指的偏移和大小
            offset.X = decreaseButtonLength;
            pieceSize.Width = thumbLength;
            Thumb?.Arrange(new Rect(offset, pieceSize));

            // 更新滑块拇指中心偏移
            ThumbCenterOffset = offset.X + thumbLength / 2;
        }

        // 返回实际使用的大小
        return arrangeSize;
    }

    #endregion
}
