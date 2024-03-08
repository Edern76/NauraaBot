using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;

namespace NauraaBot.Managers;

public static class CardSearchManager
{
    public static Tuple<string?, Card> SearchCard(string name, string? rarityShort, string? factionID,
        string? language = null)
    {
        /*
         * Approximate search algorithm (reminder for later) :
         * 1. Get all cards from the database, get their names as dictionary, merge them all together
         * 2. Map levenshtein distance from search query to each entry (maybe do it same time as step 1 to speed things up)
         * 3. Take the minimal distance, if it's below a certain threshold (copilot says 0.3 while writing this description, I'd say 5, test different stuff) it's a match, otherwise return null
         * 4. Get all cards named similarly in the language of the best match, continue with the rest of the search parameters
         */


        List<Card> allCards = DatabaseProvider.Db.Cards.Include(card => card.CurrentFaction)
            .Include(card => card.MainFaction).Include(card => card.Rarity).Include(card => card.Set)
            .Include(card => card.Type).ToList();
        List<Card> potentialMatches = null;
        string? actualLanguage = language;
        if (language is not null)
        {
            potentialMatches = allCards.FindAll(card => card.Names.Get(language) == name);
        }
        else
        {
            foreach (string supportedLanguage in
                     ConfigProvider.ConfigInstance.SupportedLanguages) // Ugly but we're gonna replace this soon anyways
            {
                potentialMatches = allCards.FindAll(card => card.Names.Get(supportedLanguage) == name);
                if (potentialMatches is not null && potentialMatches.Count > 0)
                {
                    actualLanguage = supportedLanguage;
                    break;
                }
            }
        }

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

        Card result;
        if (rarityShort is not null && rarityShort.Length > 0)
        {
            if (rarityShort == "P")
            {
                result = potentialMatches.Find(card =>
                    card.ID.Split("_")[2] == "P"); // See if there is a cleaner way to handle this
            }
            else
            {
                result = potentialMatches.Find(card => card.Rarity.Short == rarityShort);
            }
        }
        else
        {
            result = potentialMatches.Find(card => card.Rarity.Short == "C"); // Default to common
            if (result is null)
            {
                result = potentialMatches.FirstOrDefault(); // If no common is available we just take the first one
            }
        }

        return new Tuple<string?, Card?>(actualLanguage, result);
    }
}