using YamlDotNet.Serialization;

namespace NauraaBot.Core.Config;

public class Config
{
    [YamlMember(Alias = "token")]
    public string token { get; set; }
    [YamlMember(Alias = "db_path")]
    public string? db_path {get; set;}
    [YamlMember(Alias = "database")]
    public string database {get; set;}
    [YamlMember(Alias = "password")]
    public string? password { get; set; }
}