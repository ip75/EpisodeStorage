using Detector.EpisodeStorage.Common;
using Detector.EpisodeStorage.DetectedDB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Detector.EpisodeStorage.Main
{
    public class Transceiver : BackgroundService
    {
        private readonly ILogger<Transceiver> _logger;
        private readonly IOptions<Config> _config;
        private readonly MessageProcessor _messageProcessor;
        private readonly GlobalSettingsStorage _storage;

        public Transceiver(ILogger<Transceiver> logger, IOptions<Config> config, MessageProcessor messageProcessor, GlobalSettingsStorage storage)
        {
            _logger = logger;
            _config = config;
            _messageProcessor = messageProcessor;
            _storage = storage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting {this.GetType()} service...");

            using var router = new RouterSocket(_config.Value.ZeroMQRouter);
//            var key = router.ReceiveRoutingKey();
            while (true)
            {
                var resultMessage = new NetMQMessage();

                // start task to process incoming message
                var task = await Task.Factory.StartNew<Task<string>>(async (server) =>
                {
                    var message = ((RouterSocket) server).ReceiveMultipartMessage(4);
                    var dealerId = message[0].ConvertToString();

                    var episodeId = (ulong) message[1].ConvertToInt64();
                    if (episodeId == 0)
                    {
                        episodeId = _storage.Settings.EpisodeId;
                        await _storage.SaveSettings(stoppingToken);
                    }

                    var commandName = message[2].ConvertToString();

                    resultMessage.Append(dealerId);
                    resultMessage.Append((long)episodeId );

                    var command = message[3].ConvertToString();
                    return await _messageProcessor.ProcessMessage(episodeId, commandName, command);
                }, router, stoppingToken);

                // wait for result of message processing
                await task.ContinueWith(res =>
                {
                    resultMessage.Append(res.Result);
                }, stoppingToken);

                router.SendMultipartMessage(resultMessage);
                if (stoppingToken.IsCancellationRequested)
                    break;
            }
        }
    }
}
