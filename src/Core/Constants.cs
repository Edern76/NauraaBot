using System.Collections.Generic;

namespace NauraaBot.Core;

public static class Constants
{
    public static readonly Dictionary<string, string> LanguageHttpCodes = new Dictionary<string, string>()
    {
        { "en", "en-US" },
        { "fr", "fr-FR" },
        { "it", "it-IT" },
        { "de", "de-DE" },
        { "es", "es-ES" }
    };

    public static readonly Dictionary<string, string> LanguageFullNames = new Dictionary<string, string>()
    {
        { "en", "English" },
        { "fr", "French" },
        { "it", "Italian" },
        { "de", "German" },
        { "es", "Spanish" }
    };
}