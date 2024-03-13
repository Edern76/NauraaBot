using System.Collections.Generic;
using System.Linq;
using NauraaBot.Core.Config;
using NauraaBot.Core.Error;
using NauraaBot.Database.Models;

namespace NauraaBot.Managers;

public static class ParamsCheckerManager
{
    public static List<string> GetParamErrors(string faction, string rarity, string language)
    {
        List<string> errors = new List<string>();

        List<string> availableFactions = DatabaseProvider.Db.Factions.Select(f => f.ID).ToList();
        availableFactions.Add("OOF");

        List<string> availableRarities = DatabaseProvider.Db.Rarities.Select(r => r.Short).ToList();
        availableRarities.Add("P");
        availableRarities.Remove("U");

        List<string> availableLanguages =
            ConfigProvider.ConfigInstance.SupportedLanguages.Select(s => s.ToUpper()).ToList();

        if (faction != null && !availableFactions.Contains(faction))
        {
            errors.Add(ParamsErrorHandler.GetErrorMessage(ParamsErrorType.UNKNOWN_FACTION, faction,
                string.Join(", ", availableFactions)));
        }

        if (rarity != null && !availableRarities.Contains(rarity))
        {
            errors.Add(ParamsErrorHandler.GetErrorMessage(ParamsErrorType.UNKNOWN_RARITY, rarity,
                string.Join(", ", availableRarities)));
        }

        if (language != null && !availableLanguages.Contains(language))
        {
            errors.Add(ParamsErrorHandler.GetErrorMessage(ParamsErrorType.UNKNOWN_LANGUAGE, language,
                string.Join(", ", availableLanguages)));
        }

        return errors;
    }
}