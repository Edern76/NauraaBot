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
using NauraaBot.Discord.Embeds;
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
            MatchCollection matches = Regex.Matches(message.Content, @"\{\{([!]?)([^\}]+)\}\}");
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
            string innerString = match.Groups[2].Value;
            string optionsString = match.Groups[1].Value;
            string cardName;
            string? rarity;
            string? faction;
            string? language;
            string[] split = innerString.Split('|');
            cardName = split[0].Trim();
            if (split.Length == 1)
            {
                rarity = null;
                faction = null;
                language = null;
            }
            else if (split.Length == 2 || split.Length == 3)
            {
                string options = split[1].Trim();
                string[] optionsArray = options.Split(',');
                rarity = optionsArray[0].Trim();
                faction = optionsArray.Length > 1 ? optionsArray[1].Trim() : null;
                if (split.Length == 3)
                {
                    language = split[2].Trim().ToLower();
                }
                else
                {
                    language = null;
                }
            }
            else
            {
                throw new InvalidQueryFormatException(innerString);
            }

            Tuple<string?, Card?> searchResult = HandleCardName(cardName, rarity, faction, language);
            string actualLanguage = searchResult.Item1;
            Card foundCard = searchResult.Item2;
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
                CardRecap recap = foundCard.ToCardRecap(actualLanguage);
                result = CardRecapToEmbed(recap, optionsString);
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

    private static Tuple<string, Card> HandleCardName(string expression, string? rarity, string? faction,
        string? language)
    {
        Tuple<string, Card> result;
        switch (expression.ToLower())
        {
            case "rand()":
                result = CardRandomManager.RandomCard(expression, rarity, faction, language);
                break;
            default:
                result = CardSearchManager.SearchCard(expression, rarity, faction, language);
                break;
        }

        return result;
    }

    private static Embed CardRecapToEmbed(CardRecap recap, string optionsString = "")
    {
        if (optionsString.Contains('!'))
        {
            return ImageCardEmbedBuilder.BuildEmbed(recap);
        }
        else
        {
            return FullCardEmbedBuilder.BuildEmbed(recap);
        }
    }
}