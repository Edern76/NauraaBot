using System;
using NauraaBot.Core.Exception;
using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Discord.Handlers.Parsers;

public class RegularSearchParser : ISearchParser
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
        else if (split.Length == 2 || split.Length == 3)
        {
            string options = split[1].Trim();
            string[] optionsArray = options.Split(',');
            result.Rarity = optionsArray[0].Trim();
            result.Faction = optionsArray.Length > 1 ? optionsArray[1].Trim() : null;
            if (result.Faction is null &&
                string.Equals(result.Rarity.ToUpper(), "OOF", StringComparison.CurrentCultureIgnoreCase))
            {
                result.Rarity = "R";
                result.Faction = "OOF";
            }

            if (result.Rarity is not null && result.Rarity.Trim().Length == 0)
            {
                result.Rarity = null;
            }

            if (split.Length == 3)
            {
                result.Language = split[2].Trim().ToLower();
            }
            else
            {
                result.Language = null;
            }
        }
        else
        {
            throw new InvalidQueryFormatException(query);
        }

        return result;
    }
}