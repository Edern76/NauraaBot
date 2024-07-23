namespace DefaultNamespace;

public class AlteredAPIRequesterSettings
{
    public int? MaxCardsPerRequest { get; set; }
    public string? Rarity { get; set; }
    public bool? Pagination { get; set; }

    public static readonly AlteredAPIRequesterSettings Default = new AlteredAPIRequesterSettings()
    {
        MaxCardsPerRequest = 10000,
        Pagination = false,
    };

    public static readonly AlteredAPIRequesterSettings Uniques = new AlteredAPIRequesterSettings()
    {
        Rarity = "UNIQUE",
        Pagination = true,
        MaxCardsPerRequest = 1000,
    };
}