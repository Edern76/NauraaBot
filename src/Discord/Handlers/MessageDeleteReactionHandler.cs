using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NauraaBot.Core.Utils;

namespace NauraaBot.Discord.Handlers;

public static class MessageDeleteReactionHandler
{
    public static async Task HandleDeleteRequestOnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage,
        Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        try
        {
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message == null)
            {
                return;
            }

            if (message.ReferencedMessage?.Author?.Id != reaction.UserId)
            {
                return;
            }

            if (message.Author.Id != ClientProvider.Client.CurrentUser.Id)
            {
                return;
            }
            if (reaction.Emote.Name != "🗑️")
            {
                return;
            }

            if (reaction.User.Value.IsBot)
            {
                return;
            }

            if (reaction.MessageId != message.Id)
            {
                return;
            }
            
            await message.DeleteAsync();
        }
        catch (Exception e)
        {
            LogUtils.Error(e.ToString());
        }
    }
}