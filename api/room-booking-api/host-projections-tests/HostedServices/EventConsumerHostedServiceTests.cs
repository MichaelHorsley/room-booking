using host_projections;
using host_projections.HostedServices;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using rabbitmq_infrastructure;

namespace host_projections_tests.HostedServices
{
    [TestFixture]
    public class EventConsumerHostedServiceTests
    {
        private EventConsumerHostedService _sut;
        private Mock<ILogger<EventConsumerHostedService>> _mockLogger;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<IModel> _mockChannel;
        private Mock<IConnection> _mockConnection;
        private Mock<IMessageQueueConnectionFactory> _mockMessageQueueConnectFactory;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<EventConsumerHostedService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            
            _mockChannel = new Mock<IModel>();

            _mockConnection = new Mock<IConnection>();
            _mockConnection.Setup(x => x.CreateModel()).Returns(_mockChannel.Object);

            _mockMessageQueueConnectFactory = new Mock<IMessageQueueConnectionFactory>();
            _mockMessageQueueConnectFactory
                .Setup(x => x.CreateConnection(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockConnection.Object));

            _sut = new EventConsumerHostedService(_mockLogger.Object, _mockMessageQueueConnectFactory.Object, "", _mockServiceProvider.Object);
        }

        [Test]
        public async Task Test()
        {
            var cancellationToken = new CancellationToken(true);

            await _sut.StartAsync(cancellationToken);
        }
    }
}