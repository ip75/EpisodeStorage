using System;
using System.Threading;
using System.Threading.Tasks;
using Detector.Common;
using Microsoft.Extensions.Hosting;

namespace Detector.EpisodeStorage.DetectedDB
{
    public class Service : IHostedService, IDisposable, ICleaner
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TimeSpan KeepPeriod { get; set; }
        /// <summary>
        /// clean database based on KeepPeriod
        /// </summary>
        public void Clean()
        {
            throw new NotImplementedException();
        }
    }
}
