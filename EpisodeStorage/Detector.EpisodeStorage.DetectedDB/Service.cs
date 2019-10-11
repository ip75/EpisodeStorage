using Detector.Common;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Detector.EpisodeStorage.DetectedDB
{
    /// <summary>
    /// database storage service
    /// </summary>
    public class Service : IHostedService, IDisposable, ICleaner
    {
        private readonly GlobalSettingsStorage _settingsStorage;

        public Service(GlobalSettingsStorage settingsStorage)
        {
            _settingsStorage = settingsStorage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public TimeSpan KeepPeriod { get; set; }
        /// <summary>
        /// clean database based on KeepPeriod
        /// </summary>
        public void Clean()
        {
        }
    }
}
