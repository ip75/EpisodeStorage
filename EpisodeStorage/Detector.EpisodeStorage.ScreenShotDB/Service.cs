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
    public class Service : IHostedService, IDisposable, ICleaner
    {
        private readonly ILogger<Service> _logger;
        private readonly IOptions<Config> _config;

        private readonly FileStorage _fileStorage;
        public Service(ILogger<Service> logger, IOptions<Config> config, FileStorage fileStorage)
        {
            KeepPeriod = TimeSpan.FromHours(_config.Value.KeepPeriod);
            _logger = logger;
            _config = config;
            this._fileStorage = fileStorage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting service with configuration: " + _config.Value);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping service.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");

        }

        public TimeSpan KeepPeriod { get; set; }
        public void Clean()
        {
            _fileStorage.Clean(KeepPeriod);
        }
    }
}
