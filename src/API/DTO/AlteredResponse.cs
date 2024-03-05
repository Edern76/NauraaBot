using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NauraaBot.API.DTO;

[Serializable]
public class AlteredResponse
{
    [JsonProperty("hydra:totalItems")]
    public int TotalItems { get; set; }
    [JsonProperty("hydra:member")]
    public List<CardDTO> Members { get; set; }
}