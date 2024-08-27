using System.Collections.Generic;

namespace NauraaBot.Core.Error;

public static class ParamsErrorHandler
{
    private static readonly Dictionary<ParamsErrorType, string> Messages = new Dictionary<ParamsErrorType, string>()
    {
        { ParamsErrorType.UNKNOWN_LANGUAGE, "Language `{0}` is not supported. The supported languages are `{1}`" },
        { ParamsErrorType.UNKNOWN_FACTION, "Faction `{0}` is not supported. The supported factions are `{1}`" },
        { ParamsErrorType.UNKNOWN_RARITY, "Rarity `{0}` is not supported. The supported rarities are `{1}`" },
        { ParamsErrorType.UNKNOWN_SET, "Set `{0}` is not supported. The supported sets are `{1}`" }
    };

    public static string GetErrorMessage(ParamsErrorType errorType, string param, string supportedValues)
    {
        return string.Format(Messages[errorType], param.ToUpper(), supportedValues);
    }
}