// 创建时间：2023-09-07-15:01
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Views;

public partial class CombinationRecordFilesPage
{
    public CombinationRecordFilesPage()
    {
        InitializeComponent();
    }

    private void FlyListView_OnItemsSourceChangedEventHandler(object sender, RoutedEventArgs e)
    {
        if ( sender is not ListView listView )
        {
            return;
        }

        if ( listView.FindVisualChild<ScrollViewer>() is not { } scrollViewer )
        {
            return;
        }
        scrollViewer.ScrollToVerticalOffset(0);
        scrollViewer.ScrollToHorizontalOffset(0);
    }
}
