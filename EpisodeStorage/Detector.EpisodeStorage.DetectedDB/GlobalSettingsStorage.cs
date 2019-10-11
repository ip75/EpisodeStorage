using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Detector.EpisodeStorage.DetectedDB
{
    public class GlobalSettingsStorage : IHostedService
    {
        public GlobalSettings Settings { get; private set; }

        private readonly string _jsonFileName = Path.Combine(Environment.CurrentDirectory, "DetectedDB.json");
        private FileSystemWatcher _watcher;

        public async Task LoadSettings()
        {
            Settings = JsonSerializer.Deserialize<GlobalSettings>( await File.ReadAllTextAsync(_jsonFileName, Encoding.UTF8));
        }

        public async Task SaveSettings()
        {
            await File.WriteAllTextAsync(_jsonFileName,JsonSerializer.Serialize<GlobalSettings>(Settings ));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await LoadSettings();
            _watcher = new FileSystemWatcher
            {
                Filter = _jsonFileName, NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += async (source, e) => { await LoadSettings(); };
            _watcher.EnableRaisingEvents = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await SaveSettings();
        }
    }
}
