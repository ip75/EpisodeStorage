using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Detector.EpisodeStorage.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detector.EpisodeStorage.DetectedDB
{
    public class GlobalSettingsStorage : BackgroundService
    {
        private readonly ILogger<GlobalSettingsStorage> _logger;
        private readonly IOptions<Config> _config;
        public GlobalSettings Settings { get; private set; }

        private readonly string _jsonFileName = Path.Combine(Environment.CurrentDirectory, "DetectedDB.json");
        private FileSystemWatcher _watcher;

        public GlobalSettingsStorage(ILogger<GlobalSettingsStorage> logger, IOptions<Config> config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task LoadSettings(CancellationToken cancellationToken)
        {
            Settings = JsonSerializer.Deserialize<GlobalSettings>( await File.ReadAllTextAsync(_jsonFileName, Encoding.UTF8, cancellationToken));
        }

        public async Task SaveSettings(CancellationToken cancellationToken)
        {
            await File.WriteAllTextAsync(_jsonFileName,JsonSerializer.Serialize<GlobalSettings>(Settings ), cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting {this.GetType()} service...");
            await LoadSettings(stoppingToken);
            _watcher = new FileSystemWatcher
            {
                Filter = _jsonFileName, NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += async (source, e) => { await LoadSettings(stoppingToken); };
            _watcher.EnableRaisingEvents = true;
        }
    }
}
