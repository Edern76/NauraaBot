using System.Linq;
using System.Text.RegularExpressions;
using NauraaBot.Discord.Types.Emote;

namespace NauraaBot.Discord.Utils;

public static class EmbedEffectStringConverter
{
    public static string ToEmbedEffectString(string effect)
    {
        string result = effect.Replace("[]", "");
        foreach (EmoteInfo emote in EmoteProvider.Triggers.Concat(EmoteProvider.Biomes))
        {
            result = result.Replace(emote.Replaces, emote.Code);
        }

        result = Regex.Replace(result, @"#(.+)#", m => $"*{m.Groups[1].Value}*");
        return result;
    }
}