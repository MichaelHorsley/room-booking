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
    }

    public void SaveEvent<T>(T @event) where T : Event
    {
        var coll = _db.GetCollection<T>("events");

        coll.InsertOne(@event);
    }
}