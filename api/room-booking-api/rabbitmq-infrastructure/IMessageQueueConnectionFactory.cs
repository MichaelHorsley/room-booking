using RabbitMQ.Client;

namespace rabbitmq_infrastructure;

public interface IMessageQueueConnectionFactory
{
    Task<IConnection> CreateConnection(string uri, CancellationToken cancellationToken);
}