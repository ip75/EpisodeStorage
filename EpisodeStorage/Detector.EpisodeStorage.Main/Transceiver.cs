using Detector.EpisodeStorage.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Detector.EpisodeStorage.Main
{
    public class Transceiver : IHostedService
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

        public void StartServer()
        {

            using (var server = new RouterSocket(_config.Value.ZeroMQRouter))
            {
                while (true)
                {
                    var message = server.ReceiveMultipartMessage(4);

                    var episodeId = message[0].ConvertToInt64();
                    var resultMessage = new NetMQMessage();
                    resultMessage.Append(episodeId);

                    Task.Factory.StartNew<string>((detectorMessage) =>
                    {
                        return _messageProcessor.ProcessMessage(episodeId, (string)detectorMessage);
                    }, message[1].ConvertToString(), _cancellationToken.Token).ContinueWith(x =>
                    {
                        resultMessage.Append(x.Result);
                    });
                    
                    server.SendMultipartMessage(resultMessage);

                    if (_cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }

        public void StopServer()
        {
            _cancellationToken.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return new Task(StartServer); 
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return new Task(StopServer); 
        }
    }
}
