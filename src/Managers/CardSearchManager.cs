using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Database.Models;

namespace NauraaBot.Managers;

public static class CardSearchManager
{
    public static Card? SearchCard(string name, string? rarityShort, string? factionID)
    {
        // TODO : Support for multiple languages
        // TODO : Allow for searching even if entered name is not exact (Levenshtein distance, full text search ?)
        // TODO : Add promo support

        List<Card> potentialMatches = DatabaseProvider.Db.Cards.Where(card => card.Names.en.ToLower() == name.ToLower())
            .Include(card => card.CurrentFaction).Include(card => card.MainFaction).Include(card => card.Rarity)
            .ToList();
        // TODO : Error check on invalid faction/rarity
        if (factionID is not null)
        {
            potentialMatches = potentialMatches.FindAll(card => card.CurrentFaction.ID == factionID);
        }
        else
        {
            potentialMatches =
                potentialMatches.FindAll(card =>
                    card.CurrentFaction.ID == card.MainFaction.ID); // Exclude out of factions
        }

        Card result;
        if (rarityShort is not null)
        {
            result = potentialMatches.Find(card => card.Rarity.Short == rarityShort);
        }
        else
        {
            result = potentialMatches.Find(card => card.Rarity.Short == "C"); // Default to common
            if (result is null)
            {
                result = potentialMatches.FirstOrDefault(); // If no common is available we just take the first one
            }
        }

        return result;
    }
}