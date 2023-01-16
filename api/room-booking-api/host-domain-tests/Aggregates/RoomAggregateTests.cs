using events;
using host_domain.Aggregates;
using host_domain.Repositories;
using Moq;
using NUnit.Framework;

namespace host_domain_tests.Aggregates
{
    [TestFixture]
    public class RoomAggregateTests
    {
        private RoomAggregate _sut;
        private Mock<IEventRepository> _mockEventRepository;

        [SetUp]
        public void SetUp()
        {
            _mockEventRepository = new Mock<IEventRepository>();

            _sut = new RoomAggregate("aggregateId", _mockEventRepository.Object);
        }

        [Test]
        public void GivenRoomNotAlreadyCreated_RaisesEvent()
        {
            var hostId = Guid.NewGuid();

            var expectedEventRaised = new RoomRegisteredEvent
            {
                AggregateId = "aggregateId",
                RoomId = "room-id-123",
                HostId = hostId
            };

            _sut.RegisterNewRoom(hostId, "room-id-123");

            _mockEventRepository
                .Verify(x =>
                    x.SaveEvent(It.Is<RoomRegisteredEvent>(y =>
                        y.HostId.Equals(expectedEventRaised.HostId)
                        && y.RoomId.Equals(expectedEventRaised.RoomId)
                        && y.AggregateId.Equals(expectedEventRaised.AggregateId)
                    )), Times.Once);
        }

        [Test]
        public void GivenRoomAlreadyCreated_DoesNotRaisesEvent()
        {
            var registeredEvent = new RoomRegisteredEvent
            {
                AggregateId = "aggregateId",
                RoomId = "room-id-123",
                HostId = Guid.NewGuid()
            };

            _sut.ApplyEventAgainstAggregate(registeredEvent);

            _sut.RegisterNewRoom(Guid.NewGuid(), "room-id-123");

            _mockEventRepository
                .Verify(x =>
                    x.SaveEvent(It.IsAny<RoomRegisteredEvent>()), Times.Never);
        }
    }
}