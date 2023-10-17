// 创建时间：2023-10-10-10:51
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public class ShouluFormat
{
    static ShouluFormat()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("LiveBoost.Toolkit.Configs.ShouluFormat.json");
        if ( stream is not null )
        {
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            ShouluFormats = JsonConvert.DeserializeObject<List<ShouluFormat>>(json);
        }
        else
        {
            ShouluFormats = new List<ShouluFormat>();
        }
    }

    public static List<ShouluFormat> ShouluFormats { get; set; }

#region Property

    [JsonProperty("Format")] public string? Format { get; set; }

    [JsonProperty("videoRecordCodec")] public List<string>? VideoRecordCodec { get; set; }

    [JsonProperty("audioRecordCodec")] public List<string>? AudioRecordCodec { get; set; }

#endregion
}
