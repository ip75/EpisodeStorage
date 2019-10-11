using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NetMQ;

namespace Detector.EpisodeStorage.Main
{
    public class MessageProcessor : IHostedService
    {
        public MessageProcessor()
        {
            
        }

        public string ProcessMessage(long episodeId, string message)
        {
            return string.Empty;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
