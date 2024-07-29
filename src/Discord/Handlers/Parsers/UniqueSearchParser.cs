using NauraaBot.Core.Config;
using NauraaBot.Core.Exception;
using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Discord.Handlers.Parsers;

public class UniqueSearchParser : ISearchParser
{
    public SearchParams Parse(string query)
    {
        SearchParams result = new SearchParams();
        string[] split = query.Split('|');
        if (split.Length == 1)
        {
            result.Rarity = null;
            result.Faction = null;
            result.Language = null;
        }
        else if (split.Length == 2)
        {
            result.Language = split[1].Trim().ToLower();
            if (!ConfigProvider.ConfigInstance.SupportedLanguages.Contains(result.Language))
            {
                throw new InvalidQueryFormatException(query, SearchIntent.UNIQUE_ID);
            }
        }
        else
        {
            throw new InvalidQueryFormatException(query, SearchIntent.UNIQUE_ID);
        }

        return result;
    }
}