using Detector.Common;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Data.Common;
using System.Text.Json;
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

        /// <summary>
        /// https://github.com/godsharp/GodSharp.Data.Common.DbProvider
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settingsStorage"></param>
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
            _logger.LogInformation($"{DbProviderFactories.GetFactoryClasses().Rows.Count} data providers ADO.NET installed");
            
            

            foreach (DataRow row in DbProviderFactories.GetFactoryClasses().Rows)
            {
                _logger.LogInformation($"Provider {row["Name"]} configuration:");
                _logger.LogInformation(JsonSerializer.Serialize(row));
            }

            return Task.CompletedTask;
        }
    }
}
