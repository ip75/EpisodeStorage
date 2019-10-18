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
        private readonly RSAProvider _rsaProvider;

        public MessageProcessor(FileStorage fileStorage, RSAProvider rsaProvider)
        {
            _fileStorage = fileStorage;
            _rsaProvider = rsaProvider;
        }

        public async Task<string> ProcessMessage(ulong episodeId, string commandName, string message)
        {
            try
            {
                switch (commandName)
                {
                    case CommandNames.StoreFile:
                        var directory = _fileStorage.CreateOpenEventDirectory(episodeId, DateTime.Now);
                        var store = await JsonSerializer.DeserializeAsync<StoreFileOperation>(new MemoryStream(Encoding.UTF8.GetBytes(message)));
                        await directory.StoreFile(store.FileName, store.Data);
                        return JsonSerializer.Serialize(new StoreFileOperationResult { Result = ErrorCodes.Success});

                    case CommandNames.RetrieveFile:
                        var retrieve = await JsonSerializer.DeserializeAsync<RetrieveFileOperation>(new MemoryStream(Encoding.UTF8.GetBytes(message)));
                        directory = _fileStorage.CreateOpenEventDirectory(episodeId, retrieve.Datetime);
                        var data = await directory.RetrieveFile(retrieve.FileName);
                        return JsonSerializer.Serialize(new RetrieveFileOperationResult { Data = data, Result = ErrorCodes.Success});

                    case CommandNames.GetPublicKey:
                        return JsonSerializer.Serialize(new GetPublicKeyOperationResult { Key = _rsaProvider.PublicKey, Result = ErrorCodes.Success});

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
