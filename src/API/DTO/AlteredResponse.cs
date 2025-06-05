using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NauraaBot.API.DTO;

[Serializable]
public class AlteredResponse
{
    [JsonProperty("hydra:totalItems")] public int TotalItems { get; set; }
    [JsonProperty("hydra:member")] public List<CardDTO> Members { get; set; }

    [JsonProperty("hydra:view")] public HydraView? View { get; set; }
}

[Serializable]
public class HydraView
{
    [JsonProperty("hydra:next")] public string? Next { get; set; }
    [JsonProperty("hydra:previous")] public string? Previous { get; set; }
    [JsonProperty("hydra:first")] public string? First { get; set; }
    [JsonProperty("hydra:last")] public string? Last { get; set; }
}