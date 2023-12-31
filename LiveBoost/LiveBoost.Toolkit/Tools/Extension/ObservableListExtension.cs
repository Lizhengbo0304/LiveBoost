﻿// 创建时间：2023-09-07-11:58
// 修改时间：2023-10-13-16:17

namespace LiveBoost.ToolKit.Tools;

public static class ObservableListExtension
{
    /// <summary>
    ///     将一个集合转换为可观察的列表
    /// </summary>
    /// <typeparam name="TSource"> 元素类型 </typeparam>
    /// <param name="source"> 源集合 </param>
    /// <returns> 可观察的列表 </returns>
    public static ObservableList<TSource> ToObservableList<TSource>(this IEnumerable<TSource> source) =>
        // 将源集合转换为列表，并使用列表创建可观察的列表
        new(source.ToList());

    /// <summary>
    ///     将一个非泛型的集合转换为列表
    /// </summary>
    /// <typeparam name="TSource"> 元素类型 </typeparam>
    /// <param name="source"> 源集合 </param>
    /// <returns> 列表 </returns>
    public static List<TSource> ToList<TSource>(this IEnumerable source) =>
        // 筛选出指定类型的元素，并将筛选后的元素转换为列表
        source.OfType<TSource>().ToList();

    /// <summary>
    ///     将一个列表转换为可观察的列表
    /// </summary>
    /// <typeparam name="TSource"> 元素类型 </typeparam>
    /// <param name="source"> 源列表 </param>
    /// <returns> 可观察的列表 </returns>
    public static ObservableList<TSource> ToObservableList<TSource>(this IList source) =>
        // 将源列表转换为列表，并使用列表创建可观察的列表
        source.ToList<TSource>().ToObservableList();

    /// <summary>
    ///     将一个列表转换为列表
    /// </summary>
    /// <typeparam name="TSource"> 元素类型 </typeparam>
    /// <param name="source"> 源列表 </param>
    /// <returns> 列表 </returns>
    public static List<TSource> ToList<TSource>(this IList source) =>
        // 筛选出指定类型的元素，并将筛选后的元素转换为列表
        source.OfType<TSource>().ToList();
}