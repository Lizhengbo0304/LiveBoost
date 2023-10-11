// 创建时间：2023-09-04-15:57
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Toolkit.Controls;

[TemplatePart(Name = "FileHierarchyPopup", Type = typeof(Popup))]
[TemplatePart(Name = "FileHierarchyPopupListView", Type = typeof(Popup))]
public class FileHierarchy : Control
{
#region CTOR

    public FileHierarchy()
    {
        SizeChanged += (_, _) =>
        {
            var charWidth = Math.Round(SeparatorChar.ToString().GetTextWidth(FontWeight, FontFamily, FontSize), MidpointRounding.AwayFromZero) + 10;
            var width = Math.Round("· · ·".GetTextWidth(FontWeight, FontFamily, FontSize), MidpointRounding.AwayFromZero);
            ItemMaxWidth = ( ActualWidth - charWidth * MaxShow - width ) / MaxShow;
            PopupWidth = ItemMaxWidth + 10;
        };
        PreviewMouseLeftButtonUpCmd = new DelegateCommand<MouseButtonEventArgs>(args =>
        {
            args.Handled = true;
            if ( args.OriginalSource is not TextBlock {Name: "NameBlock", DataContext: IFileHierarchy hierarchy} textBlock ||
                 textBlock.FindVisualParent<FileHierarchy>() is not { } fileHierarchy )
            {
                return;
            }
            if ( fileHierarchy.PopupItemsSource.Count > 0 && fileHierarchy.ItemsSource[1] == hierarchy )
            {
                var listviewitem = textBlock.FindVisualParent<ListViewItem>();
                fileHierarchy.FileHierarchyPopup!.PlacementTarget = listviewitem;
                fileHierarchy.SetValue(PopupOffsetProperty, -( ( fileHierarchy.ItemMaxWidth - 26 ) / 2 ));
                fileHierarchy.FileHierarchyPopup.IsOpen = true;
            }
            else
            {
                fileHierarchy.Source = hierarchy;
            }
        });
    }

#endregion

#region Property

    public Popup? FileHierarchyPopup;
    public ListView? FileHierarchyPopupListView;

#endregion

#region Event

    private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ( d is not FileHierarchy fileHierarchy || e.NewValue is not IFileHierarchy hierarchy )
        {
            return;
        }
        var parents = hierarchy.Parents;
        if ( parents.Count > fileHierarchy.MaxShow )
        {
            var popupList = parents.GetRange(1, parents.Count - 3);
            parents.RemoveRange(1, parents.Count - 3);
            parents.Insert(1, new FileHierarchyItem {Name = "· · ·"});
            d.SetValue(ItemsSourceProperty, parents);
            d.SetValue(PopupItemsSourceProperty, popupList);
        }
        else
        {
            d.SetValue(ItemsSourceProperty, parents);
            d.SetValue(PopupItemsSourceProperty, new List<IFileHierarchy>());
        }
    }

    private static void CalculateMaxItemWidth(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ( d is not FileHierarchy fileHierarchy )
        {
            return;
        }
        var charWidth = Math.Round(fileHierarchy.SeparatorChar.ToString().GetTextWidth(fileHierarchy.FontWeight, fileHierarchy.FontFamily, fileHierarchy.FontSize), MidpointRounding.AwayFromZero) + 10;
        var width = Math.Round("· · ·".GetTextWidth(fileHierarchy.FontWeight, fileHierarchy.FontFamily, fileHierarchy.FontSize), MidpointRounding.AwayFromZero);
        fileHierarchy.ItemMaxWidth = ( fileHierarchy.ActualWidth - charWidth * fileHierarchy.MaxShow - width ) / fileHierarchy.MaxShow;
        fileHierarchy.PopupWidth = fileHierarchy.ItemMaxWidth + 10;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        FileHierarchyPopup = GetTemplateChild("PART_FileHierarchyPopup") as Popup;
        FileHierarchyPopupListView = GetTemplateChild("PART_FileHierarchyPopupListView") as ListView;
        if ( FileHierarchyPopupListView is not null )
        {
            FileHierarchyPopupListView.PreviewMouseLeftButtonDown += (_, args) =>
            {
                if ( args.OriginalSource is not TextBlock {Name: "NameBlock", DataContext: IFileHierarchy hierarchy} )
                {
                    return;
                }
                Source = hierarchy;
                if ( FileHierarchyPopup != null )
                {
                    FileHierarchyPopup.IsOpen = false;
                }
            };
        }
    }

#endregion

#region DependencyProperty

    /// <summary>
    ///     分隔符
    /// </summary>
    public static readonly DependencyProperty SeparatorCharProperty = DependencyProperty.Register(
        nameof(SeparatorChar), typeof(char), typeof(FileHierarchy), new PropertyMetadata('>', CalculateMaxItemWidth));

    /// <summary>
    ///     Popup宽度
    /// </summary>
    public static readonly DependencyProperty PopupWidthProperty = DependencyProperty.Register(
        nameof(PopupWidth), typeof(double), typeof(FileHierarchy), new PropertyMetadata(default(double)));

    /// <summary>
    ///     Popup宽度
    /// </summary>
    public double PopupWidth
    {
        get => (double) GetValue(PopupWidthProperty);
        set => SetValue(PopupWidthProperty, value);
    }

    /// <summary>
    ///     分隔符
    /// </summary>
    public char SeparatorChar
    {
        get => (char) GetValue(SeparatorCharProperty);
        set => SetValue(SeparatorCharProperty, value);
    }

    /// <summary>
    ///     Popup水平偏移
    /// </summary>
    public static readonly DependencyProperty PopupOffsetProperty = DependencyProperty.Register(
        nameof(PopupOffset), typeof(double), typeof(FileHierarchy), new PropertyMetadata(default(double)));

    /// <summary>
    ///     Popup水平偏移
    /// </summary>
    public double PopupOffset
    {
        get => (double) GetValue(PopupOffsetProperty);
        set => SetValue(PopupOffsetProperty, value);
    }

    /// <summary>
    ///     显示列表
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(List<IFileHierarchy>), typeof(FileHierarchy), new PropertyMetadata(default(List<IFileHierarchy>)));

    /// <summary>
    ///     显示列表
    /// </summary>
    public List<IFileHierarchy> ItemsSource
    {
        get => (List<IFileHierarchy>) GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    ///     Popup列表
    /// </summary>
    public static readonly DependencyProperty PopupItemsSourceProperty = DependencyProperty.Register(
        nameof(PopupItemsSource), typeof(List<IFileHierarchy>), typeof(FileHierarchy), new PropertyMetadata(new List<IFileHierarchy>()));

    /// <summary>
    ///     Popup列表
    /// </summary>
    public List<IFileHierarchy> PopupItemsSource
    {
        get => (List<IFileHierarchy>) GetValue(PopupItemsSourceProperty);
        set => SetValue(PopupItemsSourceProperty, value);
    }

    /// <summary>
    ///     子控件最大宽度
    /// </summary>
    public static readonly DependencyProperty ItemMaxWidthProperty = DependencyProperty.Register(
        nameof(ItemMaxWidth), typeof(double), typeof(FileHierarchy), new PropertyMetadata(default(double)));

    /// <summary>
    ///     最大展示数量
    /// </summary>
    public static readonly DependencyProperty MaxShowProperty = DependencyProperty.Register(
        nameof(MaxShow), typeof(int), typeof(FileHierarchy), new PropertyMetadata(3, CalculateMaxItemWidth));

    /// <summary>
    ///     当前数据
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source), typeof(IFileHierarchy), typeof(FileHierarchy), new PropertyMetadata(default(IFileHierarchy), SourcePropertyChanged));

    /// <summary>
    ///     当前数据
    /// </summary>
    public IFileHierarchy Source
    {
        get => (IFileHierarchy) GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    ///     子控件最大宽度
    /// </summary>
    public double ItemMaxWidth
    {
        get => (double) GetValue(ItemMaxWidthProperty);
        set => SetValue(ItemMaxWidthProperty, value);
    }

    /// <summary>
    ///     最大展示数量
    /// </summary>
    public int MaxShow
    {
        get => (int) GetValue(MaxShowProperty);
        set => SetValue(MaxShowProperty, value);
    }

    /// <summary>
    ///     鼠标点击
    /// </summary>
    public static readonly DependencyProperty PreviewMouseLeftButtonUpCmdProperty = DependencyProperty.Register(
        nameof(PreviewMouseLeftButtonUpCmd), typeof(DelegateCommand<MouseButtonEventArgs>), typeof(FileHierarchy), new PropertyMetadata(default(DelegateCommand<MouseButtonEventArgs>)));

    /// <summary>
    ///     鼠标点击
    /// </summary>
    public DelegateCommand<MouseButtonEventArgs> PreviewMouseLeftButtonUpCmd
    {
        get => (DelegateCommand<MouseButtonEventArgs>) GetValue(PreviewMouseLeftButtonUpCmdProperty);
        set => SetValue(PreviewMouseLeftButtonUpCmdProperty, value);
    }

#endregion
}
