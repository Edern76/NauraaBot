using Discord;
using NauraaBot.Core;
using NauraaBot.Core.Config;
using NauraaBot.Core.Types;

namespace NauraaBot.Discord.Embeds;

public class CardLocalizationsEmbedBuilder : ICardEmbedBuilder
{
    public static Embed BuildEmbed(CardRecap recap)
    {
        EmbedBuilder builder = new EmbedBuilder().WithTitle(recap.Name)
            .WithUrl(recap.URL)
            .WithThumbnailUrl(recap.ImageURL);

        if (recap.Rarity == "Rare")
        {
            builder = builder.WithColor(Color.Blue);
        }

        foreach (string lang in ConfigProvider.ConfigInstance.SupportedLanguages)
        {
            builder.AddField(Constants.LanguageFullNames[lang], recap.AllNames.Get(lang), true);
        }

        return builder.Build();
    }
}