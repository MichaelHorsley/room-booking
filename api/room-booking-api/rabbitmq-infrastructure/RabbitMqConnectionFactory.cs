using RabbitMQ.Client;
using Serilog;

namespace rabbitmq_infrastructure;

public class RabbitMqConnectionFactory : IMessageQueueConnectionFactory
{
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