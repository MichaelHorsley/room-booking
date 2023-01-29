using RabbitMQ.Client;
using Serilog;

namespace rabbitmq_infrastructure;

public class RabbitMqConnectionFactory : IMessageQueueConnectionFactory
{
    public async Task<IConnection> CreateConnection(string uri, CancellationToken cancellationToken)
    {
        IConnection connection = null;

        ConnectionFactory factory;

        if (uri.Contains("amqps"))
        {
            factory = new ConnectionFactory
            {
                Uri = new Uri(uri)
            };
        }
        else
        {
            factory = new ConnectionFactory { HostName = uri };
        }

        var connectionCount = 1;

        while (connection == null)
        {
            try
            {
                connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                Log.Logger.Warning(e, "Having issues connecting to rabbitmq");

                if (connectionCount > 10)
                {
                    Log.Logger.Fatal("Unable to connect to RabbitMqService");

                    throw new Exception("Unable to connect to message queue");
                }

                await Task.Delay(1000 * connectionCount++, cancellationToken);
            }
        }

        return connection;
    }
}