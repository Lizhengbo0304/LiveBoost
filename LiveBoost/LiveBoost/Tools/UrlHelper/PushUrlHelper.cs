// 创建时间：2023-09-07-11:57
// 修改时间：2023-09-19-14:01

#region

using LiveBoost.ToolKit.Data;

#endregion

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    /// <summary>
    ///     查询模板列表
    /// </summary>
    /// <param name = "templateType" >
    ///     1=任务模板，2=导出模板，3=推流模板，10-播单
    /// </param>
    /// <returns> </returns>
    public static async Task<List<RecordTemplate>> GetShouluTemplates(this int templateType)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/list/{templateType}";

        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser!.Token}")
                .GetStringAsync().ConfigureAwait(false);

            var projects = JsonConvert.DeserializeObject<List<RecordTemplate>>(result ?? "[]");
            return projects ?? new List<RecordTemplate>();
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "查询模板列表");
            e.LogUrlError("查询模板列表");
            return new List<RecordTemplate>();
        }
    }
    /// <summary>
    ///     获取推流通道
    /// </summary>
    public static async Task<List<PushAccess>?> GetPushAccess()
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/access/liststatus";

        try
        {
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}")
                .GetStringAsync().ConfigureAwait(false);

            var projects = JsonConvert.DeserializeObject<List<PushAccess>>(result ?? "[]");
            return projects ?? new List<PushAccess>();
        }
        catch ( Exception e )
        {
            MessageBox.Error(e.InnerException?.Message ?? e.Message, "获取推流通道");
            e.LogUrlError("获取推流通道");
            return new List<PushAccess>();
        }
    }
    /// <summary>
    ///     创建新的播单。
    /// </summary>
    /// <param name = "id" > 播单ID。 </param>
    /// <param name = "title" > 播单标题。 </param>
    /// <param name = "mode" > 播单模式。 </param>
    /// <returns> 创建播单是否成功。 </returns>
    public static async Task<bool> NewPlayList(string id, string title, int mode)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template";
        var para = new
        {
            id,
            title,
            type = 10,
            mode,
            info = "[]"
        };

        // 调用通用的Post方法来执行创建播单操作
        return await url.Post(para,
            _ => true,
            response =>
            {
                var jobj = JObject.Parse(response);
                // 创建播单失败，显示错误消息
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "新增播单");
                return false;
            },
            e =>
            {
                // 处理创建播单时的异常情况
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "新增播单");
                e.LogUrlError("新增播单");
                return false;
            });
    }

    /// <summary>
    ///     播单推流，start, stop, pause, restore 操作。
    /// </summary>
    /// <param name = "pushAccess" > 推流访问对象。 </param>
    /// <param name = "operate" > 操作类型（start, stop, pause, restore）。 </param>
    /// <returns> 操作是否成功。 </returns>
    public static async Task<bool> PlayListPush(this PushAccess pushAccess, string operate)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/access/push/{operate}";

        var para = new
        {
            accessId = pushAccess.AccessId,
            playList = pushAccess.RecordFiles.ToJson()
        };

        // 发送POST请求并异步接收字符串响应，同时传入处理委托
        return await url.Post(para,
            _ => true,
            response =>
            {
                // 处理字段错误响应，显示警告消息框
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "播单操作");
                return false;
            },
            e =>
            {
                // 处理异常，显示错误消息框，并记录异常日志
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "播单操作");
                e.LogUrlError("播单操作");
                return false;
            }).ConfigureAwait(false);
    }

    /// <summary>
    ///     导出视频的方法。
    /// </summary>
    /// <param name = "playListId" > 要导出的播单的ID。 </param>
    /// <param name = "exportName" > 导出的视频文件名。 </param>
    /// <param name = "templateId" > 播单模板的ID。 </param>
    public static async Task OutPlayList2Video(this string playListId, string exportName, string templateId)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/export";

        var para = new
        {
            exportName,
            playListId,
            templateId
        };

        // 调用通用的Post方法来执行导出视频操作，传入对应的处理委托
        await url.Post(para,
            _ =>
            {
                // 处理成功响应，显示成功消息框
                MessageBox.Success("导出视频成功", "导出视频");
            },
            response =>
            {
                // 处理字段错误响应，显示警告消息框
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "导出视频");
            },
            e =>
            {
                // 处理异常，显示错误消息框，并记录异常日志
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "导出视频");
                e.LogUrlError("导出视频");
            });
    }


    /// <summary>
    ///     导出播单为Xml的方法。
    /// </summary>
    /// <param name = "playListId" > 要导出的播单的ID。 </param>
    /// <param name = "exportName" > 导出的Xml文件名。 </param>
    public static async Task OutPlayList2Xml(this string playListId, string exportName)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template/exportxml";

        var para = new
        {
            exportName,
            playListId
        };

        // 调用通用的Post方法来执行导出Xml操作，成功时显示成功消息框，处理字段错误响应和异常情况
        await url.Post(para,
            _ =>
            {
                MessageBox.Success("导出Xml成功", "导出Xml");
            },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "导出Xml");
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "导出Xml");
                e.LogUrlError("导出Xml");
            });
    }


    /// <summary>
    ///     客户端播单推流播单修改的方法。
    /// </summary>
    /// <param name = "pushAccess" > 要编辑的推流播单访问对象。 </param>
    /// <returns> 操作是否成功的布尔值。 </returns>
    public static async Task<bool> EditPushPlayList(this PushAccess pushAccess)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/access/push/modify";

        var para = new
        {
            accessId = pushAccess.AccessId,
            playList = pushAccess.RecordFiles.ToJson()
        };

        // 调用通用的Post方法来执行编辑推流播单操作，成功时返回true，处理字段错误响应和异常情况返回false
        return await url.Post(para,
            _ => true,
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "编辑推流播单");
                return false;
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "编辑推流播单");
                e.LogUrlError("编辑推流播单");
                return false;
            });
    }


    /// <summary>
    ///     编辑播单的方法。
    /// </summary>
    /// <param name = "recordTemplate" > 要编辑的播单模板对象。 </param>
    public static async Task EditPlayList(this RecordTemplate recordTemplate)
    {
        var url = $"{AppConfig.Instance.MamApiIp}/record/template";

        // 调用通用的Post方法来执行编辑播单操作，成功时不执行任何操作，处理字段错误响应和异常情况
        await url.Put(recordTemplate,
            _ => { },
            response =>
            {
                var jobj = JObject.Parse(response);
                MessageBox.Warning(jobj["msg"]?.Value<string>(), "编辑播单");
            },
            e =>
            {
                MessageBox.Error(e.InnerException?.Message ?? e.Message, "编辑播单");
                e.LogUrlError("编辑播单");
            }).ConfigureAwait(false);
    }
}
