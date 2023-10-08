// 创建时间：2023-09-27-15:37
// 修改时间：2023-09-27-15:37

namespace LiveBoost.Toolkit.Data;

public class RecordServer
{
    [JsonProperty("clientId")] public string? ClientId { get; set; }

    [JsonProperty("clientName")] public string? ClientName { get; set; }
}
