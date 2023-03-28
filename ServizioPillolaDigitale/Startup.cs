using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Serilog;
using ServizioPillolaDigitale.ConfigModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Prometheus;

namespace ServizioPillolaDigitale
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISchedulerFactory>(new StdSchedulerFactory());
            services.AddHostedService<Worker>();

            services.AddSingleton((sp) => Configuration.GetSection("PathConfigs")
                                                       .Get<PathConfigs>());

            services.AddSingleton((sp) => Configuration.GetSection("PrometheusConfigs")
                                                       .Get<PrometheusConfigs>());

            Log.Logger = new LoggerConfiguration()
                    .CreateLogger();
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseMetricServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
