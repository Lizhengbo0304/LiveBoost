// 创建时间：2023-09-07-15:28
// 修改时间：2023-10-13-16:17

using Newtonsoft.Json.Serialization;

namespace LiveBoost.Tools;

public static class RecordProgramExtension
{
    // 获取子节点
    public static async Task<ObservableList<RecordFile>> GetRecordChildren(this string id, int type)
    {
        var children = await id.GetRecordPrograms(type).ConfigureAwait(false);
        if (children.Any())
        {
            children.ForEach(it =>
            {
                it.SearchType = type;
                if (it.Type == 1)
                {
                    it.Children = new ObservableList<RecordFile> { new() };
                }
            });
        }

        return children;
    }

    public static async Task<ObservableList<RecordFile>> GetRecordChildren(this RecordFile? program, int type)
    {
        if (string.IsNullOrEmpty(program?.Id))
        {
            return new ObservableList<RecordFile>();
        }

        var children = await program!.Id!.GetRecordPrograms(type);
        if (children.Any())
        {
            children.ForEach(it =>
            {
                it.SearchType = type;
                it.Parent = program;
                it.ParentFile = program;
                if (it.Type == 1)
                {
                    it.Children = new ObservableList<RecordFile> { new() };
                }
            });
        }

        return children;
    }


    /// <summary>
    /// 将 <see cref="IEnumerable{RecordFile}"/> 转换为 JSON 格式的字符串。
    /// </summary>
    /// <param name="recordFiles">要转换的 <see cref="RecordFile"/> 集合。</param>
    /// <returns>JSON 格式的字符串。</returns>
    public static string ToJson(this IEnumerable<RecordFile> recordFiles)
    {
        // 使用LINQ投影将RecordFile集合转换为匿名类型的集合
        var files = recordFiles.Select(t => new
        {
            // 文件属性
            t.Id,
            t.IsSub,
            t.Url,
            t.Thumb,
            CreateDate = t.CreateDate?.ToString("yyyy-MM-dd HH:mm:ss"),
            t.CreateUser,
            t.Type,
            t.Name,
            t.Status,
            t.Stream,
            t.ParentIds,
            t.ParentId,

            // 时间属性处理
            OutPoint = t.OutPoint?.ToString(@"hh\:mm\:ss"),
            RealOutPoint = t.OutPoint?.ToString(@"hh\:mm\:ss\.ffff"),
            OutPoint1 = t.OutPoint?.TotalMilliseconds ?? 0,
            InPoint = t.InPoint?.ToString(@"hh\:mm\:ss"),
            RealInPoint = t.RealInPoint?.ToString(@"hh\:mm\:ss\.ffff"),
            InPoint1 = t.InPoint?.TotalMilliseconds ?? 0
        });
        // 设置JSON序列化选项，使用CamelCasePropertyNamesContractResolver实现驼峰命名
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None // 格式化输出，可选
        };
        // 使用JsonSerializer将匿名类型集合序列化为JSON字符串
        return JsonConvert.SerializeObject(files, jsonSerializerSettings);
    }

}
