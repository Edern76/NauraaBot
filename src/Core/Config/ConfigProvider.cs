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
        string yamlContent = File.ReadAllText(configFilePath);

        Deserializer deserializer = new Deserializer();
        ConfigInstance = deserializer.Deserialize<Config>(yamlContent);
        
        foreach ( FieldInfo FI in ConfigInstance.GetType().GetFields () )
        {
            if (FI.GetValue (ConfigInstance) is string s && s.Length == 0)
            {
                FI.SetValue(ConfigInstance, null);
            }
        }
    }
}