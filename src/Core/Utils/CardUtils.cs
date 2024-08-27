using System;
using System.Diagnostics;

namespace NauraaBot.Core.Utils;

public static class CardUtils
{
    public static string GetUniqueIDFromBaseID(string baseID, string set, ulong number)
    {
        string[] split = baseID.Split('_');
        if (split.Length < 6)
        {
            throw new ArgumentException($"Base ID {baseID} is too short");
        }

        return $"{split[0]}_{set.ToUpper()}_{split[2]}_{split[3]}_{split[4]}_U_{number}";
    }
}