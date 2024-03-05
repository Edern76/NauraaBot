﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;

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
    
    // Ugly but again reflection is VERY slow
    public void Set(string lang, string value)
    {
        switch (lang)
        {
            case "en":
                en = value;
                break;
            case "fr":
                fr = value;
                break;
            case "de":
                de = value;
                break;
            case "es":
                es = value;
                break;
            case "it":
                it = value;
                break;
            default:
                throw new NotSupportedException($"Language {lang} is not supported.");
        }
    }
    
    // Same as above. Also avoids having to create a dictionary when we don't need one.
    public string Get(string lang)
    {
        switch (lang)
        {
            case "en":
                return en;
            case "fr":
                return fr;
            case "de":
                return de;
            case "es":
                return es;
            case "it":
                return it;
            default:
                throw new NotSupportedException($"Language {lang} support has not been implemented.");
        }
    }

    // Voluntarily excludes unsupported languages
    public HashSet<string> GetMissingLanguages()
    {
        HashSet<string> result = new HashSet<string>();
        foreach (string language in ConfigProvider.ConfigInstance.SupportedLanguages)
        {
            string value = Get(language);
            if (value is null || value.Length == 0)
            {
                result.Add(language);
            }
        }

        return result;
    }
}