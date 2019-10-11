using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Detector.EpisodeStorage.Common;
using Detector.EpisodeStorage.DetectedDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detector.EpisodeStorage.ScreenShotDB
{
    public class FileStorage
    {
        private readonly ILogger<FileStorage> _logger;
        private readonly IOptions<Config> _config;
        private readonly GlobalSettingsStorage _storage;
        private const string DateDirectoryFormat = "yyyy.MM.dd";
        /// <summary>
        /// Root directory of file storage
        /// </summary>
        private readonly DirectoryInfo _root;

        public FileStorage(ILogger<FileStorage> logger, IOptions<Config> config, GlobalSettingsStorage storage)
        {
            _logger = logger;
            _config = config;
            _storage = storage;
            this._root = new DirectoryInfo(config.Value.RootDirectory);
        }

        public EpisodeDirectory CreateNewEventDirectory(DateTime dateTime)
        {
            _config.Value.TimeoutMessageProcessing = 100;
            var newEpisodeId = _storage.Settings.EpisodeId++;

            var newEventDirectory = Path.Combine(
                _root.FullName,
                dateTime.ToString(format: DateDirectoryFormat),
                dateTime.ToString("HH"),
                $"{dateTime.Minute/10*10:00}",
                newEpisodeId.ToString() ); // dateTime.ToString("HH.mm.ss.sss")

            _logger.LogDebug($"Creating new event directory {newEventDirectory} ...");

            return new EpisodeDirectory {EpisodeId = newEpisodeId, Directory = Directory.CreateDirectory(newEventDirectory)};
        }

        public void Clean(TimeSpan keepPeriod)
        {
            var edgeTime = DateTime.Now - keepPeriod;
            try
            {
                // delete expired directories with contained files asynchronously in different tasks
                // 1. delete recursively previous days.
                _root.GetDirectories(searchPattern: "*", searchOption: SearchOption.TopDirectoryOnly)
                    .Where( directory => DateTime.Parse(directory.Name) < edgeTime.Date)
                    .ToList()
                    .ForEach(async dayDirectory => await Task.Run(() =>
                    {
                        _logger.LogDebug($"Deleting {dayDirectory.FullName} directory...");
                        dayDirectory.Delete(true);
                    }) );

                // 2. delete the rest directories in last day
                _root.GetDirectories(
                        searchPattern: Path.Combine(edgeTime.Date.ToString(format: DateDirectoryFormat), "*"),
                        searchOption: SearchOption.TopDirectoryOnly)
                    .Where(directory => int.Parse(directory.Name) < edgeTime.Hour)
                    .ToList()
                    .ForEach(async hourDirectory => await Task.Run(() =>
                    {
                        _logger.LogDebug($"Deleting {hourDirectory.FullName} directory...");
                        hourDirectory.Delete(true); 
                    }));
            }
            catch (Exception ex) when(ex is UnauthorizedAccessException|| ex is DirectoryNotFoundException)
            {
                _logger.LogError($"Clean error: {ex}", new object[] {_root});
            }
        }
    }
}
