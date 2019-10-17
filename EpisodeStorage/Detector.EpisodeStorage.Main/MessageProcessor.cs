using Detector.EpisodeStorage.ScreenShotDB;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Detector.Common;
using Detector.EpisodeStorage.Common;
using Detector.EpisodeStorage.Common.Operations;

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
            try
            {
                switch (commandName)
                {
                    case CommandNames.StoreFile:
                        var directory = _fileStorage.CreateNewEventDirectory(episodeId, DateTime.Now);
                        var operation = await JsonSerializer.DeserializeAsync<StoreFileOperation>(new MemoryStream(Encoding.UTF8.GetBytes(message)));
                        await directory.StoreFile(operation.FileName, operation.Data);
                        return JsonSerializer.Serialize(new OperationResult { Result = ErrorCodes.Success});

                    default:
                        throw DetectorException.CreateError(ErrorCodes.InvalidCommandName);
                }
            }
            catch (ErrorException ex)
            {
                return JsonSerializer.Serialize(new OperationResult { Result = ex.Code, Message = ex.Message});;
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new OperationResult { Result = ErrorCodes.UnhandledError, Message = ex.Message});;
            }
        }
    }
}
