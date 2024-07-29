using System;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;
using NauraaBot.Managers;

namespace NauraaBot.Discord.Handlers.Search;

public class UniqueIDSearchHandler : ISearchHandler
{
    public Tuple<string, Card> Search(string query, SearchParams searchParams)
    {
        Tuple<string, Unique> result = CardSearchManager.SeachUnique(query, searchParams);
        return new Tuple<string, Card>(result.Item1, result.Item2);
    }
}