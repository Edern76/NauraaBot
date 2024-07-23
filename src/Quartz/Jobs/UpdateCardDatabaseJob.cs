using System;
using System.Threading.Tasks;
using NauraaBot.Core.Utils;
using NauraaBot.Managers;
using Quartz;

namespace NauraaBot.Quartz.Jobs;

public class UpdateCardDatabaseJob : IJob
{
    public static readonly string JOB_NAME = "UpdateCardDatabaseJob";
    public static readonly string TRIGGER_NAME = "UpdateCardDatabaseTrigger";
    public static readonly string GROUP = "group1";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await CardImportManager.ImportCardsIntoDatabase();
            await CardImportManager.ImportUniquesIntoDatabase();
        }
        catch (Exception e)
        {
            LogUtils.Error(e.ToString());
            throw new JobExecutionException("An error occurred while updating the card database.", e, false);
        }
    }

    public static IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<UpdateCardDatabaseJob>()
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