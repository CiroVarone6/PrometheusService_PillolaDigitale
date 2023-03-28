using Quartz;
using Quartz.Impl;
using System;

namespace ServizioPillolaDigitale
{
    public class SchedulerFactory
    {
        private static readonly Lazy<IScheduler> scheduler = new Lazy<IScheduler>(() =>
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();
            return scheduler;
        });

        public static IScheduler Instance => scheduler.Value;
    }
}
