using YamlDotNet.Serialization;

namespace NauraaBot.Core.Config.Types;

public class SearchConfig
{
    [YamlMember(Alias = "max_absolute_distance")]
    public int MaxAbsoluteDistance { get; set; }

    [YamlMember(Alias = "max_relative_distance")]
    public double MaxRelativeDistance { get; set; }
}