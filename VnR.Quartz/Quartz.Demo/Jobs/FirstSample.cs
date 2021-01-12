using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Quartz.Demo.Jobs
{
    public class FirstSample : IJob, IDisposable
    {
        private readonly ILogger<FirstSample> _logger;

        public FirstSample(ILogger<FirstSample> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(context.JobDetail.Key + "First Sample.... job executing, triggered by " + context.Trigger.Key);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void Dispose()
        {
            _logger.LogInformation("FirstSample job disposing");
        }
    }
}