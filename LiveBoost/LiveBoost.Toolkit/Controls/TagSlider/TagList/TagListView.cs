namespace LiveBoost.Toolkit.Controls;

[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TagItem))]
public class TagListView : ItemsControl
{
    /// <summary>
    ///     重写IsItemItsOwnContainerOverride方法，用于检查一个项是否是它自己的容器。
    ///     如果项是TagItem，则返回true，否则返回false。
    /// </summary>
    /// <param name="item">要检查的项。</param>
    /// <returns>如果项是TagItem，则返回true；否则返回false。</returns>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is TagItem;

    /// <summary>
    ///     重写GetContainerForItemOverride方法，用于获取一个项的容器。IsItemItsOwnContainerOverride为false时，此方法被调用。
    ///     返回一个新的TagItem。
    /// </summary>
    /// <returns>一个新的TagItem。</returns>
    protected override DependencyObject GetContainerForItemOverride() => new TagItem();
}
