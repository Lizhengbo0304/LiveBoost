// 创建时间：2023-09-04-14:40
// 修改时间：2023-10-11-11:00

namespace LiveBoost.Toolkit.Tools;

public static class FindControlTool
{
    /// <summary>
    ///     在可视树中查找具有指定名称的可视子元素。
    /// </summary>
    /// <typeparam name = "T" > 要查找的子元素的类型。 </typeparam>
    /// <param name = "parent" > 要查找子元素的父元素。 </param>
    /// <param name = "childName" > 要查找的子元素的名称。 </param>
    /// <returns> 找到的子元素，如果未找到则为null。 </returns>
    public static T? FindVisualChild<T>(this DependencyObject? parent, string? childName = null)
        where T : FrameworkElement
    {
        // 如果parent为空，则直接返回
        if ( parent == null )
        {
            return null;
        }
        // 创建一个队列，用于存储待处理的元素
        var queue = new Queue<DependencyObject>();
        queue.Enqueue(parent);

        // 循环处理队列中的元素，直到队列为空
        while ( queue.Count > 0 )
        {
            // 取出队列中的当前元素
            var current = queue.Dequeue();

            switch ( current )
            {
                // 如果当前元素是TChild类型，并且其名称与指定的子元素名称相同，则返回当前元素作为子元素
                case T child when string.IsNullOrEmpty(childName) || child.Name == childName:
                    return child;
            }

            // 遍历当前元素的所有子元素
            for ( var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++ )
            {
                // 获取当前子元素
                var childElement = VisualTreeHelper.GetChild(current, i);

                // 将当前子元素加入队列，以便后续处理
                queue.Enqueue(childElement);
            }
        }

        // 如果在父元素的所有子元素中未找到具有指定名称的子元素，则返回null
        return null;
    }

    /// <summary>
    ///     在可视树中查找具有指定类型的所有子元素。
    /// </summary>
    /// <typeparam name = "T" > 要查找的子元素的类型。 </typeparam>
    /// <param name = "parent" > 要查找子元素的父元素。 </param>
    /// <param name = "childName" > 要查找的子元素的名称 </param>
    /// <returns> 找到的子元素列表。 </returns>
    public static List<T> FindVisualChildren<T>(this DependencyObject? parent, string? childName = null)
        where T : FrameworkElement
    {
        var children = new List<T>();

        // 确认父元素不为空
        if ( parent == null )
        {
            return children;
        }

        var queue = new Queue<DependencyObject>();
        queue.Enqueue(parent);

        while ( queue.Count > 0 )
        {
            var current = queue.Dequeue();
            switch ( current )
            {
                // 如果当前元素是TChild类型，并且其名称与指定的子元素名称相同，则返回当前元素作为子元素
                case T child when string.IsNullOrEmpty(childName) || child.Name == childName:
                    children.Add(child);
                    break;
            }
            // 遍历当前元素的所有子元素
            for ( var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++ )
            {
                var child = VisualTreeHelper.GetChild(current, i);
                // 将子元素加入队列，以便后续处理
                queue.Enqueue(child);
            }
        }

        return children;
    }

    /// <summary>
    ///     在可视树中查找具有指定类型的父控件。
    /// </summary>
    /// <typeparam name = "T" > 要查找的父控件的类型。 </typeparam>
    /// <param name = "child" > 要查找父控件的子控件。 </param>
    /// <param name = "childName" > 要查找的父控件的名称。 </param>
    /// <returns> 找到的父控件，如果未找到则为null。 </returns>
    public static T? FindVisualParent<T>(this DependencyObject? child, string? childName = null)
        where T : FrameworkElement
    {
        // 如果子控件为空，则直接返回null
        if ( child == null )
        {
            return null;
        }

        var queue = new Queue<DependencyObject>();
        queue.Enqueue(child);

        while ( queue.Count > 0 )
        {
            var current = queue.Dequeue();

            switch ( current )
            {
                // 如果当前元素是目标类型的实例，并且其名称与指定的名称相同，则返回当前元素作为父控件
                case T parent when string.IsNullOrEmpty(childName) || parent.Name == childName:
                    return parent;
            }

            // 获取当前元素的父控件
            var parentElement = VisualTreeHelper.GetParent(current);
            if ( parentElement != null )
            {
                // 将父控件加入队列，以便后续处理
                queue.Enqueue(parentElement);
            }
        }

        // 如果在父控件的所有父控件中未找到具有指定名称的父控件，则返回null
        return null;
    }

    /// <summary>
    ///     在可视树中查找具有指定类型的父控件。
    /// </summary>
    /// <typeparam name = "T" > 要查找的父控件的类型。 </typeparam>
    /// <param name = "child" > 要查找父控件的子控件。 </param>
    /// <param name = "childName" > 要查找的父控件的名称。 </param>
    /// <returns> 找到的父控件集合，如果未找到则为空列表。 </returns>
    public static List<T> FindVisualParents<T>(this DependencyObject? child, string? childName = null)
        where T : FrameworkElement
    {
        var list = new List<T>();
        // Confirm parent and childName are valid.
        if ( child == null )
        {
            return list;
        }
        var queue = new Queue<DependencyObject>();
        queue.Enqueue(child);
        while ( queue.Count > 0 )
        {
            var current = queue.Dequeue();
            switch ( current )
            {
                // 如果当前元素是目标类型的实例，并且其名称与指定的名称相同，则返回当前元素作为父控件
                case T parent when string.IsNullOrEmpty(childName) || parent.Name == childName:
                    list.Add(parent);
                    break;
            }
            var parent2 = VisualTreeHelper.GetParent(child);
            if ( parent2 is not null )
            {
                queue.Enqueue(parent2);
            }
        }

        return list;
    }
}
