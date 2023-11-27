namespace LiveBoost.Toolkit.Controls;

[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TagItem))]
public class TagListView : ItemsControl
{
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is TagItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new TagItem();
    }
}
