using System.Collections.Generic;
using System.Linq;
using NauraaBot.Core.Config;
using NauraaBot.Core.Error;
using NauraaBot.Database.Models;
using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Managers;

public static class ParamsCheckerManager
{
    public static List<string> GetParamErrors(SearchParams searchParams)
    {
        List<string> errors = new List<string>();

        List<string> availableFactions = DatabaseProvider.Db.Factions.Select(f => f.ID).ToList();
        availableFactions.Add("OOF");

        List<string> availableRarities = DatabaseProvider.Db.Rarities.Select(r => r.Short).ToList();
        availableRarities.Add("P");
        //availableRarities.Remove("U");

        List<string> availableLanguages =
            ConfigProvider.ConfigInstance.SupportedLanguages.Select(s => s.ToLower()).ToList();

        if (searchParams.Faction != null && !availableFactions.Contains(searchParams.Faction))
        {
            errors.Add(ParamsErrorHandler.GetErrorMessage(ParamsErrorType.UNKNOWN_FACTION, searchParams.Faction,
                string.Join(", ", availableFactions)));
        }

        if (searchParams.Rarity != null && !availableRarities.Contains(searchParams.Rarity))
        {
            errors.Add(ParamsErrorHandler.GetErrorMessage(ParamsErrorType.UNKNOWN_RARITY, searchParams.Rarity,
                string.Join(", ", availableRarities)));
        }

        if (searchParams.Language != null && !availableLanguages.Contains(searchParams.Language))
        {
            errors.Add(ParamsErrorHandler.GetErrorMessage(ParamsErrorType.UNKNOWN_LANGUAGE, searchParams.Language,
                string.Join(", ", availableLanguages)));
        }

        return errors;
    }
}