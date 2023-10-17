// 创建时间：2023-09-27-15:37
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public class RecordServer
{
    [JsonProperty("clientId")] public string? ClientId { get; set; }

    [JsonProperty("clientName")] public string? ClientName { get; set; }
    [JsonProperty("recordPath")] public string? RecordPath { get; set; }

    public string ServerInfo => $"{ClientName} | {RecordPath}";
}
