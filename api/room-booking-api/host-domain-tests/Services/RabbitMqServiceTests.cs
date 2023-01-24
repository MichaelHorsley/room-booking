using host_domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using rabbitmq_infrastructure;

namespace host_domain_tests.Services
{
    [TestFixture]
    public class RabbitMqServiceTests
    {
        private RabbitMqService _sut;
        private Mock<ILogger<RabbitMqService>> _mockLogger;
        private Mock<IMessageQueueConnectionFactory> _mockConnectionFactory;
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<RabbitMqService>>();
            
            _mockConnectionFactory = new Mock<IMessageQueueConnectionFactory>();
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            _mockConnection.Setup(x => x.CreateModel()).Returns(_mockChannel.Object);

            _mockConnectionFactory
                .Setup(x => x.CreateConnection(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockConnection.Object));

            _sut = new RabbitMqService(_mockLogger.Object, _mockConnectionFactory.Object, "");
        }

        [Test]
        public void Constructor_SetsUpCorrectExchange()
        {
            _mockChannel.Verify(x => x.ExchangeDeclare("event", "direct", true, false, null), Times.Once);
        }
    }
}