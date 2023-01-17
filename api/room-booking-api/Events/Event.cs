namespace events;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AggregateId { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}