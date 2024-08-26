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

    private static string triggerFilePath =
        StringUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory, "resources", "emotes", "triggers.json");

    private static string biomeFilePath =
        StringUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory, "resources", "emotes", "biomes.json");

    public static List<EmoteInfo> Factions { get; private set; }
    public static List<EmoteInfo> Triggers { get; private set; }
    public static List<EmoteInfo> Biomes { get; private set; }

    public static void InitializeEmotes()
    {
        Factions = LoadEmotes(factionFilePath, EmoteType.Faction);
        Triggers = LoadEmotes(triggerFilePath, EmoteType.Trigger);
        Biomes = LoadEmotes(biomeFilePath, EmoteType.Biome);
    }

    private static List<EmoteInfo> LoadEmotes(string filePath, EmoteType type)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{type} emotes file not found", filePath);
        }

        string content = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<EmoteInfo>>(content).Select(e =>
        {
            e.Type = type;
            return e;
        }).ToList();
    }
}