using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NauraaBot.API.DTO;
using NauraaBot.API.Requesters.Altered;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;

namespace NauraaBot.Managers;

public static class CardImportManager
{
    public static async Task ImportCardsIntoDatabase()
    {
        LogUtils.Log("Starting card import...");
        DateTime start = DateTime.Now;
        int cardsImported = 0;
        int currentPage = 1;
        int cardsFoundOnPage = -1;
        while (cardsFoundOnPage != 0)
        {
            int cardsImportedOnPage = 0;
            AlteredResponse apiResponse =
                await AlteredAPIRequester.GetCards("en", AlteredAPIRequesterSettings.Default, null, currentPage);
            List<CardDTO> dtos = apiResponse.Members;
            cardsFoundOnPage = dtos.Count;

            if (cardsFoundOnPage > 0)
            {
                await CreateMissingRelatedEntities(dtos);

                List<Card> existingCards = DatabaseProvider.Db.Cards.ToList();
                List<string> existingCardsIds = existingCards.Select(card => card.ID).ToList();

                List<CardSet> sets = DatabaseProvider.Db.Sets.ToList();
                List<CardType> types = DatabaseProvider.Db.Types.ToList();
                List<Rarity> rarities = DatabaseProvider.Db.Rarities.ToList();
                List<Faction> factions = DatabaseProvider.Db.Factions.ToList();

                foreach (CardDTO dto in dtos)
                {
                    Card card = CreateCard(dto, sets, types, rarities, factions);
                    if (!existingCardsIds.Contains(card.ID))
                    {
                        DatabaseProvider.Db.Add(card);
                    }
                    else
                    {
                        DatabaseProvider.Db.UpdateCard(card);
                    }

                    cardsImportedOnPage++;
                }

                if (cardsImportedOnPage > 0)
                {
                    cardsImported += cardsImportedOnPage;
                    DatabaseProvider.Db.SaveChanges();
                    LogUtils.Log($"Imported or updated {cardsImportedOnPage} cards");
                }
                else
                {
                    LogUtils.Log("No english cards to import or update.");
                }
            }

            currentPage++;
        }

        if (cardsImported > 0)
        {
            await FillCardsMissingLanguages();
        }


        DateTime end = DateTime.Now;
        LogUtils.Log(
            $"Card import finished in {(end - start).TotalSeconds} seconds with {cardsImported} cards imported.");
    }

    public static async Task ImportUniquesIntoDatabase()
    {
        LogUtils.Log("Starting unique import...");
        DateTime start = DateTime.Now;

        List<Card> existingCards = DatabaseProvider.Db.Cards.ToList();
        List<Unique> existingUniques = DatabaseProvider.Db.Uniques.ToList();
        List<string> existingUniquesIds = existingUniques.Select(card => card.ID).ToList();
        List<string> factionsIds = DatabaseProvider.Db.Factions.Select(faction => faction.ID).ToList();

        List<CardSet> sets = DatabaseProvider.Db.Sets.ToList();
        List<CardType> types = DatabaseProvider.Db.Types.ToList();
        List<Rarity> rarities = DatabaseProvider.Db.Rarities.ToList();
        List<Faction> factions = DatabaseProvider.Db.Factions.ToList();
        int totalUniquesImported = 0;
        List<Task<int>> tasks = factionsIds.Where(f => f != "NE").Select(f => ImportFactionsUniques(existingUniquesIds,
            sets, types,
            rarities, factions, f)).ToList();
        int[] results = await Task.WhenAll(tasks);
        totalUniquesImported += results.Sum();
        LogUtils.Log($"All factions processed, {totalUniquesImported} uniques imported");
        if (totalUniquesImported > 0)
        {
            await DatabaseProvider.Db.SaveChangesAsync();
        }


        DateTime end = DateTime.Now;
        LogUtils.Log(
            $"Uniques import finished in {(end - start).TotalSeconds} seconds with {totalUniquesImported} uniques imported.");
    }

    private static async Task<int> ImportFactionsUniques(List<string> existingUniquesIds, List<CardSet> sets,
        List<CardType> types, List<Rarity> rarities, List<Faction> factions, string faction)
    {
        LogUtils.Log($"Handling uniques for faction {faction}.");
        List<Unique> newUniques = new List<Unique>();
        int currentPage = 1;
        int uniquesFoundOnPage = -1;
        int uniquesImported = 0;
        int lastModulo = 0;

        while (uniquesFoundOnPage != 0)
        {
            AlteredAPIRequesterSettings settings =
                (AlteredAPIRequesterSettings)AlteredAPIRequesterSettings.Uniques.Clone();
            settings.Faction = faction;
            AlteredResponse apiResponse =
                await AlteredAPIRequester.GetCards("en", settings, null,
                    currentPage);
            List<CardDTO> dtos = apiResponse.Members;
            uniquesFoundOnPage = dtos.Count;
            if (uniquesFoundOnPage > 0)
            {
                foreach (CardDTO dto in dtos)
                {
                    Card card = CreateCard(dto, sets, types, rarities, factions);
                    Unique unique = Unique.FromCard(card);
                    if (!existingUniquesIds.Contains(unique.ID))
                    {
                        newUniques.Add(unique);
                        uniquesImported++;
                    }
                }

                int modulo = 500 & uniquesImported;
                if (modulo > lastModulo)
                {
                    lastModulo = modulo;
                    LogUtils.Log($"Imported {uniquesImported} uniques for faction {faction}.");
                }
            }

            currentPage++;
        }

        List<Unique> filledUniques = await FillUniquesMissingLanguages(newUniques, faction);
        filledUniques.ForEach(u => DatabaseProvider.Db.AddOrUpdateUnique(u));
        LogUtils.Log($"Done with faction {faction}, added {filledUniques.Count} uniques.");
        return filledUniques.Count;
    }

    private static async Task FillCardsMissingLanguages()
    {
        LogUtils.Log("Checking for missing languages...");
        List<Card> cardsMissingLanguages =
            DatabaseProvider.Db.Cards.ToList().Where(c => c.GetMissingLanguages().Count > 0 && c.Rarity.Short != "U")
                .ToList();
        if (cardsMissingLanguages.Count > 0)
        {
            LogUtils.Log($"{cardsMissingLanguages.Count} cards are missing languages.");
            Dictionary<string, List<CardDTO>> languageDtos = new Dictionary<string, List<CardDTO>>();
            IEnumerable<String> supportedLanguages =
                ConfigProvider.ConfigInstance.SupportedLanguages.Where(l => l != "en");
            foreach (string language in supportedLanguages)
            {
                int currentPage = 1;
                int cardsFoundOnPage = -1;
                while (cardsFoundOnPage != 0)
                {
                    AlteredResponse apiResponse = await AlteredAPIRequester.GetCards(language,
                        AlteredAPIRequesterSettings.Default, null, currentPage);
                    List<CardDTO> dtos = apiResponse.Members;
                    cardsFoundOnPage = dtos.Count;
                    if (cardsFoundOnPage > 0)
                    {
                        if (!languageDtos.ContainsKey(language))
                        {
                            languageDtos.Add(language, dtos);
                        }
                        else
                        {
                            languageDtos[language].AddRange(dtos);
                        }
                    }

                    currentPage++;
                }
            }

            foreach (Card cardMissingLanguages in cardsMissingLanguages)
            {
                Card mutableCard = cardMissingLanguages;
                foreach (string language in supportedLanguages)
                {
                    try
                    {
                        CardDTO dto = languageDtos[language].First(dto => dto.Reference == cardMissingLanguages.ID);
                        FillLocalizedStrings<Card>(ref mutableCard, dto, language);
                    }
                    catch (InvalidOperationException)
                    {
                        LogUtils.Error(
                            $"No data found for card {cardMissingLanguages.Names.en} ({cardMissingLanguages.ID}) in language {language}");
                    }
                }

                DatabaseProvider.Db.UpdateCard(cardMissingLanguages);
            }

            DatabaseProvider.Db.SaveChanges();
            LogUtils.Log("Done updating languages.");
        }
        else
        {
            LogUtils.Log("No cards missing languages.");
        }
    }

    private static async Task<List<Unique>> FillUniquesMissingLanguages(List<Unique> factionUniques,
        string faction = null)
    {
        LogUtils.Log("Checking for uniques missing languages...");
        HashSet<Unique> uniquesMissingLanguagesToAdd =
            factionUniques.Where(c => c.GetMissingLanguages().Count > 0).ToHashSet();
        HashSet<string> idsToAdd = uniquesMissingLanguagesToAdd.Select(u => u.ID).ToHashSet();
        HashSet<Unique> uniquesMissingLanguagesFromDb = DatabaseProvider.Db.Uniques.Include(card => card.CurrentFaction)
            .Include(card => card.MainFaction).Include(card => card.Rarity).Include(card => card.Set)
            .Include(card => card.Type).Where(u => u.CurrentFaction.ID == faction && u.Rarity.ID == "UNIQUE").ToList()
            .Where(u => u.GetMissingLanguages().Count > 0 && !idsToAdd.Contains(u.ID)).ToHashSet();
        HashSet<Unique> uniquesMissingLanguages =
            uniquesMissingLanguagesToAdd.Union(uniquesMissingLanguagesFromDb).ToHashSet();
        if (uniquesMissingLanguages.Count > 0)
        {
            LogUtils.Log($"{uniquesMissingLanguages.Count} uniques are missing languages.");

            IEnumerable<String> supportedLanguages =
                ConfigProvider.ConfigInstance.SupportedLanguages.Where(l => l != "en");
            List<Task<Tuple<string, List<CardDTO>>>> tasks = supportedLanguages
                .Select(l => HandleUniquesLanguage(l, faction)).ToList();
            Tuple<string, List<CardDTO>>[] results = await Task.WhenAll(tasks);
            Dictionary<string, List<CardDTO>> languageDtos = results.ToDictionary(r => r.Item1, r => r.Item2);

            foreach (Unique uniqueMissingLanguages in uniquesMissingLanguages)
            {
                Unique mutableUnique = uniqueMissingLanguages;
                foreach (string language in supportedLanguages)
                {
                    try
                    {
                        CardDTO dto = languageDtos[language].First(dto => dto.Reference == uniqueMissingLanguages.ID);
                        FillLocalizedStrings(ref mutableUnique, dto, language);
                        Unique existingUnique = factionUniques.FirstOrDefault(u => u.ID == mutableUnique.ID);
                        if (existingUnique is null)
                        {
                            factionUniques.Add(mutableUnique);
                        }
                        else
                        {
                            existingUnique.FillMissingLanguagesFrom(mutableUnique);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        //LogUtils.Error($"No data found for unique {uniqueMissingLanguages.Names.en} ({uniqueMissingLanguages.ID}) in language {language}");
                    }
                }
            }

            LogUtils.Log("Done updating uniques languages.");
        }
        else
        {
            LogUtils.Log("No uniques missing languages.");
        }

        return factionUniques;
    }

    private static async Task<Tuple<string, List<CardDTO>>> HandleUniquesLanguage(
        string language,
        string faction)
    {
        LogUtils.Log($"Handling uniques language {language} for faction {faction}.");
        int currentPage = 1;
        int uniquesFoundOnPage = -1;
        List<CardDTO> resultDtos = new List<CardDTO>();
        while (uniquesFoundOnPage != 0)
        {
            try
            {
                AlteredAPIRequesterSettings settings =
                    (AlteredAPIRequesterSettings)AlteredAPIRequesterSettings.Uniques.Clone();
                settings.Faction = faction;
                AlteredResponse apiResponse = await AlteredAPIRequester.GetCards(language,
                    settings, null, currentPage);
                List<CardDTO> dtos = apiResponse.Members;
                uniquesFoundOnPage = dtos.Count;
                if (uniquesFoundOnPage > 0)
                {
                    resultDtos.AddRange(dtos);
                }
                else
                {
                    LogUtils.Log(
                        $"No uniques found on page {currentPage} for faction {faction} with language {language}.");
                }

                currentPage++;
            }
            catch (Exception e)
            {
                LogUtils.Error(
                    $"Error while fetching uniques for faction {faction} with language {language} on page {currentPage} : {e.ToString()}");
                currentPage++;
            }
        }

        LogUtils.Log($"Done with language {language} for faction {faction}.");
        return new Tuple<string, List<CardDTO>>(language, resultDtos);
    }


    // For simplicity's sake we're gonna assume no new rarities will be added. We can always go back and change that if needed.
    // Also we need to make sure we're only running this on the english response. Non translated metadata should not be generated from other languages.
    private static async Task CreateMissingRelatedEntities(List<CardDTO> dtos)
    {
        HashSet<string> existingFactionsIds =
            DatabaseProvider.Db.Factions.Select((faction => faction.ID)).ToHashSet();
        HashSet<string> existingTypesIds = DatabaseProvider.Db.Types.Select(type => type.ID).ToHashSet();
        HashSet<string> existingSetsIds = DatabaseProvider.Db.Sets.Select(set => set.ID).ToHashSet();

        HashSet<Faction> factionsToCreate = new HashSet<Faction>();
        HashSet<CardType> typesToCreate = new HashSet<CardType>();
        HashSet<CardSet> setsToCreate = new HashSet<CardSet>();

        dtos.ForEach(dto =>
        {
            if (dto.CardSet is null)
            {
                string setID = dto.Reference.Split('_')[1];
                dto.CardSet = new IDNameObject()
                {
                    Reference = setID,
                    Name = setID,
                    Type = "Set",
                };
            }

            // TODO: Move that to a generic method maybe ?
            if (!existingFactionsIds.Contains(dto.MainFaction.Reference))
            {
                bool added = factionsToCreate.Add(new Faction()
                {
                    ID = dto.MainFaction.Reference,
                    Name = dto.MainFaction.Name
                });
                if (added)
                {
                    LogUtils.Log($"Adding new faction {dto.MainFaction.Reference}");
                }
            }

            if (!existingSetsIds.Contains(dto.CardSet.Reference))
            {
                bool added = setsToCreate.Add(new CardSet()
                {
                    ID = dto.CardSet.Reference,
                    Name = dto.CardSet.Name
                });
                if (added)
                {
                    LogUtils.Log($"Adding new set {dto.CardSet.Reference}");
                }
            }

            if (!existingTypesIds.Contains(dto.CardType.Reference))
            {
                bool added = typesToCreate.Add(new CardType()
                {
                    ID = dto.CardType.Reference,
                    Name = dto.CardType.Name
                });
                if (added)
                {
                    LogUtils.Log($"Adding new type {dto.CardType.Reference}");
                }
            }
        });

        if (factionsToCreate.Count > 0 || setsToCreate.Count > 0 || typesToCreate.Count > 0)
        {
            DatabaseProvider.Db.AddRange(factionsToCreate);
            DatabaseProvider.Db.AddRange(typesToCreate);
            DatabaseProvider.Db.AddRange(setsToCreate);
            await DatabaseProvider.Db.SaveChangesAsync();
        }
    }

    // Create a card object from English values
    // Filling in other languages will be handled in a different method
    private static Card CreateCard(CardDTO dto, List<CardSet> sets, List<CardType> types, List<Rarity> rarities,
        List<Faction> factions)
    {
        Card card = CardSkeletonFromDTO(dto);
        FillLocalizedStrings(ref card, dto, "en");
        LinkCardToRelatedEntities(ref card, dto, sets, types, rarities, factions);
        return card;
    }

    private static Card CardSkeletonFromDTO(CardDTO dto)
    {
        Card result = new Card()
        {
            ID = dto.Reference,
        };
        if (dto.UpdatedAt is not null)
        {
            result.LastUpdated =
                DateTime.Parse(dto.UpdatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        if (dto.Elements.HandCost is int handCost &&
            dto.Elements
                    .RecallCost is int
                recallCost) // If one is specified, the other should be too. Using an or would not assign the second variable if the first one was present.
        {
            result.Costs = new Costs()
            {
                Hand = handCost,
                Reserve = recallCost
            };
        }

        if (dto.Elements.ForestPower is int forestPower && dto.Elements.MountainPower is int mountainPower &&
            dto.Elements.OceanPower is int oceanPower)
        {
            result.Power = new Power()
            {
                Forest = forestPower,
                Mountain = mountainPower,
                Ocean = oceanPower
            };
        }

        return result;
    }

    private static Card LinkCardToRelatedEntities(ref Card card, CardDTO dto, List<CardSet> sets,
        List<CardType> types,
        List<Rarity> rarities, List<Faction> factions)
    {
        if (dto.CardSet is null)
        {
            string setID = dto.Reference.Split('_')[1];
            dto.CardSet = new IDNameObject()
            {
                Reference = setID,
                Name = setID,
                Type = "Set",
            };
        }

        string mainFactionId = dto.Reference.Split('_')[3];
        card.Set = sets.First(set => set.ID == dto.CardSet.Reference);
        card.Rarity = rarities.First(rarity => rarity.ID == dto.Rarity.Reference);
        card.Type = types.First(type => type.ID == dto.CardType.Reference);
        card.MainFaction = factions.First(faction => faction.ID == mainFactionId);
        card.CurrentFaction = factions.First(faction => faction.ID == dto.MainFaction.Reference);
        return card;
    }

    private static void FillLocalizedStrings<T>(ref T card, CardDTO dto, string language) where T : Card
    {
        if (dto.Elements.MainEffect is not null)
        {
            card.Effect.Set(language, dto.Elements.MainEffect);
        }

        if (dto.Elements.DiscardEffect is not null)
        {
            card.DiscardEffect.Set(language, dto.Elements.DiscardEffect);
        }

        if (dto.Name is not null)
        {
            card.Names.Set(language, dto.Name);
        }

        if (dto.ImagePath is not null)
        {
            card.ImagesURLs.Set(language, dto.ImagePath);
        }
    }
}