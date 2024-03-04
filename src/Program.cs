﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NauraaBot.Core.Config;
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
            Console.WriteLine("Hello World!");

            await Task.Delay(-1);
        }
    }
}