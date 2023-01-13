using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace host_domain;

public class RabbitMessageConsumerService : IHostedService
{
    private readonly ILogger<RabbitMessageConsumerService> _logger;
    private readonly string _connectionString;
    private IConnection _connection;

    public RabbitMessageConsumerService(ILogger<RabbitMessageConsumerService> logger, string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is starting up");

        var factory = new ConnectionFactory { HostName = _connectionString };

        while (_connection == null)
        {
            try
            {
                _connection = factory.CreateConnection();
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, "Having issues connecting to rabbitmq");

                await Task.Delay(1000, cancellationToken);
            }
        }

        _connection = factory.CreateConnection();

        using (var channel = _connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "command", type: "direct", durable: true, autoDelete: false);

            channel.QueueDeclare(queue: "command-domain-consumer",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: "command-domain-consumer",
                exchange: "command",
                routingKey: "RegisterNewRoomCommand");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation(message);
            };

            channel.BasicConsume(queue: "command-domain-consumer",
                autoAck: true,
                consumer: consumer);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is shutting down");

        _connection?.Close();
        _connection?.Dispose();

        return Task.CompletedTask;
    }
}