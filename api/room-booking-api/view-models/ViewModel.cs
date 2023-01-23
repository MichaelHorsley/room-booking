using MongoDB.Bson.Serialization.Attributes;

namespace view_models;

public class ViewModel
{
    [BsonId]
    public string Id { get; set; }
}