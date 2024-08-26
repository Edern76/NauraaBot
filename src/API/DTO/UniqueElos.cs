using Newtonsoft.Json;

namespace NauraaBot.API.DTO;

public struct UniqueElos
{
    [JsonProperty("elo")] public double? Elo;
    [JsonProperty("avg_elo")] public double? AverageFamilyElo;
}