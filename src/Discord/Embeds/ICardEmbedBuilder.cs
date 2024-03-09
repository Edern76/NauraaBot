using System;
using Discord;
using NauraaBot.Core.Types;

namespace NauraaBot.Discord.Embeds;

public interface ICardEmbedBuilder
{
    static Embed BuildEmbed(CardRecap recap)
    {
        throw new NotSupportedException("Cannot call this method from the interface, this needs to be overridden.");
    }
}