// 创建时间：2023-09-04-14:36
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Toolkit.Controls;

public class ItemsAttach
{
    /// <summary>
    ///     是否绑定行号
    /// </summary>
    public static readonly DependencyProperty IsAttachListViewIndexProperty = DependencyProperty.RegisterAttached(
        "IsAttachListViewIndex",
        typeof(bool),
        typeof(ItemsAttach),
        new PropertyMetadata(default(bool), IsAttachIndexPropertyChanged));

    /// <summary>
    ///     行号
    /// </summary>
    public static readonly DependencyProperty IndexProperty = DependencyProperty.RegisterAttached(
        "Index",
        typeof(int),
        typeof(ItemsAttach),
        new PropertyMetadata(0));

    private static void IsAttachIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        switch ( d )
        {
            case ListViewItem { } listViewItem:
            {
                if ( listViewItem.FindVisualParent<ListView>() is not { } listView )
                {
                    return;
                }
                if ( listView is FlyListView flyListView )
                {
                    flyListView.ItemsChangedEventHandler += (_, _) =>
                        SetIndex(listViewItem, listView.ItemContainerGenerator.IndexFromContainer(listViewItem) + 1);
                }

                var index = listView.ItemContainerGenerator.IndexFromContainer(listViewItem) + 1;
                SetIndex(listViewItem, index);
                break;
            }
        }
    }

    /// <summary>
    ///     获取是否绑定行号
    /// </summary>
    public static bool GetIsAttachListViewIndex(DependencyObject element) => (bool) element.GetValue(IsAttachListViewIndexProperty);

    /// <summary>
    ///     设置是否绑定行号
    /// </summary>
    public static void SetIsAttachListViewIndex(DependencyObject element, bool value)
    {
        element.SetValue(IsAttachListViewIndexProperty, value);
    }

    /// <summary>
    ///     获取行号
    /// </summary>
    public static int GetIndex(DependencyObject element) => (int) element.GetValue(IndexProperty);

    /// <summary>
    ///     设置行号
    /// </summary>
    public static void SetIndex(DependencyObject element, int value)
    {
        element.SetValue(IndexProperty, value);
    }
}
