// 创建时间：2023-09-26-15:15
// 修改时间：2023-10-13-16:17

#region

using Flurl;

#endregion

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     异步搜索记录通道。
    /// </summary>
    /// <param name="searchWords"> 搜索关键词。 </param>
    /// <param name="pageNum"> 页数。 </param>
    /// <param name="pageSize"> 每页大小（默认为20.0）。 </param>
    /// <returns> 包含记录通道列表和总页数的元组。 </returns>
    public static async Task<(List<RecordChannel> channels, int totalPage)> SearchRecordChannelsAsync(this string searchWords, int pageNum, int pageSize = 20)
    {
        // 构建API请求的URL
        var url = $"{AppConfig.Instance.MamApiIp}/record/channel/list";

        // 设置查询参数
        var request = url.SetQueryParam("channelName", searchWords)
            .SetQueryParam("pageNum", pageNum)
            .SetQueryParam("pageSize", pageSize)
            .WithTimeout(5);

        return await request.Get(response =>
            {
                // 解析JSON响应
                var jobj = JObject.Parse(response);

                if (jobj["total"]?.Value<int>() is not { } total || jobj["rows"] is not JArray rows)
                {
                    return (new List<RecordChannel>(), 0);
                }

                // 将 JArray 转换为 List<RecordChannel>
                var channels = rows.ToObject<List<RecordChannel>>();
                return (channels.ToList(), (int)Math.Ceiling(total * 1.0 / pageSize));
                // 返回默认值，表示未找到匹配的记录通道
            },
            response =>
            {
                // 处理字段错误的响应数据
                var jobj = JObject.Parse(response);
                // 显示错误消息
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "查询频道异常");
                return (new List<RecordChannel>(), 0);
            },
            e =>
            {
                // 处理异常情况
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "查询频道异常");
                e.LogUrlError("查询频道异常");
                return (new List<RecordChannel>(), 0);
            });
    }

    /// <summary>
    ///     异步搜索任务列表。
    /// </summary>
    /// <param name="missionName"> 搜索关键词。 </param>
    /// <param name="clientName"> 服务器名称 </param>
    /// <param name="pageNum"> 页数。 </param>
    /// <param name="pageSize"> 每页大小（默认为20.0）。 </param>
    /// <returns> 包含记录通道列表和总页数的元组。 </returns>
    public static async Task<(List<RecordMission> missions, int totalPage)> SearchRecordMissionsAsync(this string missionName, string? clientName, int pageNum, int pageSize = 20)
    {
        // 构建API请求的URL
        var url = $"{AppConfig.Instance.MamApiIp}/record/mission/list";

        // 设置查询参数
        var request = url.SetQueryParam("clientName", clientName)
            .SetQueryParam("missionName", missionName)
            .SetQueryParam("pageNum", pageNum)
            .SetQueryParam("pageSize", pageSize)
            .WithTimeout(5);

        return await request.Get(response =>
            {
                // 解析JSON响应
                var jobj = JObject.Parse(response);

                if (jobj["total"]?.Value<int>() is not { } total || jobj["rows"] is not JArray rows)
                {
                    return (new List<RecordMission>(), 0);
                }

                // 将 JArray 转换为 List<RecordChannel>
                var channels = rows.ToObject<List<RecordMission>>();
                return (channels.ToList(), (int)Math.Ceiling(total * 1.0 / pageSize));
                // 返回默认值，表示未找到匹配的记录通道
            },
            response =>
            {
                // 处理字段错误的响应数据
                var jobj = JObject.Parse(response);
                // 显示错误消息
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "查询频道异常");
                return (new List<RecordMission>(), 0);
            },
            e =>
            {
                // 处理异常情况
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "查询频道异常");
                e.LogUrlError("查询频道异常");
                return (new List<RecordMission>(), 0);
            });
    }

    /// <summary>
    ///     修改任务状态
    /// </summary>
    public static async Task<bool> EditMissionStatus(this RecordMission mission)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/mission/status";
        var para = new
        {
            missionId = mission.MissionId,
            status = mission.Status
        };
        return await url.Put(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "修改任务状态");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "修改任务状态");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "修改任务状态");
                e.LogUrlError("修改任务状态");
                return false;
            });
    }

    /// <summary>
    ///     新增任务
    /// </summary>
    public static async Task<bool> AddMission(this object para)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/mission";

        return await url.Post(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "新增任务");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "新增任务");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "新增任务");
                e.LogUrlError("新增任务");
                return false;
            });
    }

    /// <summary>
    ///     编辑任务
    /// </summary>
    public static async Task<bool> EditMission(this object para)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/mission";

        return await url.Put(para,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "编辑任务");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "编辑任务");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "编辑任务");
                e.LogUrlError("编辑任务");
                return false;
            });
    }

    /// <summary>
    ///     删除任务
    /// </summary>
    public static async Task<bool> DeleteMission(this RecordMission mission)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/mission/{mission.MissionId}";

        return await url.Delete(
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Success(jobj["msg"]?.ToString(), "删除任务");
                return true;
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.ToString(), "删除任务");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "删除任务");
                e.LogUrlError("删除任务");
                return false;
            });
    }
}