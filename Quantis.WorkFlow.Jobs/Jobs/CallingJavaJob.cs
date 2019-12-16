using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Quantis.WorkFlow.Jobs.Jobs
{
    [DisallowConcurrentExecution]
    public class CallingJavaJob : IJob
    {
        private readonly IServiceProvider _provider;

        public CallingJavaJob(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                // Resolve the Scoped service
                var service = scope.ServiceProvider.GetService<IInformationService>();
                var dbcontext = scope.ServiceProvider.GetService<WorkFlowPostgreSqlContext>();
                var val = service.GetConfiguration("be_scheduler", "slave_1");
                if (val != null)
                {
                    string f = string.Format("java -jar {0}", val.Value);
                    dbcontext.LogInformation("Job To run: The command is: " + f);
                    var res = f.Bash();
                    dbcontext.LogInformation("Job Executed: The command is: " + f + " and result is: " + res);
                }
            }

            return Task.CompletedTask;
        }
    }
}