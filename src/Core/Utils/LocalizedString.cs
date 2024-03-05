using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NauraaBot.Core.Utils;

[Owned]
public class LocalizedString
{
    public string en { get; set; }
    public string fr { get; set; }
    public string de { get; set; }
    public string es { get; set; }
    public string it { get; set; }
    
    // Using reflection would be cleaner but VERY slow, especially considering we're gonna do this for each card
    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "en", en },
            { "fr", fr },
            { "de", de },
            { "es", es },
            { "it", it }
        };
    }
}