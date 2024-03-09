using System.Collections.Generic;
using NauraaBot.Database.Models;

namespace NauraaBot.Managers.Utils;

public static class CardFilter
{
    public static List<Card> FilterMatches(List<Card> potentialMatches, string? rarityShort, string? factionID)
    {
        // TODO : Error check on invalid faction/rarity
        if (factionID is not null)
        {
            if (factionID == "OOF")
            {
                potentialMatches = potentialMatches.FindAll(card => card.CurrentFaction.ID != card.MainFaction.ID);
            }
            else
            {
                potentialMatches = potentialMatches.FindAll(card => card.CurrentFaction.ID == factionID);
            }
        }
        else
        {
            potentialMatches =
                potentialMatches.FindAll(card =>
                    card.CurrentFaction.ID == card.MainFaction.ID); // Exclude out of factions
        }

        if (rarityShort is not null && rarityShort.Length > 0)
        {
            if (rarityShort == "P")
            {
                potentialMatches = potentialMatches.FindAll(card =>
                    card.ID.Split("_")[2] == "P"); // See if there is a cleaner way to handle this
            }
            else
            {
                potentialMatches = potentialMatches.FindAll(card => card.Rarity.Short == rarityShort);
            }
        }
        else
        {
            potentialMatches = potentialMatches.FindAll(card => card.Rarity.Short == "C"); // Default to common
        }

        return potentialMatches;
    }
}