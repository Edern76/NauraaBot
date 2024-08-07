﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core;
using NauraaBot.Core.Types;
using NauraaBot.Core.Utils;

namespace NauraaBot.Database.Models;

[Table("Card")]
[Index(nameof(ID), IsUnique = true)]
public class Card
{
    public string ID { get; set; } // Card reference
    public CardSet Set { get; set; }
    public CardType Type { get; set; }
    public Rarity Rarity { get; set; }
    public DateTime LastUpdated { get; set; }
    public Faction MainFaction { get; set; }
    public Faction CurrentFaction { get; set; }
    public LocalizedString ImagesURLs { get; set; } = new LocalizedString();
    public LocalizedString Names { get; set; } = new LocalizedString();
    public LocalizedString Effect { get; set; } = new LocalizedString();
    public LocalizedString DiscardEffect { get; set; } = new LocalizedString();
    public Costs? Costs { get; set; }
    public Power? Power { get; set; }

    [NotMapped] public double? Elo { get; set; }
    [NotMapped] public double? AverageFamilyElo { get; set; }

    public CardRecap ToCardRecap(string language = "en")
    {
        string name = this.Names.Get(language);
        string imageUrl = this.ImagesURLs.Get(language);
        string cardType = this.Type.Name;
        string rarity = this.Rarity.Name;
        string cardSet = this.Set.Name;
        string currentFaction = this.CurrentFaction.Name;
        List<String> effects = new List<string>();
        if (this.Effect.Get(language) is string mainEffect && mainEffect.Length > 0)
        {
            effects.Add(mainEffect);
        }

        if (this.DiscardEffect.Get(language) is string discardEffect && discardEffect.Length > 0)
        {
            effects.Add(discardEffect);
        }

        string effect = String.Join(effects.Count > 0 ? "\n\n" : "\n", effects);
        string URL = $"https://altered.gg/{Constants.LanguageHttpCodes[language.ToLower()].ToLower()}/cards/{this.ID}";
        CardRecap recap = new CardRecap()
        {
            AllNames = this.Names,
            Name = name,
            URL = URL,
            ImageURL = imageUrl,
            CardType = cardType,
            Rarity = rarity,
            CardSet = cardSet,
            CurrentFaction = currentFaction,
            Effect = effect
        };
        if (this.Costs is not null)
        {
            recap.CostString = $"{this.Costs.Hand}/{this.Costs.Reserve}";
        }

        if (this.Power is not null)
        {
            recap.PowerString = $"{this.Power.Forest}/{this.Power.Mountain}/{this.Power.Ocean}";
        }

        if (this.Elo is not null)
        {
            recap.Elo = this.Elo.Value;
        }

        if (this.AverageFamilyElo is not null)
        {
            recap.AverageFamilyElo = this.AverageFamilyElo.Value;
        }

        return recap;
    }

    public HashSet<string> GetMissingLanguages()
    {
        return new HashSet<string>(
            (ImagesURLs.GetMissingLanguages().Union(Names.GetMissingLanguages())));
    }
}

// Can't use file access restrictor on C# 10 :(
[Owned]
public class Costs
{
    public int Hand { get; set; }
    public int Reserve { get; set; }
}

[Owned]
public class Power
{
    public int Forest { get; set; }
    public int Mountain { get; set; }
    public int Ocean { get; set; }
}