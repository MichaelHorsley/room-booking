using events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using rabbitmq_infrastructure;

namespace host_domain.Services;

public class RabbitMqService : IEventDispatcher
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IMessageQueueConnectionFactory _messageQueueConnectionFactory;
    private readonly string _connectionString;
    private IConnection _connection;

    public RabbitMqService(
        ILogger<RabbitMqService> logger,
        IMessageQueueConnectionFactory messageQueueConnectionFactory, 
        string connectionString)
    {
        _logger = logger;
        _messageQueueConnectionFactory = messageQueueConnectionFactory;
        _connectionString = connectionString;

        SetUpConnection();

        if (_connection == null)
        {
            _logger.LogCritical("Can't create a good connection to RabbitMq");
        }
        else
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "event", type: "direct", durable: true, autoDelete: false);
            }
        }
    }

    private void SetUpConnection()
    {
        var tryCount = 1;

        while (tryCount < 5)
        {
            try
            {
                _connection = _messageQueueConnectionFactory
                    .CreateConnection(_connectionString, new CancellationToken())
                    .Result;

                break;
            }
            catch
            {
                _logger.LogWarning("Trying to connect to RabbitMq for Event Dispatcher");

                Thread.Sleep(tryCount * 1000);

                tryCount++;
            }
        }
    }

    public void Dispatch<T>(T @event) where T : Event
    {
        if (_connection == null)
        {
            SetUpConnection();
        }

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

        using (var channel = _connection.CreateModel())
        {
            var eventTypeName = typeof(T).Name.ToLower();

            channel.BasicPublish(exchange: "event",
                routingKey: eventTypeName,
                basicProperties: null,
                body: body);
        }
    }
}