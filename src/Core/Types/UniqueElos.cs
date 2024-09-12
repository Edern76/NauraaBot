using Newtonsoft.Json;

namespace NauraaBot.API.DTO;

public struct UniqueElos
{
    [JsonProperty("elo")] public double? Elo;
    [JsonProperty("avg_elo")] public double? AverageFamilyElo;

    public static UniqueElos FromResponse(UniquesRankingResponse response)
    {
        return new UniqueElos()
        {
            Elo = response.Elo,
            AverageFamilyElo = response.AverageFamilyElo
        };
    }
}