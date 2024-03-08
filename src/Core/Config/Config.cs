using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace NauraaBot.Core.Config;

public class Config
{
    private List<String> _additionalLanguages;

    [YamlMember(Alias = "additional_languages")]
    public List<String> AdditionalLanguages
    {
        get => _additionalLanguages; //YamlDotNet does not want to have the getter private but you SHOULD NOT access this
        set
        {
            _additionalLanguages = value;
            SupportedLanguages = new List<string>(_additionalLanguages);
            SupportedLanguages.Insert(0, "en");
        }
    }

    public List<String> SupportedLanguages { get; private set; }
    [YamlMember(Alias = "token")] public string Token { get; set; }

    [YamlMember(Alias = "big_image")] public bool BigImage { get; set; }

    [YamlMember(Alias = "force_update_db_on_start")]
    public bool ForceUpdateDbOnStart { get; set; }

    [YamlMember(Alias = "db_path")] public string? DbPath { get; set; }
    [YamlMember(Alias = "database")] public string Database { get; set; }
    [YamlMember(Alias = "password")] public string? Password { get; set; }

    [YamlMember(Alias = "max_replies_per_message")]
    public int MaxRepliesPerMessages { get; set; }

    [YamlMember(Alias = "update_periodicity")]
    public string UpdatePeriodicity { get; set; }
}