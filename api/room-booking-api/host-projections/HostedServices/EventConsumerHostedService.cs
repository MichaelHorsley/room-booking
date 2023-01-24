using System.Reflection;
using System.Text;
using host_projections.Projections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
        _logger.LogInformation("Event Consumer Service is starting up");

        _connection = await _connectionFactory.CreateConnection(_connectionString, cancellationToken);

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "event", type: "direct", durable: true, autoDelete: false);

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
        var viewModelProjectionTypes = Assembly.GetAssembly(typeof(HostListViewModelProjection))
            .GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleEvent"))).ToList();

        foreach (var viewModelProjectionType in viewModelProjectionTypes)
        {
            var service = _serviceProvider.GetService(viewModelProjectionType);

            var projectTypeName = viewModelProjectionType.Name.ToLower();

            var eventProjectionIsListeningTo = viewModelProjectionType.GetInterfaces().Select(x => x.GenericTypeArguments.First());

            var queue = $"host-projection-{projectTypeName}";

            channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            foreach (var eventType in eventProjectionIsListeningTo)
            {
                var eventTypeName = eventType.Name.ToLower();

                channel.QueueBind(queue: queue,
                    exchange: "event",
                    routingKey: eventTypeName);
            }

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var eaRoutingKey = ea.RoutingKey;
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var methodInfos = service.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

                var methodInfo = methodInfos.First(x => x.GetParameters().Any(y => y.ParameterType.Name.ToLower().Equals(eaRoutingKey)));

                var parameterType = methodInfo.GetParameters().First(x => x.ParameterType.Name.ToLower().Equals(eaRoutingKey));

                methodInfo.Invoke(service, new object[] { JsonConvert.DeserializeObject(message, parameterType.ParameterType) });
            };

            channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: consumer);
        }
    }
}