using System.Reflection;
using System.Text;
using commands;
using host_domain.CommandHandlers;
using host_domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace host_domain.HostedServices;

public class RabbitMessageConsumerService : IHostedService
{
    private readonly ILogger<RabbitMessageConsumerService> _logger;
    private readonly IMessageQueueConnectionFactory _connectionFactory;
    private readonly string _connectionString;
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMessageConsumerService(ILogger<RabbitMessageConsumerService> logger, IMessageQueueConnectionFactory connectionFactory, string connectionString, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
        _connectionString = connectionString;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is starting up");

        _connection = await _connectionFactory.CreateConnection(_connectionString, cancellationToken);

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "command", type: "direct", durable: true, autoDelete: false);

        CreateQueuesForAllCommands(_channel);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(5000, cancellationToken);
        }
    }

    private void CreateQueuesForAllCommands(IModel channel)
    {
        var types = Assembly.GetAssembly(typeof(RegisterNewRoomCommand))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(Command)))
            .ToList();

        var commandHandlerTypes = Assembly.GetAssembly(typeof(RegisterNewRoomCommandHandler))
            .GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleCommand"))).ToList();

        foreach (var commandType in types)
        {
            var commandHandlerForCommandType = commandHandlerTypes.FirstOrDefault(x =>
                x.GetInterfaces().First().GenericTypeArguments.First() == commandType);

            var service = _serviceProvider.GetService(commandHandlerForCommandType);

            var commandHandlerTypeName = commandHandlerForCommandType.Name.ToLower();
            var commandTypeName = commandType.Name.ToLower();

            var queue = $"domain-consumer-{commandHandlerTypeName}";

            channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: queue,
                exchange: "command",
                routingKey: commandTypeName);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var method = service.GetType().GetMethod("Handle");
                
                method.Invoke(service, new object[] { JsonConvert.DeserializeObject(message, service.GetType().GetInterfaces().First().GenericTypeArguments.First()) });
            };

            channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: consumer);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is shutting down");

        _channel?.Close();
        _channel?.Dispose();

        _connection?.Close();
        _connection?.Dispose();

        return Task.CompletedTask;
    }
}