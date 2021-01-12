using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Demo.TriggerListeners
{
    public class SampleOneTriggerListener : TriggerListenerSupport
    {
        private readonly ILogger<SampleOneTriggerListener> _logger;

        public SampleOneTriggerListener(ILogger<SampleOneTriggerListener> logger)
        {
            _logger = logger;
        }

        public override string Name => "Sample ONE Trigger Listener";

        public override Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Observed trigger fire by trigger {TriggerKey}", trigger.Key);
            return Task.CompletedTask;
        }
    }
}