using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;

namespace NauraaBot.Database;

public class DatabaseContext : DbContext
{
    private string DbPath { get; set; }
    
    public DbSet<Card> Cards;
    public DbSet<Faction> Factions;
    public DbSet<CardSet> Sets;
    public DbSet<Rarity> Rarities;

    public DatabaseContext()
    {
        Config config = ConfigProvider.ConfigInstance;
        string basePath = config.db_path;
        if (basePath is null)
        {
            LogUtils.Log("No set path for database, using default");
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        if (config.database is null)
        {
            LogUtils.Log("No database name set, using default");
            config.database = "NauraaBot.db";
        }
        DbPath = Path.Combine(basePath, config.database); 
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Faction>().HasData(new Faction { ID="AX", Name="Axiom" }, 
            new Faction{ID="BR", Name="Bravos"}, 
            new Faction{ID="LY", Name="Lyra"}, 
            new Faction{ID="MU", Name="Muna"},
            new Faction{ID="OR", Name="Ordis"},
            new Faction{ID="YZ", Name="Yzmir"}
            );
        modelBuilder.Entity<Rarity>().HasData(new Rarity{ID="COMMON", Name="Common", Short="C"}, 
            new Rarity{ID="RARE", Name="Rare", Short="R"}, 
            new Rarity{ID="UNIQUE", Name="Unique", Short="U"}
            );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        string connectionString = $"Data Source={DbPath};";
        if (ConfigProvider.ConfigInstance.password is not null)
        {
            connectionString += $"Password={ConfigProvider.ConfigInstance.password};";
        }
        optionsBuilder.UseSqlite(connectionString);
        
    }
}