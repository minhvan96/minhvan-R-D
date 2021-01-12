using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Demo.SchedulerListeners
{
    public class SecondSampleSchedulerListener : SchedulerListenerSupport
    {
        private readonly ILogger<SecondSampleSchedulerListener> _logger;

        public SecondSampleSchedulerListener(ILogger<SecondSampleSchedulerListener> logger)
        {
            _logger = logger;
        }

        public override Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Observed scheduler start");
            return Task.CompletedTask;
        }
    }
}