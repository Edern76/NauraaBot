namespace NauraaBot.Core.Types;

public class CardRecap
{
    public string URL { get; set; }
    public string ImageURL { get; set; }
    public string Name { get; set; }
    public string CardType { get; set; }
    public string Rarity { get; set; }
    public string CurrentFaction { get; set; }
    public string Effect { get; set; }
    public string CardSet { get; set; }
    public string? CostString { get; set; }
    public string? PowerString { get; set; }
}