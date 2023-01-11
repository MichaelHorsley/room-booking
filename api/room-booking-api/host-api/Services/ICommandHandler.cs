using System.Reflection;
using System.Text;
using System.Threading.Channels;
using host_api.Controllers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace host_api.Services
{
    public interface ICommandHandler
    {
        void Dispatch(Command command);
    }

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
                    channel.ExchangeDeclare(exchange: "command", type: "direct");
                }
            }
            catch(Exception e)
            {
                Log.Error(e, "Error when trying to create command exchange");
            }
        }

        public void Dispatch(Command command)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));

            using (var channel = _connection.CreateModel())
            {
                channel.BasicPublish(exchange: "command",
                    routingKey: nameof(command),
                    basicProperties: null,
                    body: body);
            }
        }
    }
}