using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NauraaBot.API.DTO;
using NauraaBot.API.Requesters.Altered;
using NauraaBot.Core.Config;
using NauraaBot.Core.Providers;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;
using NauraaBot.Managers.Utils;

namespace NauraaBot.Managers;

public static class CardSearchManager
{
    public async static Task<Tuple<string?, Card>> SearchCard(string name, SearchParams searchParams)
    {
        /*
         * Approximate search algorithm (reminder for later) :
         * 1. Get all cards from the database, get their names as dictionary, merge them all together
         * 2. Map levenshtein distance from search query to each entry (maybe do it same time as step 1 to speed things up)
         * 3. Take the minimal distance, if it's below a certain threshold (copilot says 0.3 while writing this description, I'd say 5, test different stuff) it's a match, otherwise return null
         * 4. Get all cards named similarly in the language of the best match, continue with the rest of the search parameters
         */


        IIncludableQueryable<Card, CardType> query = DatabaseProvider.Db.Cards.Include(card => card.CurrentFaction)
            .Include(card => card.MainFaction).Include(card => card.Rarity).Include(card => card.Set)
            .Include(card => card.Type);
        if (searchParams.Rarity != "U")
        {
            query = query.Where(card => card.Rarity.ID != "UNIQUE")
                .Include(card =>
                    card.Type); // Exclude uniques at the DB level for performance boost, the last include is useless but it's required for type compliance
        }

        List<Card> allCards = query.ToList();
        Tuple<string, List<Card>> nameSearchResult = GetPotentialMatches(allCards, name, searchParams.Language);

        string actualLanguage = nameSearchResult.Item1;
        List<Card> potentialMatches = nameSearchResult.Item2;

        potentialMatches = CardFilter.FilterMatches(potentialMatches, searchParams.Rarity, searchParams.Faction)
            .Where(c => searchParams.RequiredLanguage is null || c.Names.Get(searchParams.RequiredLanguage) is not null)
            .ToList();
        Card result = potentialMatches.FirstOrDefault();

        if (searchParams.Rarity == "U" && result is not null)
        {
            List<Unique> matchingUniques = DatabaseProvider.Db.Uniques.Include(unique => unique.CurrentFaction)
                .Include(unique => unique.MainFaction).Include(unique => unique.Rarity).Include(unique => unique.Set)
                .Include(unique => unique.Type).ToList().Where(u =>
                    u.Names.Get(actualLanguage) == result.Names.Get(actualLanguage) &&
                    ((searchParams.Faction == null && (u.CurrentFaction.ID == result.CurrentFaction.ID)) ||
                     u.CurrentFaction.ID == result.CurrentFaction.ID) && (searchParams.RequiredLanguage is null ||
                                                                          result.Names.Get(
                                                                                  searchParams.RequiredLanguage) is not
                                                                              null)).ToList();
            result = matchingUniques.ElementAt(RandomProvider.Random.Next(0, matchingUniques.Count));
        }
        else if (result is not null && searchParams.Set is not null && searchParams.Number is not null)
        {
            string uniqueID = CardUtils.GetUniqueIDFromBaseID(result.ID, searchParams.Set, searchParams.Number.Value);
            Tuple<string, Unique> searchResult = await SearchUnique(uniqueID, searchParams);
            result = searchResult.Item2;
        }

        return new Tuple<string?, Card?>(actualLanguage, result);
    }

    public async static Task<Tuple<string, Unique>> SearchUnique(string ID, SearchParams searchParams)
    {
        Unique unique = DatabaseProvider.Db.Uniques.Include(card => card.CurrentFaction)
            .Include(card => card.MainFaction).Include(card => card.Rarity).Include(card => card.Set)
            .Include(card => card.Type).FirstOrDefault(u => u.ID.ToUpper() == ID.ToUpper());
        string actualLanguage = searchParams.Language ?? (searchParams.RequiredLanguage ?? "en");

        if (unique is null)
        {
            Card tempCard = DatabaseProvider.PendingAdditions.FirstOrDefault(c => c.ID.ToUpper() == ID.ToUpper());
            if (tempCard?.Names.Get(actualLanguage) is string name && !String.IsNullOrEmpty(name))
            {
                unique = Unique.FromCard(tempCard);
            }
            else
            {
                try
                {
                    LogUtils.Log($"Unique {ID} not found in database, fetching from Altered API");
                    CardDTO dto = await AlteredAPIRequester.GetCard(ID, actualLanguage);
                    Card card = CardImportManager.DirectCreateUniqueSkeleton(dto, actualLanguage);
                    DatabaseProvider.EnqueueCard(card);
                    unique = Unique.FromCard(card);
                }
                catch (Exception e)
                {
                    LogUtils.Error(e.ToString());
                    unique = null;
                }
            }
        }

        return new Tuple<string, Unique>(actualLanguage, unique);
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