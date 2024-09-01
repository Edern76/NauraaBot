using System.Threading;
using System.Threading.Tasks;
using NauraaBot.Quartz.Jobs;
using NauraaBot.Quartz.Utils;
using Quartz;

namespace NauraaBot.Quartz.Listeners;

public class UpdateJobListener : IJobListener
{
    public string Name => "UpdateJobListener";

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        if (context.JobDetail.JobType == typeof(UpdateCardDatabaseJob))
        {
            JobUpdateLock.IsLocked = true;
        }

        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken)
    {
        if (context.JobDetail.JobType == typeof(UpdateCardDatabaseJob))
        {
            JobUpdateLock.IsLocked = false;
        }

        return Task.CompletedTask;
    }
}