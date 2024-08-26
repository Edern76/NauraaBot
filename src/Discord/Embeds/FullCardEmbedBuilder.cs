using Discord;
using NauraaBot.Core.Config;
using NauraaBot.Core.Types;
using NauraaBot.Discord.Types.Emote;
using NauraaBot.Discord.Utils;

namespace NauraaBot.Discord.Embeds;

public class FullCardEmbedBuilder : ICardEmbedBuilder
{
    public static Embed BuildEmbed(CardRecap recap)
    {
        string currentFaction = recap.CurrentFaction;
        if (EmoteProvider.Factions.Find(emote => emote.Replaces.ToLower() == currentFaction.ToLower()) is EmoteInfo
            factionEmote)
        {
            currentFaction = factionEmote.Code;
        }

        EmbedBuilder builder = new EmbedBuilder().WithTitle(recap.Name)
            .WithUrl(recap.URL)
            .WithThumbnailUrl(recap.ImageURL)
            .WithDescription(EmbedEffectStringConverter.ToEmbedEffectString(recap.Effect))
            .AddField("Type", recap.CardType, true)
            .AddField("Rarity", recap.Rarity, true)
            .AddField("Set", recap.CardSet, true)
            .AddField("Current faction", currentFaction, true);

        if (recap.Rarity == "Rare")
        {
            builder = builder.WithColor(Color.Blue);
        }

        if (ConfigProvider.ConfigInstance.BigImage)
        {
            builder = builder.WithImageUrl(recap.ImageURL);
        }

        if (recap.CostString is not null)
        {
            builder = builder.AddField("Cost", recap.CostString, true);
        }

        if (recap.PowerString is not null)
        {
            builder = builder.AddField("Power", recap.PowerString, true);
        }

        if (recap.Elo is not null && recap.Elo.HasValue)
        {
            double? familyElo = recap.AverageFamilyElo;
            string familyEloString = familyElo.HasValue ? familyElo.Value.ToString("#.#") : "-";
            builder = builder.AddField("Elo (vs family average)",
                $"{recap.Elo.Value.ToString("#.#")} ({familyEloString})");
        }

        return builder.Build();
    }
}