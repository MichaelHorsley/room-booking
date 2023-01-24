using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using host_api.Mapping;
using host_api.Repositories;
using host_api.Services;
using host_api.Validation;
using Serilog;

namespace host_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Health checks
            builder.Services.AddHealthChecks();
            
            // Fluent Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterNewRoomRequestValidator>();
            
            // Automapping
            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(RegisterNewRoomRequest_RegisterNewRoomCommand_Profile)));

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

            logger.Information("Starting up host api");

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            // Add Services
            builder.Services.AddSingleton<ICommandHandler>(_ => new RabbitMqCommandHandler(configuration.GetConnectionString("RabbitMq")));

            builder.Services.AddSingleton<IViewModelRepository>(_ => new ViewModelRepository(configuration.GetConnectionString("MongoDb")));

            // Build application
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/CheckHealth");

            app.Run();
        }
    }
}