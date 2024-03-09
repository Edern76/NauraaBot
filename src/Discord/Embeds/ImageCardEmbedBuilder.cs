using Discord;
using NauraaBot.Core.Types;

namespace NauraaBot.Discord.Embeds;

public class ImageCardEmbedBuilder : ICardEmbedBuilder
{
    public static Embed BuildEmbed(CardRecap recap)
    {
        EmbedBuilder builder = new EmbedBuilder().WithTitle(recap.Name).WithUrl(recap.URL).WithImageUrl(recap.ImageURL);
        if (recap.Rarity == "Rare")
        {
            builder = builder.WithColor(Color.Blue);
        }

        return builder.Build();
    }
}