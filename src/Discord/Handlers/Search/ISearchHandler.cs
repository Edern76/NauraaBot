using System;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Discord.Handlers.Search;

public interface ISearchHandler
{
    public Tuple<string, Card> Search(string query, SearchParams searchParams);
}