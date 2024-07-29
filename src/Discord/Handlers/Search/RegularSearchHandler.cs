using System;
using System.Threading.Tasks;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;
using NauraaBot.Managers;

namespace NauraaBot.Discord.Handlers.Search;

public class RegularSearchHandler : ISearchHandler
{
    public Task<Tuple<string, Card>> Search(string query, SearchParams searchParams)
    {
        Tuple<string, Card?> result;
        switch (query.ToLower())
        {
            case "rand()":
                result = CardRandomManager.RandomCard(query, searchParams);
                break;
            default:
                result = CardSearchManager.SearchCard(query, searchParams);
                break;
        }

        return Task.FromResult(result);
    }
}