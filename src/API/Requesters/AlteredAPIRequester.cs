using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NauraaBot.API.DTO;
using NauraaBot.Core;
using NauraaBot.Core.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace NauraaBot.API.Requesters;

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

    public static async Task<AlteredResponse> GetCards(string language = "en")
    {
        if (!Constants.LanguageHttpCodes.ContainsKey(language))
        {
            throw new NotSupportedException($"Language code {language} is not supported");
        }

        string languageHttpCode = Constants.LanguageHttpCodes[language];
        RestRequest request = new RestRequest("/cards", Method.Get);
        LogUtils.Log($"Using language code {languageHttpCode} for request to Altered API.");
        request.AddHeader("Accept-Language", languageHttpCode);
        request.AddHeader("User-Agent",
            "NauraaBot/0.4.0"); // TODO: Find a way to increment this automatically from the assembly (not a priority though)
        request.AddHeader("Accept", "application/json");
        request.AddParameter("pagination", "false");
        request.AddParameter("itemsPerPage", "10000");
        RestResponse response = await _client.ExecuteGetAsync(request);
        string content = StringUtils.Decode(response!.Content);
        // Built-in RestSharp deserialization uses .NET deserialization which I do not trust
        List<CardDTO> cardDtos = JsonConvert.DeserializeObject<List<CardDTO>>(content);
        AlteredResponse
            alteredResponse = new AlteredResponse()
            {
                Members = cardDtos, TotalItems = cardDtos.Count
            }; // Quick and dirty hack cause I don't want to rewrite other stuff that expects an AlteredResponse
        return alteredResponse;
    }
}