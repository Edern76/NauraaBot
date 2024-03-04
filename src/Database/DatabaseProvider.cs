using NauraaBot.Core.Utils;

namespace NauraaBot.Database.Models;

public static class DatabaseProvider
{
    public static DatabaseContext Db { get; private set; }

    public static void InitializeDatabase()
    {
        LogUtils.Log("Initializing database...");
        Db = new DatabaseContext();
        LogUtils.Log("Initialized database");
    }
}