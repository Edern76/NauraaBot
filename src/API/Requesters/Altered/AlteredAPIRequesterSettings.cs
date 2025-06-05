using System;

namespace NauraaBot.API.Requesters.Altered;

public class AlteredAPIRequesterSettings : ICloneable
{
    public int? MaxCardsPerRequest { get; set; }
    public string? Rarity { get; set; }
    public string? Faction { get; set; }
    public bool? Pagination { get; set; }

    public static readonly AlteredAPIRequesterSettings Default = new AlteredAPIRequesterSettings()
    {
        MaxCardsPerRequest =
            100, // SOMEHOW, this fails with 36, but works with 100 (I don't think it returns 100 though)
        Pagination = true,
    };

    public static readonly AlteredAPIRequesterSettings Uniques = new AlteredAPIRequesterSettings()
    {
        Rarity = "UNIQUE",
        Pagination = true,
        MaxCardsPerRequest = 100,
    };

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}