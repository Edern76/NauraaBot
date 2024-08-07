﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NauraaBot.API.DTO;

[Serializable]
public class CardDTO
{
    [JsonProperty("reference")] public string Reference { get; set; }
    [JsonProperty("cardType")] public IDNameObject CardType { get; set; }
    [JsonProperty("cardSet")] public IDNameObject CardSet { get; set; }
    [JsonProperty("rarity")] public IDNameObject Rarity { get; set; }
    [JsonProperty("imagePath")] public string ImagePath { get; set; }

    [JsonProperty("updatedAt")]
    public string
        UpdatedAt
    {
        get;
        set;
    } // Note to self, don't forget to convert to TimeDate when creating the actual DB object from this DTO

    [JsonProperty("mainFaction")] public IDNameObject MainFaction { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("elements")] public JToken ElementsToken { get; set; }
    public ElementsDTO Elements { get; set; }
}

[Serializable]
public class ElementsDTO
{
    [JsonProperty("MAIN_EFFECT")] public string? MainEffect { get; set; }
    [JsonProperty("ECHO_EFFECT")] public string? DiscardEffect { get; set; }
    [JsonProperty("PERMANENT")] public int? PermanentsNumber { get; set; }
    [JsonProperty("RESERVE")] public int? ReserveSize { get; set; }
    [JsonProperty("MAIN_COST")] public string? HandCostRaw { get; set; }
    public int? HandCost { get; set; }
    [JsonProperty("RECALL_COST")] public string? RecallCostRaw { get; set; }
    public int? RecallCost { get; set; }
    [JsonProperty("OCEAN_POWER")] public string? OceanPowerRaw { get; set; }
    public int? OceanPower { get; set; }
    [JsonProperty("MOUNTAIN_POWER")] public string? MountainPowerRaw { get; set; }
    public int? MountainPower { get; set; }
    [JsonProperty("FOREST_POWER")] public string? ForestPowerRaw { get; set; }
    public int? ForestPower { get; set; }

    public void CleanCostsAndPowers()
    {
        HandCost = ParseStringToInt(HandCostRaw);
        RecallCost = ParseStringToInt(RecallCostRaw);
        OceanPower = ParseStringToInt(OceanPowerRaw);
        MountainPower = ParseStringToInt(MountainPowerRaw);
        ForestPower = ParseStringToInt(ForestPowerRaw);
    }

    private int? ParseStringToInt(string? str)
    {
        if (str is null || str.Length == 0)
        {
            return null;
        }

        string cleanedStr = str.Replace("#", "");
        if (int.TryParse(cleanedStr, out int parsedInt))
        {
            return parsedInt;
        }
        else
        {
            throw new InvalidCastException($"Could not parse string {str} to int");
        }
    }
}