// 创建时间：2023-09-17-13:00
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Controls;

[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(JggItem))]
public class JggListView : ListView
{
    protected override bool IsItemItsOwnContainerOverride(object item) => item is JggItem;

    protected override DependencyObject GetContainerForItemOverride()
    {
        var item = new JggItem();
        return item;
    }
}
