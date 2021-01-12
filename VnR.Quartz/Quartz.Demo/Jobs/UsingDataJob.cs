using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Quartz.Demo.Jobs
{
    public class UsingDataJob : IJob, IDisposable
    {
        private readonly ILogger<UsingDataJob> _logger;

        public UsingDataJob(ILogger<UsingDataJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var key = context.JobDetail.Key;
            var dataMap = context.JobDetail.JobDataMap;

            var name = dataMap.GetString("Name");
            var age = dataMap.GetInt("Age");
            var address = dataMap.GetString("Address");

            _logger.LogInformation($"{context.JobDetail.Key}  Name: {name} -- Age:: {age} -- Address: {address} ");
            _logger.LogInformation($"Instance key {key}");
            _logger.LogInformation($"Job is fired by {context.Trigger.Key} Trigger");

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void Dispose()
        {
            _logger.LogInformation("UsingDataJob disposing");
        }
    }
}