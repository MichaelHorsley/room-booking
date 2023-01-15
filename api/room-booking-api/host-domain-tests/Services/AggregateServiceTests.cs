using host_domain.Aggregates;
using host_domain.Repositories;
using host_domain.Services;
using Moq;
using NUnit.Framework;

namespace host_domain_tests.Services
{
    [TestFixture]
    public class AggregateServiceTests
    {
        private AggregateService _sut;
        private Mock<IEventRepository> _mockEventRepository;

        [SetUp]
        public void SetUp()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _sut = new AggregateService(_mockEventRepository.Object);
        }

        [Test]
        public void Get_ReturnsAggregateWithId()
        {
            var roomAggregate = _sut.Get<RoomAggregate>("1");

            Assert.AreEqual("1", roomAggregate.Id);
        }
    }
}