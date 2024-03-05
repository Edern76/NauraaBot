using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NauraaBot.API.DTO;
using NauraaBot.API.Requesters;
using NauraaBot.Database.Models;

namespace NauraaBot.Managers;

public static class CardImportManager
{
    public static async Task ImportCardsIntoDatabase()
    {
        //TODO: Implement multiple languages
        AlteredResponse apiResponse = await AlteredAPIRequester.GetCards("en");
        List<CardDTO> dtos = apiResponse.Members;

        await CreateMissingRelatedEntities(dtos);

        // TODO: Finish this
    }

    // For simplicity's sake we're gonna assume no new rarities or card types will be added. We can always go back and change that if needed.
    // Also we need to make sure we're only running this on the english response. Non translated metadata should not be generated from other languages.
    private static async Task CreateMissingRelatedEntities(List<CardDTO> dtos)
    {
        HashSet<string> existingFactionsIds = DatabaseProvider.Db.Factions.Select((faction => faction.ID)).ToHashSet();
        HashSet<string> existingSetsIds = DatabaseProvider.Db.Sets.Select(set => set.ID).ToHashSet();
        
        List<Faction> factionsToCreate = new List<Faction>();
        List<CardSet> setsToCreate = new List<CardSet>();
        
        dtos.ForEach(dto =>
        {
            if (!existingFactionsIds.Contains(dto.MainFaction.Reference))
            {
                factionsToCreate.Add(new Faction()
                {
                    ID = dto.MainFaction.Reference,
                    Name = dto.MainFaction.Name
                });
            }
            if (!existingSetsIds.Contains(dto.CardSet.Reference))
            {
                setsToCreate.Add(new CardSet()
                {
                    ID = dto.CardSet.Reference,
                    Name = dto.CardSet.Name
                });
            }
        });

        if (factionsToCreate.Count > 0 || setsToCreate.Count > 0)
        {
            DatabaseProvider.Db.AddRange(factionsToCreate);
            DatabaseProvider.Db.AddRange(setsToCreate);
            await DatabaseProvider.Db.SaveChangesAsync();
        }
    }
    
    private static Card CardSkeletonFromDTO(CardDTO dto)
    {
        Card result = new Card()
        {
            ID = dto.Reference
        };
        if (dto.Elements.HandCost is int handCost && dto.Elements.RecallCost is int recallCost) // If one is specified, the other should be too. Using an or would not assign the second variable if the first one was present.
        {
            result.Costs = new Costs()
            {
                Hand = handCost,
                Reserve = recallCost
            };
        }
        if (dto.Elements.ForestPower is int forestPower && dto.Elements.MountainPower is int mountainPower && dto.Elements.OceanPower is int oceanPower)
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

    private static void FillLocalizedStrings(ref Card card, CardDTO dto, string language)
    {
        card.Effect.Set(language, dto.Elements.MainEffect);
        card.Names.Set(language, dto.Name);
        card.ImagesURLs.Set(language, dto.ImagePath);
    }
}