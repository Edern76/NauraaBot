using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NauraaBot.Core.Config;
using NauraaBot.Core.Exception;
using NauraaBot.Core.Types;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;
using NauraaBot.Managers;

namespace NauraaBot.Discord.Handlers;

public static class MessageCardNameHandler
{
    public static async Task HandleNameSearchOnMessageReceived(SocketMessage socketMessage)
    {
        if (socketMessage.Author.IsBot || socketMessage == null)
        {
            return;
        }

        // Cast to SocketUserMessage to access the Channel property
        if (socketMessage is not SocketUserMessage message)
        {
            return;
        }

        ITextChannel channel = message.Channel as ITextChannel;
        if (channel == null)
        {
            return; // Channel is not a text channel
        }

        try
        {
            MatchCollection matches = Regex.Matches(message.Content, @"\{\{([^\}]+)\}\}");
            List<Embed> embedsToSend = new List<Embed>();
            foreach (Match match in matches)
            {
                Embed embed = HandleMatch(match);
                embedsToSend.Add(embed);
            }

            if (embedsToSend.Count > ConfigProvider.ConfigInstance.MaxRepliesPerMessages)
            {
                embedsToSend = embedsToSend.GetRange(0, ConfigProvider.ConfigInstance.MaxRepliesPerMessages);
                Embed maxLimitReached = new EmbedBuilder().WithTitle("Max replies limit reached")
                    .WithDescription(
                        "The maximum number of replies per message has been reached. Please send another message to get the rest of the results.")
                    .Build();
                embedsToSend.Add(maxLimitReached);
            }

            foreach (Embed embed in embedsToSend)
            {
                await channel.SendMessageAsync(embed: embed, messageReference: new MessageReference(message.Id));
            }
        }
        catch (Exception e)
        {
            LogUtils.Error(e.ToString());
            try
            {
                Embed embed = new EmbedBuilder().WithTitle("Error")
                    .WithDescription("An error occurred while trying to handle a message update.")
                    .WithColor(Color.Red).Build();
                await ((ITextChannel)channel).SendMessageAsync(embed: embed);
            }
            catch (Exception nestedE)
            {
                LogUtils.Error($"Could not notify or error because of exception {nestedE.ToString()}");
            }
        }
    }

    private static Embed HandleMatch(Match match)
    {
        Embed result;
        try
        {
            string innerString = match.Groups[1].Value;
            string cardName;
            string? rarity;
            string? faction;
            string[] split = innerString.Split('|');
            cardName = split[0].Trim();
            if (split.Length == 1)
            {
                rarity = null;
                faction = null;
            }
            else if (split.Length == 2)
            {
                string options = split[1].Trim();
                string[] optionsArray = options.Split(',');
                rarity = optionsArray[0].Trim();
                faction = optionsArray.Length > 1 ? optionsArray[1].Trim() : null;
            }
            else
            {
                throw new InvalidQueryFormatException(innerString);
            }

            Card foundCard = CardSearchManager.SearchCard(cardName, rarity, faction);
            if (foundCard is null)
            {
                result = new EmbedBuilder().WithTitle("Not found")
                    .WithDescription(
                        $"The card with name {cardName} was not found. \n Make sure the name matches exactly the name of the card (search with approximate names may come in a future version)")
                    .WithColor(Color.Orange)
                    .Build();
            }
            else
            {
                CardRecap recap = foundCard.ToCardRecap();
                result = CardRecapToEmbed(recap);
            }
        }
        catch (InvalidQueryFormatException iqfe)
        {
            result = new EmbedBuilder().WithTitle("Invalid format")
                .WithDescription($"The search query \"{iqfe.Query}\"'s format has not been recognized.")
                .WithColor(Color.Orange)
                .Build();
        }

        return result;
    }

    private static Embed CardRecapToEmbed(CardRecap recap)
    {
        EmbedBuilder builder = new EmbedBuilder().WithTitle(recap.Name)
            .WithUrl(recap.URL)
            .WithThumbnailUrl(recap.ImageURL)
            .WithDescription(recap.Effect)
            .AddField("Type", recap.CardType, true)
            .AddField("Rarity", recap.Rarity, true)
            .AddField("Set", recap.CardSet, true)
            .AddField("Current faction", recap.CurrentFaction, true);
        
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

        return builder.Build();
    }
}