// 创建时间：2023-09-04-15:57
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Toolkit.Controls;

public class FileHierarchyItemSelector : StyleSelector
{
    public override Style? SelectStyle(object item, DependencyObject container)
    {
        // 检查容器是否为ListViewItem，并获取其父级ListView
        if ( container is not ListViewItem listViewItem || listViewItem.FindVisualParent<ListView>() is not { } listView )
        {
            return null;
        }

        // 检查当前项是否为最后一项
        if ( listView.ItemContainerGenerator.IndexFromContainer(listViewItem) == listView.Items.Count - 1 )
        {
            // 返回最后一项的样式
            return Application.Current.TryFindResource("FileHierarchyListItemStyle") as Style;
        }
        // 返回默认样式
        return Application.Current.TryFindResource("FileHierarchyDefaultListItemStyle") as Style;
    }
}
