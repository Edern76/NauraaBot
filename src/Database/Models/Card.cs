using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core;
using NauraaBot.Core.Types;
using NauraaBot.Core.Utils;

namespace NauraaBot.Database.Models;

[Table("Card")]
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
    public Costs? Costs { get; set; }
    public Power? Power { get; set; }

    public CardRecap ToCardRecap(string language="en")
    {
        CardRecap recap = new CardRecap()
        {
            Name = this.Names.Get(language),
            URL = $"https://altered.gg/{Constants.LanguageHttpCodes[language].ToLower()}/cards/{this.ID}",
            ImageURL = this.ImagesURLs.Get(language),
            CardType = this.Type.Name,
            Rarity = this.Rarity.Name,
            CardSet = this.Set.Name,
            CurrentFaction = this.CurrentFaction.Name,
            Effect = this.Effect.Get(language)
        };
        if (this.Costs is not null)
        {
            recap.CostString = $"{this.Costs.Hand}/{this.Costs.Reserve}";
        }

        if (this.Power is not null)
        {
            recap.PowerString = $"{this.Power.Forest}/{this.Power.Mountain}/{this.Power.Ocean}";
        }

        return recap;
    }
    
    
}


// Can't use file access restrictor on C# 10 :(
[Owned]
public class Costs
{
    public int Hand { get; set; }
    public int Reserve {get; set; }
}

[Owned]
public class Power
{
    public int Forest { get; set; }
    public int Mountain { get; set;}
    public int Ocean { get; set; }
}