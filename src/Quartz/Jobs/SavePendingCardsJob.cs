using System;
using System.Threading.Tasks;
using NauraaBot.Core.Utils;
using NauraaBot.Database.Models;
using NauraaBot.Managers;
using NauraaBot.Quartz.Utils;
using Quartz;

namespace NauraaBot.Quartz.Jobs;

[DisallowConcurrentExecution]
public class SavePendingCardsJob : IJob
{
    public static readonly string JOB_NAME = "SavePendingCardsJob";
    public static readonly string TRIGGER_NAME = "SavePendingCardsTrigger";
    public static readonly string GROUP = "group1";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (JobUpdateLock.IsLocked)
            {
                LogUtils.Warn("SavePendingCardsJob execution skipped because UpdateCardDatabaseJob is running.");
                return;
            }

            if (DatabaseProvider.PendingAdditions.Count > 0)
            {
                LogUtils.Log("Saving pending cards...");
                await CardImportManager.SavePendingCardsToDatabase();
            }
        }
        catch (Exception e)
        {
            LogUtils.Error(e.ToString());
            throw new JobExecutionException("An error occurred while saving pending cards.", e, false);
        }
    }

    public static IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<SavePendingCardsJob>()
            .WithIdentity(JOB_NAME, GROUP)
            .Build();
    }

    public static ITrigger GetTrigger(string cron)
    {
        return TriggerBuilder.Create()
            .WithIdentity(TRIGGER_NAME, GROUP)
            .WithCronSchedule(cron)
            .ForJob(JOB_NAME, GROUP)
            .Build();
    }
}