using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NauraaBot.API.DTO;
using NauraaBot.Core;
using NauraaBot.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace NauraaBot.API.Requesters.Altered;

public static class AlteredAPIRequester
{
    private static RestClient _client;

    static AlteredAPIRequester()
    {
        RestClientOptions options = new RestClientOptions("https://api.altered.gg/");
        _client = new RestClient(options);
        ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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
        RestRequest request = BuildBaseRequest("/cards", languageHttpCode, Method.Get);

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

        RestResponse response = await _client.ExecuteGetAsync(request);
        string content = StringUtils.Decode(response!.Content);
        // Built-in RestSharp deserialization uses .NET deserialization which I do not trust
        List<CardDTO> cardDtos = JsonConvert.DeserializeObject<List<CardDTO>>(content);
        cardDtos.ForEach(c => { FixCardDto(ref c); });
        AlteredResponse
            alteredResponse = new AlteredResponse()
            {
                Members = cardDtos, TotalItems = cardDtos.Count
            }; // Quick and dirty hack cause I don't want to rewrite other stuff that expects an AlteredResponse
        return alteredResponse;
    }

    public static async Task<CardDTO> GetCard(string ID, string language = "en")
    {
        if (!Constants.LanguageHttpCodes.ContainsKey(language))
        {
            throw new NotSupportedException($"Language code {language} is not supported");
        }

        string languageHttpCode = Constants.LanguageHttpCodes[language];
        RestRequest request = BuildBaseRequest($"/cards/{ID}", languageHttpCode, Method.Get);
        RestResponse response = await _client.ExecuteGetAsync(request);
        string content = StringUtils.Decode(response!.Content);
        CardDTO cardDto = JsonConvert.DeserializeObject<CardDTO>(content);
        FixCardDto(ref cardDto);
        return cardDto;
    }

    private static RestRequest BuildBaseRequest(string endpoint, string language, Method method = Method.Get)
    {
        RestRequest request = new RestRequest(endpoint, method);
        LogUtils.Log($"Using language code {language} for request to Altered API.");
        request.AddHeader("Accept-Language", language);
        request.AddHeader("User-Agent",
            "NauraaBot/0.7.3"); // TODO: Find a way to increment this automatically from the assembly (not a priority though)
        request.AddHeader("Accept", "application/json");

        return request;
    }

    private static void FixCardDto(ref CardDTO c)
    {
        c.Elements = c.ElementsToken is JObject obj ? obj.ToObject<ElementsDTO>() : new ElementsDTO();
        c.Elements.CleanCostsAndPowers();
    }
}