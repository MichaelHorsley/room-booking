using System.Reflection;
using events;
using host_projections.Projections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using rabbitmq_infrastructure;

namespace host_projections.HostedServices;

public class EventConsumerHostedService : IHostedService
{
    private readonly ILogger<EventConsumerHostedService> _logger;
    private readonly IMessageQueueConnectionFactory _connectionFactory;
    private readonly string _connectionString;
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;

    public EventConsumerHostedService(ILogger<EventConsumerHostedService> logger, IMessageQueueConnectionFactory connectionFactory, string connectionString, IServiceProvider serviceProvider)
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

        CreateQueuesForAllProjections(_channel);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(5000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Consumer Service is shutting down");

        _channel?.Close();
        _channel?.Dispose();

        _connection?.Close();
        _connection?.Dispose();

        return Task.CompletedTask;
    }

    private void CreateQueuesForAllProjections(IModel channel)
    {
        var types = Assembly.GetAssembly(typeof(Event))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(Event)))
            .ToList();

        var viewModelProjectionTypes = Assembly.GetAssembly(typeof(HostListViewModelProjection))
            .GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleEvent"))).ToList();

        foreach (var viewModelProjectionType in viewModelProjectionTypes)
        {
                
        }

        foreach (var eventType in types)
        {
            //    var commandHandlerForCommandType = viewModelProjectionTypes.FirstOrDefault(x =>
            //        x.GetInterfaces().First().GenericTypeArguments.First() == commandType);

            //    var service = _serviceProvider.GetService(commandHandlerForCommandType);

            //    var commandHandlerTypeName = commandHandlerForCommandType.Name.ToLower();
            //    var commandTypeName = commandType.Name.ToLower();

            //    var queue = $"domain-consumer-{commandHandlerTypeName}";

            //    channel.QueueDeclare(queue: queue,
            //        durable: true,
            //        exclusive: false,
            //        autoDelete: false,
            //        arguments: null);

            //    channel.QueueBind(queue: queue,
            //        exchange: "command",
            //        routingKey: commandTypeName);

            //    var consumer = new EventingBasicConsumer(channel);

            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body.ToArray();
            //        var message = Encoding.UTF8.GetString(body);

            //        var method = service.GetType().GetMethod("Handle");

            //        method.Invoke(service, new object[] { JsonConvert.DeserializeObject(message, service.GetType().GetInterfaces().First().GenericTypeArguments.First()) });
            //    };

            //    channel.BasicConsume(queue: queue,
            //        autoAck: true,
            //        consumer: consumer);
        }
    }
}