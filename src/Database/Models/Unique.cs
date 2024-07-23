using System.ComponentModel.DataAnnotations.Schema;
using NauraaBot.Database.Models;

namespace NauraaBot;

// Having a separate class for uniques allows us to avoid flooding the card table, and thus keep search times reasonable.
// At least this was the plan but EFCore is fucking dumb 
public class Unique : Card
{
    public static Unique FromCard(Card card)
    {
        return new Unique()
        {
            ID = card.ID,
            LastUpdated = card.LastUpdated,
            Costs = card.Costs,
            Power = card.Power,
            Set = card.Set,
            Type = card.Type,
            Rarity = card.Rarity,
            MainFaction = card.MainFaction,
            CurrentFaction = card.CurrentFaction,
            ImagesURLs = card.ImagesURLs,
            Names = card.Names,
            Effect = card.Effect,
            DiscardEffect = card.DiscardEffect
        };
    }
}