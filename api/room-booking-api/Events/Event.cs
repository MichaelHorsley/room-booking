using MongoDB.Bson.Serialization.Attributes;

namespace events;

[BsonIgnoreExtraElements]
public class Event
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AggregateId { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public int AggregateVersion { get; set; }
}