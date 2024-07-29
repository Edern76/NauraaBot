using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Providers;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;
using NauraaBot.Managers.Utils;

namespace NauraaBot.Managers;

public static class CardRandomManager
{
    public static Tuple<string, Card?> RandomCard(string name, SearchParams searchParams)
    {
        if (searchParams.Language is null || searchParams.Language.Length == 0)
        {
            searchParams.Language = "en";
        }

        List<Card> allCards = DatabaseProvider.Db.Cards.Include(card => card.CurrentFaction)
            .Include(card => card.MainFaction).Include(card => card.Rarity).Include(card => card.Set)
            .Include(card => card.Type).ToList();
        List<Card> filteredCards = CardFilter.FilterMatches(allCards, searchParams.Rarity, searchParams.Faction);

        if (filteredCards.Count == 0)
        {
            return new Tuple<string, Card>("en", null);
        }

        Card foundCard = filteredCards[RandomProvider.Random.Next(filteredCards.Count)];
        return new Tuple<string, Card>(searchParams.Language, foundCard);
    }
}