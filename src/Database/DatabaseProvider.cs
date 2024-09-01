using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NauraaBot.API.DTO;
using NauraaBot.Core.Utils;

namespace NauraaBot.Database.Models;

public static class DatabaseProvider
{
    public static DatabaseContext Db { get; private set; }

    public static ConcurrentQueue<Card> PendingAdditions { get; private set; }

    public static void InitializeDatabase()
    {
        LogUtils.Log("Initializing database...");
        Db = new DatabaseContext();
        PendingAdditions = new ConcurrentQueue<Card>();
        LogUtils.Log("Initialized database");
    }

    public static void EnqueueCard(Card card)
    {
        if (PendingAdditions.All(c => c.ID != card.ID))
        {
            PendingAdditions.Enqueue(card);
        }
    }
}