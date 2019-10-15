using System;
using System.Threading;
using System.Threading.Tasks;
using Detector.Common;
using Detector.EpisodeStorage.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detector.EpisodeStorage.ScreenShotDB
{
    public class Service : BackgroundService, ICleaner
    {
        private readonly ILogger<Service> _logger;
        private readonly IOptions<Config> _config;
        private readonly FileStorage _fileStorage;

        public Service(ILogger<Service> logger, IOptions<Config> config, FileStorage fileStorage)
        {
            _logger = logger;
            _config = config;
            KeepPeriod = TimeSpan.FromHours(_config.Value.KeepPeriod);
            this._fileStorage = fileStorage;
        }

        public TimeSpan KeepPeriod { get; set; }
        public void Clean()
        {
            _fileStorage.Clean(KeepPeriod);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting service with configuration: " + _config.Value);
            return Task.CompletedTask;
        }
    }
}
