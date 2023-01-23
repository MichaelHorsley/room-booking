using System.Reflection;
using host_domain.CommandHandlers;
using host_domain.HostedServices;
using host_domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;

namespace host_domain_tests.HostedServices
{
    [TestFixture]
    public class RabbitMessageConsumerServiceTests
    {
        private CommandConsumerHostedService _sut;

        private Mock<ILogger<CommandConsumerHostedService>> _mockLogger;
        private Mock<IMessageQueueConnectionFactory> _mockMessageQueueConnectFactory;
        private Mock<IServiceProvider> _mockServiceProvider;

        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<CommandConsumerHostedService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            _mockChannel = new Mock<IModel>();

            _mockConnection = new Mock<IConnection>();
            _mockConnection.Setup(x => x.CreateModel()).Returns(_mockChannel.Object);

            _mockMessageQueueConnectFactory = new Mock<IMessageQueueConnectionFactory>();
            _mockMessageQueueConnectFactory
                .Setup(x => x.CreateConnection(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockConnection.Object));

            _sut = new CommandConsumerHostedService(_mockLogger.Object, _mockMessageQueueConnectFactory.Object, "", _mockServiceProvider.Object);
        }

        [Test]
        public async Task StartAsync_RegistersExpectedExchanges()
        {
            await _sut.StartAsync(new CancellationToken(true));

            _mockChannel.Verify(x => x.ExchangeDeclare("command", "direct", true, false, null));
        }

        [Test]
        public async Task StartAsync_RegistersExpectedQueues()
        {
            await _sut.StartAsync(new CancellationToken(true));

            var commandHandlerTypes = Assembly.GetAssembly(typeof(RegisterNewRoomCommandHandler))
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleCommand"))).ToList();

            foreach (var handlerType in commandHandlerTypes)
            {
                var handlerTypeTypeName = handlerType.Name.ToLower();

                var queue = $"domain-consumer-{handlerTypeTypeName}";

                _mockChannel.Verify(x => x.QueueDeclare(queue, true, false, false, null));
            }
        }

        [Test]
        public async Task StartAsync_BindsQueuesToExpectedExchanges()
        {
            await _sut.StartAsync(new CancellationToken(true));

            var commandHandlerTypes = Assembly.GetAssembly(typeof(RegisterNewRoomCommandHandler))
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IHandleCommand"))).ToList();

            foreach (var handlerType in commandHandlerTypes)
            {
                var commandTypeName = handlerType.Name.ToLower();

                var queue = $"domain-consumer-{commandTypeName}";
                var routingKey = $"{handlerType.GetInterfaces().First().GetGenericArguments().First().Name.ToLower()}";

                _mockChannel.Verify(x => x.QueueBind(queue, "command", routingKey, null));
            }
        }

        [Test]
        public async Task StopAsync_ClosesChannel()
        {
            await _sut.StartAsync(new CancellationToken(true));
            await _sut.StopAsync(new CancellationToken(true));

            _mockChannel.Verify(x => x.Close(), Times.Once);
            _mockChannel.Verify(x => x.Dispose(), Times.Once);
            
            _mockConnection.Verify(x => x.Close(), Times.Once);
            _mockConnection.Verify(x => x.Dispose(), Times.Once);
        }
    }
}