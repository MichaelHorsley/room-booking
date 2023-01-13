using host_domain.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace host_domain
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration from appsettings
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();

            // Build logging
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            logger.Information("Starting up domain service");
            
            await Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService((serviceProvider) => new RabbitMessageConsumerService(serviceProvider.GetService<ILogger<RabbitMessageConsumerService>>(), configuration.GetConnectionString("RabbitMq")));
                })
                .UseSerilog(logger)
                .RunConsoleAsync();
        }
    }
}