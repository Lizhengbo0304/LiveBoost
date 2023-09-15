// 创建时间：2023-09-05-11:18
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Controls;

[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(CombinationItem))]
public class CombinationListView : ListView
{
    protected override bool IsItemItsOwnContainerOverride(object item) => item is CombinationItem;

    protected override DependencyObject GetContainerForItemOverride()
    {
        var item = new CombinationItem();
        return item;
    }
}
