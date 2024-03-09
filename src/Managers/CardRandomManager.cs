using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Providers;
using NauraaBot.Database.Models;
using NauraaBot.Managers.Utils;

namespace NauraaBot.Managers;

public static class CardRandomManager
{
    public static Tuple<string, Card> RandomCard(string name, string? rarityShort, string? factionID,
        string? language = null)
    {
        if (language is null || language.Length == 0)
        {
            language = "en";
        }

        List<Card> allCards = DatabaseProvider.Db.Cards.Include(card => card.CurrentFaction)
            .Include(card => card.MainFaction).Include(card => card.Rarity).Include(card => card.Set)
            .Include(card => card.Type).ToList();
        List<Card> filteredCards = CardFilter.FilterMatches(allCards, rarityShort, factionID);

        Card foundCard = filteredCards[RandomProvider.Random.Next(filteredCards.Count)];
        return new Tuple<string, Card>(language, foundCard);
    }
}