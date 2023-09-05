﻿// 创建时间：2023-09-05-14:54
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Data;

// 收录通道
public class RecordServerConfig
{
    [JsonProperty("clientId")] public string? ClientId { get; set; }

    [JsonProperty("clientName")] public string? ClientName { get; set; }

    [JsonProperty("accessNumber")] public int AccessNumber { get; set; }

    [JsonProperty("accessConfig")] public AccessConfig? AccessConfig { get; set; }
}

// 通道配置
public class AccessConfig
{
    [JsonProperty("IP")] public int Ip { get; set; }

    [JsonProperty("SDI")] public int Sdi { get; set; }

    [JsonProperty("PUSH")] public int Push { get; set; }
}
