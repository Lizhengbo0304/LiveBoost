// 创建时间：2023-09-07-15:31
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     查询收录列表的方法。
    /// </summary>
    /// <param name = "id" > 查询的ID。 </param>
    /// <param name = "type" > 查询类型（可选，默认为0）：0 - programList, 1 - myList, 其他 - publicList。 </param>
    /// <returns> 查询到的收录列表。 </returns>
    public static async Task<ObservableList<RecordFile>> GetRecordPrograms(this string id, int type = 0)
    {
        var param = type switch
        {
            0 => "programList",
            1 => "myList",
            _ => "publicList"
        };

        var url = $"{AppConfig.Instance.MamCommonIp}/a/rest/record/{param}";
        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            id,
            froms = "liveBoostWindows",
            loginName = AppProgram.Instance.LoginUser?.LoginName,
            stamp,
            sign = Hash.Content(Md5Str + stamp)
        };

        // 调用通用的Post方法来执行查询操作，如果为空则返回空列表
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);

                var programs = JsonConvert.DeserializeObject<ObservableList<RecordFile>>(
                    jobj["data"]?["list"]?.ToString() ?? "[]"
                );
                return programs ?? new ObservableList<RecordFile>();
            },
            _ => new ObservableList<RecordFile>(),
            e =>
            {
                // 处理异常情况，记录日志并返回空列表
                e.LogUrlError("【收录】资源列表\r\n收录资源列表查询");
                return new ObservableList<RecordFile>();
            }) ?? new ObservableList<RecordFile>();
    }

    /// <summary>
    ///     添加标记点的方法。
    /// </summary>
    /// <param name = "item" > 标记点数据。 </param>
    /// <returns> 新添加标记点的ID。 </returns>
    public static async Task<string?> SaveMark(this RecordMark item)
    {
        var url = $"{AppConfig.Instance.MamCommonIp}/a/rest/record/marker/save";
        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            loginName = AppProgram.Instance.LoginUser!.LoginName,
            froms = "liveBoostWindows",
            stamp,
            sign = Hash.Content(Md5Str + stamp),
            id = item.Id,
            recordId = item.RecordId,
            name = item.Name,
            inPoint = item.InPoint,
            outPoint = item.OutPoint,
            description = item.Description,
            duration = item.DurationStr,
            thumb = item.Thumb,
            url = item.Url,
            channelId = item.ChannelId
        };

        // 调用通用的Post方法来执行添加标记点操作，如果为空则返回null
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                // 返回新添加标记点的ID
                return jobj["entity"]?["id"]?.Value<string>();
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                // 处理字段错误响应数据，显示错误消息并返回null
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "新增标记点异常");
                return null;
            },
            e =>
            {
                // 处理异常情况，显示错误消息并记录日志
                MessageBox.Error(e.Message, "新增标记点异常");
                e.LogUrlError("新增标记点异常");
                return null;
            });
    }


    /// <summary>
    ///     获取标记点的方法。
    /// </summary>
    /// <param name = "channelId" > 标记点所属的频道ID。 </param>
    /// <param name = "fileUrl" > 标记点关联的文件URL。 </param>
    /// <returns> 标记点列表。 </returns>
    public static async Task<ObservableList<RecordMark>> GetMarks(this string channelId, string fileUrl)
    {
        var url = $"{AppConfig.Instance.MamCommonIp}/a/rest/record/markerList";
        var stamp = DateTimeHelper.GetTimeStamp();
        var para = new
        {
            loginName = AppProgram.Instance.LoginUser!.LoginName,
            froms = "liveBoostWindows",
            stamp,
            sign = Hash.Content(Md5Str + stamp),
            channelId,
            url = fileUrl
        };
        // 调用通用的Post方法来执行查询标记点操作，如果为空则返回空列表
        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);

                // 解析标记点列表
                return JsonConvert.DeserializeObject<ObservableList<RecordMark>>(
                    jobj["data"]?["list"]?.ToString() ?? "[]"
                );
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                // 处理字段错误响应数据，显示错误消息并返回空列表
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "查询标记点异常");
                return new ObservableList<RecordMark>();
            },
            e =>
            {
                // 处理异常情况，显示错误消息并记录日志
                MessageBox.Error(e.Message, "查询标记点异常");
                e.LogUrlError("查询标记点异常");
                return new ObservableList<RecordMark>();
            }) ?? new ObservableList<RecordMark>();
    }
}
