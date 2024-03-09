using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;
using NauraaBot.Managers.Utils;

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
            potentialMatches = allCards.FindAll(card =>
                string.Equals(card.Names.Get(language), name, StringComparison.CurrentCultureIgnoreCase));
        }
        else
        {
            foreach (string supportedLanguage in
                     ConfigProvider.ConfigInstance.SupportedLanguages) // Ugly but we're gonna replace this soon anyways
            {
                potentialMatches =
                    allCards.FindAll(card => string.Equals(card.Names.Get(supportedLanguage), name,
                        StringComparison.CurrentCultureIgnoreCase));
                if (potentialMatches is not null && potentialMatches.Count > 0)
                {
                    actualLanguage = supportedLanguage;
                    break;
                }
            }
        }

        potentialMatches = CardFilter.FilterMatches(potentialMatches, rarityShort, factionID);
        Card result = potentialMatches.FirstOrDefault();

        return new Tuple<string?, Card?>(actualLanguage, result);
    }
}