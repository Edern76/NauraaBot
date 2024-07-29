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

namespace NauraaBot.API.Requesters.UniquesRanker;

public class UniquesRankerRequester
{
    private static readonly HttpClient _httpClient;

    static UniquesRankerRequester()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    public static async Task<UniqueElos> GetRankingAsync(string uniqueId)
    {
        try
        {
            string html = await _httpClient.GetStringAsync($"https://uniquesranking.onrender.com/card/{uniqueId}");
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            IDocument document = await context.OpenAsync(req => req.Content(html));
            IEnumerable<IElement> elements = document.QuerySelectorAll("span.stat-value");
            if (elements.Count() < 2)
            {
                throw new KeyNotFoundException("Unique not found");
            }

            return new UniqueElos()
            {
                Elo = double.Parse(elements.ElementAt(0).TextContent, CultureInfo.InvariantCulture),
                AverageFamilyElo = double.Parse(elements.ElementAt(1).TextContent, CultureInfo.InvariantCulture),
            };
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