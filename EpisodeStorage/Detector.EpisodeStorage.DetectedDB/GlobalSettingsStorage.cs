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
        private GlobalSettings _settings;
        public GlobalSettings Settings {
            get
            {
                if(_settings == null)
                    LoadSettings();
                return _settings;
            }
            private set
            {
                _settings = value;
                SaveSettings();
            }
        }

        private readonly string _jsonFileName = Path.Combine(Environment.CurrentDirectory, "DetectedDB.json");
        private FileSystemWatcher _watcher;
        private CancellationToken _cancellationToken;

        public GlobalSettingsStorage(ILogger<GlobalSettingsStorage> logger, IOptions<Config> config)
        {
            _logger = logger;
            _config = config;
            _cancellationToken = new CancellationToken();

            _logger.LogInformation($"Initialize {this.GetType()} object...");
            LoadSettings();
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

        public void LoadSettings()
        {
            Settings = JsonSerializer.Deserialize<GlobalSettings>( File.ReadAllText(_jsonFileName, Encoding.UTF8));
        }
        public async Task LoadSettings(CancellationToken cancellationToken)
        {
            Settings = JsonSerializer.Deserialize<GlobalSettings>( await File.ReadAllTextAsync(_jsonFileName, Encoding.UTF8, cancellationToken));
        }

        public void SaveSettings()
        {
            File.WriteAllText(_jsonFileName,JsonSerializer.Serialize<GlobalSettings>(Settings ));
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
