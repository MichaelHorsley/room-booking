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
        [Test]
        public void SetUpUniqueConstraintsToPreventDuplicateEvents()
        {
            var options = new MongoRunnerOptions
            {
                AdditionalArguments = "--quiet"
            };

            using (var runner = MongoRunner.Run(options))
            {
                var connectionString = runner.ConnectionString;

                var sut = new EventRepository(connectionString);

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

                sut.SaveEvent(goodEvent);

                Assert.Throws<MongoWriteException>(() => sut.SaveEvent(duplicateEventThatIsHappeningOnAnotherThread));

                var anotherGoodEvent = new RoomRegisteredEvent
                {
                    Id = Guid.NewGuid(),
                    AggregateId = aggregateId,
                    AggregateVersion = aggregateVersion + 1
                };

                sut.SaveEvent(anotherGoodEvent);
            }
        }
    }
}