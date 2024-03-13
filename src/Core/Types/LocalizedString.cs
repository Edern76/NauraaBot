using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;
using Quickenshtein;

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

    // Oh man this is pretty ugly
    // But it works
    // Kinda
    public List<LevenshteinResult> GetLevenshteinDistances(string searchQuery, bool handleHero = false,
        string? language = null)
    {
        searchQuery = StringUtils.ReplaceSpecialCharacters(searchQuery).Trim().ToLower();
        List<LevenshteinResult> results = new List<LevenshteinResult>();
        List<string> languages = language is not null
            ? new List<string>() { language }
            : ConfigProvider.ConfigInstance.SupportedLanguages;
        string[] splitSearch = searchQuery.Split(' ');
        foreach (string lang in languages)
        {
            int distance;
            string actualName = Get(lang);
            string value = StringUtils.ReplaceSpecialCharacters(actualName).Trim().ToLower();

            string[] splitValue = value.Split(' ');

            //TODO: Relative distance for each case
            if (handleHero)
            {
                if (splitValue.Length == 3)
                {
                    distance = new[]
                    {
                        Levenshtein.GetDistance(searchQuery, splitValue[0].Trim()),
                        Levenshtein.GetDistance(searchQuery, splitValue[2].Trim()),
                        Levenshtein.GetDistance(searchQuery, value)
                    }.Min();
                }
                else
                {
                    distance = Levenshtein.GetDistance(searchQuery, value);
                }
            }
            else if (splitSearch.Length < splitValue.Length)
            {
                string[] groups = StringUtils.SplitIntoGroups(splitValue, splitSearch.Length);

                List<int> distanceList =
                    groups.Select(g =>
                        {
                            if (g.Split(" ")
                                .Any(word =>
                                    splitValue.Contains(
                                        word))) // Avoids outlandish suggestion, might need to change it though
                            {
                                int dist = Levenshtein.GetDistance(searchQuery, g) + 2; // +2 to favorise exact matches
                                return dist;
                            }

                            return 999999;
                        })
                        .ToList();
                distanceList.Add(Levenshtein.GetDistance(searchQuery, value));
                distance = distanceList.Min();
            }
            else
            {
                distance = Levenshtein.GetDistance(searchQuery, value);
            }

            results.Add(new LevenshteinResult
            {
                Language = lang,
                Name = actualName,
                Distance = distance
            });
        }

        return results;
    }
}

public class LevenshteinResult
{
    public string Language { get; set; }
    public string Name { get; set; }
    public int Distance { get; set; }
}