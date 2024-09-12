using NauraaBot.Core.Utils.JSON;
using Newtonsoft.Json;

namespace NauraaBot.API.DTO;

public struct UniquesRankingResponse
{
    [JsonProperty("elo")] public double? Elo;
    [JsonProperty("avg_elo")] public double? AverageFamilyElo;

    [JsonProperty("error"), JsonConverter(typeof(NumericBoolConverter))]
    public bool Error;

    [JsonProperty("nb_duel")] public int? NbDuel;
}