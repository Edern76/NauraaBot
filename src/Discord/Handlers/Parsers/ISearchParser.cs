using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Discord.Handlers.Parsers;

public interface ISearchParser
{
    public SearchParams Parse(string query);
}