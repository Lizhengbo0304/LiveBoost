// 创建时间：2023-09-07-11:58
// 修改时间：2023-09-19-14:02

#region

using LiveBoost.ToolKit.Data;

#endregion

namespace LiveBoost.ToolKit.Tools;

public class RecordTemplateConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if ( value is not RecordTemplate recordTemplate )
        {
            return;
        }
        writer.WriteStartObject();
        writer.WritePropertyName("id");
        writer.WriteValue(recordTemplate.Id);
        writer.WritePropertyName("type");
        writer.WriteValue(recordTemplate.Type);
        writer.WritePropertyName("title");
        writer.WriteValue(recordTemplate.Title);
        writer.WritePropertyName("IsSelected");
        writer.WriteValue(recordTemplate.IsSelected);
        writer.WritePropertyName("mode");
        writer.WriteValue(recordTemplate.Mode);
        writer.WritePropertyName("info");
        var files = recordTemplate.RecordFiles.Aggregate("[",
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
        writer.WriteValue(files);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var recordTemplate = new RecordTemplate();
        if ( obj["type"]?.Value<int>() == 10 )
        {
            try
            {
                var recordFiles =
                    JsonConvert.DeserializeObject<ObservableList<RecordFile>>(obj["info"]?.ToString() ?? "[]");
                if ( recordFiles is not null )
                {
                    recordTemplate.RecordFiles = recordFiles;
                }
            }
            catch ( Exception )
            {
                // ignored
            }
        }

        serializer.Populate(obj.CreateReader(), recordTemplate);

        return recordTemplate;
    }

    public override bool CanConvert(Type objectType) => typeof(RecordTemplate).IsAssignableFrom(objectType);
}
