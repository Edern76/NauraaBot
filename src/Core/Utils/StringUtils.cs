using System.Collections.Generic;
using System.Linq;

namespace NauraaBot.Core.Utils;

public static class StringUtils
{
    // TODO: Find a way to replace \u0022 with " without messing everything up
    private static readonly Dictionary<string, string> EncodingCharMap = new Dictionary<string, string>()
    {
        { "\\/", "/" },
        { "\u00A0", " " },
        { "\u00A9", " " },
        { "\u0026", "&" },
        { "\u0027", "'" },
        { "\u00DF", "ß" }, // German eszett (ß) (from then on it's ChatGPT so take it with a grain of salt)
        { "\u00E1", "á" }, // Spanish letter with acute accent (á)
        { "\u00E9", "é" }, // Spanish letter with acute accent (é)
        { "\u00ED", "í" }, // Spanish letter with acute accent (í)
        { "\u00F3", "ó" }, // Spanish letter with acute accent (ó)
        { "\u00FA", "ú" }, // Spanish letter with acute accent (ú)
        { "\u00FC", "ü" }, // Spanish letter with umlaut (ü)
        { "\u00F1", "ñ" }, // Spanish letter with tilde (ñ)
        { "\u00E7", "ç" }, // French letter with cedilla (ç)
    };

    private static readonly Dictionary<string, string> SpecialCharReplacementMap = new Dictionary<string, string>()
    {
        { ",", "" },
        { "-", " " },
        { "é", "e" },
        { "è", "e" },
        { "à", "a" },
        { "â", "a" },
        { "ê", "e" },
        { "î", "i" },
        { "ô", "o" },
        { "û", "u" },
        { "ü", "u" },
        { "ç", "c" },
        { "ß", "ss" },
    };

    public static string Decode(string input)
    {
        foreach (var (key, value) in EncodingCharMap)
        {
            input = input.Replace(key, value);
        }

        return input;
    }

    public static string ReplaceSpecialCharacters(string input)
    {
        foreach (var (key, value) in SpecialCharReplacementMap)
        {
            input = input.Replace(key, value);
        }

        return input;
    }

    public static string[] SplitIntoGroups(string[] input, int groupSize)
    {
        IEnumerable<string> splitString = input.Select(s => s.Replace(",", "").Trim());
        IEnumerable<string> enumerable = splitString as string[] ?? splitString.ToArray(); // Avoid multiple enumeration
        if (enumerable.Count() < groupSize)
        {
            return new string[] { string.Join(" ", input) };
        }
        else
        {
            return Enumerable.Range(0, enumerable.Count() - groupSize + 1)
                .Select(i => string.Join(" ", splitString.Skip(i).Take(groupSize)))
                .ToArray();
        }
    }
}