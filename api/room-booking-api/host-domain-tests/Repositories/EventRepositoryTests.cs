using EphemeralMongo;
using events;
using host_domain.Repositories;
using MongoDB.Driver;
using NUnit.Framework;

namespace host_domain_tests.Repositories
{
    [TestFixture]
    public class EventRepositoryTests
    {
        private IMongoRunner _mongoInstance;

        private EventRepository _sut;

        [SetUp]
        public void SetUp()
        {
            var options = new MongoRunnerOptions
            {
                AdditionalArguments = "--quiet"
            };

            _mongoInstance = MongoRunner.Run(options);

            var connectionString = _mongoInstance.ConnectionString;

            _sut = new EventRepository(connectionString);
        }

        [TearDown]
        public void TearDown()
        {
            _mongoInstance.Dispose();
        }

        [Test]
        public void SetUpUniqueConstraintsToPreventDuplicateEvents()
        {
            var aggregateId = Guid.NewGuid().ToString();
            var aggregateVersion = 1;

            var goodEvent = new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = aggregateVersion
            };

            var duplicateEventThatIsHappeningOnAnotherThread = new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = aggregateVersion
            };

            _sut.SaveEvent(goodEvent);

            Assert.Throws<MongoWriteException>(() => _sut.SaveEvent(duplicateEventThatIsHappeningOnAnotherThread));

            var anotherGoodEvent = new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = aggregateVersion + 1
            };

            _sut.SaveEvent(anotherGoodEvent);
        }

        [Test]
        public void GetEvents_GivenNoEvents_ReturnsEmptyList()
        {
            var events = _sut.GetEvents<Event>("");

            Assert.IsEmpty(events);
        }

        [Test]
        public void GetEvents_GivenThereAreEvents_ReturnsAggregateEventsOnly()
        {
            var aggregateId = Guid.NewGuid().ToString();

            _sut.SaveEvent(new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = 1
            });

            _sut.SaveEvent(new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = 2
            });

            _sut.SaveEvent(new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.NewGuid().ToString(),
                AggregateVersion = 1
            });

            var events = _sut.GetEvents<Event>(aggregateId);

            Assert.AreEqual(2, events.Count);
            Assert.IsTrue(events.All(x => x.AggregateId.Equals(aggregateId)));
        }

        [Test]
        public void GetEvents_GivenThereAreEvents_ReturnsEventsInCorrectAggregateVersionOrder()
        {
            var aggregateId = Guid.NewGuid().ToString();

            _sut.SaveEvent(new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = 2,
                HostId = Guid.NewGuid(),
                RoomId = "room 1"
            });

            _sut.SaveEvent(new RoomRegisteredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateVersion = 1,
                HostId = Guid.NewGuid(),
                RoomId = "room 1"
            });

            var events = _sut.GetEvents<Event>(aggregateId);

            Assert.AreEqual(2, events.Count);

            Assert.AreEqual(1, events[0].AggregateVersion);
            Assert.AreEqual(2, events[1].AggregateVersion);
        }
    }
}