using Detector.Common;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Detector.EpisodeStorage.DetectedDB
{
    /// <summary>
    /// database storage service
    /// </summary>
    public class Service : BackgroundService, ICleaner
    {
        private readonly ILogger<Service> _logger;
        private readonly GlobalSettingsStorage _settingsStorage;

        public Service(ILogger<Service> logger, GlobalSettingsStorage settingsStorage)
        {
            _logger = logger;
            _settingsStorage = settingsStorage;
        }

        public TimeSpan KeepPeriod { get; set; }
        /// <summary>
        /// clean database based on KeepPeriod
        /// </summary>
        public void Clean()
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting {this.GetType()} service...");
            return Task.CompletedTask;
        }
    }
}
