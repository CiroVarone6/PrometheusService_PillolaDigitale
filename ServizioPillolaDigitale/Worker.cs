using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServizioPillolaDigitale.ConfigModels;
using System.Threading.Tasks;
using System.Threading;
using Prometheus;
using Quartz;
using System;
using static Quartz.Logging.OperationName;
using ServiceStack.Text;

namespace ServizioPillolaDigitale
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MetricServer _metricServer;
        private readonly IScheduler _scheduler;




        public Worker(ISchedulerFactory schedulerFactory, ILogger<Worker> logger, PathConfigs pathConfigs, PrometheusConfigs prometheusConfigs)
        {
            _logger = logger;
            _metricServer = new MetricServer(prometheusConfigs.Host, prometheusConfigs.Port);
            _scheduler = schedulerFactory.GetScheduler().Result;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _metricServer.Start();

            // Impostazione dei parametri della classe FilesNumber
            var jobData = new JobDataMap();
            jobData["baseFolders"] = new string[] { "C:\\tempFolder" };

            // Creazione del job
            var jobDetail = JobBuilder.Create<FilesNumber>()
                .SetJobData(jobData)
                .WithIdentity("myJob", "group1")
                .Build();

            // Definizione del trigger
            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(5))
                    .RepeatForever())
                .Build();

            // Programmazione dell'esecuzione periodica del job
            await _scheduler.ScheduleJob(jobDetail, trigger);
            await _scheduler.Start();
        }
    }
}
