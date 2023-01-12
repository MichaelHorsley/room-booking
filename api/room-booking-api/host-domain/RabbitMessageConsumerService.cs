using System.Reflection;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace host_domain;

public class RabbitMessageConsumerService : IHostedService
{
    private readonly ILogger<RabbitMessageConsumerService> _logger;

    public RabbitMessageConsumerService(ILogger<RabbitMessageConsumerService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is starting up");

        var factory = new ConnectionFactory { HostName = "localhost" };
        
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "command", type: "direct");

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
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service is shutting down");

        return Task.CompletedTask;
    }
}