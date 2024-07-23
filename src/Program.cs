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
using NauraaBot.Quartz;
using NauraaBot.Quartz.Jobs;
using Quartz;

namespace NauraaBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SQLitePCL.Batteries.Init();
            ConfigProvider.LoadConfig();
            DatabaseProvider.InitializeDatabase();
            await SchedulerProvider.InitializeScheduler();

            bool forceUpdate = ConfigProvider.ConfigInstance.ForceUpdateDbOnStart ||
                               args.Contains("--force-update-db");

            if (!DatabaseProvider.Db.Cards.Any() || forceUpdate)
            {
                await CardImportManager.ImportCardsIntoDatabase();
            }

            if (forceUpdate)
            {
                _ = Task.Run(async () => await CardImportManager.ImportUniquesIntoDatabase());
            }

            ScheduleUpdateJob();
            EmoteProvider.InitializeEmotes();
            await ClientProvider.InitializeClient(ConfigProvider.ConfigInstance.Token);

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            await Task.Delay(-1);
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            LogUtils.Log("Exiting...");
            ClientProvider.Client?.Dispose();
            SchedulerProvider.Scheduler?.Shutdown();
            DatabaseProvider.Db.Dispose();
        }

        private static void ScheduleUpdateJob()
        {
            IJobDetail updateJobDetails = UpdateCardDatabaseJob.GetJobDetail();
            ITrigger updateJobTrigger =
                UpdateCardDatabaseJob.GetTrigger(ConfigProvider.ConfigInstance.UpdatePeriodicity);

            SchedulerProvider.Scheduler.ScheduleJob(updateJobDetails, updateJobTrigger);
        }
    }
}