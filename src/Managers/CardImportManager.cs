using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore.Storage;
using NauraaBot.API.DTO;
using NauraaBot.API.Requesters;
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
        AlteredResponse apiResponse = await AlteredAPIRequester.GetCards("en");
        List<CardDTO> dtos = apiResponse.Members;

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

            cardsImported++;
        }

        if (cardsImported > 0)
        {
            DatabaseProvider.Db.SaveChanges();
            LogUtils.Log($"Imported or updated {cardsImported} cards");
        }
        else
        {
            LogUtils.Log("No english cards to import or update.");
        }

        await FillCardsMissingLanguages();

        DateTime end = DateTime.Now;
        LogUtils.Log($"Card import finished in {(end - start).TotalSeconds} seconds.");
    }

    public static async Task ImportUniquesIntoDatabase()
    {
        LogUtils.Log("Starting unique import...");
        DateTime start = DateTime.Now;

        List<Card> existingCards = DatabaseProvider.Db.Cards.ToList();
        List<Unique> existingUniques = DatabaseProvider.Db.Uniques.ToList();
        List<string> existingUniquesIds = existingCards.Select(card => card.ID).ToList();

        List<CardSet> sets = DatabaseProvider.Db.Sets.ToList();
        List<CardType> types = DatabaseProvider.Db.Types.ToList();
        List<Rarity> rarities = DatabaseProvider.Db.Rarities.ToList();
        List<Faction> factions = DatabaseProvider.Db.Factions.ToList();
        int currentPage = 1;
        int uniquesFoundOnPage = -1;
        int uniquesImported = 0;
        while (uniquesFoundOnPage != 0)
        {
            AlteredResponse apiResponse =
                await AlteredAPIRequester.GetCards("en", AlteredAPIRequesterSettings.Uniques, currentPage);
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
                        DatabaseProvider.Db.Add(unique);
                        uniquesImported++;
                    }
                }
            }

            currentPage++;
        }

        if (uniquesImported > 0)
        {
            DatabaseProvider.Db.SaveChanges();
            LogUtils.Log($"Imported or updated {uniquesImported} uniques");
        }

        await FillUniquesMissingLanguages();

        DateTime end = DateTime.Now;
        LogUtils.Log($"Uniques import finished in {(end - start).TotalSeconds} seconds.");
    }

    private static async Task FillCardsMissingLanguages()
    {
        LogUtils.Log("Checking for missing languages...");
        List<Card> cardsMissingLanguages =
            DatabaseProvider.Db.Cards.ToList().Where(c => c.GetMissingLanguages().Count > 0).ToList();
        if (cardsMissingLanguages.Count > 0)
        {
            LogUtils.Log($"{cardsMissingLanguages.Count} cards are missing languages.");
            Dictionary<string, List<CardDTO>> languageDtos = new Dictionary<string, List<CardDTO>>();
            IEnumerable<String> supportedLanguages =
                ConfigProvider.ConfigInstance.SupportedLanguages.Where(l => l != "en");
            foreach (string language in supportedLanguages)
            {
                languageDtos.Add(language, (await AlteredAPIRequester.GetCards(language)).Members);
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

    private static async Task FillUniquesMissingLanguages()
    {
        LogUtils.Log("Checking for  uniques missing languages...");
        List<Unique> uniquesMissingLanguages =
            DatabaseProvider.Db.Uniques.ToList().Where(c => c.GetMissingLanguages().Count > 0).ToList();
        if (uniquesMissingLanguages.Count > 0)
        {
            LogUtils.Log($"{uniquesMissingLanguages.Count} uniques are missing languages.");
            Dictionary<string, List<CardDTO>> languageDtos = new Dictionary<string, List<CardDTO>>();
            IEnumerable<String> supportedLanguages =
                ConfigProvider.ConfigInstance.SupportedLanguages.Where(l => l != "en");
            foreach (string language in supportedLanguages)
            {
                int currentPage = 1;
                int uniquesFoundOnPage = -1;
                while (uniquesFoundOnPage != 0)
                {
                    AlteredResponse apiResponse = await AlteredAPIRequester.GetCards(language,
                        AlteredAPIRequesterSettings.Uniques, currentPage);
                    List<CardDTO> dtos = apiResponse.Members;
                    uniquesFoundOnPage = dtos.Count;
                    if (uniquesFoundOnPage > 0)
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

            foreach (Unique uniqueMissingLanguages in uniquesMissingLanguages)
            {
                Unique mutableUnique = uniqueMissingLanguages;
                foreach (string language in supportedLanguages)
                {
                    try
                    {
                        CardDTO dto = languageDtos[language].First(dto => dto.Reference == uniqueMissingLanguages.ID);
                        FillLocalizedStrings(ref mutableUnique, dto, language);
                    }
                    catch (InvalidOperationException)
                    {
                        LogUtils.Error(
                            $"No data found for unique {uniqueMissingLanguages.Names.en} ({uniqueMissingLanguages.ID}) in language {language}");
                    }
                }

                DatabaseProvider.Db.UpdateUnique(uniqueMissingLanguages);
            }

            DatabaseProvider.Db.SaveChanges();
            LogUtils.Log("Done updating uniques languages.");
        }
        else
        {
            LogUtils.Log("No uniques missing languages.");
        }
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