using System.Reflection;
using host_domain.CommandHandlers;
using host_domain.HostedServices;
using host_domain.Repositories;
using host_domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using rabbitmq_infrastructure;
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
            configurationBuilder.AddEnvironmentVariables();

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
                    services.AddHostedService(serviceProvider => 
                        new CommandConsumerHostedService(
                            serviceProvider.GetService<ILogger<CommandConsumerHostedService>>(), 
                            serviceProvider.GetService<IMessageQueueConnectionFactory>(),
                            configuration.GetConnectionString("RabbitMq"), 
                            serviceProvider));

                    RegisterCommandHandlers(services);
                    RegisterServices(services, configuration);
                    RegisterRepositories(services, configuration);
                })
                .UseSerilog(logger)
                .RunConsoleAsync();
        }

        private static void RegisterServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<IAggregateService, AggregateService>();
            services.AddSingleton<IMessageQueueConnectionFactory, RabbitMqConnectionFactory>();
            services.AddSingleton<IEventDispatcher>(serviceProvider => new RabbitMqService(serviceProvider.GetService<ILogger<RabbitMqService>>(), serviceProvider.GetService<IMessageQueueConnectionFactory>(), configuration.GetConnectionString("RabbitMq")));
        }

        private static void RegisterRepositories(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<IEventRepository>(serviceProvider => new EventRepository(configuration.GetConnectionString("MongoDb")));
        }

        private static void RegisterCommandHandlers(IServiceCollection services)
        {
            var types = Assembly.GetAssembly(typeof(RegisterNewRoomCommandHandler)).GetTypes();

            var commandHandlerTypes = types.Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleCommand"))).ToList();

            foreach (var handlerType in commandHandlerTypes)
            {
                services.AddSingleton(handlerType);
            }
        }
    }
}