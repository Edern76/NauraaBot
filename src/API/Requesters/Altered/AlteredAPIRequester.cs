using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NauraaBot.API.DTO;
using NauraaBot.Core;
using NauraaBot.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RestSharp;

namespace NauraaBot.API.Requesters.Altered;

public static class AlteredAPIRequester
{
    private static RestClient _client;
    private static readonly AsyncRetryPolicy<RestResponse> RetryPolicy;

    static AlteredAPIRequester()
    {
        RestClientOptions options = new RestClientOptions("https://api.altered.gg/");
        options.MaxTimeout = 2 * 60 * 1000;
        _client = new RestClient(options);
        ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        RetryPolicy = Policy.Handle<Exception>()
            .OrResult<RestResponse>(r =>
                (r.StatusCode == HttpStatusCode.RequestTimeout || r?.Content is null || (r?.Content?.StartsWith("<") ?? false)))
            .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    LogUtils.Log(
                        $"Retrying request to Altered API after {timeSpan.TotalSeconds} seconds due to exception ${exception.Exception.Message}. Retry attempt {retryCount}.");
                });
    }

    public static async Task<AlteredResponse> GetCards(string language = "en",
        AlteredAPIRequesterSettings settings = null, string? name = null, int? page = null)
    {
        if (settings is null)
        {
            settings = AlteredAPIRequesterSettings.Default;
        }

        if (!Constants.LanguageHttpCodes.ContainsKey(language))
        {
            throw new NotSupportedException($"Language code {language} is not supported");
        }

        string languageHttpCode = Constants.LanguageHttpCodes[language];
        RestRequest request = new RestRequest("/cards", Method.Get);
        //LogUtils.Log($"Using language code {languageHttpCode} for request to Altered API.");
        request.AddHeader("Accept-Language", languageHttpCode);
        request.AddHeader("User-Agent",
            "NauraaBot/0.6.0"); // TODO: Find a way to increment this automatically from the assembly (not a priority though)
        request.AddHeader("Accept", "application/json");

        if (settings.Pagination is not null)
        {
            request.AddParameter("pagination", settings.Pagination.ToString());
        }

        if (settings.MaxCardsPerRequest is not null)
        {
            request.AddParameter("itemsPerPage", settings.MaxCardsPerRequest.ToString());
        }

        if (settings.Rarity is not null)
        {
            request.AddParameter("rarity[]", "UNIQUE");
        }

        if (settings.Faction is not null)
        {
            request.AddParameter("factions[]", settings.Faction);
        }

        if (page is not null)
        {
            request.AddParameter("page", page.ToString());
        }


        if (name is not null)
        {
            request.AddParameter("translations.name", Uri.EscapeDataString(name));
        }

        RestResponse response = await RetryPolicy.ExecuteAsync(() => _client.ExecuteGetAsync(request));
        string content = StringUtils.Decode(response!.Content);
        // Built-in RestSharp deserialization uses .NET deserialization which I do not trust
        List<CardDTO> cardDtos = JsonConvert.DeserializeObject<List<CardDTO>>(content);
        cardDtos.ForEach(c =>
        {
            c.Elements = c.ElementsToken is JObject obj ? obj.ToObject<ElementsDTO>() : new ElementsDTO();
            c.Elements.CleanCostsAndPowers();
        });
        AlteredResponse
            alteredResponse = new AlteredResponse()
            {
                Members = cardDtos, TotalItems = cardDtos.Count
            }; // Quick and dirty hack cause I don't want to rewrite other stuff that expects an AlteredResponse
        return alteredResponse;
    }
}