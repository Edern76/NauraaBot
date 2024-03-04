using System;
using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;

namespace NauraaBot.Core.Config;

public static class ConfigProvider
{
    public static Config ConfigInstance { get; private set; }

    public static void LoadConfig()
    {
        string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yml");
        if (!File.Exists(configFilePath))
        {
            throw new FileNotFoundException("Config file not found", configFilePath);
        }
        string yamlContent = File.ReadAllText(configFilePath);

        Deserializer deserializer = new Deserializer();
        ConfigInstance = deserializer.Deserialize<Config>(yamlContent);
    }
}