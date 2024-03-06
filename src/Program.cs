using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NauraaBot.API.DTO;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database;
using NauraaBot.Database.Models;
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

            await Task.Delay(-1);
        }
    }
}