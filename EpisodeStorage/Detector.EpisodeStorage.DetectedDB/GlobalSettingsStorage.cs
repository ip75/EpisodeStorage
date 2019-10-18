using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Detector.EpisodeStorage.DetectedDB
{
    public class GlobalSettingsStorage : BackgroundService
    {
        private readonly ILogger<GlobalSettingsStorage> _logger;
        private GlobalSettings _settings;
        public GlobalSettings Settings {
            get
            {
                if(_settings == null)
                    LoadSettings(_cancellationToken).Wait();
                return _settings;
            }
            private set
            {
                _settings = value;
                SaveSettings(_cancellationToken).Wait();
            }
        }

        private readonly string _jsonFileName = Path.Combine(Environment.CurrentDirectory, "DetectedDB.json");
        private FileSystemWatcher _watcher;
        private readonly CancellationToken _cancellationToken;

        public GlobalSettingsStorage(ILogger<GlobalSettingsStorage> logger)
        {
            _logger = logger;
            _cancellationToken = new CancellationToken();

            _logger.LogInformation($"Initialize {this.GetType()} object...");
            LoadSettings(_cancellationToken).Wait();
            //StartWatcher(_cancellationToken );
        }


        private void StartWatcher(CancellationToken cancellationToken)
        {
            _watcher = new FileSystemWatcher
            {
                Filter = _jsonFileName, NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += async (source, e) => { await LoadSettings(cancellationToken); };
            _watcher.EnableRaisingEvents = true;
        }

        public async Task LoadSettings(CancellationToken cancellationToken)
        {
            Settings = JsonSerializer.Deserialize<GlobalSettings>( await File.ReadAllTextAsync(_jsonFileName, Encoding.UTF8, cancellationToken));
        }

        public async Task SaveSettings(CancellationToken cancellationToken)
        {
            await File.WriteAllTextAsync(_jsonFileName,JsonSerializer.Serialize<GlobalSettings>(Settings ), cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
