// 创建时间：2023-06-06-14:22
// 修改时间：2023-07-18-9:37

namespace LiveBoost.ToolKit.Tools;

public static class ObservableListExtension
{
    public static ObservableList<TSource> ToObservableList<TSource>(this IEnumerable<TSource> source) => new(source.ToList());

    public static List<TSource> ToList<TSource>(this IEnumerable source) => new(source.OfType<TSource>().ToList());

    public static ObservableList<TSource> ToObservableList<TSource>(this IList source) => source.ToList<TSource>().ToObservableList();

    public static List<TSource> ToList<TSource>(this IList source) => source.OfType<TSource>().ToList();
}
