using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantis.WorkFlow.Jobs.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Quantis.WorkFlow.Jobs
{
    public static class BaseJobRegistry
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration conf)
        {
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            if (!string.IsNullOrEmpty(conf["CronJobJavaCallingExpression"]))
            {
                services.AddSingleton<CallingJavaJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(CallingJavaJob),
                    cronExpression: conf["CronJobJavaCallingExpression"])); // run every 5 seconds
            }
            if (!string.IsNullOrEmpty(conf["CronJobCreatingTicketsExpression"]))
            {
                services.AddSingleton<CreateTicketsJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(CreateTicketsJob),
                    cronExpression: conf["CronJobCreatingTicketsExpression"])); // run every 5 seconds
            }
            services.AddHostedService<QuartzHostedService>();
        }
    }
}