using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace NauraaBot.Core.Config;

public class Config
{
    [YamlMember(Alias = "additional_languages")]
    public List<String> AdditionalLanguages { get; set; }
    [YamlMember(Alias = "token")]
    public string Token { get; set; }
    [YamlMember(Alias = "db_path")]
    public string? DbPath {get; set;}
    [YamlMember(Alias = "database")]
    public string Database {get; set;}
    [YamlMember(Alias = "password")]
    public string? Password { get; set; }
}