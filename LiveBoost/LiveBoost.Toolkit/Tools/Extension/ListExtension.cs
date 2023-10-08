// 创建时间：2023-10-08-14:03
// 修改时间：2023-10-08-14:03

namespace LiveBoost.ToolKit.Tools;

public static class ListExtension
{
    /// <summary>
    /// 为实现了 IIndex 接口的对象列表设置 Index 属性值，从0开始自动递增。
    /// </summary>
    /// <typeparam name="T">实现了 IIndex 接口的对象类型。</typeparam>
    /// <param name="list">要设置 Index 属性的对象列表。</param>
    public static void SetIndexes<T>(this List<T> list) where T : IIndex
    {
        // 遍历列表中的每个元素
        for (var i = 0; i < list.Count; i++)
        {
            // 设置当前元素的 Index 属性为当前索引值
            var item = list[i]; // 将临时值存储为变量
            item.Index = i + 1;   // 修改变量的成员
            list[i] = item;   // 将修改后的变量重新赋值回列表
        }
    }
}
