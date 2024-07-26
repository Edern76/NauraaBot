using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;

namespace NauraaBot.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Card> Cards { get; set; }
    public DbSet<Unique> Uniques { get; set; }
    public DbSet<Faction> Factions { get; set; }
    public DbSet<CardSet> Sets { get; set; }
    public DbSet<CardType> Types { get; set; }
    public DbSet<Rarity> Rarities { get; set; }

    public void UpdateCard(Card card)
    {
        Card existingCard = this.Cards.Local.SingleOrDefault(c => c.ID == card.ID);
        if (existingCard != null)
        {
            this.Entry(existingCard).State = EntityState.Detached;
            card.Names.FillMissingLanguagesFrom(existingCard.Names);
            card.ImagesURLs.FillMissingLanguagesFrom(existingCard.ImagesURLs);
            card.Effect.FillMissingLanguagesFrom(existingCard.Effect);
            card.DiscardEffect.FillMissingLanguagesFrom(existingCard.DiscardEffect);
        }


        this.Update(card);
    }

    public void UpdateUnique(Unique unique)
    {
        Unique existingUnique = this.Uniques.Local.SingleOrDefault(c => c.ID == unique.ID);
        if (existingUnique != null)
        {
            this.Entry(existingUnique).State = EntityState.Detached;
            unique.Names.FillMissingLanguagesFrom(existingUnique.Names);
            unique.ImagesURLs.FillMissingLanguagesFrom(existingUnique.ImagesURLs);
            unique.Effect.FillMissingLanguagesFrom(existingUnique.Effect);
            unique.DiscardEffect.FillMissingLanguagesFrom(existingUnique.DiscardEffect);
        }

        this.Update(unique);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Faction>().HasData(new Faction { ID = "AX", Name = "Axiom" },
            new Faction { ID = "BR", Name = "Bravos" },
            new Faction { ID = "LY", Name = "Lyra" },
            new Faction { ID = "MU", Name = "Muna" },
            new Faction { ID = "OR", Name = "Ordis" },
            new Faction { ID = "YZ", Name = "Yzmir" }
        );
        modelBuilder.Entity<Rarity>().HasData(new Rarity { ID = "COMMON", Name = "Common", Short = "C" },
            new Rarity { ID = "RARE", Name = "Rare", Short = "R" },
            new Rarity { ID = "UNIQUE", Name = "Unique", Short = "U" }
        );
        modelBuilder.Entity<CardType>().HasData(new CardType { ID = "HERO", Name = "Hero" },
            new CardType { ID = "SPELL", Name = "Spell" },
            new CardType { ID = "PERMANENT", Name = "Permanent" }
        );
        modelBuilder.Entity<CardSet>();
        modelBuilder.Entity<Card>().ToTable("Card");
        modelBuilder.Entity<Unique>().ToTable("Unique");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // DotNet really DOES NOT like to have anything in constructor so we dump everything in here
        base.OnConfiguring(optionsBuilder);
        LogUtils.Log("Creating context");
        Config config = ConfigProvider.ConfigInstance;
        if (config is null)
        {
            ConfigProvider.LoadConfig(); // Workaround to get migrations working
            config = ConfigProvider.ConfigInstance;
        }

        string basePath = config.DbPath;
        if (basePath is null)
        {
            LogUtils.Log("No set path for database, using default");
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NauraaBot");
        }

        if (config.Database is null)
        {
            LogUtils.Log("No database name set, using default");
            config.Database = "NauraaBot.db";
        }

        Directory.CreateDirectory(basePath);
        string dbPath = Path.Combine(basePath, config.Database);
        LogUtils.Log($"Database path: {dbPath}");
        string connectionString = $"Data Source={dbPath};";
        if (ConfigProvider.ConfigInstance.Password is not null)
        {
            connectionString += $"Password={ConfigProvider.ConfigInstance.Password};";
        }

        SqliteConnection connection = new SqliteConnection(connectionString);
        optionsBuilder.UseSqlite(connection);
    }
}