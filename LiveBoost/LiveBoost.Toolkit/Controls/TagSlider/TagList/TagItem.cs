namespace LiveBoost.Toolkit.Controls;

public class TagItem : ContentControl
{
    /// <summary>
    ///     当鼠标左键按下时，此方法被调用。
    /// </summary>
    /// <param name="e">包含事件数据的 MouseButtonEventArgs。</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        // 查找TagItem的父级TagListView
        if (this.FindVisualParent<TagListView>() is not { } tagListView)
            // 如果找不到TagListView，则返回
            return;

        // 遍历TagListView的所有子项
        for (var i = 0; i < tagListView.Items.Count; i++)
        {
            // 获取TagListView的第i个子项
            var item = (TagItem)tagListView.ItemContainerGenerator.ContainerFromIndex(i);
            if (item == null)
                // 如果子项为null，则跳过当前循环
                continue;

            // 如果当前子项是被点击的TagItem，则将其设置为最顶层显示
            if (item == this)
                item.SetCurrentValue(Panel.ZIndexProperty, tagListView.Items.Count + 1);
            // 否则，保持其原有顺序不变
            else
                item.SetCurrentValue(Panel.ZIndexProperty, i);
        }
    }
}
