using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Demo.SchedulerListeners
{
    public class SampleOneSchedulerListener : SchedulerListenerSupport
    {
        private readonly ILogger<SampleOneSchedulerListener> _logger;

        public SampleOneSchedulerListener(ILogger<SampleOneSchedulerListener> logger)
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