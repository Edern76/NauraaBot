using System;
using System.Threading.Tasks;
using NauraaBot.API.DTO;
using NauraaBot.API.Requesters.UniquesRanker;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;
using NauraaBot.Managers;

namespace NauraaBot.Discord.Handlers.Search;

public class UniqueIDSearchHandler : ISearchHandler
{
    public async Task<Tuple<string, Card>> Search(string query, SearchParams searchParams)
    {
        Tuple<string, Unique> result = CardSearchManager.SeachUnique(query, searchParams);
        return new Tuple<string, Card>(result.Item1, result.Item2);
    }
}