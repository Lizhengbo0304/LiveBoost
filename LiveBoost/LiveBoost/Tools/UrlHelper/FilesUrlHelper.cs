// 创建时间：2023-09-07-15:31
// 修改时间：2023-09-07-15:51

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary> 收录列表查询 </summary>
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
            loginName = AppProgram.Instance.LoginUser!.LoginName,
            stamp,
            sign = Hash.Content(Md5Str + stamp)
        };

        try
        {
            var result = await url.PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jobj = JObject.Parse(result);

            // Success = 0 : 请求失败
            if ( jobj["success"]?.Value<int>() != 1 )
            {
                return new ObservableList<RecordFile>();
            }

            var programs =
                JsonConvert.DeserializeObject<ObservableList<RecordFile>>(jobj["data"]?["list"]?.ToString()
                                                                          ?? "[]");
            return programs ?? new ObservableList<RecordFile>();
        }
        catch ( Exception e )
        {
            e.LogUrlError("【收录】资源列表\r\n收录资源列表查询");
            return new ObservableList<RecordFile>();
        }
    }
    /// <summary> 添加标记点 </summary>
    /// <param
    ///     name = "item" >
    ///     标记点数据
    /// </param>
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

        try
        {
            var result = await url.PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jobj = JObject.Parse(result);

            // Success = 0 : 请求失败
            if ( jobj["success"]?.Value<int>() == 1 )
            {
                return jobj["entity"]?["id"]?.Value<string>();
            }

            MessageBox.Warning(jobj["msg"]?.Value<string>(), "新增标记点异常");
            return null;

        }
        catch ( Exception e )
        {
            MessageBox.Error(e.Message, "新增标记点异常");
            e.LogUrlError("新增标记点异常");
            return null;
        }
    }

    /// <summary> 获取标记点 </summary>
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
            channelId, url = fileUrl
        };

        try
        {
            var result = await url.PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jobj = JObject.Parse(result);

            // Success = 0 : 请求失败
            if ( jobj["success"]?.Value<int>() == 1 )
            {
                return JsonConvert.DeserializeObject<ObservableList<RecordMark>>(jobj["data"]?["list"]?.ToString()
                                                                                 ?? "[]");
            }

            MessageBox.Warning(jobj["msg"]?.Value<string>(), "查询标记点异常");
            return new ObservableList<RecordMark>();
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.Message, "新增标记点异常");
            e.LogUrlError("新增标记点异常");
            return new ObservableList<RecordMark>();
        }
    }
}
