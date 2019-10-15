using Detector.EpisodeStorage.Common;
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
        private readonly CancellationTokenSource _cancellationToken;

        public Transceiver(ILogger<Transceiver> logger, IOptions<Config> config, MessageProcessor messageProcessor)
        {
            _logger = logger;
            _config = config;
            _messageProcessor = messageProcessor;
            _cancellationToken = new CancellationTokenSource(_config.Value.TimeoutMessageProcessing);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting {this.GetType()} service...");

            using var router = new RouterSocket(_config.Value.ZeroMQRouter);
            while (true)
            {
                var resultMessage = new NetMQMessage();

                await Task.Factory.StartNew<Task<string>>(async (server) =>
                {
                    var message = ((RouterSocket) server).ReceiveMultipartMessage(4);
                    var dealerId = message[0].ConvertToString();
                    var episodeId = 1;
                    
                    resultMessage.Append(dealerId);
                    resultMessage.Append(episodeId);

                    return await _messageProcessor.ProcessMessage(episodeId, (string) message[1].ConvertToString());
                }, router, stoppingToken).ContinueWith(async x =>
                {
                    resultMessage.Append(await x.Result);
                    router.SendMultipartMessage(resultMessage);
                }, stoppingToken);


                if (stoppingToken.IsCancellationRequested)
                    break;
            }
        }
    }
}
