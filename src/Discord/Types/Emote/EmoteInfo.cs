using Newtonsoft.Json;

namespace NauraaBot.Discord.Types.Emote;

public class EmoteInfo
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("replaces", Required = Required.Default)]
    public string? Replaces { get; set; }

    [JsonProperty("code", Required = Required.Always)]
    public string Code { get; set; }

    [JsonIgnore] public EmoteType Type { get; set; }
}