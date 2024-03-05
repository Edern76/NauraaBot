using System;
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
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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
        request.AddHeader("User-Agent", "NauraaBot/0.1.0");
        request.AddHeader("Accept", "*/*");
        request.AddParameter("pagination", "false");
        RestResponse response = await _client.ExecuteGetAsync(request);
        string content = response!.Content.Replace("\\/", "/").Replace('\u00A0', ' ')
           .Replace('\u00A9', ' '); // TODO: Find a way to replace \u0022 with " without messing everything up
        // Built-in RestSharp deserialization uses .NET deserialization which I do not trust
        return JsonConvert.DeserializeObject<AlteredResponse>(content);
    }
}