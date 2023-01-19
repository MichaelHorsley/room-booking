using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace host_domain.Services;

public class RabbitMqConnectionFactory : IMessageQueueConnectionFactory
{
    private readonly ILogger<RabbitMqConnectionFactory> _logger;

    public RabbitMqConnectionFactory(ILogger<RabbitMqConnectionFactory> logger)
    {
        _logger = logger;
    }

    public async Task<IConnection> CreateConnection(string hostName, CancellationToken cancellationToken)
    {
        IConnection connection = null;

        var factory = new ConnectionFactory { HostName = hostName };

        var connectionCount = 1;

        while (connection == null)
        {
            try
            {
                connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Having issues connecting to rabbitmq");

                if (connectionCount > 10)
                {
                    _logger.LogCritical("Unable to connect to RabbitMqService");

                    throw new Exception("Unable to connect to message queue");
                }

                await Task.Delay(1000 * connectionCount++, cancellationToken);
            }
        }

        return connection;
    }
}