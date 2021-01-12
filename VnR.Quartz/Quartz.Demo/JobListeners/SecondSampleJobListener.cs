using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Demo.JobListeners
{
    public class SecondSampleJobListener : JobListenerSupport
    {
        private readonly ILogger<SecondSampleJobListener> _logger;

        public SecondSampleJobListener(ILogger<SecondSampleJobListener> logger)
        {
            _logger = logger;
        }

        public override string Name => "Second Sample Job Listener";

        public override Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("The job is about to be executed, prepare yourself!");
            return Task.CompletedTask;
        }
    }
}