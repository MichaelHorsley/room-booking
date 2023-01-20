﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

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

                })
                .UseSerilog(logger)
                .RunConsoleAsync();
        }
    }
}