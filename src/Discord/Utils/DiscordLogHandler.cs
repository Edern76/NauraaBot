using System.Threading.Tasks;
using Discord;
using NauraaBot.Core.Utils;

namespace NauraaBot.Discord.Utils;

public static class DiscordLogHandler
{
    public static Task LogDiscordMessage(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                LogUtils.Error(message.ToString());
                break;
            case LogSeverity.Warning:
                LogUtils.Warn(message.ToString());
                break;
            case LogSeverity.Info:
                LogUtils.Log(message.ToString());
                break;
        }

        return Task.CompletedTask;
    }
}