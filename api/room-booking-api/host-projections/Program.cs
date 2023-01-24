﻿using host_projections.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using host_projections.HostedServices;
using Microsoft.Extensions.Logging;
using rabbitmq_infrastructure;
using System.Reflection;
using host_projections.Projections;

namespace host_projections
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

            logger.Information("Starting up host projection service");

            await Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    RegisterHostedServices(services, configuration);

                    RegisterViewModelProjections(services);
                    RegisterRepositories(services, configuration);
                    RegisterServices(services);
                })
                .UseSerilog(logger)
                .RunConsoleAsync();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IMessageQueueConnectionFactory, RabbitMqConnectionFactory>();
        }

        private static void RegisterViewModelProjections(IServiceCollection services)
        {
            var types = Assembly.GetAssembly(typeof(AllRoomsViewModelProjection)).GetTypes();

            var projectionTypes = types.Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleEvent"))).ToList();

            foreach (var handlerType in projectionTypes)
            {
                services.AddSingleton(handlerType);
            }
        }

        private static void RegisterHostedServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddHostedService(serviceProvider =>
                new EventConsumerHostedService(
                    serviceProvider.GetService<ILogger<EventConsumerHostedService>>(),
                    serviceProvider.GetService<IMessageQueueConnectionFactory>(),
                    configuration.GetConnectionString("RabbitMq"),
                    serviceProvider));
        }

        private static void RegisterRepositories(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<IViewModelRepository>(_ => new ViewModelRepository(configuration.GetConnectionString("MongoDb")));
        }
    }
}