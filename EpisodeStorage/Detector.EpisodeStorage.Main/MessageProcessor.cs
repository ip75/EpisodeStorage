using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Detector.EpisodeStorage.ScreenShotDB;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Detector.EpisodeStorage.Main.Operations;

namespace Detector.EpisodeStorage.Main
{
    public class MessageProcessor : IHostedService
    {
        private readonly FileStorage _fileStorage;

        public MessageProcessor(FileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task<string> ProcessMessage(long episodeId, string message)
        {
            var directory = _fileStorage.CreateNewEventDirectory(DateTime.Now);
            var operation = await JsonSerializer.DeserializeAsync<StoreFileOperation>(new MemoryStream(Encoding.UTF8.GetBytes(message)));
            await directory.StoreFile(operation.FileName, operation.Data);
            return JsonSerializer.Serialize(new OperationResult { Result = "OK"});
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
