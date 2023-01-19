using events;
using host_domain.Aggregates;
using host_domain.Repositories;
using Moq;
using NUnit.Framework;

namespace host_domain_tests.Aggregates
{
    [TestFixture]
    public class AggregateTests
    {
        private Mock<IEventRepository> _mockEventRepository;
        private Mock<IEventDispatcher> _mockEventDispatcher;
        private Aggregate _sut;

        [SetUp]
        public void SetUp()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _mockEventDispatcher = new Mock<IEventDispatcher>();

            _sut = new Aggregate("123", _mockEventRepository.Object, _mockEventDispatcher.Object);
        }

        [Test]
        public void SetsIdFromConstructor()
        {
            var aggregate = new Aggregate("id", null, null);

            Assert.AreEqual("id", aggregate.Id);
        }

        [Test]
        public void SetsVersionToZero()
        {
            var aggregate = new Aggregate("id", null, null);

            Assert.AreEqual(0, aggregate.Version);
        }

        [Test]
        public void Raise_GivenEvent_SetsAggregateIdOnEventToId()
        {
            _sut.Raise(new RoomRegisteredEvent());

            _mockEventRepository
                .Verify(x =>
                    x.SaveEvent(It.Is<RoomRegisteredEvent>(y => y.AggregateId.Equals(_sut.Id))));
        }

        [Test]
        public void Raise_GivenEvent_SetsEventVersionToOneGreaterThanAggregateVersion()
        {
            _sut.Raise(new RoomRegisteredEvent());

            _mockEventRepository
                .Verify(x =>
                    x.SaveEvent(It.Is<RoomRegisteredEvent>(y => y.AggregateVersion.Equals(_sut.Version + 1))));
        }
    }
}