using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Demo.TriggerListeners
{
    public class SecondSampleTriggerListener : TriggerListenerSupport
    {
        private readonly ILogger<SecondSampleTriggerListener> _logger;

        public SecondSampleTriggerListener(ILogger<SecondSampleTriggerListener> logger)
        {
            _logger = logger;
        }

        public override string Name => "Second Sample Trigger Listener";

        public override Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Observed trigger fire by trigger {TriggerKey}", trigger.Key);
            return Task.CompletedTask;
        }
    }
}