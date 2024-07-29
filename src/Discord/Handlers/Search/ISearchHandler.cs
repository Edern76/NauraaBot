using System;
using System.Threading.Tasks;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Discord.Handlers.Search;

public interface ISearchHandler
{
    public Task<Tuple<string, Card>> Search(string query, SearchParams searchParams);
}