using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NauraaBot.Core.Utils;
using NauraaBot.Discord.Types.Emote;
using Newtonsoft.Json;

namespace NauraaBot.Discord;

public static class EmoteProvider
{
    private static string factionFilePath =
        StringUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory, "resources", "emotes", "factions.json");

    public static List<EmoteInfo> Factions { get; private set; }

    public static void InitializeEmotes()
    {
        if (!File.Exists(factionFilePath))
        {
            throw new FileNotFoundException("Faction emotes file not found", factionFilePath);
        }

        string factionContent = File.ReadAllText(factionFilePath);
        Factions = JsonConvert.DeserializeObject<List<EmoteInfo>>(factionContent).Select(e =>
        {
            e.Type = EmoteType.Faction;
            return e;
        }).ToList();
    }
}