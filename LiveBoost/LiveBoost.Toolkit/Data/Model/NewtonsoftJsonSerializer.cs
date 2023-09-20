// 创建时间：2023-09-04-11:28
// 修改时间：2023-09-19-14:01

#region

using Flurl.Http.Configuration;

#endregion

namespace LiveBoost.Toolkit.
    Data;

public class NewtonsoftJsonSerializer : ISerializer
{
    private readonly JsonSerializerSettings? _settings;

    public NewtonsoftJsonSerializer(JsonSerializerSettings? settings = null) => _settings = settings;

    public string Serialize(object obj) => JsonConvert.SerializeObject(obj, _settings);

    public T? Deserialize<T>(string s) => JsonConvert.DeserializeObject<T>(s, _settings);

    public T? Deserialize<T>(Stream stream)
    {
        using var sr = new StreamReader(stream);
        using var jr = new JsonTextReader(sr);
        return JsonSerializer.CreateDefault(_settings).Deserialize<T>(jr);
    }
}
