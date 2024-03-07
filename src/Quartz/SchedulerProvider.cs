﻿using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;

namespace NauraaBot.Quartz;

public class SchedulerProvider
{
    public static IScheduler Scheduler { get; private set; }

    public static async Task InitializeScheduler()
    {
        NameValueCollection properties = new NameValueCollection();
        Scheduler = await SchedulerBuilder.Create(properties)
            .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
            .BuildScheduler();
        await Scheduler.Start();
    }
}