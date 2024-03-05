using System;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Utils;

namespace NauraaBot.Database.Models;

public class Card
{
    public string ID { get; set; } // Card reference
    public string SetCode { get; set; }
    public string RarityCode { get; set; }
    public DateTime LastUpdated { get; set; }
    public Faction MainFaction { get; set; }
    public Faction CurrentFaction { get; set; }
    public LocalizedString ImagesURLs { get; set; }
    public LocalizedString Names { get; set; }
    public LocalizedString Effect { get; set; }
    public Costs? Costs { get; set; }
    public Power? Power { get; set; }
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