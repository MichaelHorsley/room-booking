using RabbitMQ.Client;

namespace host_domain.Services;

public interface IMessageQueueConnectionFactory
{
    Task<IConnection> CreateConnection(string hostName, CancellationToken cancellationToken);
}