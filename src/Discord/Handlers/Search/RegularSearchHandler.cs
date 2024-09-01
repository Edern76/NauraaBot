using System;
using System.Threading.Tasks;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;
using NauraaBot.Managers;

namespace NauraaBot.Discord.Handlers.Search;

public class RegularSearchHandler : ISearchHandler
{
    public async Task<Tuple<string, Card>> Search(string query, SearchParams searchParams)
    {
        Tuple<string, Card?> result;
        switch (query.ToLower())
        {
            case "rand()":
                result = CardRandomManager.RandomCard(query, searchParams);
                break;
            default:
                result = await CardSearchManager.SearchCard(query, searchParams);
                break;
        }

        return result;
    }
}