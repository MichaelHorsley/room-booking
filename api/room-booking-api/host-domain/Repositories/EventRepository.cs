using events;
using MongoDB.Driver;

namespace host_domain.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoDatabase _db;

    public EventRepository(string connectionString)
    {
        var client = new MongoClient(connectionString);

        _db = client.GetDatabase("room-booking");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var collection = _db.GetCollection<Event>("events");

        var options = new CreateIndexOptions { Unique = true };

        var fieldDefinition = new StringFieldDefinition<Event>("AggregateId");
        FieldDefinition<Event> secondFieldDefinition = "AggregateVersion";

        var indexKeysDefinition = new IndexKeysDefinitionBuilder<Event>().Ascending(fieldDefinition).Ascending(secondFieldDefinition);

        var indexModel = new CreateIndexModel<Event>(indexKeysDefinition, options);

        collection.Indexes.CreateOne(indexModel);
    }

    public void SaveEvent<T>(T @event) where T : Event
    {
        var collection = _db.GetCollection<T>("events");

        collection.InsertOne(@event);
    }
}