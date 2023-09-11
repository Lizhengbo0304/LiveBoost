// 创建时间：2023-06-09-9:31
// 修改时间：2023-07-18-9:37

#region

#endregion

namespace LiveBoost.Tools;

public static class RecordProgramExtension
{
    // 获取子节点
    public static async Task<ObservableList<RecordFile>> GetRecordChildren(this string id, int type)
    {
        var children = await id.GetRecordPrograms(type).ConfigureAwait(false);
        if ( children.Any() )
        {
            children.ForEach(it =>
            {
                it.SearchType = type;
                if ( it.Type == 1 )
                {
                    it.Children = new ObservableList<RecordFile> {new()};
                }
            });
        }

        return children;
    }

    public static async Task<ObservableList<RecordFile>> GetRecordChildren(this RecordFile? program, int type)
    {
        if ( string.IsNullOrEmpty(program?.Id) )
        {
            return new ObservableList<RecordFile>();
        }
        var children = await program!.Id!.GetRecordPrograms(type);
        if ( children.Any() )
        {
            children.ForEach(it =>
            {
                it.SearchType = type;
                it.Parent = program;
                it.ParentFile = program;
                if ( it.Type == 1 )
                {
                    it.Children = new ObservableList<RecordFile> {new()};
                }
            });
        }

        return children;
    }
    public static string ToJson(this IEnumerable<RecordFile> recordFiles)
    {
        var files = recordFiles.Aggregate("[",
            (current, t) => current + "{\"id\":\"" + t.Id
                            + "\",\"isSub\":" + ( t.IsSub ? "true" : "false" )
                            + ",\"url\":\"" + t.Url
                            + "\",\"thumb\":\"" + t.Thumb
                            + "\",\"createDate\":\"" + t.CreateDate?.ToString("yyyy-MM-dd HH:mm:ss")
                            + "\",\"createUser\":\"" + t.CreateUser
                            + "\",\"type\":" + t.Type
                            + ",\"name\":\"" + t.Name
                            + "\",\"status\":" + t.Status
                            + ",\"stream\":\"" + t.Stream
                            + "\",\"parentIds\":\"" + t.ParentIds
                            + "\",\"parentId\":\"" + t.ParentId
                            + "\",\"outPoint\":\"" + t.OutPoint?.ToString(@"hh\:mm\:ss")
                            + "\",\"realOutPoint\":\"" + t.OutPoint?.ToString(@"hh\:mm\:ss\.ffff")
                            + "\",\"outPoint1\":\"" +
                            ( t.OutPoint is null ? 0 : (int) t.OutPoint.Value.TotalMilliseconds )
                            + "\",\"inPoint\":\"" + t.InPoint?.ToString(@"hh\:mm\:ss")
                            + "\",\"realInPoint\":\"" + t.RealInPoint?.ToString(@"hh\:mm\:ss\.ffff")
                            + "\",\"inPoint1\":\"" + ( t.InPoint is null ? 0 : (int) t.InPoint.Value.TotalMilliseconds )
                            + "\"},");

        files = files.TrimEnd(',');
        files += "]";
        return files;
    }
}
