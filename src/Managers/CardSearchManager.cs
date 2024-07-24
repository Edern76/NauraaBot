using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;
using NauraaBot.Core.Providers;
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
        Tuple<string, List<Card>> nameSearchResult = GetPotentialMatches(allCards, name, language);

        string actualLanguage = nameSearchResult.Item1;
        List<Card> potentialMatches = nameSearchResult.Item2;

        potentialMatches = CardFilter.FilterMatches(potentialMatches, rarityShort, factionID);
        Card result = potentialMatches.FirstOrDefault();

        if (rarityShort == "U" && result is not null)
        {
            List<Unique> matchingUniques = DatabaseProvider.Db.Uniques.Include(unique => unique.CurrentFaction)
                .Include(unique => unique.MainFaction).Include(unique => unique.Rarity).Include(unique => unique.Set)
                .Include(unique => unique.Type).ToList().Where(u =>
                    u.Names.Get(actualLanguage) == result.Names.Get(actualLanguage) &&
                    ((factionID == null && (u.CurrentFaction.ID == result.CurrentFaction.ID)) ||
                     u.CurrentFaction.ID == result.CurrentFaction.ID)).ToList();
            result = matchingUniques.ElementAt(RandomProvider.Random.Next(0, matchingUniques.Count));
        }

        return new Tuple<string?, Card?>(actualLanguage, result);
    }

    public static Tuple<string, List<Card>> GetPotentialMatches(List<Card> cards, string name, string? language = null)
    {
        IEnumerable<LevenshteinResult> distances =
            cards.SelectMany(c => c.Names.GetLevenshteinDistances(name,
                string.Equals(c.Type.ID.ToUpper(), "HERO", StringComparison.CurrentCultureIgnoreCase), language));
        LevenshteinResult bestMatch = distances.OrderBy(d => d.Distance).FirstOrDefault(d =>
            (double)d.Distance / d.Name.Length <= ConfigProvider.ConfigInstance.SearchConfig.MaxRelativeDistance);
        if (bestMatch is null)
        {
            return new Tuple<string, List<Card>>("en", new List<Card>());
        }

        if (language is null)
        {
            IEnumerable<LevenshteinResult> equalDists = distances.Where(d => d.Distance == bestMatch.Distance);
            if (equalDists.FirstOrDefault(d => d.Language == "en") is LevenshteinResult englishResult)
            {
                bestMatch = englishResult;
            }
        }

        double relativeDistance =
            bestMatch.Name.Length > 0 ? (double)bestMatch.Distance / bestMatch.Name.Length : 999.9;
        if (bestMatch.Name.Length == 0 ||
            bestMatch.Distance > ConfigProvider.ConfigInstance.SearchConfig.MaxAbsoluteDistance ||
            (double)bestMatch.Distance / bestMatch.Name.Length >
            ConfigProvider.ConfigInstance.SearchConfig.MaxRelativeDistance)
        {
            return new Tuple<string, List<Card>>("en", new List<Card>());
        }
        else
        {
            List<Card> potentialMatches = cards.FindAll(c => string.Equals(c.Names.Get(bestMatch.Language),
                bestMatch.Name, StringComparison.CurrentCultureIgnoreCase));
            if (potentialMatches.Any(c =>
                    !string.Equals(c.Type.ID.ToUpper(), "HERO", StringComparison.CurrentCultureIgnoreCase)))
            {
                potentialMatches = potentialMatches.FindAll(c => !string.Equals(c.Type.ID.ToUpper(), "HERO",
                    StringComparison
                        .CurrentCultureIgnoreCase)); // Handles the case where part of the hero card name is another card's name (ex: "Kojo & Booda" and "Booda")
            }

            return new Tuple<string, List<Card>>(bestMatch.Language, potentialMatches);
        }
    }
}