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
        private RabbitMessageConsumerService _sut;

        private Mock<ILogger<RabbitMessageConsumerService>> _mockLogger;
        private Mock<IMessageQueueConnectionFactory> _mockMessageQueueConnectFactory;
        private Mock<IServiceProvider> _mockServiceProvider;

        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<RabbitMessageConsumerService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            _mockChannel = new Mock<IModel>();

            _mockConnection = new Mock<IConnection>();
            _mockConnection.Setup(x => x.CreateModel()).Returns(_mockChannel.Object);

            _mockMessageQueueConnectFactory = new Mock<IMessageQueueConnectionFactory>();
            _mockMessageQueueConnectFactory
                .Setup(x => x.CreateConnection(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockConnection.Object));

            _sut = new RabbitMessageConsumerService(_mockLogger.Object, _mockMessageQueueConnectFactory.Object, "", _mockServiceProvider.Object);
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
    }
}