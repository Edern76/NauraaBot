using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using NauraaBot.API.DTO;
using NauraaBot.Core.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace NauraaBot.API.Requesters.UniquesRanker;

public class UniquesRankerRequester
{
    private static RestClient _client;

    static UniquesRankerRequester()
    {
        RestClientOptions options = new RestClientOptions("https://uniquesranking.com/");
        _client = new RestClient(options);
        ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    public static async Task<UniqueElos> GetRankingAsync(string uniqueId)
    {
        try
        {
            RestRequest request = new RestRequest($"/json/{uniqueId}", Method.Get);
            RestResponse response = await _client.ExecuteGetAsync(request);
            string content = StringUtils.Decode(response!.Content);
            UniquesRankingResponse uniquesResponse = JsonConvert.DeserializeObject<UniquesRankingResponse>(content);
            if (uniquesResponse.Error)
            {
                throw new KeyNotFoundException($"UniquesRanking has no data for unique {uniqueId}");
            }

            UniqueElos uniqueElos = UniqueElos.FromResponse(uniquesResponse);
            return uniqueElos;
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound || e.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new KeyNotFoundException("Unique not found");
            }

            throw;
        }
    }
}