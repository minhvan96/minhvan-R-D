using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Quartz.Demo.Jobs
{
    public class SecondSample : IJob
    {
        private readonly ILogger<FirstSample> _logger;

        public SecondSample(ILogger<FirstSample> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(context.JobDetail.Key + "Second Sample.... job executing, triggered by " + context.Trigger.Key);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void Dispose()
        {
            _logger.LogInformation("Second job disposing");
        }
    }
}