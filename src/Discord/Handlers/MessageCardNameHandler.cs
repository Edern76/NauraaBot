using System;
using System.Collections.Generic;
using System.Linq;
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
using NauraaBot.Discord.Handlers.Parsers;
using NauraaBot.Discord.Handlers.Search;
using NauraaBot.Discord.Types.Search;
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
            MatchCollection matches = Regex.Matches(message.Content, @"\{\{((?:[!@]?(?:U:)?)?)([^\}]+)\}\}");
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
            List<string> errors = new List<string>();
            string innerString = match.Groups[2].Value;
            string optionsString = match.Groups[1].Value;
            string cardName;
            string? rarity;
            string? faction;
            string? language;
            string[] split = innerString.Split('|');
            cardName = split[0].Trim();
            SearchIntent intent = cardName.Contains('_') ? SearchIntent.UNIQUE_ID : SearchIntent.GENERIC;
            ISearchParser parser;
            switch (intent)
            {
                case SearchIntent.UNIQUE_ID:
                    parser = new UniqueSearchParser();
                    break;
                case SearchIntent.GENERIC:
                    parser = new RegularSearchParser();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"No parser has been implemented for intent {intent.ToString()}");
            }

            SearchParams parsedParams = parser.Parse(innerString);

            List<string> paramErrors = ParamsCheckerManager.GetParamErrors(parsedParams);
            if (paramErrors.Count > 0)
            {
                string errorString = string.Join("\n", paramErrors.Select(s => $"- {s}"));
                result = new EmbedBuilder()
                    .WithTitle("The following errors have been encountered in the supplied parameters")
                    .WithDescription(errorString)
                    .WithColor(Color.Orange)
                    .Build();
                return result;
            }

            Tuple<string?, Card?> searchResult = HandleCardName(cardName, parsedParams, intent);
            string actualLanguage = searchResult.Item1;
            if (searchResult.Item2 is null)
            {
                SearchParams secondPassParams = new SearchParams()
                {
                    Faction = parsedParams.Faction,
                    Rarity = parsedParams.Rarity,
                    Language = null,
                };
                searchResult = CardSearchManager.SearchCard(cardName, secondPassParams);
                actualLanguage = parsedParams.Language?.ToLower() ?? searchResult.Item1;
            }

            if (actualLanguage is null)
            {
                actualLanguage = searchResult.Item1;
            }

            Card foundCard = searchResult.Item2;
            if (foundCard is null)
            {
                result = new EmbedBuilder().WithTitle("Not found")
                    .WithDescription(
                        $"The card with name {cardName} was not found. \n Check your spelling or try typing a query closer to the card's full name.")
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
            string errorMessage = $"The search query \"{iqfe.Query}\"'s format has not been recognized.";
            if (iqfe.Intent == SearchIntent.UNIQUE_ID)
            {
                errorMessage +=
                    "\n\n Please note that unique IDs queries only accept a single language parameter after the card name (ex : {{UNIQUE_ID|FR}})";
            }

            result = new EmbedBuilder().WithTitle("Invalid format")
                .WithDescription(errorMessage)
                .WithColor(Color.Orange)
                .Build();
        }

        return result;
    }

    private static Tuple<string, Card?> HandleCardName(string expression, SearchParams searchParams,
        SearchIntent intent)
    {
        ISearchHandler handler;
        switch (intent)
        {
            case SearchIntent.UNIQUE_ID:
                handler = new UniqueIDSearchHandler();
                break;
            case SearchIntent.GENERIC:
                handler = new RegularSearchHandler();
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"No handler has been implemented for intent {intent.ToString()}");
        }

        Tuple<string, Card?> result = handler.Search(expression, searchParams);
        return result;
    }

    private static Embed CardRecapToEmbed(CardRecap recap, string optionsString = "")
    {
        if (optionsString.Contains('!'))
        {
            return ImageCardEmbedBuilder.BuildEmbed(recap);
        }
        else if (optionsString.Contains('@'))
        {
            return CardLocalizationsEmbedBuilder.BuildEmbed(recap);
        }
        else
        {
            return FullCardEmbedBuilder.BuildEmbed(recap);
        }
    }
}