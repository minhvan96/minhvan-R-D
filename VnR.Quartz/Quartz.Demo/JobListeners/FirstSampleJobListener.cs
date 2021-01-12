using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Demo.JobListeners
{
    public class FirstSampleJobListener : JobListenerSupport
    {
        private readonly ILogger<FirstSampleJobListener> _logger;

        public FirstSampleJobListener(ILogger<FirstSampleJobListener> logger)
        {
            _logger = logger;
        }

        public override string Name => "Sample ONE Job Listener";

        public override Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("The job is about to be executed, prepare yourself!");
            return Task.CompletedTask;
        }
    }
}