using events;
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
            _mockEventRepository.Setup(x => x.GetEvents<Event>(It.IsAny<string>())).Returns(new List<Event>());

            _sut = new AggregateService(_mockEventRepository.Object);
        }

        [Test]
        public void Get_GivenNoAggregateEvents_ReturnsAggregateWithId()
        {
            var roomAggregate = _sut.Get<RoomAggregate>("1");

            Assert.AreEqual("1", roomAggregate.Id);
        }

        [Test]
        public void Get_GivenNoAggregateEvents_ReturnsAggregateWithVersion0()
        {
            var roomAggregate = _sut.Get<RoomAggregate>("1");

            Assert.AreEqual(0, roomAggregate.Version);
        }

        [Test]
        public void Get_GivenPreviousAggregateEvents_ReturnsAggregateWithCorrectVersion()
        {
            var aggregateId = "1";

            _mockEventRepository
                .Setup(x => x.GetEvents<Event>(It.IsAny<string>()))
                .Returns(new List<Event>
                {
                    new RoomRegisteredEvent
                    {
                        AggregateVersion = 1
                    },
                    new RoomRegisteredEvent
                    {
                        AggregateVersion = 2
                    },
                    new RoomRegisteredEvent
                    {
                        AggregateVersion = 3
                    },
                });

            var roomAggregate = _sut.Get<RoomAggregate>(aggregateId);

            Assert.AreEqual(3, roomAggregate.Version);
        }
    }
}