using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NauraaBot.Core.Utils;
using NauraaBot.Discord.Handlers;
using NauraaBot.Discord.Utils;

namespace NauraaBot.Discord;

public static class ClientProvider
{
    public static DiscordSocketClient Client { get; private set; }

    public static async Task InitializeClient(string token)
    {
        LogUtils.Log("Starting Discord client...");
        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100
        });

        Client.Log += DiscordLogHandler.LogDiscordMessage;
        Client.MessageReceived += MessageCardNameHandler.HandleNameSearchOnMessageReceived;

        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        LogUtils.Log("Discord client started.");
    }
}