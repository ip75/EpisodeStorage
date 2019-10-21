using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;
using Detector.EpisodeStorage.Common;
using Detector.EpisodeStorage.DetectedDB;
using Detector.EpisodeStorage.ScreenShotDB;

namespace Detector.EpisodeStorage.Main
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    config.AddJsonFile("EpisodeStorage.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("db.providers.json");

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<Config>(hostContext.Configuration.GetSection("Common"));
                    services.Configure<Config>(hostContext.Configuration.GetSection("Database"));
                    services.Configure<Config>(hostContext.Configuration.GetSection("Lightener"));
                    services.Configure<Config>(hostContext.Configuration.GetSection("Transmitter"));

                    services.AddSingleton<GlobalSettingsStorage>();
                    services.AddSingleton<ScreenShotDB.FileStorage>();
                    services.AddSingleton<RSAProvider>();
                    services.AddScoped<MessageProcessor>();

                    services.AddHostedService<Transceiver>();
                    services.AddHostedService<ScreenShotDB.Service>();
                    services.AddHostedService<DetectedDB.Service>();
                })
                .ConfigureLogging( configureLogging: (hostingContext, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom
                        .Configuration(hostingContext.Configuration, "Serilog")
                        .CreateLogger();
                    logging.AddSerilog(dispose: true);
                })
                .UseSerilog();

            await builder.RunConsoleAsync();
        }
    }
}
