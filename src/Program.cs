using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NauraaBot.API.DTO;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database;
using NauraaBot.Database.Models;
using NauraaBot.Discord;
using NauraaBot.Managers;

namespace NauraaBot
{
    class Program
    {
        static async Task Main(string[] args)
        { ;
            ConfigProvider.LoadConfig();
            DatabaseProvider.InitializeDatabase();
            
            if (!DatabaseProvider.Db.Cards.Any() || ConfigProvider.ConfigInstance.ForceUpdateDbOnStart || args.Contains("--force-update-db"))
            {
                await CardImportManager.ImportCardsIntoDatabase();
            }
            
            // TODO : Schedule periodic card import
            
            await ClientProvider.InitializeClient(ConfigProvider.ConfigInstance.Token);

            await Task.Delay(-1);
        }
    }
}