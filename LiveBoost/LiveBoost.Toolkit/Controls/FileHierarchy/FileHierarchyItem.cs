// 创建时间：2023-09-04-15:57
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Toolkit.Controls;

/// <summary>
///     表示文件层次结构中的一个文件项。
/// </summary>
public class FileHierarchyItem : IFileHierarchy
{
    /// <summary>
    ///     获取或设置文件项的名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置文件项的父文件。
    /// </summary>
    public IFileHierarchy? ParentFile { get; set; }

    /// <summary>
    ///     获取文件项的所有父级文件列表，包括自身。
    /// </summary>
    public List<IFileHierarchy> Parents
    {
        get
        {
            var list = new List<IFileHierarchy> {this};
            var item = ParentFile;
            while ( item != null )
            {
                list.Add(item);
                item = item.ParentFile;
            }

            list.Reverse();
            return list;
        }
    }
}
