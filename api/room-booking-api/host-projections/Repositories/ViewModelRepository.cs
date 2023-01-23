using MongoDB.Driver;
using view_models;

namespace host_projections.Repositories;

public class ViewModelRepository : IViewModelRepository
{
    private readonly IMongoDatabase _db;

    public ViewModelRepository(string connectionString)
    {
        var client = new MongoClient(connectionString);

        _db = client.GetDatabase("room-booking");
    }

    public T Get<T>(string id) where T : ViewModel
    {
        var mongoCollection = _db.GetCollection<T>(nameof(T));

        return mongoCollection.AsQueryable().FirstOrDefault(x => x.Id == id);
    }

    public void Save<T>(T objectToBeSaved) where T : ViewModel
    {
        var mongoCollection = _db.GetCollection<T>(nameof(T));

        mongoCollection.InsertOne(objectToBeSaved);
    }
}