using events;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace host_domain.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoDatabase _db;

    public EventRepository(string connectionString)
    {
        RegisterKnownTypes();

        var client = new MongoClient(connectionString);

        _db = client.GetDatabase("room-booking");

        CreateIndexes();
    }

    private void RegisterKnownTypes()
    {
        try
        {
            BsonClassMap.RegisterClassMap<EventWrapper>();
            BsonClassMap.RegisterClassMap<Event>();
            BsonClassMap.RegisterClassMap<RoomRegisteredEvent>();
            BsonClassMap.RegisterClassMap<HostSignedUpEvent>();
        }
        catch {}
    }

    public void SaveEvent<T>(T @event) where T : Event
    {
        var eventWrapper = new EventWrapper
        {
            AggregateId = @event.AggregateId,
            AggregateVersion = @event.AggregateVersion,
            EventType = typeof(T).Name,
            Event = @event
        };

        var collection = _db.GetCollection<EventWrapper>("events");

        collection.InsertOne(eventWrapper);
    }

    public List<T> GetEvents<T>(string aggregateId) where T : Event
    {
        var collection = _db.GetCollection<EventWrapper>("events");

        return collection
            .AsQueryable()
            .Where(x => x.AggregateId.Equals(aggregateId))
            .Select(x => (T)x.Event)
            .ToList();
    }

    private void CreateIndexes()
    {
        var collection = _db.GetCollection<EventWrapper>("events");

        var options = new CreateIndexOptions { Unique = true };

        var firstFieldDefinition = new StringFieldDefinition<EventWrapper>("AggregateId");
        FieldDefinition<EventWrapper> secondFieldDefinition = "AggregateVersion";

        var indexKeysDefinition = new IndexKeysDefinitionBuilder<EventWrapper>().Ascending(firstFieldDefinition).Ascending(secondFieldDefinition);

        var indexModel = new CreateIndexModel<EventWrapper>(indexKeysDefinition, options);

        collection.Indexes.CreateOne(indexModel);
    }

    internal class EventWrapper
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AggregateId { get; set; }
        public int AggregateVersion { get; set; }
        public string EventType { get; set; }
        public object Event { get; set; }
    }
}