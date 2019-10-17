using Detector.EpisodeStorage.Main.Operations;
using Detector.EpisodeStorage.ScreenShotDB;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Detector.EpisodeStorage.Main
{
    public class MessageProcessor
    {
        private readonly FileStorage _fileStorage;

        public MessageProcessor(FileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task<string> ProcessMessage(ulong episodeId, string commandName, string message)
        {
            var directory = _fileStorage.CreateNewEventDirectory(episodeId, DateTime.Now);
            var operation = await JsonSerializer.DeserializeAsync<StoreFileOperation>(
                    new MemoryStream(Encoding.UTF8.GetBytes(message)));

            await directory.StoreFile(operation.FileName, operation.Data);

            return JsonSerializer.Serialize(new OperationResult {Result = "Success"});
        }
    }
}
