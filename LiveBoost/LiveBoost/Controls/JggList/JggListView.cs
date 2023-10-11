// 创建时间：2023-09-20-9:14
// 修改时间：2023-10-11-10:59

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
