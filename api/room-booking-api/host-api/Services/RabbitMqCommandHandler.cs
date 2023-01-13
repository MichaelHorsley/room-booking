using System.Text;
using host_api.Controllers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace host_api.Services;

public class RabbitMqCommandHandler : ICommandHandler
{
    private readonly IConnection _connection;

    public RabbitMqCommandHandler(string hostname)
    {
        var factory = new ConnectionFactory { HostName = hostname };

        _connection = factory.CreateConnection();

        DeclareExchanges();
    }

    private void DeclareExchanges()
    {
        try
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "command", type: "direct", durable: true, autoDelete: false);
            }
        }
        catch(Exception e)
        {
            Log.Error(e, "Error when trying to create command exchange");
        }
    }

    public void Dispatch<T>(T command) where T : Command
    {
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));

        using (var channel = _connection.CreateModel())
        {
            var commandTypeName = typeof(T).Name;

            channel.BasicPublish(exchange: "command",
                routingKey: commandTypeName,
                basicProperties: null,
                body: body);
        }
    }
}