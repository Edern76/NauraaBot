using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NauraaBot.API.DTO;
using NauraaBot.Core.Config;
using NauraaBot.Core.Utils;
using NauraaBot.Database;
using NauraaBot.Database.Models;

namespace NauraaBot
{
    class Program
    {
        static async Task Main(string[] args)
        { ;
            ConfigProvider.LoadConfig();
            DatabaseProvider.InitializeDatabase();

            await Task.Delay(-1);
        }
    }
}