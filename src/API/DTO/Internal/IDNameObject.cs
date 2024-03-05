using System;
using Newtonsoft.Json;

namespace NauraaBot.API.DTO;

[Serializable]
public class IDNameObject
{
    [JsonProperty("@type")]
    public string Type { get; set; }
    [JsonProperty("reference")]
    public string Reference { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
}